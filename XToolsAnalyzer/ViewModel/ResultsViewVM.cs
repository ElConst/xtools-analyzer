using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XToolsAnalyzer.ViewModel
{
    public class ResultsViewVM : ViewModelBase
    {
        public static ResultsViewVM Instance;

        private ObservableCollection<ToolStatisticVM> toolsStatistics = new ObservableCollection<ToolStatisticVM>();
        /// <summary>Collection of 'ToolStatisticVM' instances containing information about certain statistic of certain tool.</summary>
        public ObservableCollection<ToolStatisticVM> ToolsStatistics
        {
            get => toolsStatistics;
            set { SetProperty(ref toolsStatistics, value); } // Raises PropertyChanged event to update view.
        }

        public ResultsViewVM()
        {
            Instance = this;
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
}
