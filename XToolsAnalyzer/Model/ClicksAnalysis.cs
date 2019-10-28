using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses the amount of tools calls.</summary>
    public class ClicksAnalysis : Analysis
    {
        public ClicksAnalysis() => Name = "Кол-во запусков";

        /// <summary>Collects info about how many times each tool was called by users within the period.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            AnalysisResult result = new AnalysisResult(); // Value to return

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    // Skip if there's no information about tool calls
                    if (!tool.CommandStats.ContainsKey("Click")) { continue; }

                    // Else get the info
                    int clicksFromReport = int.Parse(tool.CommandStats["Click"]);

                    // Add tool to the dictionary if it wasn't done before
                    if (!result.Statistics.ContainsKey(tool.ToolName))
                    {
                        result.Statistics.Add(tool.ToolName, new Dictionary<string, int>()); // Add tool
                        result.Statistics[tool.ToolName].Add(Name, 0); // Add container for the statistic
                    }

                    result.Statistics[tool.ToolName][Name] += clicksFromReport;
                }
            }

            return result;
        }
    }
}
