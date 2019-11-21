using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Parent for analyses which count an average number of some statistic.</summary>
    public abstract class AvgAnalysis : Analysis
    {
        /// <summary>Russian name of the statistic whose average we are looking for.</summary>
        public string AverageStatisticName;

        /// <summary>Collects info needed to calculate an average number of some statistic.</summary>
        protected class StatAverageInfo
        {
            public float StatSum;
            public int ReportsCount;
        }

        /// <summary>A special temporary storage to put sum of the statistics and count reports to calculate average numbers later.</summary>
        protected Dictionary<string, StatAverageInfo> statsAverageData;

        /// <summary>Does the analysis and returns a result.</summary>
        public override AnalysisResult GetAnalysisResult()
        {
            // Free temporary storages
            stats = new Dictionary<string, Dictionary<string, int>>();
            statsAverageData = new Dictionary<string, StatAverageInfo>();

            // Go through all tools in all reports and collect statistics sum
            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    ProcessToolUsageData(report, tool);
                }
            }

            // Finally calculate average numbers and put them into a storage.
            foreach (var dataGroupKeyValue in statsAverageData)
            {
                string dataGroupName = dataGroupKeyValue.Key;
                StatAverageInfo infoToCountAverage = dataGroupKeyValue.Value;

                float statisticAverage = infoToCountAverage.StatSum / infoToCountAverage.ReportsCount;

                if (!stats.ContainsKey(dataGroupName)) { stats.Add(dataGroupName, new Dictionary<string, int>()); }
                // Add this new value to the resulting collection
                stats[dataGroupName][AverageStatisticName] = (int)statisticAverage;
            }

            // Place the resulting dictionary inside an AnalysisResult (here it gets converted into a more convenient way of data presentation)
            AnalysisResult result = new AnalysisResult(stats);

            return result;
        }
    }
}
