using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Parent class for any analysis.</summary>
    public abstract class Analysis
    {
        /// <summary>List of existing analyses.</summary>
        public static List<Analysis> Instances;

        /// <summary>Enumeration of diffent kinds of analyses.</summary>
        public enum AnalysisType
        {
            ToolsStats,
            XToolsSettings
        }

        /// <summary>What kind of analysis it is.</summary>
        public AnalysisType Type;

        /// <summary>Russian name of the analysis.</summary>
        public string Name { get; protected set; }

        /// <summary>Grouping modes available for the analysis.</summary>
        public abstract string[] Groupings { get; }
        /// <summary>Grouping mode selected for this analysis at the moment.</summary>
        public string SelectedGrouping;

        /// <summary>A temporary storage for data being collected during the analysis.</summary>
        protected Dictionary<string, Dictionary<string, int>> stats;

        /// <summary>Does the analysis and returns a result.</summary>
        public virtual AnalysisResult GetAnalysisResult()
        {
            // Free the temporary storage
            stats = new Dictionary<string, Dictionary<string, int>>();

            // Go through all tools in all reports and collect statistics
            foreach (StatisticsReport report in Filter.FilteredData)
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    ProcessToolUsageData(report, tool);
                }
            }

            // Place the resulting dictionary inside an AnalysisResult (here it gets converted into a more convenient way of data presentation)
            AnalysisResult result = new AnalysisResult(stats);

            return result;
        }

        /// <summary>Collects something in a single ToolUsageData (depends on the info a concrete analysis needs).</summary>
        protected abstract void ProcessToolUsageData(StatisticsReport report, ToolUsageData tool);
    }

    /// <summary>Class containing results of an analysis.</summary>
    public class AnalysisResult
    {
        /// <summary>A group of statistics which are put together for some reason (e.g. they're from the same product version).</summary>
        public class DataGroup
        {
            /// <summary>Identifier of the group.</summary>
            public string Name;

            /// <summary>The statistics analysed.</summary>
            public Dictionary<string, int> Statistics;
        }

        /// <summary>Collection of grouped statistics which were analysed.</summary>
        public DataGroup[] Data;

        /// <summary>Contains names of all data series analysed.</summary>
        public List<string> SeriesNames { get; }

        public AnalysisResult(Dictionary<string, Dictionary<string, int>> statsCollected)
        {
            // Convert received data to a DataGroup array
            Data = (from keyValue in statsCollected
                   select new DataGroup()
                   { Name = keyValue.Key, Statistics = keyValue.Value })
                   .ToArray();

            SeriesNames = new List<string>();

            // Go through all data groups and make a list of different data series
            foreach (DataGroup dataGroup in Data)
            {
                // Check data series inside each dictionary with stats
                foreach (KeyValuePair<string, int> statKeyValue in dataGroup.Statistics)
                {
                    string statisticName = statKeyValue.Key;
                    // If it's the first time we meet this data series, add its name to the series list
                    if (!SeriesNames.Contains(statisticName)) { SeriesNames.Add(statisticName); }
                }
            }
        }
    }
}
