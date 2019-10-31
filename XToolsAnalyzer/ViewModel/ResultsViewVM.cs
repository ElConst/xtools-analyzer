﻿using LiveCharts;
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

        // Delegate type for a sorting function.
        public delegate IOrderedEnumerable<KeyValuePair<string, Dictionary<string, int>>> AnalysisSortFunc(AnalysisResult data);

        /// <summary>Represents a sorting for analyses results.</summary>
        public class SortingVM
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

        /// <summary>Available sortings.</summary>
        public List<SortingVM> Sortings { get; } = new List<SortingVM>
        {
            // Alphabetically
            new SortingVM("По алфавиту", // OrderByDescending is used because of upside down list inside the chart.
                (AnalysisResult analysisResult) => analysisResult.Statistics.OrderByDescending(toolKeyValue => toolKeyValue.Key)),
            
            // Values ascending
            new SortingVM("По возрастанию",
                (AnalysisResult analysisResult) => analysisResult.Statistics.OrderByDescending(toolKeyValue => toolKeyValue.Value.Sum(statKeyValue => statKeyValue.Value))),

            // Values descending
            new SortingVM("По убыванию",
                (AnalysisResult analysisResult) => analysisResult.Statistics.OrderBy(toolKeyValue => toolKeyValue.Value.Sum(statKeyValue => statKeyValue.Value)))
        };

        private SortingVM selectedSorting;
        /// <summary>The sorting selected from the view.</summary>
        public SortingVM SelectedSorting
        {
            get => selectedSorting;
            set
            {
                SetProperty(ref selectedSorting, value); // Raises PropertyChanged event to sync with the view.
                SortAnalysisResult = SelectedSorting.SortingFunction;

                if (AnalysisVM.SelectedAnalysis == null) { return; }

                AnalysisVM.SelectedAnalysis.AnalysisCommand.Execute("");
            }
        }

        /// <summary>The sorting function of the selected sorting.</summary>
        private static AnalysisSortFunc SortAnalysisResult { get; set; }

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
                // Sort the raw info at first.
                var resultStats = SortAnalysisResult(analysisResult);

                // Get names of objects which were analysed.
                Labels = resultStats.Select(objKeyValue => objKeyValue.Key).ToArray();

                List<string> seriesNames = new List<string>();

                // Find out which statistics were analysed (their names will be used for series names).
                foreach (var objKeyValue in resultStats)
                {
                    var objStats = objKeyValue.Value;
                    foreach (var statKeyValue in objStats)
                    {
                        if (seriesNames.Contains(statKeyValue.Key)) { continue; }

                        seriesNames.Add(statKeyValue.Key);
                    }
                }

                // Collect info about each statistic for each object.
                foreach (var name in seriesNames)
                {
                    int[] values = new int[Labels.Length];

                    int i = 0;
                    foreach (var objKeyValue in resultStats)
                    {
                        var objStats = objKeyValue.Value;

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

            ChartData data = new ChartData(analysisResult);

            if (data.SeriesList.Count == 1) // Create a normal row series if there's only 1 series to show. 
            {
                var series = data.SeriesList.First();

                SeriesCollection.Add(new RowSeries
                {
                    Title = series.Name,
                    Values = new ChartValues<int>(series.Values)
                }) ;
            }
            else // Create stacked row series.
            {
                // Collect info about each statistic for each tool
                foreach (var series in data.SeriesList)
                {
                    SeriesCollection.Add(new StackedRowSeries
                    {
                        Title = series.Name,
                        Values = new ChartValues<int>(series.Values)
                    });
                }
            }

            Labels = new ObservableCollection<string>(data.Labels);

            ChartHeight = 20 * Labels.Count;
        }

        public ResultsViewVM()
        {
            Instance = this;

            SelectedSorting = Sortings.First();
        }
    }
}
