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
        private static string STOPS = "Resources\\Raw\\stops.txt";
        private static string STOP_TIMES = "Resources\\Raw\\stop_times.txt";
        private static string TRIPS = "Resources\\Raw\\trips.txt";

        // PROPS
        public List<sStop> Stops { get; set; }
        public List<StopTime> StopTimes { get; set; }
        public List<Trip> Trips { get; set; }

        // SINGLETON
        private static RawResourceHandler _instance;
        public static RawResourceHandler Instance => _instance ??= new RawResourceHandler();

        // ***********************************************
        //                  CONSTRUCTION
        // ***********************************************
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

        // ***********************************************
        //                      MISC
        // ***********************************************
        public float GetSafeTimeParsed(string time)
        {
            // Split the string by ':'
            string[] timeParts = time.Split(':');

            // Convert to floats
            float hours = float.Parse(timeParts[0]);
            float minutes = float.Parse(timeParts[1]);
            float seconds = float.Parse(timeParts[2]);

            float outTime = .0f;

            if (hours < 2)
            {
                outTime += 3600 * 24;
            }

            outTime += hours * 3600;
            outTime += minutes * 60;
            outTime += seconds;

            return outTime;
        }

        public bool IsTimeInFuture(string time)
        {
            string nowStr = DateTime.Now.ToString("HH:mm:ss");
            float fnow = GetSafeTimeParsed(nowStr);
            float ftime = GetSafeTimeParsed(time);

            return fnow < ftime;
        }

        // ***********************************************
        //                  STOPS
        // ***********************************************
        public sStop GetStop(string stopID)
        {
            foreach (sStop stop in Stops)
            {
                if (stop.stopID == stopID)
                {

                    //Debug.WriteLine(stop.stopID);
                    return stop;
                }
            }

            sStop nullS = new sStop();
            nullS.stopID = "FAILED";

            return nullS;
        }

        public bool ValidateStopID(string stopID)
        {
            foreach (sStop stop in Stops)
            {
                if (stop.stopID == stopID)
                {
                    return true;
                }
            }

            return false;
        }

        // ***********************************************
        //                  STOP TIMES
        // ***********************************************
        public List<StopTime> GetStopTimes(string stopID, string tripID)
        {
            List<StopTime> times = new();

            foreach (StopTime st in StopTimes)
            {
                if (st.stopID == stopID && st.tripID == tripID)
                {

                    //Debug.WriteLine(st.tripID + " | " + st.stopID);
                    times.Add(st);
                }
            }

            return times;
        }


        // ***********************************************
        //                  TRIPS
        // ***********************************************
        public Trip GetTrip(string tripID)
        {
            foreach (Trip trip in Trips)
            {
                if (trip.tripID == tripID)
                {

                    //Debug.WriteLine(trip.tripID);
                    return trip;
                }
            }

            Trip nullT = new Trip();
            nullT.tripID = "FAILED";

            return nullT;
        }

        public List<Trip> GetNextThreeTripsByStopID(string stopID) {
            DateTime time = DateTime.Now;
            List<StopTime> stopTimes = new();
            List<Trip> trips = new();


            foreach (StopTime st in StopTimes) {
                if (st.stopID == stopID) {
                    if (IsTimeInFuture(st.arrivalTime)) stopTimes.Add(st);
                    else continue;
                }
            }

            stopTimes = stopTimes.OrderBy(st => GetSafeTimeParsed(st.arrivalTime)).ToList();
            for (int i = 0; i < 3; i++) {
                Trip trip = GetTrip(stopTimes[i].tripID);
                if (trip.tripID != "FAILED") trips.Add(trip);
            }

            return trips;
        }

    }
}
