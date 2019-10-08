using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Viewmodel containing logic for the 'MainWindow' view.</summary>
    public class MainVM : ViewModelBase
    {
        /// <summary>Collection of analyses which can be applied for the data.</summary>
        public ObservableCollection<AnalysisVM> Analyses { get; set; } = new ObservableCollection<AnalysisVM>();

        public static MainVM Instance;
        public MainVM()
        {
            Instance = this;

            // Path to the folder with JSONs required here.
            DataLoader.DefaultFolderToLoad = @"D:\XTools";

            Analyses = new ObservableCollection<AnalysisVM>(AnalysesManager.Analyses.Select(analysis => new AnalysisVM(analysis)));
        }
    }

    /// <summary>All the information needed in the view about an analysis including command to make this analysis.</summary>
    public class AnalysisVM
    {
        /// <summary>An analysis interface containing its name and the function to make the analysis.</summary>
        private readonly IAnalysis analysis; 

        /// <summary>Analysis name.</summary>
        public string Name => analysis.Name;

        private RelayCommand resultsCommand;
        /// <summary>Command calling a function which replaces current info on the screen with results of the analysis.</summary>
        public RelayCommand ResultsCommand
        {
            get
            {
                return resultsCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (resultsCommand = new RelayCommand(obj =>
                {
                    AnalysesManager.SelectedAnalysis = analysis; // Remember this analysis

                    ResultsViewVM.Instance.CreateRowChart(analysis.GetAnalysisResult(), Name);
                }));
            }
        }

        public AnalysisVM(IAnalysis analysis) => this.analysis = analysis;
    }
}
