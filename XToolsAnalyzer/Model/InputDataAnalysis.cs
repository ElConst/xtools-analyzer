using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses amount and types of tools input data.</summary>
    public class InputDataAnalysis : Analysis
    {
        public InputDataAnalysis() => Name = "Входные данные";

        /// <summary>Gets amount of uses of objects of different data types by each tool</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            AnalysisResult result = new AnalysisResult(); // Value to return

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    foreach (var stat in tool.ExecutorStats)
                    {
                        // Make regex match in the tool name to find out if the statistic is what we are looking for.
                        Match dataTypeMatch = Regex.Match(stat.Key, @"(?<=(InputData\.Type\.)).*");

                        if (!dataTypeMatch.Success) { continue; } // Skip if it's not about input data types.
                        string dataType = dataTypeMatch.Value; // Else get the type name.

                        if (!result.Statistics.ContainsKey(tool.ToolName))
                        {
                            // If there was no information about this tool before, add a container for the info
                            result.Statistics.Add(tool.ToolName, new Dictionary<string, int>());
                        }
                        if (!result.Statistics[tool.ToolName].ContainsKey(dataType))
                        {
                            // Also add a container for the info about the type
                            result.Statistics[tool.ToolName].Add(dataType, int.Parse(stat.Value));
                        }
                        else
                        {
                            // If there already is a container, just sum old and new values.
                            result.Statistics[tool.ToolName][dataType] += int.Parse(stat.Value);
                        }
                    }
                }
            }

            return result;
        }
    }
}
