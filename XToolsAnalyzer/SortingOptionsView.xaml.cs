using System.Windows.Controls;
using XToolsAnalyzer.ViewModel;

namespace XToolsAnalyzer
{
    public partial class SortingOptionsView : UserControl
    {
        public SortingOptionsView()
        {
            InitializeComponent();
            DataContext = new SortingOptionsVM();
        }
    }
}
