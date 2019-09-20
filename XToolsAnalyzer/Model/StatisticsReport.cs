using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XToolsAnalyzer.Model
{
    public class StatisticsReport
    {
        public string ProductVersion,
            DateSentUtc, StartDateUtc, EndDateUtc;

        public List<ToolUsageData> ToolsUsed = new List<ToolUsageData>();
    }
}
