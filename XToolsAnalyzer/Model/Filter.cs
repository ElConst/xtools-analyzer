using System;
using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Contains filter settings for data selection.</summary>
    public class Filter
    {
        public static Filter Instance;

        /// <summary>Tells if we need to take this product statistics into account.</summary>
        public bool ShowXToolsAgp = true,
                    ShowXToolsPro = true;

        /// <summary>Collection of tool names each connected to a bool which tells if it is selected for the filter or not.</summary>
        public Dictionary<string, bool> ToolsFilter;

        /// <summary>One of borders of suitable dates (suitable includes this date).</summary>
        public DateTime StartDate, EndDate;
        /// <summary>Is needed to have an opportunity to clear the filter.</summary>
        private DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;

        /// <summary>Changes the date filter back to the whole range of dates.</summary>
        public void ClearDateFilter()
        {
            StartDate = minDate;
            EndDate = maxDate;
        }

        /// <summary>Checks if a date suits the date filter.</summary>
        /// <param name="date">The date which needs to be checked.</param>
        public bool IsDateSuitable(DateTime date)
        {
            // The date is not suitable if it's earlier than the start date selected for the filter
            if (DateTime.Compare(date, StartDate) < 0) { return false; }

            // And it's not suitable too if it's later than the end date of the filter
            if (DateTime.Compare(date, EndDate) > 0) { return false; }

            // Else it is suitable
            return true;
        }

        public Filter()
        {
            LoadFilters();
        }

        /// <summary>Gets all tools names and creates the ToolsFilter.</summary>
        private void LoadFilters()
        {
            ToolsFilter = new Dictionary<string, bool>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                // Find min and max dates
                if (DateTime.Compare(report.StartDate, minDate) < 0) { minDate = report.StartDate; }
                if (DateTime.Compare(report.EndDate, maxDate) > 0) { maxDate = report.EndDate; }

                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    if (!ToolsFilter.ContainsKey(tool.ToolName)) { ToolsFilter.Add(tool.ToolName, true); }
                }
            }

            // Start with all the range of dates
            StartDate = minDate;
            EndDate = maxDate;
        }

    }
}
