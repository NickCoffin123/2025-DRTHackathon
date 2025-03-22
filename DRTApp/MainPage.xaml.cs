<<<<<<< HEAD
﻿using System.Diagnostics;
using System.Net;
using DRTApp.Classes;
using ProtoBuf;
using TransitRealtime;
using static DRTApp.Classes.GtfsService;
=======
﻿using ProtoBuf;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using TransitRealtime;
>>>>>>> be313355e608d843c8d4775e300de01306ffc072

namespace DRTApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        string vehiclePositionsURL = "https://drtonline.durhamregiontransit.com/gtfsrealtime/TripUpdates";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            //TripUpdates();
            await ReadRoutesFileAsync();

<<<<<<< HEAD
        }
        private void TripUpdates() {
            WebRequest req = HttpWebRequest.Create("https://drtonline.durhamregiontransit.com/gtfsrealtime/TripUpdates");
            FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            foreach (FeedEntity entity in feed.Entities) {
                Debug.WriteLine(entity.TripUpdate.Vehicle);
            }
=======
            WebRequest req = HttpWebRequest.Create(vehiclePositionsURL);
            FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            foreach (FeedEntity entity in feed.Entities)
            {
                Debug.WriteLine(entity.TripUpdate.Vehicle.Id);
            }

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
>>>>>>> be313355e608d843c8d4775e300de01306ffc072
        }
    }

}
