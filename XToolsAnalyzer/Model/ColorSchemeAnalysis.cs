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
            SelectedGrouping = Groupings[0];
        }

        private static string ThemeGrouping = "По схемам";

        private string[] groupings =
        {
            ThemeGrouping
        };
        public override string[] Groupings => groupings;

        /// <summary>Gets the number of times each color scheme was used.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

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

                        if (!stats.ContainsKey(color))
                        {
                            // If there was no information about this scheme before, add a container for the info
                            stats.Add(color, new Dictionary<string, int>());
                        }
                        if (!stats[color].ContainsKey("Кол-во использований"))
                        {
                            // Also add a container for the info about the scheme usage
                            stats[color].Add("Кол-во использований", 0);
                        }

                        // Add the new value to the old info.
                        stats[color]["Кол-во использований"] += int.Parse(stat.Value);
                    }
                }
            }

            AnalysisResult result = new AnalysisResult();
            result.Statistics = stats.ToArray();

            SelectedSorting.SortingFunction(ref result);

            return result;
        }
    }
}
