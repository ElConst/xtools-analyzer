﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using XToolsAnalyzer.Model;
using System.Collections.ObjectModel;

namespace XToolsAnalyzer.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<ToolStatisticVM> toolsStatistics = new ObservableCollection<ToolStatisticVM>();

        public MainVM()
        {
            DataHandler.CollectDataFromFolder(@"D:\XTools");

            var toolsStatsIEnum = ClicksAnalysis.Instance.GetAnalysisResult().Select(toolStatPair => new ToolStatisticVM(toolStatPair.Key, toolStatPair.Value));
            toolsStatistics = new ObservableCollection<ToolStatisticVM>(toolsStatsIEnum);
        }
    }

    public class ToolStatisticVM
    {
        public string Name { get; }
        public float Statistic { get; }

        public ToolStatisticVM(string name, float statistic)
        {
            Name = name;
            Statistic = statistic;
        }
    }
}
