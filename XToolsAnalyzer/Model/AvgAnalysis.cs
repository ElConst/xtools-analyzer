using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Parent for analyses which count some statistic's average number.</summary>
    public abstract class AvgAnalysis : Analysis
    {
        /// <summary>Russian name for the statistic analysed.</summary>
        public string StatisticName;

        /// <summary>Contains statistic sum and count of reports where the statistic was collected.</summary>
        protected class ToolAvgStat
        {
            public float AvgStat;
            public int ReportsCount;
        }

        /// <summary>Does the analysis and returns a result.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, ToolAvgStat> toolsStatInfo = new Dictionary<string, ToolAvgStat>();

            // Collect data
            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    ProcessToolUsageData(ref toolsStatInfo, report, tool);
                }
            }

            // A collection where to put the analysis results
            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            foreach (var toolKeyValue in toolsStatInfo)
            {
                // Finally calculate the average number.
                toolKeyValue.Value.AvgStat /= toolKeyValue.Value.ReportsCount;

                // Add this new value to the result
                stats.Add(toolKeyValue.Key, new Dictionary<string, int>());
                stats[toolKeyValue.Key][StatisticName] = (int)toolKeyValue.Value.AvgStat;
            }

            AnalysisResult result = new AnalysisResult();
            result.Statistics = stats.ToArray();

            SelectedSorting.SortingFunction(ref result);

            return result;
        }

        /// <summary>Collect some statistics in single ToolUsageData.</summary>
        protected abstract void ProcessToolUsageData(ref Dictionary<string, ToolAvgStat> toolsStatInfo, StatisticsReport report, ToolUsageData tool);

        // Actually this function is a duct tape just to implement parent's functionality.
        protected override void ProcessToolUsageData(ref Dictionary<string, Dictionary<string, int>> stats, StatisticsReport report, ToolUsageData tool) { }
    }
}
