using System.Diagnostics;
using System.Net;
using ProtoBuf;
using TransitRealtime;
using DRTApp.Classes;

namespace DRTApp
{
    public partial class MainPage : ContentPage
    {
        //CONSTS
        string TRIP_UPDATES_URL = "https://drtonline.durhamregiontransit.com/gtfsrealtime/TripUpdates";

        // COMPONENTS
        RawResourceHandler res;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            string stopID = myEntry.Text;
            ValidateStopID(stopID);

        }

        private bool ValidateStopID(string stopID)
        {
            List<sStop> stops = RawResourceHandler.Instance.Stops;
            foreach (sStop stop in stops)
            {
                if (stop.stopID == stopID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
