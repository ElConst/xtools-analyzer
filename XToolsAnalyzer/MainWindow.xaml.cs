using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XToolsAnalyzer.ViewModel;

namespace XToolsAnalyzer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var viewModel = new MainVM();
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
