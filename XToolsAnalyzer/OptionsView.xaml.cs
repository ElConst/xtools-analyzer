using System.Windows.Controls;
using XToolsAnalyzer.ViewModel;

namespace XToolsAnalyzer
{
    public partial class OptionsView : UserControl
    {
        public OptionsView()
        {
            InitializeComponent();
            DataContext = new OptionsVM();
        }
    }
}
