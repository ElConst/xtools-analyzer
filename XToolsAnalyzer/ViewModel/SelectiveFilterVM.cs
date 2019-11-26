using System.Collections.Generic;
using System.Linq;

namespace XToolsAnalyzer.ViewModel
{
    public class SelectiveFilterVM : ViewModelBase
    {
        public static SelectiveFilterVM Instance;
        public SelectiveFilterVM()
        {
            Instance = this;
        }

        private bool visibility = false;
        /// <summary>Visibility of this view.</summary>
        public bool Visibility
        {
            get => visibility;
            set => SetProperty(ref visibility, value); // Raises PropertyChanged event to sync with the view.
        }

        private string title;
        /// <summary>The title of the filter in the view.</summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        /// <summary>Contains name of some selectable thing from the filter and tells if it's selected at the moment.</summary>
        public class Selectable : ViewModelBase
        {
            public string Name { get; set; }

            private bool selected;
            public bool Selected
            {
                get => selected;
                set => SetProperty(ref selected, value); // Raises PropertyChanged event to sync with the view.
            }
        }

        private Selectable[] filter;
        /// <summary>A copy of the given filter to modify in the view.</summary>
        public Selectable[] Filter
        {
            get => filter;
            set => SetProperty(ref filter, value); // Raises PropertyChanged event to sync with the view.
        }

        /// <summary>Puts data from a filter to the view so it could be modified by user.</summary>
        public void SetFilter(Dictionary<string, bool> filterToModify)
        {
            Filter = filterToModify
                        .Select(keyValue => new Selectable() { Name = keyValue.Key, Selected = keyValue.Value })
                        .OrderBy(t => t.Name).ToArray();
        }

        /// <summary>Converts the contained filter to a dictionary and returns it.</summary>
        public Dictionary<string, bool> GetFilter() => Filter.ToDictionary(t => t.Name, t => t.Selected);

        private RelayCommand selectAll;
        /// <summary>Selects all entries in the filter.</summary>
        public RelayCommand SelectAll
        {
            get
            {
                return selectAll ?? // Make sure that the command's backing field was initialized and return it. 
                (selectAll = new RelayCommand(args =>
                {
                    foreach (var entry in Filter) { entry.Selected = true; }
                }));
            }
        }

        private RelayCommand clearSelection;
        /// <summary>Unselects all entries in the filter.</summary>
        public RelayCommand ClearSelection
        {
            get
            {
                return clearSelection ?? // Make sure that the command's backing field was initialized and return it. 
                (clearSelection = new RelayCommand(args =>
                {
                    foreach (var entry in Filter) { entry.Selected = false; }
                }));
            }
        }

        private RelayCommand cancelChanges;
        /// <summary>Hide selective filter without saving changes.</summary>
        public RelayCommand CancelChanges
        {
            get
            {
                return cancelChanges ?? // Make sure that the command's backing field was initialized and return it. 
                (cancelChanges = new RelayCommand(args =>
                {
                    // Hide the selective filter, show the main filter
                    FilterVM.Instance.SwitchSelectiveFilterVisibility();
                }));
            }
        }

        private RelayCommand close;
        /// <summary>Hides this view and shows the main filter after saving changes.</summary>
        public RelayCommand Close
        {
            get
            {
                return close ?? // Make sure that the command's backing field was initialized and return it. 
                (close = new RelayCommand(args =>
                {
                    // Update modified filter inside the model
                    FilterVM.Instance.SaveSelectiveFilterChanges();

                    // Make last analysis again with new filter
                    AnalysisVM.MakeSelectedAnalysis();

                    // Hide the selective filter view, show the main filter
                    FilterVM.Instance.SwitchSelectiveFilterVisibility();
                }));
            }
        }
    }
}
