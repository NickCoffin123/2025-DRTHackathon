using System.Diagnostics;
using System.Net;
using DRTApp.Classes;
using ProtoBuf;
using TransitRealtime;
using static DRTApp.Classes.GtfsService;

namespace DRTApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            //TripUpdates();
            await ReadRoutesFileAsync();

        }
        private void TripUpdates() {
            WebRequest req = HttpWebRequest.Create("https://drtonline.durhamregiontransit.com/gtfsrealtime/TripUpdates");
            FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
            foreach (FeedEntity entity in feed.Entities) {
                Debug.WriteLine(entity.TripUpdate.Vehicle);
            }
        }
    }

}
