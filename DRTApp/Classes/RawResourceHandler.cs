using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using DRTApp.Properties;

namespace DRTApp.Classes
{
    public struct sStop
    {
        public string stopID;
        public string stopName;
        public string stopLat;
        public string stopLon;
    }

    public struct StopTime
    {
        public string tripID;
        public string arrivalTime;
        public string departureTime;
        public string stopID;
        public string stopSequence;
    }

    public struct Trip
    {
        public string routeID;
        public string serviceID;
        public string tripID;
        public string tripHeadsign;
        public string directionID;
        public string blockID;
        public string shapeID;
    }

    public class RawResourceHandler
    {
        // CONSTS
        // Fixed name of the stops files we downloaded
        private static string STOPS = "Resources\\raw\\stops.txt";
        private static string STOP_TIMES = "Resources\\raw\\stop_times.txt";
        private static string TRIPS = "Resources\\raw\\trips.txt";

       

        // PROPS
        public List<sStop> Stops { get; set; }
        public List<StopTime> StopTimes { get; set; }
        public List<Trip> Trips { get; set; }

        // SINGLETON
        private static RawResourceHandler _instance;
        public static RawResourceHandler Instance => _instance ??= new RawResourceHandler();

        private RawResourceHandler()
        {
            Stops = new();
            StopTimes = new();
            Trips = new();

            GetStops();
            GetStopTimes();
            GetTrips();
        }

        private static async void GetStops()
        {
            // file stream setups
            Stream fileStream = await FileSystem.OpenAppPackageFileAsync(STOPS);
            StreamReader reader = new(fileStream);

            // parser
            string content = await reader.ReadToEndAsync();
            string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // For each line
            foreach (string line in lines)
            {
                // Split row
                var fields = line.Split(',');
                sStop stop = new()
                {
                    stopID = fields[0],
                    stopName = fields[2],
                    stopLat = fields[4],
                    stopLon = fields[5]
                };

                Instance.Stops.Add(stop);
            }
        }

        private static async void GetStopTimes()
        {
            // file stream setups
            Stream fileStream = await FileSystem.OpenAppPackageFileAsync(STOP_TIMES);
            StreamReader reader = new(fileStream);

            // parser
            string content = await reader.ReadToEndAsync();
            string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // For each line
            foreach (string line in lines)
            {
                // Split row
                var fields = line.Split(',');
                StopTime stopTime = new()
                {
                    tripID = fields[0],
                    arrivalTime = fields[1],
                    departureTime = fields[2],
                    stopID = fields[3],
                    stopSequence = fields[4]
                };

                Instance.StopTimes.Add(stopTime);
            }
        }

        private static async void GetTrips()
        {
            // file stream setups
            Stream fileStream = await FileSystem.OpenAppPackageFileAsync(TRIPS);
            StreamReader reader = new(fileStream);

            // parser
            string content = await reader.ReadToEndAsync();
            string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // For each line
            foreach (string line in lines)
            {
                // Split row
                var fields = line.Split(',');
                Trip trip = new()
                {
                    routeID = fields[0],
                    serviceID = fields[1],
                    tripID = fields[2],
                    tripHeadsign = fields[3],
                    directionID = fields[4],
                    blockID = fields[5],
                    shapeID = fields[6]
                };

                Instance.Trips.Add(trip);
            }
        }

        private Trip GetTrip(string tripID)
        {
            foreach (Trip trip in Trips)
            {
                if (trip.tripID == tripID)
                {
                    return trip;
                }
            }

            return new Trip();
        }

        public List<Trip> GetNextThreeTripsByStopID(string stopID) {
            DateTime time = DateTime.Now;
            List<StopTime> stopTimes = new();
            List<Trip> trips = new();


            foreach (StopTime stopTime in StopTimes) {
                if (stopTime.stopID == stopID) {
                    string[] timeParts = stopTime.arrivalTime.Split(":");
                    if (int.Parse(timeParts[0]) > 23) timeParts[0] = (int.Parse(timeParts[0]) - 24).ToString();
                    string hoursMinutes = timeParts[0] + ":" + timeParts[1];

                    DateTime targetTime = DateTime.Today.Add(TimeSpan.Parse(hoursMinutes));
                    //Debug.WriteLine("Arrival time: " + targetTime + ", Now: " + time);
                    bool future = targetTime > time;
                    //Debug.WriteLine("Future: " + future);

                    if (!future) continue;
                    else stopTimes.Add(stopTime);
                }
            }

            if (stopTimes.Count == 0) return trips;

            stopTimes = stopTimes.OrderBy(st => DateTime.Parse(st.arrivalTime)).ToList();
            if (stopTimes.Count > 3) {
                for (int i = 0; i < 3; i++) {
                    Trip trip = GetTrip(stopTimes[i].tripID);
                    if (trip.tripID != null) trips.Add(trip);
                }
            }
            else {
                foreach (StopTime stopTime in stopTimes) {
                    Trip trip = GetTrip(stopTime.tripID);
                    if (trip.tripID != null) trips.Add(trip);
                }

            }

            //foreach (Trip trip in trips) {
            //    Debug.WriteLine($"{trip.tripID} - {trip.tripHeadsign}");
            //}
            return trips;

        }

    }
}
