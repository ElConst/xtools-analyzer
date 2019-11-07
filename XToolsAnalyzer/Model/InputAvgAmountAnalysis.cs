using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    class InputAvgAmountAnalysis : Analysis
    {
        public InputAvgAmountAnalysis()
        {
            Name = "Кол-во объектов на входе (ср.)";
            Type = AnalysisType.ToolsStats;
            SelectedGrouping = Groupings[0];
        }

        private static string
            ToolGrouping = "По инструментам";

        private string[] groupings = { ToolGrouping };
        public override string[] Groupings => groupings;

        private class Tool
        {
            public float InputObjectsAvg;
            public int Reports;
        }

        /// <summary>Gets input objects amount for each tool.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, Tool> toolsInfo = new Dictionary<string, Tool>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    int objAmountFromReport = 0;

                    foreach (var stat in tool.ExecutorStats)
                    {
                        // Make regex match in the statistic name to find out if the statistic contains amount of objects of certain type.
                        Match dataTypeMatch = Regex.Match(stat.Key, $@"InputData\.Type\.");

                        if (!dataTypeMatch.Success) { continue; } // Skip if it's not about data types.
                        objAmountFromReport += int.Parse(stat.Value); // Else get the amount of objects.
                    }

                    if (objAmountFromReport == 0) { continue; }

                    if (!toolsInfo.ContainsKey(tool.ToolName)) { toolsInfo.Add(tool.ToolName, new Tool()); }
                    Tool toolInfo = toolsInfo[tool.ToolName];

                    toolInfo.Reports++;
                    toolInfo.InputObjectsAvg += objAmountFromReport;
                }
            }

            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            foreach (var toolKeyValue in toolsInfo)
            {
                toolKeyValue.Value.InputObjectsAvg /= toolKeyValue.Value.Reports; // Finally calculate the average number.

                stats.Add(toolKeyValue.Key, new Dictionary<string, int>());
                stats[toolKeyValue.Key]["Среднее кол-во объектов на входе"] = (int)toolKeyValue.Value.InputObjectsAvg;
            }

            AnalysisResult result = new AnalysisResult();
            result.Statistics = stats.ToArray();

            SelectedSorting.SortingFunction(ref result);

            return result;
        }
    }
}
