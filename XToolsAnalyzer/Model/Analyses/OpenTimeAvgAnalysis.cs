﻿using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses how much time on average each tool needs for one launch.</summary>
    public class OpenTimeAvgAnalysis : AvgAnalysis
    {
        public OpenTimeAvgAnalysis()
        {
            Type = AnalysisType.ToolsStats;
            Name = "Время работы";
            StatisticName = "Среднее время работы(мс)";

            SelectedGrouping = Groupings[0];
        }

        /// <summary>A grouping mode identifier.</summary>
        private static string ToolGrouping = "По инструментам";

        private string[] groupings = { ToolGrouping };
        /// <summary>Data grouping modes available for the analysis.</summary>
        public override string[] Groupings => groupings;

        /// <summary>Collects tool average open time from a report.</summary>
        protected override void ProcessToolUsageData(ref Dictionary<string, ToolAvgStat> toolsStatInfo, StatisticsReport report, ToolUsageData tool)
        {
            // Skip if there's no information about open time
            if (!tool.UIStats.ContainsKey("OpenTime.Avg")) { return; }

            // Else get the info
            float avgTimeFromReport = float.Parse(tool.UIStats["OpenTime.Avg"]);

            // If there was no information about open time of this tool before, add a container for it.
            if (!toolsStatInfo.ContainsKey(tool.ToolName)) { toolsStatInfo.Add(tool.ToolName, new ToolAvgStat()); }
            ToolAvgStat toolInfo = toolsStatInfo[tool.ToolName];

            // Add info
            toolInfo.ReportsCount++;
            toolInfo.AvgStat += avgTimeFromReport;
        }
    }
}
