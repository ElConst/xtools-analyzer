using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses the amount of tools calls.</summary>
    public class ClicksAnalysis : Analysis
    {
        public ClicksAnalysis()
        {
            Name = "Кол-во запусков";
            Type = AnalysisType.ToolsStats;
            SelectedGrouping = Groupings[0];
        }

        private static string ToolGrouping = "По инструментам";

        private string[] groupings =
        {
            ToolGrouping
        };
        public override string[] Groupings => groupings;

        /// <summary>Collects info about how many times each tool was called by users within the period.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    // Skip if there's no information about tool calls
                    if (!tool.CommandStats.ContainsKey("Click")) { continue; }

                    // Else get the info
                    int clicksFromReport = int.Parse(tool.CommandStats["Click"]);

                    // Add tool to the dictionary if it wasn't done before
                    if (!stats.ContainsKey(tool.ToolName))
                    {
                        stats.Add(tool.ToolName, new Dictionary<string, int>()); // Add tool
                        stats[tool.ToolName].Add(Name, 0); // Add container for the statistic
                    }

                    stats[tool.ToolName][Name] += clicksFromReport;
                }
            }

            AnalysisResult result = new AnalysisResult();
            result.Statistics = stats.ToArray();

            SelectedSorting.SortingFunction(ref result);

            return result;
        }
    }
}
