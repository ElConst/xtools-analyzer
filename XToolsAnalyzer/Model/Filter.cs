using System;
using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Contains filter settings for data selection.</summary>
    public static class Filter
    {
        /// <summary>Tells if we need to take this product statistics into account.</summary>
        public static bool ShowXToolsAgp = true,
                    ShowXToolsPro = true;

        /// <summary>Collection of tool names each connected to a bool which tells if it is selected for the filter or not.</summary>
        public static Dictionary<string, bool> ToolsFilter;

        /// <summary>Collection of product version names each connected to a bool which tells if it is selected for the filter or not.</summary>
        public static Dictionary<string, bool> VersionsFilter;

        /// <summary>One of borders of suitable dates (suitable includes this date).</summary>
        public static DateTime StartDate, EndDate;
        /// <summary>Is needed to have an opportunity to clear the filter.</summary>
        private static DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;

        /// <summary>Changes the date filter back to the whole range of dates.</summary>
        public static void ClearDateFilter()
        {
            StartDate = minDate;
            EndDate = maxDate;
        }

        /// <summary>Checks if a date suits the date filter.</summary>
        /// <param name="date">The date which needs to be checked.</param>
        public static bool IsDateSuitable(DateTime date)
        {
            // The date is not suitable if it's earlier than the start date selected for the filter
            if (DateTime.Compare(date, StartDate) < 0) { return false; }

            // And it's not suitable too if it's later than the end date of the filter
            if (DateTime.Compare(date, EndDate) > 0) { return false; }

            // Else it is suitable
            return true;
        }

        /// <summary>Tells if the filter is ready to use.</summary>
        public static bool Loaded = false;

        /// <summary>Initializes data for filters.</summary>
        public static void LoadFilters()
        {
            ToolsFilter = new Dictionary<string, bool>();
            VersionsFilter = new Dictionary<string, bool>();

            foreach (StatisticsReport report in DataLoader.LoadFromFolder())
            {
                // Find min and max dates
                if (DateTime.Compare(report.StartDate, minDate) < 0) { minDate = report.StartDate; }
                if (DateTime.Compare(report.EndDate, maxDate) > 0) { maxDate = report.EndDate; }

                // Add all product versions which reports contain
                if (!VersionsFilter.ContainsKey(report.ProductVersion)) { VersionsFilter.Add(report.ProductVersion, true); }

                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    // Add all tools mentioned in reports
                    if (!ToolsFilter.ContainsKey(tool.ToolName)) { ToolsFilter.Add(tool.ToolName, true); }
                }
            }

            // Start with all the range of dates
            StartDate = minDate;
            EndDate = maxDate;

            Loaded = true;
        }

    }
}
