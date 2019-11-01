using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.Model
{
    /// <summary>Parent class for all analyses.</summary>
    public abstract class Analysis
    {
        /// <summary>Existing analyses list.</summary>
        public static List<Analysis> Instances;

        public enum AnalysisType
        {
            ToolsStats,
            XToolsSettings
        }

        /// <summary>Russian name of the analysis.</summary>
        public string Name { get; protected set; }

        public AnalysisType Type;

        public abstract string[] Groupings { get; }

        public string SelectedGrouping;

        public class Sorting
        {
            // Delegate type for a sorting function.
            public delegate void SortFunc(ref AnalysisResult data);

            /// <summary>Instructions for the sorting.</summary>
            public SortFunc SortingFunction { get; set; }

            /// <summary>Russian name of the sorting.</summary>
            public string Name { get; set; }

            public Sorting(string name, SortFunc sortingFunction)
            {
                Name = name;
                SortingFunction = sortingFunction;
            }

            public static List<Sorting> Instances { get; } = new List<Sorting>
            {
                // Alphabetically
                new Sorting("По алфавиту", // OrderByDescending is used because of upside down list inside the chart.
                    (ref AnalysisResult analysisResult) =>
                    {
                        analysisResult.Statistics = 
                            analysisResult.Statistics.OrderByDescending(obj => obj.Key).ToArray();
                    }),
            
                // Values ascending
                new Sorting("По возрастанию",
                    (ref AnalysisResult analysisResult) =>
                    {
                        analysisResult.Statistics =
                            analysisResult.Statistics.OrderByDescending(obj => obj.Value.Sum(stat => stat.Value)).ToArray();
                    }),

                // Values descending
                new Sorting("По убыванию",
                    (ref AnalysisResult analysisResult) =>
                    {
                        analysisResult.Statistics = 
                            analysisResult.Statistics.OrderBy(obj => obj.Value.Sum(stat => stat.Value)).ToArray();
                    })
            };
        }

        public static Sorting SelectedSorting = Sorting.Instances.First();

        /// <summary>Does the analysis</summary>
        public abstract AnalysisResult GetAnalysisResult();
    }

    /// <summary>Class containing results of an analysis.</summary>
    public class AnalysisResult
    {
        /// <summary>Collection of statistics.</summary>
        public KeyValuePair<string, Dictionary<string, int>>[] Statistics;
    }
}
