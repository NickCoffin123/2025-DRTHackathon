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
            if (int.TryParse(myEntry.Text, out int stopID))
            {
                if (ValidateStopID(stopID))
                {

                }
            }

            else
            {
                Debug.WriteLine("Invalid stop ID");
            }

        }

        private bool ValidateStopID(int stopID)
        {
            List<sStop> stops = RawResourceHandler.Instance.Stops;

            return true;
        }
    }
}
