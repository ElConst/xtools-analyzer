using System;
using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Class containing data from a single statistics report of the tools usage.</summary>
    public class StatisticsReport
    {
        public string ProductName, ProductVersion;

        public DateTime StartDate, EndDate;

        public List<ToolUsageData> ToolsUsed = new List<ToolUsageData>();
    }
}
