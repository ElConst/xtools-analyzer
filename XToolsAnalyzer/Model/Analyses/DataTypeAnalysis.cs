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

        private static string
            ToolGrouping = "По инструментам",
            VersionGrouping = "По версиям";

        private string[] groupings = { ToolGrouping, VersionGrouping };
        public override string[] Groupings => groupings;

        public enum Data { Input, Output }

        public Data DataAnalysed;

        /// <summary>Gets amount of times objects of different types used by a tool within a report.</summary>
        protected override void ProcessToolUsageData(ref Dictionary<string, Dictionary<string, int>> stats, StatisticsReport report, ToolUsageData tool)
        {
            foreach (var stat in tool.ExecutorStats)
            {
                // Make regex match in the statistic name to find out if the statistic is what we are looking for.
                Match dataTypeMatch = Regex.Match(stat.Key, $@"(?<=({(DataAnalysed == Data.Input ? "Input" : "Output")}Data\.Type\.)).*");

                if (!dataTypeMatch.Success) { continue; } // Skip if it's not about data types.
                string dataType = dataTypeMatch.Value; // Else get the type name.

                string objKey = SelectedGrouping == ToolGrouping ? tool.ToolName : report.ProductVersion;

                if (!stats.ContainsKey(objKey))
                {
                    // If there was no information about this object of analysis before, add a container for it.
                    stats.Add(objKey, new Dictionary<string, int>());
                }
                if (!stats[objKey].ContainsKey(dataType))
                {
                    // Also add a container for the info about the type
                    stats[objKey].Add(dataType, int.Parse(stat.Value));
                }
                else
                {
                    // If there already is a container, just sum old and new values.
                    stats[objKey][dataType] += int.Parse(stat.Value);
                }
            }
        }

        /// <summary>Analyses types of tools input data.</summary>
        public class InputDataTypeAnalysis : DataTypeAnalysis
        {
            public InputDataTypeAnalysis()
            {
                Name = "Тип входных данных";
                DataAnalysed = Data.Input;
            }
        }

        /// <summary>Analyses types of tools output data.</summary>
        public class OutputDataTypeAnalysis : DataTypeAnalysis
        {
            public OutputDataTypeAnalysis()
            {
                Name = "Тип выходных данных";
                DataAnalysed = Data.Output;
            }
        }
    }
}
