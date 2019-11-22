using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    /// <summary>Analyses data types used by tools</summary>
    public abstract class DataTypeAnalysis : Analysis
    {
        public DataTypeAnalysis()
        {
            Type = AnalysisType.ToolsStats;
            Name = "Тип входных данных";

            SelectedGrouping = Groupings[0];
        }

        /// <summary>A grouping mode identifier.</summary>
        private static string
            ToolGrouping = "По инструментам",
            VersionGrouping = "По версиям";

        private string[] groupings = { ToolGrouping, VersionGrouping };
        /// <summary>Data grouping modes available for the analysis.</summary>
        public override string[] Groupings => groupings;

        /// <summary>Defines if the analysis will take data from the input of tools. Otherwise it will do it from the output.</summary>
        public bool DoesAnalyseInput;

        /// <summary>Gets amount of times objects of different types were used by a tool within a report.</summary>
        protected override void ProcessToolUsageData(StatisticsReport report, ToolUsageData tool)
        {
            // Go through all statisticts of the tool
            foreach (var stat in tool.ExecutorStats)
            {
                // Make regex match in the statistic name to find out if the statistic is what we are looking for.
                Match dataTypeMatch = Regex.Match(stat.Key, $@"(?<=({(DoesAnalyseInput ? "Input" : "Output")}Data\.Type\.)).*");

                if (!dataTypeMatch.Success) { continue; } // Skip if it's not about data types.
                string dataType = dataTypeMatch.Value; // Else get the type name.

                string objKey = SelectedGrouping == ToolGrouping ? tool.ToolName : report.ProductVersion;

                // If there was no information about this object of analysis before, add a container for it.
                if (!stats.ContainsKey(objKey)) { stats.Add(objKey, new Dictionary<string, int>()); }

                // Also add a container for the info about the type and place the new information here
                if (!stats[objKey].ContainsKey(dataType)) { stats[objKey].Add(dataType, int.Parse(stat.Value)); }
                // If there already is a container, just sum old and new values.
                else { stats[objKey][dataType] += int.Parse(stat.Value); }
            }
        }

        /// <summary>Analyses types of tools input data.</summary>
        public class InputDataTypeAnalysis : DataTypeAnalysis
        {
            public InputDataTypeAnalysis()
            {
                Name = "Тип входных данных";
                DoesAnalyseInput = true;
            }
        }

        /// <summary>Analyses types of tools output data.</summary>
        public class OutputDataTypeAnalysis : DataTypeAnalysis
        {
            public OutputDataTypeAnalysis()
            {
                Name = "Тип выходных данных";
                DoesAnalyseInput = false;
            }
        }
    }
}
