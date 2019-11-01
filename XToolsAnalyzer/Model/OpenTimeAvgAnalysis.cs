using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses how much time on average each tool needs for one launch.</summary>
    public class OpenTimeAvgAnalysis : Analysis
    {
        public OpenTimeAvgAnalysis()
        {
            Name = "Время работы";
            Type = AnalysisType.ToolsStats;
            SelectedGrouping = Groupings[0];
        }

        private static string
            ToolGrouping = "По инструментам";

        private string[] groupings = { ToolGrouping };
        public override string[] Groupings => groupings;

        private class Tool
        {
            public float OpenTimeAvg;
            public int Reports;
        }

        /// <summary>Collects info about average open time for each tool.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, Tool> toolsInfo = new Dictionary<string, Tool>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    // Skip if there's no information about open time
                    if (!tool.UIStats.ContainsKey("OpenTime.Avg")) { continue; }

                    // Else get the info
                    float avgTimeFromReport = float.Parse(tool.UIStats["OpenTime.Avg"]);

                    if (!toolsInfo.ContainsKey(tool.ToolName)) { toolsInfo.Add(tool.ToolName, new Tool()); }
                    Tool toolInfo = toolsInfo[tool.ToolName];

                    toolInfo.Reports++;
                    toolInfo.OpenTimeAvg += avgTimeFromReport;
                }
            }

            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            foreach (var toolKeyValue in toolsInfo)
            {
                toolKeyValue.Value.OpenTimeAvg /= toolKeyValue.Value.Reports; // Finally calculate the average number.

                stats.Add(toolKeyValue.Key, new Dictionary<string, int>());
                stats[toolKeyValue.Key]["Среднее время работы(мс)"] = (int)toolKeyValue.Value.OpenTimeAvg;
            }

            AnalysisResult result = new AnalysisResult();
            result.Statistics = stats.ToArray();

            SelectedSorting.SortingFunction(ref result);

            return result;
        }
    }
}
