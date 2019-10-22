using LiveCharts;
using LiveCharts.Events;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Viewmodel containing logic for the 'ResultsView'.</summary>
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

        // Type for a sorting function.
        public delegate IOrderedEnumerable<KeyValuePair<string, Dictionary<string, int>>> AnalysisSortFunc(AnalysisResult data);

        public class SortingVM : ViewModelBase
        {
            /// <summary>Russian name of the sorting.</summary>
            public string Name { get; set; }
            /// <summary>Instructions for the sorting.</summary>
            public AnalysisSortFunc SortingFunction { get; set; }

            public SortingVM(string name, AnalysisSortFunc sortingFunction)
            {
                Name = name;
                SortingFunction = sortingFunction;
            }
        }

        /// <summary>Existing sortings.</summary>
        public List<SortingVM> Sortings { get; } = new List<SortingVM>
        {
            // Alphabetically
            new SortingVM("По алфавиту", // OrderByDescending is used because of upside down list inside the chart.
                (AnalysisResult analysisResult) => analysisResult.ToolsStatistics.OrderByDescending(toolKeyValue => toolKeyValue.Key)),
            
            // Values ascending
            new SortingVM("По возрастанию",
                (AnalysisResult analysisResult) => analysisResult.ToolsStatistics.OrderByDescending(toolKeyValue => toolKeyValue.Value.Sum(statKeyValue => statKeyValue.Value))),

            // Values descending
            new SortingVM("По убыванию",
                (AnalysisResult analysisResult) => analysisResult.ToolsStatistics.OrderBy(toolKeyValue => toolKeyValue.Value.Sum(statKeyValue => statKeyValue.Value)))
        };

        private SortingVM selectedSorting;
        /// <summary>Sorting selected from the view.</summary>
        public SortingVM SelectedSorting
        {
            get => selectedSorting;
            set
            {
                SetProperty(ref selectedSorting, value); // Raises PropertyChanged event to sync with view.
                SortAnalysisResult = SelectedSorting.SortingFunction;

                if (AnalysisVM.SelectedAnalysis == null) { return; }

                AnalysisVM.SelectedAnalysis.AnalysisCommand.Execute("");
            }
        }

        /// <summary>Sorting function of the selected sorting.</summary>
        private static AnalysisSortFunc SortAnalysisResult { get; set; }

        /// <summary>Series of the chart (pie, rows, columns etc.).</summary>
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        /// <summary>Replaces chart series with new row series. Sets needed values for the series.</summary>
        /// <param name="toolsData">A data class object where the information will be taken from</param>
        /// <param name="statisticName">Analysed statistic name.</param>
        public void CreateRowChart(AnalysisResult analysisResult)
        {
            // Clear old information.
            if (SeriesCollection != null) { SeriesCollection.Clear(); }

            // Get and sort new information.
            var resultStats = SortAnalysisResult(analysisResult);

            if (!analysisResult.MultipleStatistics) // Create a normal single row series if there's only 1 statistic to show. 
            {
                var stats = resultStats.Select(toolKeyValue => toolKeyValue.Value.First().Value);
                SeriesCollection.Add(new RowSeries
                {
                    Title = resultStats.First().Value.First().Key, // Get the statistic name from one the data.
                    Values = new ChartValues<int>(stats)
                });
            }
            else // Create row series with information stacked in each row.
            {
                List<string> statsNames = new List<string>(); 

                // Find out which values were analysed(number of series equals number of these values).
                foreach (var toolKeyValue in resultStats)
                {
                    var toolStats = toolKeyValue.Value;

                    foreach (var statKeyValue in toolStats)
                    {
                        if (statsNames.Contains(statKeyValue.Key)) { continue; }

                        statsNames.Add(statKeyValue.Key);
                    }
                }

                // Collect info about each statistic for each tool
                foreach (var statName in statsNames)
                {
                    List<int> statValues = new List<int>(); // Values for the chart series

                    foreach (var toolKeyValue in resultStats)
                    {
                        var toolStats = toolKeyValue.Value;

                        statValues.Add(toolStats.ContainsKey(statName) ? toolStats[statName] : 0);
                    }

                    SeriesCollection.Add(new StackedRowSeries
                    {
                        Title = statName,
                        Values = new ChartValues<int>(statValues)
                    });
                }
            }

            var toolsNames = resultStats.Select(toolKeyValue => toolKeyValue.Key);
            Labels = new ObservableCollection<string>(toolsNames);

            ChartHeight = 20 * Labels.Count;
        }

        public ResultsViewVM()
        {
            Instance = this;

            SelectedSorting = Sortings.First();
        }
    }
}
