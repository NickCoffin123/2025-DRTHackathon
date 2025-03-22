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

        public MainPage() {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e) {
            string stopID = myEntry.Text;
            if (ValidateStopID(stopID)) {
                stop = GetStop(stopID);
                stopTime = GetStopTime(stopID);
                trip = GetTrip(stopTime.tripID);
            }

            Debug.WriteLine($"{stop.stopName} - {stopTime.arrivalTime} - {trip.tripHeadsign}");
            
            
            //TripUpdates();

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
                Debug.WriteLine(trip.tripID);
                if (trip.tripID == tripID) {
                    return trip;
                }
            }
            return new Trip();
        }

        private void TripUpdates() {
            WebRequest req = HttpWebRequest.Create(VEHICLE_POSITIONS_URL);
            FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            foreach (FeedEntity entity in feed.Entities) {
                if (entity.Vehicle.StopId == stop.stopID) {
                    Debug.WriteLine("Matching trip found! : ");
                    Debug.WriteLine(entity.Vehicle.Position.Latitude);
                    Debug.WriteLine(entity.Vehicle.Position.Longitude);
                }
            }
        }
    }
}
