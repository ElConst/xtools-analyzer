using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

                StatisticsReport stats = new StatisticsReport(); // Will contain info from one JSON file

                JObject rootJObj = JObject.Parse(json); // The whole JSON in a JObject

                string productVersion = rootJObj.GetValue("ProductVersion")?.ToString();
                if (productVersion == null) { productVersion = rootJObj.GetValue("Version")?.ToString(); }

                stats.ProductVersion = productVersion;

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

                    stats.ToolsUsed.Add(toolData);
                }

                reports.Add(stats);
            }

            return reports;
        }
    }
}
