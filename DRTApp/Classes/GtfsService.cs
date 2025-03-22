using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using CsvHelper.Configuration;

namespace DRTApp.Classes {
    internal class GtfsService {

        public static async Task ReadRoutesFileAsync() {
            try {
                // Path to routes.txt in resources (works across platforms)
                string fileName = "\\Resources\\Raw\\stops.txt";

                // Attempt to open the file from the app's package
                using (Stream fileStream = await FileSystem.OpenAppPackageFileAsync(fileName)) {
                    Debug.WriteLine("✅ File opened from resources.");

                    using (StreamReader reader = new(fileStream)) {
                        string content = await reader.ReadToEndAsync();
                        Debug.WriteLine($"🔍 Content of the file: {content.Substring(0, Math.Min(100, content.Length))}");
                    }
                }
            }
            catch (Exception ex) {
                Debug.WriteLine($"🚨 ERROR: {ex.Message}");
            }
        }




    }
}
