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
}
