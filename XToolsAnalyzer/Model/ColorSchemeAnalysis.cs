using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses number of times different color schemes were applied.</summary>
    public class ColorSchemeAnalysis : Analysis
    {
        public ColorSchemeAnalysis()
        {
            Name = "Цветовая схема";
            Type = AnalysisType.XToolsSettings;
        }

        /// <summary>Gets the number of times each color scheme was used.</summary>
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
                        Match colorSchemeMatch = Regex.Match(stat.Key, @"(?<=(ColorScheme\.)).*");

                        if (!colorSchemeMatch.Success) { continue; } // Skip if it's not about color schemes.
                        string color = colorSchemeMatch.Value; // Else get the color name.

                        if (!result.Statistics.ContainsKey(color))
                        {
                            // If there was no information about this scheme before, add a container for the info
                            result.Statistics.Add(color, new Dictionary<string, int>());
                        }
                        if (!result.Statistics[color].ContainsKey("Кол-во использований"))
                        {
                            // Also add a container for the info about the scheme usage
                            result.Statistics[color].Add("Кол-во использований", 0);
                        }

                        // Add the new value to the old info.
                        result.Statistics[color]["Кол-во использований"] += int.Parse(stat.Value);
                    }
                }
            }

            return result;
        }
    }
}
