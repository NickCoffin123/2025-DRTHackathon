using ProtoBuf;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using TransitRealtime;

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

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

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
        }
    }

}
