using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    class InputAvgAmountAnalysis : AvgAnalysis
    {
        public InputAvgAmountAnalysis()
        {
            Type = AnalysisType.ToolsStats;
            Name = "Кол-во объектов на входе (ср.)";
            StatisticName = "Среднее кол-во объектов на входе";
            
            SelectedGrouping = Groupings[0];
        }

        private static string
            ToolGrouping = "По инструментам";

        private string[] groupings = { ToolGrouping };
        public override string[] Groupings => groupings;

        protected override void ProcessToolUsageData(ref Dictionary<string, ToolAvgStat> toolsStatInfo, StatisticsReport report, ToolUsageData tool)
        {
            int objAmountFromReport = 0;

            foreach (var stat in tool.ExecutorStats)
            {
                // Make regex match in the statistic name to find out if the statistic contains amount of objects of certain type.
                Match dataTypeMatch = Regex.Match(stat.Key, $@"InputData\.Type\.");

                if (!dataTypeMatch.Success) { continue; } // Skip if it's not about data types.
                objAmountFromReport += int.Parse(stat.Value); // Else get the amount of objects.
            }

            if (objAmountFromReport == 0) { return; }

            if (!toolsStatInfo.ContainsKey(tool.ToolName)) { toolsStatInfo.Add(tool.ToolName, new ToolAvgStat()); }
            ToolAvgStat toolInfo = toolsStatInfo[tool.ToolName];

            toolInfo.ReportsCount++;
            toolInfo.AvgStat += objAmountFromReport;
        }
    }
}
