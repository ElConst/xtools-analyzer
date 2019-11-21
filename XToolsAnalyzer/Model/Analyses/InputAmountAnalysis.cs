using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses amount of objects on input of tools.</summary>
    class InputAmountAnalysis : AvgAnalysis
    {
        public InputAmountAnalysis()
        {
            Type = AnalysisType.ToolsStats;
            Name = "Кол-во объектов на входе";
            AverageStatisticName = "Среднее кол-во объектов на входе";
            
            SelectedGrouping = Groupings[0];
        }

        /// <summary>A grouping mode identifier.</summary>
        private static string ToolGrouping = "По инструментам";

        private string[] groupings = { ToolGrouping };
        /// <summary>Data grouping modes available for the analysis.</summary>
        public override string[] Groupings => groupings;

        /// <summary>An identifier for a data series.</summary>
        private static string
            minSeriesIdentifier = "Минимальное кол-во объектов на входе",
            maxSeriesIdentifier = "Максимальное кол-во объектов на входе";

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

            // If there was no information about average input of this tool before, add a container for it.
            if (!statsAverageData.ContainsKey(tool.ToolName)) { statsAverageData.Add(tool.ToolName, new StatAverageInfo()); }
            StatAverageInfo averageInfo = statsAverageData[tool.ToolName];

            // Add info to count average
            averageInfo.StatSum += objAmountFromReport;
            averageInfo.ReportsCount++;

            // If there was no information about min/max input of this tool before, add containers for it
            if (!stats.ContainsKey(tool.ToolName))
            {
                stats.Add(tool.ToolName, new Dictionary<string, int>());

                stats[tool.ToolName].Add(minSeriesIdentifier, objAmountFromReport);
                stats[tool.ToolName].Add(maxSeriesIdentifier, objAmountFromReport);
            }
            else 
            {
                // Check if current min is greater than this new value and set this value as min if it is
                if (stats[tool.ToolName][minSeriesIdentifier] > objAmountFromReport)
                {
                    stats[tool.ToolName][minSeriesIdentifier] = objAmountFromReport;
                }
                // Check if current max is lower than this new value and set this value as max if it is
                else if (stats[tool.ToolName][maxSeriesIdentifier] < objAmountFromReport)
                {
                    stats[tool.ToolName][maxSeriesIdentifier] = objAmountFromReport;
                }
            }
        }
    }
}
