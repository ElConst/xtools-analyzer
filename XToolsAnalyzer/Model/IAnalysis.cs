using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XToolsAnalyzer.Model
{
    public interface IAnalysis
    {
        string Name { get; }

        Dictionary<string, float> GetAnalysisResult();
    }

    public class ClicksAnalysis : IAnalysis
    {
        private static ClicksAnalysis instance;
        public static ClicksAnalysis Instance
        {
            get
            {
                if (instance == null) { instance = new ClicksAnalysis(); }
                return instance;
            }
        }

        public string Name { get; } = "Кол-во запусков";

        public Dictionary<string, float> GetAnalysisResult()
        {
            Dictionary<string, float> toolsClicks = new Dictionary<string, float>();

            foreach (StatisticsReport report in DataHandler.FilteredData)
            {
                foreach (ToolUsageData tool in report.ToolsUsed)
                {
                    if (!tool.CommandStats.ContainsKey("Click")) { continue; }

                    // TODO: Translate tool names to russian

                    int clicksFromReport = int.Parse(tool.CommandStats["Click"]);

                    if (toolsClicks.ContainsKey(tool.ToolName))
                    {
                        toolsClicks[tool.ToolName] += clicksFromReport;
                    }
                    else
                    {
                        toolsClicks.Add(tool.ToolName, clicksFromReport);
                    }
                }
            }

            return toolsClicks;
        }
    }
}
