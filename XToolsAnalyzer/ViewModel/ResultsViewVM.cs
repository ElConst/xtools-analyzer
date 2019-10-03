using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace XToolsAnalyzer.ViewModel
{
    public class ResultsViewVM : ViewModelBase
    {
        public static ResultsViewVM Instance;

        private ObservableCollection<string> labels = new ObservableCollection<string>();
        /// <summary>Labels for the chart's y axis.</summary>
        public ObservableCollection<string> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value); // Raises PropertyChanged event to update view.
        }

        private int chartHeight = 1000;
        /// <summary>Height of the chart control on the view.</summary>
        public int ChartHeight
        {
            get => chartHeight;
            set => SetProperty(ref chartHeight, value); // Raises PropertyChanged event to update view.
        }

        /// <summary>Series of the chart (pie, rows, columns etc.).</summary>
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        /// <summary>
        /// Replaces chart series with a new row series. Sets needed corresponding values for the chart.
        /// </summary>
        /// <param name="toolsData">Data (tool name + statistic) which will be displayed in the chart</param>
        /// <param name="statisticName">Analysed statistic name.</param>
        public void CreateRowChart(Dictionary<string, float> toolsData, string statisticName)
        {
            if (SeriesCollection != null) { SeriesCollection.Clear(); }

            var stats = toolsData.Select(toolData => toolData.Value);
            SeriesCollection.Add(new RowSeries
            {
                Title = statisticName,
                Values = new ChartValues<float>(stats),
            });

            var toolsNames = toolsData.Select(toolData => toolData.Key);
            Labels = new ObservableCollection<string>(toolsNames);

            ChartHeight = 20 * Labels.Count;
        }

        public ResultsViewVM()
        {
            Instance = this;
        }
    }
}
