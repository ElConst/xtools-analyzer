using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Class containing data from a single statistics report of the tools usage.</summary>
    public class StatisticsReport
    {
        public string ProductVersion,
            DateSentUtc, StartDateUtc, EndDateUtc;

        public List<ToolUsageData> ToolsUsed = new List<ToolUsageData>();
    }
}
