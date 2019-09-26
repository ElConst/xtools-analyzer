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
        
        private ObservableCollection<ToolStatisticVM> toolsStatistics = new ObservableCollection<ToolStatisticVM>();
        /// <summary>Collection of 'ToolStatisticVM' instances containing information about certain statistic of certain tool.</summary>
        public ObservableCollection<ToolStatisticVM> ToolsStatistics
        {
            get => toolsStatistics;
            set { SetProperty(ref toolsStatistics, value); } // Raises PropertyChanged event to update view.
        }

        public static MainVM Instance;
        public MainVM()
        {
            Instance = this;

            // Path to the folder with JSONs required here.
            DataLoader.DefaultFolderToLoad = @"D:\XTools";

            Analyses = new ObservableCollection<AnalysisVM>(AnalysesManager.Analyses.Select(analysis => new AnalysisVM(analysis)));
        }
    }

    /// <summary>Represents tool statistic to be shown in the view.</summary>
    public class ToolStatisticVM
    {
        public string Name { get; }
        public float Statistic { get; }

        public ToolStatisticVM(string name, float statistic)
        {
            Name = name;
            Statistic = statistic;
        }
    }

    /// <summary>All about an analysis needed in the view including command to make this analysis.</summary>
    public class AnalysisVM
    {
        private IAnalysis analysis; // An analysis interface containing its name and the function to make the analysis.

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
                    // Make a ToolStatisticVM IEnumerable from the results of the analysis.
                    var toolsStatsIEnum = analysis.GetAnalysisResult().Select(toolStatPair => new ToolStatisticVM(toolStatPair.Key, toolStatPair.Value));
                    // Replace old info in the VM with analysis results in the IEnumerable sorted by descending.
                    MainVM.Instance.ToolsStatistics = new ObservableCollection<ToolStatisticVM>(toolsStatsIEnum.OrderByDescending(tool => tool.Statistic));

                    // TODO: Separate the sorting process.
                }));
            }
        }

        public AnalysisVM(IAnalysis analysis) => this.analysis = analysis;
    }
}
