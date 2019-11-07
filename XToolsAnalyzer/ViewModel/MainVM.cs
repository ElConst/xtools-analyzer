using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Viewmodel containing logic for the 'MainWindow' view.</summary>
    public class MainVM : ViewModelBase
    {
        /// <summary>Collection of analyses working with common statistics of tools.</summary>
        public ObservableCollection<AnalysisVM> ToolsStatsAnalyses { get; set; } = new ObservableCollection<AnalysisVM>();
        /// <summary>Collection of analyses working with information about options of the XTools itself.</summary>
        public ObservableCollection<AnalysisVM> XToolsSettingsAnalyses { get; set; } = new ObservableCollection<AnalysisVM>();

        private string resultsTitleText = "Результаты анализа";
        /// <summary>Caption for the area with analyses results.</summary>
        public string ResultsTitleText
        {
            get => resultsTitleText;
            set => SetProperty(ref resultsTitleText, value);
        }

        private Analysis.Sorting selectedSorting = Analysis.Sorting.Instances.First();
        /// <summary>The sorting selected from the view.</summary>
        public Analysis.Sorting SelectedSorting
        {
            get => selectedSorting;
            set
            {
                SetProperty(ref selectedSorting, value); // Raises PropertyChanged event to sync with the view.
                Analysis.SelectedSorting = selectedSorting;

                AnalysisVM.MakeSelectedAnalysis();
            }
        }

        private string[] groupings;
        /// <summary>Data grouping modes available for the selected analysis.</summary>
        public string[] Groupings
        {
            get => groupings;
            set => SetProperty(ref groupings, value); // Raises PropertyChanged event to sync with the view.
        }

        private string selectedGrouping;
        /// <summary>The grouping selected from the view.</summary>
        public string SelectedGrouping
        {
            get => selectedGrouping;
            set
            {
                SetProperty(ref selectedGrouping, value); // Raises PropertyChanged event to sync with the view.
                AnalysisVM.SelectedAnalysis.Analysis.SelectedGrouping = selectedGrouping;

                AnalysisVM.MakeSelectedAnalysis();
            }
        }

        public List<Analysis.Sorting> Sortings => Analysis.Sorting.Instances;

        private RelayCommand openFilterWindowCommand;
        /// <summary>Shows the filter window.</summary>
        public RelayCommand OpenFilterWindowCommand
        {
            get
            {
                return openFilterWindowCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (openFilterWindowCommand = new RelayCommand(args =>
                {
                    FilterWindow.Instance.Show();
                }));
            }
        }

        public static MainVM Instance;
        public MainVM()
        {
            Instance = this;

            // Path to the folder with JSONs required here.
            DataLoader.DefaultFolderToLoad = @"d:\a";

            // Find existing analyses.
            Analysis.Instances = ReflectiveEnumerator.GetEnumerableOfType<Analysis>().OrderBy(a => a.Name).ToList();

            // Create objects which the View can interact with.

            ToolsStatsAnalyses = new ObservableCollection<AnalysisVM>(Analysis.Instances
                .Where(analysis => analysis.Type == Analysis.AnalysisType.ToolsStats)
                .Select(analysis => new AnalysisVM(analysis)));

            XToolsSettingsAnalyses = new ObservableCollection<AnalysisVM>(Analysis.Instances
                .Where(analysis => analysis.Type == Analysis.AnalysisType.XToolsSettings)
                .Select(analysis => new AnalysisVM(analysis)));
        }
    }
}
