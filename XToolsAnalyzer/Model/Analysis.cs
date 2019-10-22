using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Parent class for all analyses.</summary>
    public abstract class Analysis
    {
        /// <summary>Existing analyses list.</summary>
        public static List<Analysis> Instances;

        /// <summary>Russian name of the analysis.</summary>
        public string Name { get; protected set; }

        /// <summary>Does the analysis</summary>
        public abstract AnalysisResult GetAnalysisResult();
    }

    /// <summary>Class containing results of an analysis.</summary>
    public class AnalysisResult
    {
        /// <summary>Collection of tools statistics (Dictionary (Tool name -> Its statistics))</summary>
        public Dictionary<string, Dictionary<string, int>> ToolsStatistics = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>Contains more than one value for each tool.</summary>
        public bool MultipleStatistics { get; }

        public AnalysisResult(bool multipleStatistics = false) => MultipleStatistics = multipleStatistics;
    }
}
