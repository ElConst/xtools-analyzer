using LiveCharts;
using LiveCharts.Events;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XToolsAnalyzer.Model;

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

        /// <summary>Chart sortings selectable from view.</summary>
        public IReadOnlyList<string> SortTypes { get; } = new List<string>()
        {
            "По алфавиту", "По возрастанию", "По убыванию"
        };

        private string selectedSort;
        /// <summary>Sorting type which is selected at the moment.</summary>
        public string SelectedSort
        {
            get => selectedSort;
            set
            {
                SetProperty(ref selectedSort, value); // Raises the PropertyChanged event to sync with the view.

                if (AnalysesManager.SelectedAnalysis == null) { return; }

                var analysisSelected = AnalysesManager.SelectedAnalysis;
                // Recreate the results chart to match selected sorting.
                CreateRowChart(analysisSelected.GetAnalysisResult(), analysisSelected.Name);
            }
        }

        /// <summary>
        /// Replaces chart series with a new row series. Sets needed corresponding values for the chart.
        /// </summary>
        /// <param name="toolsData">Data (tool name + statistic) which will be displayed in the chart</param>
        /// <param name="statisticName">Analysed statistic name.</param>
        public void CreateRowChart(Dictionary<string, float> toolsData, string statisticName)
        {
            if (SeriesCollection != null) { SeriesCollection.Clear(); }

            List<KeyValuePair<string, float>> toolsDataList = toolsData.ToList();

            // Sort data
            if (SelectedSort == SortTypes[0]) // Alphabetically
            {
                toolsDataList = toolsData.OrderByDescending(toolKeyValue => toolKeyValue.Key).ToList();
                // OrderByDescending is used because of reversed list inside the chart.
            }
            else if (SelectedSort == SortTypes[1]) // By values ascending
            {
                toolsDataList = toolsData.OrderByDescending(toolKeyValue => toolKeyValue.Value).ToList();
            }
            else if (SelectedSort == SortTypes[2]) // By values descending
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

            SelectedSort = SortTypes.First();
        }
    }
}
