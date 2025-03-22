using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Microsoft.Maui.Storage;
using System.Diagnostics;

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
        private static string STOPS = "C:\\Users\\rjmac\\OneDrive\\Documents\\DC Resources\\2025-DRTHackathon\\DRTApp\\Resources\\Raw\\stops.txt";
        private static string STOP_TIMES = "C:\\Users\\rjmac\\OneDrive\\Documents\\DC Resources\\2025-DRTHackathon\\DRTApp\\Resources\\Raw\\stop_times.txt";
        private static string TRIPS = "C:\\Users\\rjmac\\OneDrive\\Documents\\DC Resources\\2025-DRTHackathon\\DRTApp\\Resources\\Raw\\trips.txt";

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
    }
}
