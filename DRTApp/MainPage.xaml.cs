using System;
using System.Diagnostics;
using System.Net;
using ProtoBuf;
using TransitRealtime;
using Microsoft.Maui.Controls;
using DRTApp.Classes;

namespace DRTApp
{
    public partial class MainPage : ContentPage {

        //CONSTS
        private const string HTML_DIR = "Resources\\web\\index.html";

        string TRIP_UPDATES_URL = "https://drtonline.durhamregiontransit.com/gtfsrealtime/TripUpdates";
        string VEHICLE_POSITIONS_URL = "https://drtonline.durhamregiontransit.com/gtfsrealtime/VehiclePositions";
        
        // COMPONENTS
        RawResourceHandler res = RawResourceHandler.Instance;
        Timer _timer;

        // VARS
        int tickInterval = 3000;

        float hdrHeightPercent = .075f;
        float mapHeightPercent = .5f;
        float inputWidthPercent = .80f;

        sStop stop;
        Trip trip;

        List<string> busIDs = new();
        List<string> busPositions = new();

        // CONSTRUCTOR
        public MainPage() {
            InitializeComponent();
            InitializeTimer();

            //UpdateMap(stop, busPositions);
        }
        private async void OnCounterClicked(object sender, EventArgs e) {
            string stopID = myEntry.Text;
            if (res.ValidateStopID(stopID)) {
                stop = res.GetStop(stopID);

                GetIncomingTripsLive();
                UpdateMap(stop, busPositions);

                // For testing
                //await Navigation.PushAsync(new MapPage(stop, busPositions));
                //ErrorArea.IsVisible = false;
            }

            else
            {
                lblIncomingBusses.Text = "STOP NOT FOUND";
                //ErrorArea.IsVisible = true;
            }
        }

        // ***********************************************
        //                      MAP
        // ***********************************************
        public async void UpdateMap(sStop stop, List<string> busPositions)
        {
            // Grab HTML source
            WebMapArea.Source = await MapHtmlBuilder.GrabHtml(stop, busPositions);
        }

        // ***********************************************
        //                  TICKER
        // ***********************************************
        private void InitializeTimer()
        {
            // Create a new Timer with a callback that runs at every interval
            _timer = new Timer(CallTick, null, 0, tickInterval);
        }

        protected void CallTick(object state)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnTick();
            });
        }

        // Cleanup
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _timer?.Dispose(); // timer doesnt garbage collect
        }

        // ***If you want to add functionality call on tick events,
        // add it here. Ticker was made to live update bus pos's.
        private void OnTick()
        {
            Debug.WriteLine("Tick @ timestamp:" + DateTime.Now.ToString());
            if (busPositions.Count > 0)
            {
                UpdateBusPositions();
                UpdateMap(stop, busPositions);
            }
        }

        // ***********************************************
        //            DYNAMIC PAGE FORMATTING
        // ***********************************************
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetDynamicHeights();
            SetDynamicWidths();
        }

        // Event handler for when the window size changes
        private void OnSizeChanged(object sender, EventArgs e)
        {
            SetDynamicHeights();
            SetDynamicWidths();
        }

        private void SetDynamicHeights()
        {
            // Get the screen height
            double pageHeight = this.Height;

            // Calculate the desired heights based on percentages of the screen height
            double firstFrameHeight = pageHeight * hdrHeightPercent; // 15% of screen height
            double secondFrameHeight = pageHeight * mapHeightPercent; // 40% of screen height

            // Set the HeightRequest for each frame
            hdr.HeightRequest = firstFrameHeight;
            map.HeightRequest = secondFrameHeight;
        }

        private void SetDynamicWidths()
        {
            double pageWidth = this.Width;

            // Set the WidthRequest for both controls
            myEntry.WidthRequest = pageWidth * inputWidthPercent;
            CounterBtn.WidthRequest = pageWidth * inputWidthPercent;
            lblIncomingBusses.WidthRequest = pageWidth * inputWidthPercent;
        }

        // ***********************************************
        //                  MISC
        // ***********************************************
        private void UpdateBusPositions()
        {
            List<string> newPositions = new();
            foreach (string busID in busIDs)
            {
                // Vehicles HTTP request
                Debug.WriteLine("Making Vehicles HTTP request...");
                WebRequest vReq = HttpWebRequest.Create(VEHICLE_POSITIONS_URL);
                FeedMessage vFeed = Serializer.Deserialize<FeedMessage>(vReq.GetResponse().GetResponseStream());
                foreach (FeedEntity entity in vFeed.Entities)
                {
                    if (busIDs.Contains(entity.Vehicle.Vehicle.Id))
                    {
                        newPositions.Add(entity.Vehicle.Position.Latitude + "," + entity.Vehicle.Position.Longitude);
                    }
                }
            }

            busPositions = newPositions;
        }

        private void GetIncomingTripsLive() {
            busPositions.Clear();

            List<Trip> incomingTrips = RawResourceHandler.Instance.GetNextThreeTripsByStopID(stop.stopID);
            List<string> incomingTripIds = new();

            foreach (Trip trip in incomingTrips) {
                Debug.WriteLine("Incoming trip: " + trip.tripID);
                incomingTripIds.Add(trip.tripID);
            }

            // Trips HTTP request
            Debug.WriteLine("Making Trips HTTP request...");
            WebRequest tripsReq = HttpWebRequest.Create(VEHICLE_POSITIONS_URL);
            FeedMessage tripsFeed = Serializer.Deserialize<FeedMessage>(tripsReq.GetResponse().GetResponseStream());
            foreach (FeedEntity entity in tripsFeed.Entities)
            {
                //Debug.WriteLine("Accessing entity...");
                //Debug.WriteLine("Entity details: " +
                //    entity.ToString() + " | " +
                //    entity.Id + " | " +
                //    "Vehicle: " + entity.Vehicle.Vehicle.Id //+ "|" + entity.Vehicle.Vehicle.Label
                //);

                if (incomingTripIds.Contains(entity.Vehicle.Trip.TripId)) {
                    // needs separate trip_update http call
                    //Debug.WriteLine("Incoming Bus: " + entity.TripUpdate.Vehicle.Label + ", delay: " + entity.TripUpdate.Delay); 
                    busIDs.Add(entity.Vehicle.Vehicle.Id);
                    busPositions.Add(entity.Vehicle.Position.Latitude + "," + entity.Vehicle.Position.Longitude);
                }
            }

            if (busPositions.Count <= 0)
            {
                lblIncomingBusses.Text = "No busses are inbound for stop " + myEntry.Text;
                return;
            } else {
                lblIncomingBusses.Text = "Incoming busses found for stop " + myEntry.Text;
            }

            foreach (string pos in busPositions) {
                Debug.WriteLine("Bus " + (busPositions.IndexOf(pos) + 1) + ": " + pos);
            }
        }
    }
}
