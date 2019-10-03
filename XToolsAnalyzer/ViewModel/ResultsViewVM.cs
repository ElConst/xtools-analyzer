using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;

namespace XToolsAnalyzer.ViewModel
{
    public class ResultsViewVM : ViewModelBase
    {
        public static ResultsViewVM Instance;

        private ObservableCollection<string> labels = new ObservableCollection<string>();
        public ObservableCollection<string> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value); // Raises PropertyChanged event to update view.
        }

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        public void CreateChart(Dictionary<string, float> toolsData, string analysisName)
        {
            if (SeriesCollection != null) { SeriesCollection.Clear(); }

            var stats = toolsData.Select(toolData => toolData.Value);
            SeriesCollection.Add(new RowSeries
            {
                Title = analysisName,
                Values = new ChartValues<float>(stats),
            });

            var toolsNames = toolsData.Select(toolData => toolData.Key);
            Labels = new ObservableCollection<string>(toolsNames);
        }

        public ResultsViewVM()
        {
            Instance = this;
        }
    }
}
