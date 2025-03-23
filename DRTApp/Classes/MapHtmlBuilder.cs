using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using System.Diagnostics;

namespace DRTApp.Classes
{
    internal class MapHtmlBuilder
    {
        private static string HTML_DIR = "index.html";

        public static async Task<HtmlWebViewSource> GrabHtml(sStop stop, List<string> busPositions)
        {
            // Setup reader
            Stream fileStream = await FileSystem.OpenAppPackageFileAsync(HTML_DIR);
            StreamReader reader = new(fileStream);

            // Read file contents, return html source
            string htmlContent = await reader.ReadToEndAsync();
            return new HtmlWebViewSource
            {
                Html = InjectJSON(htmlContent, stop, busPositions),
                BaseUrl = HTML_DIR
            };
        }

        public static string InjectJSON(string htmlContent, sStop stop, List<string> busPositions)
        {
            // anonymous object for json
            var data = new
            {
                stopData = stop,
                busPositionsData = busPositions
            };

            // Add JSON of stop data to html source
            return htmlContent.Replace("const data = {}", $"const data={JsonSerializer.Serialize(data, new JsonSerializerOptions { IncludeFields = true })}");

        }
    }
}
