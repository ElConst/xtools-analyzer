using System;
using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Viewmodel containing logic for the 'FilterWindow' and representing the filter for the view.</summary>
    public class FilterVM : ViewModelBase
    {
        private bool mainUIVisibility = true;
        /// <summary>Visibility of the main filter menu.</summary>
        public bool MainUIVisibility
        {
            get => mainUIVisibility;
            set => SetProperty(ref mainUIVisibility, value); // Raises PropertyChanged event to sync with the view.
        }

        #region Products

        private bool xtoolsAgpSelected;
        /// <summary>Is "XTools AGP" checked in the filter view.</summary>
        public bool XToolsAgpSelected
        {
            get => xtoolsAgpSelected;
            set
            {
                SetProperty(ref xtoolsAgpSelected, value); // Raises PropertyChanged event to sync with the view.
                Filter.ShowXToolsAgp = value;

                // Make last analysis again with new filter
                AnalysisVM.MakeSelectedAnalysis();
            }
        }

        private bool xtoolsProSelected;
        /// <summary>Is "XTools Pro" checked in the filter view.</summary>
        public bool XToolsProSelected
        {
            get => xtoolsProSelected;
            set
            {
                SetProperty(ref xtoolsProSelected, value); // Raises PropertyChanged event to sync with the view.
                Filter.ShowXToolsPro = value;

                // Make last analysis again with new filter
                AnalysisVM.MakeSelectedAnalysis();
            }
        }

        #endregion

        #region Date

        private DateTime startDate = DateTime.Now;
        /// <summary>Defines the lower border of suitable report dates.</summary>
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                DateTime valueToSet = value;
                // Make sure that the start date won't be later than the end date
                if (DateTime.Compare(valueToSet, EndDate) > 0) { valueToSet = EndDate; }

                SetProperty(ref startDate, valueToSet); // Raises PropertyChanged event to sync with the view.
                Filter.StartDate = valueToSet;

                // Make last analysis again with new filter
                AnalysisVM.MakeSelectedAnalysis();
            }
        }

        private DateTime endDate = DateTime.Now;
        /// <summary>Defines the upper border of suitable report dates.</summary>
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                DateTime valueToSet = value;
                // Make sure that the end date won't be earlier than the start date
                if (DateTime.Compare(valueToSet, StartDate) < 0) { valueToSet = StartDate; }

                SetProperty(ref endDate, valueToSet); // Raises PropertyChanged event to sync with the view.
                Filter.EndDate = valueToSet;

                // Make last analysis again with new filter
                AnalysisVM.MakeSelectedAnalysis();
            }
        }

        private RelayCommand clearDateFilter;
        /// <summary>Changes the date filter back to the whole range of dates.</summary>
        public RelayCommand ClearDateFilter
        {
            get
            {
                return clearDateFilter ?? // Make sure that the command's backing field was initialized and return it. 
                (clearDateFilter = new RelayCommand(args => 
                {
                    Filter.ClearDateFilter();
                    SyncWithModel();

                    // Make last analysis again with new filter
                    AnalysisVM.MakeSelectedAnalysis();
                }));            
            }
        }

        #endregion

        private bool toolsFilterOpened = false;

        private RelayCommand openToolsFilter;
        /// <summary>Opens tools filter wrapped in the selective filter view.</summary>
        public RelayCommand OpenToolsFilter
        {
            get
            {
                return openToolsFilter ?? // Make sure that the command's backing field was initialized and return it. 
                (openToolsFilter = new RelayCommand(args =>
                {
                    // Put the tools filter to the selective filter view so it could be modified
                    SelectiveFilterVM.Instance.SetFilter(Filter.ToolsFilter);

                    SelectiveFilterVM.Instance.Title = "Фильтр по инструментам";
                    toolsFilterOpened = true;

                    // Hide the main filter, show the selective filter with this tools filter data
                    SwitchSelectiveFilterVisibility();
                }));
            }
        }

        private bool versionsFilterOpened = false;

        private RelayCommand openVersionsFilter;
        /// <summary>Opens product versions filter wrapped in the selective filter view.</summary>
        public RelayCommand OpenVersionsFilter
        {
            get
            {
                return openVersionsFilter ?? // Make sure that the command's backing field was initialized and return it. 
                (openVersionsFilter = new RelayCommand(args =>
                {
                    // Put the versions filter to the selective filter view so it could be modified
                    SelectiveFilterVM.Instance.SetFilter(Filter.VersionsFilter);

                    SelectiveFilterVM.Instance.Title = "Фильтр по версиям продукта";
                    versionsFilterOpened = true;

                    // Hide the main filter, show the selective filter with this versions filter data
                    SwitchSelectiveFilterVisibility();
                }));
            }
        }

        /// <summary>Swaps visibility of the main filter UI and the selective filter UI.</summary>
        public void SwitchSelectiveFilterVisibility()
        {
            SelectiveFilterVM.Instance.Visibility = MainUIVisibility;
            MainUIVisibility = !MainUIVisibility;

            // If the selective filter is being closed, mark as closed all filters which are edited inside it
            if (!SelectiveFilterVM.Instance.Visibility)
            {
                toolsFilterOpened = versionsFilterOpened = false;
            }
        }

        /// <summary>Apply changes made in the selective filter view to the filter (from the model) which have been edited.</summary>
        public void SaveSelectiveFilterChanges()
        {
            if (toolsFilterOpened) { Filter.ToolsFilter = SelectiveFilterVM.Instance.GetFilter(); }
            else if (versionsFilterOpened) { Filter.VersionsFilter = SelectiveFilterVM.Instance.GetFilter(); }
        }

        /// <summary>Synchronizes properties of this VM with corresponding properties of the filter model.</summary>
        private void SyncWithModel()
        {
            XToolsAgpSelected = Filter.ShowXToolsAgp;
            XToolsProSelected = Filter.ShowXToolsPro;

            StartDate = Filter.StartDate;
            EndDate = Filter.EndDate;
        }

        public static FilterVM Instance;
        public FilterVM()
        {
            Instance = this;

            SyncWithModel();
        }
    }
}
