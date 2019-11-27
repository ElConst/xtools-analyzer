using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Handles data load and its primary processing.</summary>
    public static class DataLoader
    {
        /// <summary>A data collection got from JSONs.</summary>
        public static List<StatisticsReport> Data { get; private set; }

        /// <summary>Collects statistics from JSON reports inside a folder.</summary> 
        /// <param name="jsonsRootPath">Path to a folder with JSONs.</param>
        public static void LoadFromFolder(string jsonsRootPath)
        {
            List<string> jsons = new List<string>();

            // Get list of .json files inside the folder and nested ones
            var userJsonFiles = Directory.EnumerateFiles(jsonsRootPath, "*.json", SearchOption.AllDirectories);
            // Read the content of each json into a string
            var jsonsСontent = userJsonFiles.Select(path => File.ReadAllText(path));

            Data = GetInfoFromJsons(jsonsСontent.ToList());
        }

        /// <summary>Collects statistics for each user from JSONs given.</summary>
        private static List<StatisticsReport> GetInfoFromJsons(List<string> jsons)
        {
            List<StatisticsReport> reports = new List<StatisticsReport>(); // List to return

            foreach (string json in jsons)
            {
                if (string.IsNullOrEmpty(json)) { continue; }

                StatisticsReport infoFromReport = new StatisticsReport(); // Will contain info from one JSON file

                JObject rootJObj = JObject.Parse(json); // The whole JSON in a JObject

                // Get info about the product where the report came from
                string productName = rootJObj.GetValue("ProductName")?.ToString();
                string productVersion = (rootJObj.GetValue("ProductVersion") ?? rootJObj.GetValue("Version"))?.ToString();

                // Get info about the period when statistics were collected
                string startDateStr = (rootJObj.GetValue("StartDate") ?? rootJObj.GetValue("StartDateUtc"))?.ToString();
                string endDateStr = (rootJObj.GetValue("EndDate") ?? rootJObj.GetValue("EndDateUtc"))?.ToString();

                // Convert dates from string to DateTime

                DateTime startDate, endDate;

                // If a string is in an uncommon format, suppose that the format is "dd.mm.yyyy" and parse exactly from it
                if (!DateTime.TryParse(startDateStr, out startDate))
                {
                    startDate = DateTime.ParseExact(startDateStr, "dd.mm.yyyy", CultureInfo.InvariantCulture);
                }
                if (!DateTime.TryParse(endDateStr, out endDate))
                {
                    endDate = DateTime.ParseExact(endDateStr, "dd.mm.yyyy", CultureInfo.InvariantCulture);
                }

                // Place the info from the report into corresponding containers
                infoFromReport.ProductName = productName;
                infoFromReport.ProductVersion = productVersion;
                infoFromReport.StartDate = startDate;
                infoFromReport.EndDate = endDate;

                // Get list of tools from the value of the "Items" property
                IEnumerable<JProperty> toolsUsed = rootJObj.GetValue("Items").ToObject<JObject>().Properties();
                // Collect statistics from each tool
                foreach (var toolJProp in toolsUsed)
                {
                    // Will contain statistics for one XTool usage within a period
                    ToolUsageData toolData = new ToolUsageData(toolJProp.Name);

                    JObject toolJObj = toolJProp.Value.ToObject<JObject>();

                    JObject commandJObj = toolJObj.GetValue("Command")?.ToObject<JObject>();
                    JObject executorJObj = toolJObj.GetValue("Executor")?.ToObject<JObject>();
                    JObject uiJObj = toolJObj.GetValue("UI")?.ToObject<JObject>();

                    if (commandJObj != null)
                    {
                        foreach (KeyValuePair<string, JToken> property in commandJObj)
                        {
                            toolData.CommandStats.Add(property.Key, property.Value.ToString());
                        }
                    }
                    if (executorJObj != null)
                    {
                        foreach (KeyValuePair<string, JToken> property in executorJObj)
                        {
                            toolData.ExecutorStats.Add(property.Key, property.Value.ToString());
                        }
                    }
                    if (uiJObj != null)
                    {
                        foreach (KeyValuePair<string, JToken> property in uiJObj)
                        {
                            toolData.UIStats.Add(property.Key, property.Value.ToString());
                        }
                    }

                    infoFromReport.ToolsUsed.Add(toolData);
                }

                reports.Add(infoFromReport);
            }

            return reports;
        }
    }
}
