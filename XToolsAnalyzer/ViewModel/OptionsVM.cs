namespace XToolsAnalyzer.ViewModel
{
    /// <summary>Viewmodel containing logic for the 'OptionsView'</summary>
    public class OptionsVM : ViewModelBase
    {
        public static OptionsVM Instance;
        public OptionsVM() => Instance = this;


        private RelayCommand openFilterWindowCommand;
        /// <summary>Shows the filter window.</summary>
        public RelayCommand OpenFilterWindowCommand
        {
            get
            {
                return openFilterWindowCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (openFilterWindowCommand = new RelayCommand(args =>
                {
                    FilterWindow.Instance.Show();
                }));
            }
        }

        private bool groupingComboBoxEnabled;
        /// <summary>Tells if user can choose a data grouping mode from the view.</summary>
        public bool GroupingComboBoxEnabled
        {
            get => groupingComboBoxEnabled;
            set => SetProperty(ref groupingComboBoxEnabled, value);
        }

        private string[] groupings;
        /// <summary>Data grouping modes available for the selected analysis.</summary>
        public string[] Groupings
        {
            get => groupings;
            set
            {
                SetProperty(ref groupings, value); // Raises PropertyChanged event to sync with the view.

                GroupingComboBoxEnabled = Groupings.Length > 1;
            }
        }

        private string selectedGrouping;
        /// <summary>The grouping selected from the view.</summary>
        public string SelectedGrouping
        {
            get => selectedGrouping;
            set
            {
                SetProperty(ref selectedGrouping, value); // Raises PropertyChanged event to sync with the view.
                AnalysisVM.SelectedAnalysis.Analysis.SelectedGrouping = selectedGrouping;

                AnalysisVM.MakeSelectedAnalysis();
            }
        }

        private bool separateSeries;
        /// <summary>Tells if we do not need to stack chart series but to show them separately instead.</summary>
        public bool SeparateSeries
        {
            get => separateSeries;
            set
            {
                SetProperty(ref separateSeries, value); // Raises PropertyChanged event to sync with the view.

                AnalysisVM.MakeSelectedAnalysis();
            }
        }
    }
}
