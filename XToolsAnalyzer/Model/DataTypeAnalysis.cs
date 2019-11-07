using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XToolsAnalyzer.Model
{
    public abstract class DataTypeAnalysis : Analysis
    {
        public DataTypeAnalysis()
        {
            Name = "Тип входных данных";
            Type = AnalysisType.ToolsStats;
            SelectedGrouping = Groupings[0];
        }

        private static string
            ToolGrouping = "По инструментам",
            VersionGrouping = "По версиям";

        private string[] groupings = { ToolGrouping, VersionGrouping };
        public override string[] Groupings => groupings;

        public enum Data { Input, Output }

        public Data DataAnalysed;

        /// <summary>Gets amount of times objects of different types used.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
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
            }

            AnalysisResult result = new AnalysisResult();
            result.Statistics = stats.ToArray();

            SelectedSorting.SortingFunction(ref result);

            return result;
        }
    }

    public class InputDataTypeAnalysis : DataTypeAnalysis
    {
        public InputDataTypeAnalysis()
        {
            Name = "Тип входных данных";
            DataAnalysed = Data.Input;
        }
    }

    public class OutputDataTypeAnalysis : DataTypeAnalysis
    {
        public OutputDataTypeAnalysis()
        {
            Name = "Тип выходных данных";
            DataAnalysed = Data.Output;
        }
    }
}
