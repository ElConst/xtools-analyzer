using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Interface containing name of the analysis and a function for making the analysis.</summary>
    public interface IAnalysis
    {
        /// <summary>Analysis name.</summary>
        string Name { get; }

        /// <summary>Makes the analysis and returns its results.</summary>
        /// <returns>Analysis results in a dictionary(Tool name -> Tool statistic).</returns>
        Dictionary<string, float> GetAnalysisResult();
    }

    /// <summary>Main class for handling all the analyses.</summary>
    public static class AnalysesManager
    {
        // TODO: Get analyses from assemblies.

        // Last analysis that was made
        public static IAnalysis SelectedAnalysis;

        // Possible analyses list.
        public static List<IAnalysis> Analyses = new List<IAnalysis>() { ClicksAnalysis.Instance };
    }
}
