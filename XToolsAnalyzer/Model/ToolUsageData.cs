using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XToolsAnalyzer.Model
{
    public class ToolUsageData
    {
        public string ToolName;

        public Dictionary<string, string> CommandStats = new Dictionary<string, string>();
        public Dictionary<string, string> ExecutorStats = new Dictionary<string, string>();
        public Dictionary<string, string> UIStats = new Dictionary<string, string>();
        
        public ToolUsageData(string toolName) => ToolName = toolName;
    }
}
