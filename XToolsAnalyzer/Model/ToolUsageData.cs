using System.Collections.Generic;

namespace XToolsAnalyzer.Model
{
    /// <summary>Class containing info about usage of a tool.</summary>
    public class ToolUsageData
    {
        public string ToolName;

        public Dictionary<string, string> CommandStats = new Dictionary<string, string>();
        public Dictionary<string, string> ExecutorStats = new Dictionary<string, string>();
        public Dictionary<string, string> UIStats = new Dictionary<string, string>();
        
        public ToolUsageData(string toolName) => ToolName = toolName;
    }
}
