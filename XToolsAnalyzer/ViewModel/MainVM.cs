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

        public static MainVM Instance;
        public MainVM()
        {
            Instance = this;

            // Path to the folder with JSONs required here.
            DataLoader.DefaultFolderToLoad = @"d:\a";

            // Set a primary filter
            Filter.LoadFilters();

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
