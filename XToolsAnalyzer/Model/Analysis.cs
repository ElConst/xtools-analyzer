using System.Collections.Generic;

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

        /// <summary>Does the analysis</summary>
        public abstract AnalysisResult GetAnalysisResult();
    }

    /// <summary>Class containing results of an analysis.</summary>
    public class AnalysisResult
    {
        /// <summary>Collection of statistics.</summary>
        public Dictionary<string, Dictionary<string, int>> Statistics = new Dictionary<string, Dictionary<string, int>>();
    }
}
