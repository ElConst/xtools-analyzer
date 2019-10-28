using System.Collections.ObjectModel;
using System.Linq;
using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Viewmodel containing logic for the 'MainWindow' view.</summary>
    public class MainVM : ViewModelBase
    {
        public ObservableCollection<AnalysisVM> ToolsStatsAnalyses { get; set; } = new ObservableCollection<AnalysisVM>();
        public ObservableCollection<AnalysisVM> XToolsSettingsAnalyses { get; set; } = new ObservableCollection<AnalysisVM>();

        public static MainVM Instance;
        public MainVM()
        {
            Instance = this;

            // Path to the folder with JSONs required here.
            DataLoader.DefaultFolderToLoad = @"d:\xtools";

            // Find existing analyses.
            Analysis.Instances = ReflectiveEnumerator.GetEnumerableOfType<Analysis>().ToList();

            // Create objects which the View can interact with.

            ToolsStatsAnalyses = new ObservableCollection<AnalysisVM>(Analysis.Instances
                .Where(analysis => analysis.Type == Analysis.AnalysisType.ToolsStats)
                .Select(analysis => new AnalysisVM(analysis)));

            XToolsSettingsAnalyses = new ObservableCollection<AnalysisVM>(Analysis.Instances
                .Where(analysis => analysis.Type == Analysis.AnalysisType.XToolsSettings)
                .Select(analysis => new AnalysisVM(analysis)));
        }
    }

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
