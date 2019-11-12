using System.Collections.Generic;
using System.Linq;
using XToolsAnalyzer.Model;

namespace XToolsAnalyzer.ViewModel
{
    public class FilterVM : ViewModelBase 
    {
        private bool mainUIVisibility = true;
        /// <summary>Visibility of the main filter.</summary>
        public bool MainUIVisibility 
        {
            get => mainUIVisibility;
            set => SetProperty(ref mainUIVisibility, value); // Raises PropertyChanged event to sync with the view.
        }

        private bool toolsSelectUIVisibility = false;
        /// <summary>Visibility of the tools selection menu.</summary>
        public bool ToolsSelectUIVisibility
        {
            get => toolsSelectUIVisibility;
            set => SetProperty(ref toolsSelectUIVisibility, value); // Raises PropertyChanged event to sync with the view.
        }

        /// <summary>ViewModel that contains name of a tool and tells if it is selected for the tools filter.</summary>
        public class ToolSelection : ViewModelBase
        { 
            public string Name { get; set; }

            private bool selected;
            public bool Selected 
            {
                get => selected;
                set => SetProperty(ref selected, value); // Raises PropertyChanged event to sync with the view.
            }
        }

        private ToolSelection[] toolsFilter;
        /// <summary>Collection of tool names with bool variables telling if the tool is selected for the filter or not.</summary>
        public ToolSelection[] ToolsFilter
        {
            get => toolsFilter;
            set => SetProperty(ref toolsFilter, value); // Raises PropertyChanged event to sync with the view.
        }

        private RelayCommand openToolsFilterCommand;
        /// <summary>Open tools filter menu and hide main filter.</summary>
        public RelayCommand OpenToolsFilterCommand
        {
            get
            {
                return openToolsFilterCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (openToolsFilterCommand = new RelayCommand(args =>
                {
                    // Get the current tools filter to show it in the view
                    ToolsFilter = Filter.Instance.ToolsFilter
                        .Select(keyValue => new ToolSelection() { Name = keyValue.Key, Selected = keyValue.Value })
                        .OrderBy(t => t.Name).ToArray();

                    SwitchToolsFilterVisibility(); // Hide the main filter, the show tools filter
                }));
            }
        }

        private RelayCommand closeToolsFilterCommand;
        /// <summary>Close tools filter menu and show main filter after saving changes.</summary>
        public RelayCommand CloseToolsFilterCommand
        {
            get
            {
                return closeToolsFilterCommand ?? // Make sure that the command's backing field was initialized and return it. 
                (closeToolsFilterCommand = new RelayCommand(args =>
                {
                    // Update the tools filter inside the model
                    Filter.Instance.ToolsFilter = ToolsFilter.ToDictionary(t => t.Name, t => t.Selected);

                    SwitchToolsFilterVisibility(); // Hide the tools filter, show the main filter
                }));
            }
        }

        /// <summary>Swaps visibility of the main filter UI and the tools filter UI.</summary>
        private void SwitchToolsFilterVisibility()
        {
            ToolsSelectUIVisibility = MainUIVisibility;
            MainUIVisibility = !MainUIVisibility;
        }
    }
}
