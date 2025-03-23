using System.Diagnostics;
using System.Net;
using ProtoBuf;
using TransitRealtime;
using DRTApp.Classes;

namespace DRTApp
{
    public partial class MainPage : ContentPage {
        //CONSTS
        string TRIP_UPDATES_URL = "https://drtonline.durhamregiontransit.com/gtfsrealtime/TripUpdates";
        string VEHICLE_POSITIONS_URL = "https://drtonline.durhamregiontransit.com/gtfsrealtime/VehiclePositions";
        // COMPONENTS
        RawResourceHandler res;
        sStop stop;
        StopTime stopTime;
        Trip trip;
        List<string> busPositions = new();

        public MainPage() {
            InitializeComponent();

            // Initial validate call to trigger instnatiation
            ValidateStopID(StopIDEntry.Text);
        }

        private async void OnMapGeneratorClicked(object sender, EventArgs e) {
            string stopID = StopIDEntry.Text;
            if (ValidateStopID(stopID)) {
                stop = GetStop(stopID);
                stopTime = GetStopTime(stopID);
                trip = GetTrip(stopTime.tripID);
                GetIncomingTripsLive();
                await Navigation.PushAsync(new MapPage(stop, busPositions));
            }

            Debug.WriteLine($"{stop.stopName} - Arriving at: {stopTime.arrivalTime} - Travelling to: {trip.tripHeadsign}");




        }

        private bool ValidateStopID(string stopID) {
            List<sStop> stops = RawResourceHandler.Instance.Stops;

            foreach (sStop stop in stops) {
                if (stop.stopID == stopID) {
                    return true;
                }
            }

            return false;
        }

        private sStop GetStop(string stopID) {
            foreach (sStop stop in RawResourceHandler.Instance.Stops) {
                if (stop.stopID == stopID) {
                    return stop;
                }
            }
            return new sStop();
        }

        private StopTime GetStopTime(string stopID) {
            foreach (StopTime stopTime in RawResourceHandler.Instance.StopTimes) {
                if (stopTime.stopID == stopID) {
                    return stopTime;
                }
            }
            return new StopTime();
        }

        private Trip GetTrip(string tripID) {
            foreach (Trip trip in RawResourceHandler.Instance.Trips) {
                if (trip.tripID == tripID) {
                    return trip;
                }
            }
            return new Trip();
        }

        private void GetIncomingTripsLive() {
            busPositions.Clear();

            List<Trip> incomingTrips = RawResourceHandler.Instance.GetNextThreeTripsByStopID(stop.stopID);
            List<string> incomingTripIds = new();

            if (incomingTrips.Count == 0) {
                Debug.WriteLine("No incoming busses");
                return;
            }

            foreach (Trip trip in incomingTrips) {
                Debug.WriteLine("Incoming trip: " + trip.tripID);
                incomingTripIds.Add(trip.tripID);
            }

            WebRequest req = HttpWebRequest.Create(VEHICLE_POSITIONS_URL);
            FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            foreach (FeedEntity entity in feed.Entities) {
                if (incomingTripIds.Contains(entity.Vehicle.Trip.TripId)) {
                    Debug.WriteLine("Matching trip found! : " + entity.Vehicle.Vehicle.Label);
                    busPositions.Add(entity.Vehicle.Position.Latitude + "," + entity.Vehicle.Position.Longitude);
                }
            }

            foreach (string pos in busPositions) {
                Debug.WriteLine("Bus " + (busPositions.IndexOf(pos) + 1) + ": " + pos);

            }
        }
    }
}
