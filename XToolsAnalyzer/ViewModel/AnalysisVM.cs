using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Class representing an analysis for the view so it can work with it.</summary>
    public class AnalysisVM
    { 
        private static AnalysisVM selectedAnalysis;
        /// <summary>Actually the last analysis that was made.</summary>
        public static AnalysisVM SelectedAnalysis
        {
            get => selectedAnalysis;
            set
            {
                selectedAnalysis = value;

                MainVM.Instance.ResultsTitleText = $"Результаты анализа \"{selectedAnalysis.Name}\"";
            }
        }

        /// <summary>Makes the selected analysis if it is specified.</summary>
        public static void MakeSelectedAnalysis()
        {
            if (SelectedAnalysis == null) { return; }

            SelectedAnalysis.AnalysisCommand.Execute("");
        }

        /// <summary>Model of the analysis.</summary>
        public readonly Analysis Analysis;

        /// <summary>Russian name of the analysis.</summary>
        public string Name => Analysis.Name;

        private RelayCommand analysisCommand;
        /// <summary>Command calling a function which replaces current info on the screen with results of the analysis.</summary>
        public RelayCommand AnalysisCommand
        {
            get
            {
                return analysisCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (analysisCommand = new RelayCommand(obj =>
                {
                    AnalysisResult analysisResult = Analysis.GetAnalysisResult();

                    // If analysis is changed
                    if (SelectedAnalysis != this)
                    {
                        // Remember this analysis
                        SelectedAnalysis = this; 

                        // Update view
                        OptionsVM.Instance.SelectedGrouping = Analysis.SelectedGrouping;
                        OptionsVM.Instance.Groupings = Analysis.Groupings;
                        SortingOptionsVM.Instance.CreateOptionsToSortAnAnalysisResult(analysisResult);
                    }

                    SortingOptionsVM.Instance.Sort(ref analysisResult);
                    ResultsViewVM.Instance.CreateRowChart(analysisResult);
                }));
            }
        }

        public AnalysisVM(Analysis analysis) => Analysis = analysis;
    }
}
