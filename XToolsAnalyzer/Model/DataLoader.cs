using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XToolsAnalyzer
{
    public class XToolsStatistics
    {
        public string UserName;

        public string ProductVersion,
            DateSentUtc, StartDateUtc, EndDateUtc;

        public List<XToolUsageData> ToolsUsed = new List<XToolUsageData>();

        public XToolsStatistics(string userName) => UserName = userName;
    }

    public class XToolUsageData
    {
        public string ToolName;

        public int ClicksCount;

        public float OpenTimeAvg;

        public XToolUsageData(string toolName) => ToolName = toolName;
    }

    public static class DataLoader
    {
        /// <summary>
        /// Takes path to the folder where JSONs are located and collects the tools usage statistics for each user
        /// </summary>
        public static List<XToolsStatistics> Load(string jsonRootPath)
        {
            List<XToolsStatistics> allUsersStatistics = new List<XToolsStatistics>(); // List to return

            // Get list of folders each containing statistics about XTools usage by a user
            var userStatisticsPaths = Directory.EnumerateDirectories(jsonRootPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string userPath in userStatisticsPaths)
            {
                string userName = Path.GetFileName(userPath);

                // Get list of .json files inside the folder
                var userJsons = Directory.EnumerateFiles(userPath, "*.json");
                foreach (string json in userJsons)
                {
                    XToolsStatistics stats = new XToolsStatistics(userName); // Will contain info from one JSON file

                    
                    JObject rootJObj = JObject.Parse(File.ReadAllText(json)); // The whole JSON in a JObject

                    stats.ProductVersion = rootJObj.GetValue("ProductVersion").ToString();

                    // Get list of tools from the value of "Items" property
                    IEnumerable<JProperty> toolsUsed = rootJObj.GetValue("Items").ToObject<JObject>().Properties();
                    // Collect statistics from each tool
                    foreach (var toolJProp in toolsUsed)
                    {
                        // Will contain statistics for one XTool used
                        XToolUsageData toolData = new XToolUsageData(toolJProp.Name);

                        JObject toolJObj = toolJProp.Value.ToObject<JObject>();
                        JObject commandJObj = toolJObj.GetValue("Command").ToObject<JObject>();

                        toolData.ClicksCount = Convert.ToInt32(commandJObj.GetValue("Click"));

                        JObject uiJObj = toolJObj.GetValue("UI").ToObject<JObject>();

                        toolData.OpenTimeAvg = (float)Convert.ToDouble(uiJObj.GetValue("OpenTime.Avg"));

                        stats.ToolsUsed.Add(toolData);
                    }

                    allUsersStatistics.Add(stats);
                }
            }

            return allUsersStatistics;
        }
    }
}
