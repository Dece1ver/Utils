using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsInfo.Models
{
    public class ToolsInfo
    {
        public ToolsInfo(string toolName, int count)
        {
            ToolName = toolName;
            Count = count;
        }

        public string ToolName { get; set; }
        public int Count { get; set; }


    }
}
