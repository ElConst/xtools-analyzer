using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses number of times different color schemes were applied.</summary>
    public class ColorSchemeAnalysis : Analysis
    {
        public ColorSchemeAnalysis()
        {
            Type = AnalysisType.XToolsSettings;
            Name = "Цветовая схема";
            
            SelectedGrouping = Groupings[0];
        }

        private static string ThemeGrouping = "По схемам";

        private string[] groupings =
        {
            ThemeGrouping
        };
        public override string[] Groupings => groupings;

        /// <summary>Checks how many times a color scheme was applied within a report.</summary>
        protected override void ProcessToolUsageData(ref Dictionary<string, Dictionary<string, int>> stats, StatisticsReport report, ToolUsageData tool)
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
}
