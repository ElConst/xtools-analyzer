using System.Windows;
using XToolsAnalyzer.ViewModel;

namespace XToolsAnalyzer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainVM();
        }
    }
}
