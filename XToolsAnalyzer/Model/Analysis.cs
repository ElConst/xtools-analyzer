using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Parent class for all analyses.</summary>
    public abstract class Analysis
    {
        /// <summary>Existing analyses list.</summary>
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

        /// <summary>Does the analysis and returns a result.</summary>
        public virtual AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            // Go through all tools in all reports and collect statistics
            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    ProcessToolUsageData(ref stats, report, tool);
                }
            }

            // Place the resulting dictionary inside an AnalysisResult (here it gets converted into a more convenient way of data presentation)
            AnalysisResult result = new AnalysisResult(stats);

            return result;
        }

        /// <summary>Collects something in single ToolUsageData (depends on the info a concrete analysis needs).</summary>
        protected abstract void ProcessToolUsageData(ref Dictionary<string, Dictionary<string, int>> stats, StatisticsReport report, ToolUsageData tool);
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

        /// <summary>Contains names of all data series (statistics) analysed.</summary>
        public List<string> SeriesNames { get; }

        public AnalysisResult(Dictionary<string, Dictionary<string, int>> statsCollected)
        {
            // Convert received data to a DataGroup array
            Data = (from keyValue in statsCollected
                   select new DataGroup()
                   { Name = keyValue.Key, Statistics = keyValue.Value })
                   .ToArray();

            SeriesNames = new List<string>();
            
            // Go through all data groups
            foreach (DataGroup dataGroup in Data)
            {
                // And check data series inside each dictionary with stats
                foreach (KeyValuePair<string, int> statKeyValue in dataGroup.Statistics)
                {
                    string statisticName = statKeyValue.Key;
                    if (!SeriesNames.Contains(statisticName)) { SeriesNames.Add(statisticName); }
                }
            }
        }
    }
}
