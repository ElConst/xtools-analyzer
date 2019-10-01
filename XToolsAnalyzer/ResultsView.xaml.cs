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
    }
}
