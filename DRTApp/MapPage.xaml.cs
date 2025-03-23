using DRTApp.Classes;
using static System.Formats.Asn1.AsnWriter;
namespace DRTApp;

using System.Diagnostics;
using System.Text.Json;

public partial class MapPage : ContentPage
{
	public MapPage(sStop stop, List<string> busPositions)
	{
		InitializeComponent();
		InitializeMap(stop, busPositions);
    }

	public async void InitializeMap(sStop stop, List<string> busPositions)
	{
		// Grab HTML source
        WebMapArea.Source = await MapHtmlBuilder.GrabHtml(stop, busPositions);
	}
}