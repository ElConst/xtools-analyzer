using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses how much time on average each tool needs for one launch.</summary>
    public class OpenTimeAvgAnalysis : Analysis
    {
        public OpenTimeAvgAnalysis() => Name = "Время работы";

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

            AnalysisResult result = new AnalysisResult();

            foreach (var toolKeyValue in toolsInfo)
            {
                toolKeyValue.Value.OpenTimeAvg /= toolKeyValue.Value.Reports; // Finally calculate the average number.

                // Add it to the result.
                result.ToolsStatistics.Add(toolKeyValue.Key, new Dictionary<string, int>());
                result.ToolsStatistics[toolKeyValue.Key]["Среднее время работы(мс)"] = (int)toolKeyValue.Value.OpenTimeAvg;
            }

            return result;
        }
    }
}
