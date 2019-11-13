using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses the amount of tools calls.</summary>
    public class ClicksAnalysis : Analysis
    {
        public ClicksAnalysis()
        {
            Type = AnalysisType.ToolsStats;
            Name = "Кол-во запусков";
            
            SelectedGrouping = Groupings[0];
        }

        private static string ToolGrouping = "По инструментам";

        private string[] groupings =
        {
            ToolGrouping
        };
        public override string[] Groupings => groupings;

        /// <summary>Checks how many times a tool was called within one report.</summary>
        protected override void ProcessToolUsageData(ref Dictionary<string, Dictionary<string, int>> stats, StatisticsReport report, ToolUsageData tool)
        {
            // Skip if there's no information about tool calls
            if (!tool.CommandStats.ContainsKey("Click")) { return; }

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
}
