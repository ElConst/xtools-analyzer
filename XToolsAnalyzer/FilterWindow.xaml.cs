using System.Windows;
using XToolsAnalyzer.ViewModel;

namespace XToolsAnalyzer
{
    public partial class FilterWindow : Window
    {
        private static FilterWindow instance;
        /// <summary>The only instance of the window.</summary>
        public static FilterWindow Instance
        {
            get
            {
                if (instance == null) { instance = new FilterWindow(); }

                return instance;
            }
        }

        public FilterWindow()
        {
            InitializeComponent();
            DataContext = new FilterVM();
        }
    }
}
