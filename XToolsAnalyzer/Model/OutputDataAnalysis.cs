﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses amount and types of tools output data.</summary>
    public class OutputDataAnalysis : Analysis
    {
        public OutputDataAnalysis() => Name = "Выходные данные";

        /// <summary>Gets amount of objects of different data types in the output of each tool</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            AnalysisResult result = new AnalysisResult(true); // Value to return

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    foreach (var stat in tool.ExecutorStats)
                    {
                        // Make regex match in the tool name to find out if the statistic is what we are looking for.
                        Match dataTypeMatch = Regex.Match(stat.Key, @"(?<=(OutputData\.Type\.)).*");

                        if (!dataTypeMatch.Success) { continue; } // Skip if it's not about output data types.
                        string dataType = dataTypeMatch.Value; // Else get the type name.

                        if (!result.ToolsStatistics.ContainsKey(tool.ToolName))
                        {
                            // If there was no information about this tool before, add a container for the info
                            result.ToolsStatistics.Add(tool.ToolName, new Dictionary<string, int>());
                        }
                        if (!result.ToolsStatistics[tool.ToolName].ContainsKey(dataType))
                        {
                            // Also add a container for the info about the type
                            result.ToolsStatistics[tool.ToolName].Add(dataType, int.Parse(stat.Value));
                        }
                        else
                        {
                            // If there already is a container, just sum old and new values.
                            result.ToolsStatistics[tool.ToolName][dataType] += int.Parse(stat.Value);
                        }
                    }
                }
            }

            return result;
        }
    }
}