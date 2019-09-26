using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Collects info about how many times the tool was called by users within the period.</summary>
    public class ClicksAnalysis : IAnalysis
    {
        // Singleton
        private static ClicksAnalysis instance;
        public static ClicksAnalysis Instance
        {
            get
            {
                if (instance == null) { instance = new ClicksAnalysis(); }
                return instance;
            }
        }

        /// <summary>Russian name.</summary>
        public string Name { get; } = "Кол-во запусков";

        /// <summary>Makes the actual analysis</summary>
        /// <returns>Analysis results in a dictionary(Tool name -> Times clicked)</returns>
        public Dictionary<string, float> GetAnalysisResult()
        {
            Dictionary<string, float> toolsClicks = new Dictionary<string, float>(); // Result to return

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    // Skip if there's no information about tool calls
                    if (!tool.CommandStats.ContainsKey("Click")) { continue; }

                    // Else get the info
                    int clicksFromReport = int.Parse(tool.CommandStats["Click"]);

                    if (toolsClicks.ContainsKey(tool.ToolName))
                    {
                        // Add the info from the report if there is any info for the tool already
                        toolsClicks[tool.ToolName] += clicksFromReport;
                    }
                    else
                    {
                        // Else create key for the tool and set info from the report as a value for this key
                        toolsClicks.Add(tool.ToolName, clicksFromReport);
                    }
                }
            }

            return toolsClicks;
        }
    }
}
