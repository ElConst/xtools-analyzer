using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XToolsAnalyzer.Model
{
    public static class DataHandler
    {
        private static List<StatisticsReport> rawData = new List<StatisticsReport>();
        public static List<StatisticsReport> FilteredData { get; private set; } = rawData;

        /// <summary>
        /// Reads data for future analyses from folder
        /// </summary>
        /// <param name="path">Path to a folder with JSONs</param>
        public static void CollectDataFromFolder(string path)
        {
            rawData = DataLoader.LoadFromFolder(path);
            FilteredData = rawData;
        }

        public struct FilterOptions
        {
            // TODO: Target product versions, date of report etc. Send an instance of this structure as a set of
            //       parameters for the "ApplyFilter" function
        }
        public static void ApplyFilter(FilterOptions filter)
        {
            // TODO: Exclude inappropriate reports from the "rawData" list
        }
    }
}
