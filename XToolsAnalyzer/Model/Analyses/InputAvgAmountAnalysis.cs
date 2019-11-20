﻿using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    class InputAvgAmountAnalysis : AvgAnalysis
    {
        public InputAvgAmountAnalysis()
        {
            Type = AnalysisType.ToolsStats;
            Name = "Кол-во объектов на входе (ср.)";
            AverageStatisticName = "Среднее кол-во объектов на входе";
            
            SelectedGrouping = Groupings[0];
        }

        /// <summary>A grouping mode identifier.</summary>
        private static string ToolGrouping = "По инструментам";

        private string[] groupings = { ToolGrouping };
        /// <summary>Data grouping modes available for the analysis.</summary>
        public override string[] Groupings => groupings;

        /// <summary>Collects info about amounts of data on the input of a tool for one report.</summary>
        protected override void ProcessToolUsageData(StatisticsReport report, ToolUsageData tool)
        {
            int objAmountFromReport = 0;

            // Go through all statisticts of the tool
            foreach (var stat in tool.ExecutorStats)
            {
                // Make regex match in the statistic name to find out if it contains amount of objects of certain type.
                Match dataTypeMatch = Regex.Match(stat.Key, $@"InputData\.Type\.");

                if (!dataTypeMatch.Success) { continue; } // Skip if it's not about data types.
                objAmountFromReport += int.Parse(stat.Value); // Else get the amount of objects.
            }

            // Exit if there's no input objects here
            if (objAmountFromReport == 0) { return; }

            // If there was no information about input of this tool before, add a container for it.
            if (!statsAverageData.ContainsKey(tool.ToolName)) { statsAverageData.Add(tool.ToolName, new StatAverageInfo()); }
            StatAverageInfo averageInfo = statsAverageData[tool.ToolName];

            // Add info
            averageInfo.StatSum += objAmountFromReport;
            averageInfo.ReportsCount++;
        }
    }
}
