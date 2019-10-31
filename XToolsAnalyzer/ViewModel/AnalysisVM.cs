using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Class representing an analysis for the view so it can work with it.</summary>
    public class AnalysisVM
    {
        /// <summary>Actually the last analysis that was made.</summary>
        public static AnalysisVM SelectedAnalysis;

        /// <summary>Model of the analysis.</summary>
        private readonly Analysis analysis;

        /// <summary>Analysis name.</summary>
        public string Name => analysis.Name;

        private RelayCommand analysisCommand;
        /// <summary>Command calling a function which replaces current info on the screen with results of the analysis.</summary>
        public RelayCommand AnalysisCommand
        {
            get
            {
                return analysisCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (analysisCommand = new RelayCommand(obj =>
                {
                    SelectedAnalysis = this; // Remember this analysis

                    ResultsViewVM.Instance.CreateRowChart(analysis.GetAnalysisResult());
                }));
            }
        }

        public AnalysisVM(Analysis analysis) => this.analysis = analysis;
    }
}
