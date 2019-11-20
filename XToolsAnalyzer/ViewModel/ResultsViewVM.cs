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


        private bool welcomeTextVisibility = true;
        /// <summary>Text which is shown when the chart is hidden.</summary>
        public bool WelcomeTextVisibility => welcomeTextVisibility;

        private bool chartVisibility = false;
        /// <summary>Is used to hide chart if it is empty.</summary>
        public bool ChartVisibility
        {
            get => chartVisibility;
            set
            {
                SetProperty(ref chartVisibility, value);
                SetProperty(ref welcomeTextVisibility, !value, "WelcomeTextVisibility");
            }
        }

        private ObservableCollection<string> labels = new ObservableCollection<string>();
        /// <summary>Labels for the chart's y axis.</summary>
        public ObservableCollection<string> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value); // Raises PropertyChanged event to sync with the view.
        }

        private int chartHeight = 1000;
        /// <summary>Height of the chart control on the view.</summary>
        public int ChartHeight
        {
            get => chartHeight;
            set => SetProperty(ref chartHeight, value); // Raises PropertyChanged event to sync with the view.
        }

        private RelayCommand xAxisRangeChangedCommand;
        /// <summary>Forces the x axis to start at 0 if the user zooms horizontally.</summary>
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

        /// <summary>Series of the chart.</summary>
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        /// <summary>Contains data needed for creating a chart.</summary>
        private class ChartData
        {
            public struct Series
            {
                public string Name;
                public int[] Values;
            }

            /// <summary>Chart labels.</summary>
            public string[] Labels;
            /// <summary>Chart series.</summary>
            public List<Series> SeriesList = new List<Series>();

            public ChartData(AnalysisResult analysisResult)
            {
                var resultStats = analysisResult.Data;

                // Get names of data groups and place them as labels in the chart.
                Labels = resultStats.Select(group => group.Name).ToArray();

                List<string> seriesNames = new List<string>();

                // Find out which data series an analysis gave us.
                foreach (var group in resultStats)
                {
                    var objStats = group.Statistics;
                    foreach (var statKeyValue in objStats)
                    {
                        if (seriesNames.Contains(statKeyValue.Key)) { continue; }

                        seriesNames.Add(statKeyValue.Key);
                    }
                }

                // Collect info about each statistic in each group.
                foreach (var name in seriesNames)
                {
                    int[] values = new int[Labels.Length];

                    int i = 0;
                    foreach (var group in resultStats)
                    {
                        var objStats = group.Statistics;

                        values[i] = objStats.ContainsKey(name) ? objStats[name] : 0;
                        i++;
                    }

                    SeriesList.Add(new Series { Name = name, Values = values });
                }
            }
        }

        /// <summary>Replaces chart series with new row series.</summary>
        /// <param name="analysisResult">A data class object where the information will be taken from</param>
        public void CreateRowChart(AnalysisResult analysisResult)
        {
            // Clear the old information.
            if (SeriesCollection != null) { SeriesCollection.Clear(); }

            // If it's the first chart in this session, show the chart view because it is hidden by default
            if (!ChartVisibility) { ChartVisibility = true; }

            ChartData data = new ChartData(analysisResult);

            Labels = new ObservableCollection<string>(data.Labels);

            // Create a normal row series if there's only 1 series to show or if a user told not to stack series.
            if (OptionsVM.Instance.SeparateSeries || data.SeriesList.Count == 1) 
            {
                // Add row series
                foreach (var series in data.SeriesList)
                {
                    SeriesCollection.Add(new RowSeries
                    {
                        Title = series.Name,
                        Values = new ChartValues<int>(series.Values)
                    });
                }

                ChartHeight = 20 * Labels.Count * data.SeriesList.Count;
            }
            else // Create stacked row series.
            {
                // Add stacked row series
                foreach (var series in data.SeriesList)
                {
                    SeriesCollection.Add(new StackedRowSeries
                    {
                        Title = series.Name,
                        Values = new ChartValues<int>(series.Values)
                    });
                }

                ChartHeight = 20 * Labels.Count;
            }
        }

        public ResultsViewVM()
        {
            Instance = this;
        }
    }
}
