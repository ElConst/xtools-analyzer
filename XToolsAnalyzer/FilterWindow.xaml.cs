using System.Windows;

namespace XToolsAnalyzer
{
    public partial class FilterWindow : Window
    {
        private static FilterWindow instance;
        /// <summary>The only instance of the window.</summary>
        public static FilterWindow Instance => instance ?? (instance = new FilterWindow());

        public FilterWindow()
        {
            InitializeComponent();

            Closed += (object sender, System.EventArgs e) => { instance = null; };
        }
    }
}
