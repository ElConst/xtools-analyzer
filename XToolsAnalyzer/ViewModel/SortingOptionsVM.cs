using System.Collections.ObjectModel;
using System.Linq;
using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Viewmodel containing logic for the 'SortingOptionsView'.</summary>
    public class SortingOptionsVM : ViewModelBase
    {
        public static SortingOptionsVM Instance;
        public SortingOptionsVM() => Instance = this;

        private ObservableCollection<string> sortingKeys;
        /// <summary>Variants of what to sort the data by (e.g. labels, series values etc).</summary>
        public ObservableCollection<string> SortingKeys
        {
            get => sortingKeys;
            set => SetProperty(ref sortingKeys, value); // Raises PropertyChanged event to sync with the view.
        }

        /// <summary>Name of a standart sorting key.</summary>
        private const string ObjNameKey = "Название", SeriesSumKey = "Сумма рядов";

        private string selectedSortKey = ObjNameKey;
        /// <summary>What to sort the data by.</summary>
        public string SelectedSortKey
        {
            get => selectedSortKey;
            set
            {
                SetProperty(ref selectedSortKey, value); // Raises PropertyChanged event to sync with the view.

                Sort(ref AnalysisVM.LastAnalysisResult);
                ResultsViewVM.Instance.RecreateLastChart();
            }
        }

        private bool sortDescending = false;
        /// <summary>Do sort values descending or not to do.</summary>
        public bool SortDescending
        {
            get => sortDescending;
            set
            { 
                SetProperty(ref sortDescending, value); // Raises PropertyChanged event to sync with the view.

                Sort(ref AnalysisVM.LastAnalysisResult);
                ResultsViewVM.Instance.RecreateLastChart();
            }
        }
        /// <summary>Do sort values ascending or not to do.</summary>
        public bool SortAscending => !SortDescending;

        /// <summary>Sorts data inside an analysis result corresponding to options that were chosen in the view.</summary>
        /// <param name="analysisResult">An analysis result data of which should be sorted.</param>
        public void Sort(ref AnalysisResult analysisResult)
        {
            // Note: Opposite sortings (e.g. ascending if descending is selected) are used because of upside down list inside the chart.

            var data = analysisResult?.Data;
            if (data == null) { return; }
            
            switch (SelectedSortKey)
            {
                case ObjNameKey:
                    // If a sorting by name is selected, just sort by names of data groups
                    analysisResult.Data = 
                        (SortDescending ? data.OrderBy(group => group.Name) :
                                          data.OrderByDescending(group => group.Name)).ToArray();
                    return;

                case SeriesSumKey:
                    // If a sorting by sum of all series is selected,
                    // calculate value sums from stats dictionaries
                    // and sort by these sums
                    analysisResult.Data =
                        (SortDescending ? data.OrderBy(group => group.Statistics.Sum(stat => stat.Value)) :
                                          data.OrderByDescending(group => group.Statistics.Sum(stat => stat.Value))).ToArray();
                    break;

                default:
                    // Else assume that sorting keys are values of a series named SelectedSortKey and sort by those values
                    analysisResult.Data =
                        (SortDescending ? data.OrderBy(group => group.Statistics.ContainsKey(SelectedSortKey) ? group.Statistics[SelectedSortKey] : 0) :
                                          data.OrderByDescending(group => group.Statistics.ContainsKey(SelectedSortKey) ? group.Statistics[SelectedSortKey] : 0)).ToArray();
                    break;
            }
        }

        /// <summary>Updates sorting options corresponding to results of an analysis.</summary>
        /// <param name="analysisResult">An analysis result that is needed to be sorted.</param>
        public void CreateOptionsToSortAnAnalysisResult(AnalysisResult analysisResult)
        {
            var keys = new ObservableCollection<string>();

            // Add a sorting by names of grouping objects by default since it can be applied to a result of any analysis
            keys.Add(ObjNameKey);
            // Add names of all data series to sorting keys to be able to sort by values of each of them
            analysisResult.SeriesNames.ForEach(dataSeriesName => keys.Add(dataSeriesName));
            // If there is more than one data series, add an option to sort by sum off all data series;
            if (analysisResult.SeriesNames.Count > 1) { keys.Add(SeriesSumKey); }

            // If the new set of sorting keys doesn't contain previous selected one, select sorting by names
            if (!keys.Contains(SelectedSortKey)) { SelectedSortKey = ObjNameKey; }

            SortingKeys = keys;
        }
    }
}
