using LiveCharts;
using LiveCharts.Events;
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

        private RelayCommand xAxisRangeChangedCommand;
        /// <summary>Command calling a function which forces the x axis scale to start at 0 regardless of zoom.</summary>
        public RelayCommand XAxisRangeChangedCommand
        {
            get
            {
                return xAxisRangeChangedCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (xAxisRangeChangedCommand = new RelayCommand(args =>
                {
                    var rangeChangedArgs = (RangeChangedEventArgs)args;
                    ((Axis)rangeChangedArgs.Axis).MinValue = 0;
                }));
            }
        }

        /// <summary>
        /// Replaces chart series with a new row series. Sets needed corresponding values for the chart.
        /// </summary>
        /// <param name="toolsData">Data (tool name + statistic) which will be displayed in the chart</param>
        /// <param name="statisticName">Analysed statistic name.</param>
        /// <param name="sorting">Sorting type (one of SortTypes from MainVM)</param>
        public void CreateRowChart(Dictionary<string, float> toolsData, string statisticName, string sorting)
        {
            if (SeriesCollection != null) { SeriesCollection.Clear(); }

            List<KeyValuePair<string, float>> toolsDataList = toolsData.ToList();

            // Sort data
            if (sorting == MainVM.Instance.SortTypes[0]) // Alphabetically
            {
                toolsDataList = toolsData.OrderByDescending(toolKeyValue => toolKeyValue.Key).ToList();
            }
            else if (sorting == MainVM.Instance.SortTypes[1]) // By values ascending
            {
                toolsDataList = toolsData.OrderByDescending(toolKeyValue => toolKeyValue.Value).ToList();
            }
            else if (sorting == MainVM.Instance.SortTypes[2]) // By values descending
            {
                toolsDataList = toolsData.OrderBy(toolKeyValue => toolKeyValue.Value).ToList();
            }

            var stats = toolsDataList.Select(toolData => toolData.Value);
            SeriesCollection.Add(new RowSeries
            {
                Title = statisticName,
                Values = new ChartValues<float>(stats),
            });

            var toolsNames = toolsDataList.Select(toolData => toolData.Key);
            Labels = new ObservableCollection<string>(toolsNames);

            ChartHeight = 20 * Labels.Count;
        }

        public ResultsViewVM()
        {
            Instance = this;
        }
    }
}
