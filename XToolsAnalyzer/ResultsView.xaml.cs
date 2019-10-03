using LiveCharts.Events;
using LiveCharts.Wpf;
using System.Windows.Controls;
using XToolsAnalyzer.ViewModel;

namespace XToolsAnalyzer
{
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
            DataContext = new ResultsViewVM();
        }

        /// <summary>Forces the x axis scale to start at 0 no matter how does the user zoom.</summary>
        public void XAxisOnRangeChanged(RangeChangedEventArgs args) => ((Axis)args.Axis).MinValue = 0;

        // TODO: Wrap the method into a command and place it in the ResultsViewVM
    }
}
