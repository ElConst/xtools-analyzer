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

        /// <summary>What kind of analysis this is.</summary>
        public AnalysisType Type;

        /// <summary>Russian name of the analysis.</summary>
        public string Name { get; protected set; }

        /// <summary>Grouping modes available for the analysis.</summary>
        public abstract string[] Groupings { get; }

        /// <summary>Grouping mode selected for this analysis at the moment.</summary>
        public string SelectedGrouping;

        /// <summary>Does the analysis and returns a result</summary>
        public virtual AnalysisResult GetAnalysisResult()
        {
            Dictionary<string, Dictionary<string, int>> stats = new Dictionary<string, Dictionary<string, int>>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    ProcessToolUsageData(ref stats, report, tool);
                }
            }

            AnalysisResult result = new AnalysisResult(stats.ToArray());

            return result;
        }

        /// <summary>Collects something in single ToolUsageData</summary>
        protected abstract void ProcessToolUsageData(ref Dictionary<string, Dictionary<string, int>> stats, StatisticsReport report, ToolUsageData tool);
    }

    /// <summary>Class containing results of an analysis.</summary>
    public class AnalysisResult
    {
        /// <summary>Collection of statistics which were analysed.</summary>
        public KeyValuePair<string, Dictionary<string, int>>[] Statistics;

        /// <summary>Contains names of all data series (statistics) analysed.</summary>
        public List<string> SeriesNames { get; }

        public AnalysisResult(KeyValuePair<string, Dictionary<string, int>>[] statsCollection)
        {
            Statistics = statsCollection;

            SeriesNames = new List<string>();
            
            foreach (KeyValuePair<string, Dictionary<string, int>> objKeyValue in Statistics)
            {
                // Check data series inside each dictionary with stats
                foreach (KeyValuePair<string, int> statKeyValue in objKeyValue.Value)
                {
                    string statisticName = statKeyValue.Key;
                    if (!SeriesNames.Contains(statisticName)) { SeriesNames.Add(statisticName); }
                }
            }
        }
    }
}
