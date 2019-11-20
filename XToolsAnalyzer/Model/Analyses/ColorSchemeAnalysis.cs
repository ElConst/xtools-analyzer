using System.Collections.Generic;
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

        /// <summary>A grouping mode identifier.</summary>
        private static string ThemeGrouping = "По схемам";

        private string[] groupings = { ThemeGrouping };
        /// <summary>Data grouping modes available for the analysis.</summary>
        public override string[] Groupings => groupings;

        /// <summary>Checks how many times a color scheme was applied within a report.</summary>
        protected override void ProcessToolUsageData(StatisticsReport report, ToolUsageData tool)
        {
            foreach (var stat in tool.ExecutorStats)
            {
                // Make regex match in the statistic name to find out if the it's what we are looking for.
                Match colorSchemeMatch = Regex.Match(stat.Key, @"(?<=(ColorScheme\.)).*");

                if (!colorSchemeMatch.Success) { continue; } // Skip if it's not about color schemes.
                string color = colorSchemeMatch.Value; // Else get the color name.

                // If there was no information about this scheme before, add a container for the info
                if (!stats.ContainsKey(color)) { stats.Add(color, new Dictionary<string, int>()); }

                // Also add a container for the info about the scheme usage
                if (!stats[color].ContainsKey("Кол-во использований")) { stats[color].Add("Кол-во использований", 0); }

                // Add the new value to the old info.
                stats[color]["Кол-во использований"] += int.Parse(stat.Value);
            }
        }
    }
}
