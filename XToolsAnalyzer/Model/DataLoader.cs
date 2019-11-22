using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    /// <summary>Handles data load and its primary processing.</summary>
    public static class DataLoader
    {
        /// <summary>By default if data is being loaded locally from the computer it will be loaded from this folder.</summary>
        public static string DefaultFolderToLoad { get; set; }

        /// <summary>Collects statistics for each user from JSONs inside the default folder</summary>
        /// <returns>Statistics collected from JSONs</returns>
        public static List<StatisticsReport> LoadFromFolder() => LoadFromFolder(DefaultFolderToLoad);

        /// <summary>
        /// Collects statistics for each user from JSONs inside a folder
        /// </summary>
        /// <param name="jsonsRootPath">Path to a folder with JSONs</param>
        /// <returns>Statistics collected from JSONs</returns>
        public static List<StatisticsReport> LoadFromFolder(string jsonsRootPath)
        {
            List<string> jsons = new List<string>();

            // Get list of .json files inside the folder and nested ones
            var userJsonFiles = Directory.EnumerateFiles(jsonsRootPath, "*.json", SearchOption.AllDirectories);
            // Read the content of each json into a string
            var jsonsСontent = userJsonFiles.Select(path => File.ReadAllText(path));

            return GetInfoFromJsons(jsonsСontent.ToList());
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

                // Get dates from string
                DateTime startDate = DateTime.Parse(startDateStr), 
                         endDate = DateTime.Parse(endDateStr);

                // Skip if the filter tells not to show statistics from this product.
                if (
                    // Check if the filter have already been set
                    // Because if not, it's being set at the moment and we don't want to use it unprepared
                    Filter.Instance != null &&
                    
                    // Check if the product of the report doesn't fit the filter
                    ((productName == "XTools Pro" && !Filter.Instance.ShowXToolsPro ||
                     productName == "XTools AGP" && !Filter.Instance.ShowXToolsAgp) ||

                     // Check if the date of the statistics collection doesn't fit the filter
                     (!Filter.Instance.IsDateSuitable(startDate) ||
                     !Filter.Instance.IsDateSuitable(endDate))))
                {
                    continue;
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
                    // Skip if the filter tells not to show info about the tool.
                    if (Filter.Instance != null && !Filter.Instance.ToolsFilter[toolJProp.Name]) { continue; }

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
