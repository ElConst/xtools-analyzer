using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Contains filter settings for data selection.</summary>
    public class Filter
    {
        public static Filter Instance;

        /// <summary>Collection of tool names each connected to a bool which tells if it is selected for the filter or not.</summary>
        public Dictionary<string, bool> ToolsFilter;

        public Filter()
        {
            LoadTools();
        }

        private void LoadTools()
        {
            ToolsFilter = new Dictionary<string, bool>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    if (!ToolsFilter.ContainsKey(tool.ToolName)) { ToolsFilter.Add(tool.ToolName, true); }
                }
            }
        }
    }
}
