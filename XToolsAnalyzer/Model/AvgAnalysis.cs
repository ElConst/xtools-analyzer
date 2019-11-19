using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Parent for analyses which count an average number of some statistic.</summary>
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
            // Here we will store preliminary data to be converted in a normal collection of statistics later
            Dictionary<string, ToolAvgStat> toolsStatInfo = new Dictionary<string, ToolAvgStat>();

            // Go through all tools in all reports and collect statistics sum
            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    ProcessToolUsageData(ref toolsStatInfo, report, tool);
                }
            }

            // A collection where to put the statistics
            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            foreach (var toolKeyValue in toolsStatInfo)
            {
                // Finally calculate the average number.
                toolKeyValue.Value.AvgStat /= toolKeyValue.Value.ReportsCount;

                // Add this new value to the resulting collection
                stats.Add(toolKeyValue.Key, new Dictionary<string, int>());
                stats[toolKeyValue.Key][StatisticName] = (int)toolKeyValue.Value.AvgStat;
            }

            // Place the resulting dictionary inside an AnalysisResult (here it gets converted into a more convenient way of data presentation)
            AnalysisResult result = new AnalysisResult(stats);

            return result;
        }

        /// <summary>Collect some statistics in single ToolUsageData.</summary>
        protected abstract void ProcessToolUsageData(ref Dictionary<string, ToolAvgStat> toolsStatInfo, StatisticsReport report, ToolUsageData tool);

        // Actually this function is a duct tape just to implement parent's functionality.
        protected override void ProcessToolUsageData(ref Dictionary<string, Dictionary<string, int>> stats, StatisticsReport report, ToolUsageData tool) { }
    }
}
