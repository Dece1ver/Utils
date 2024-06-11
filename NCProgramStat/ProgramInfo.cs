using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCProgramStat
{
    public class ProgramInfo
    {
        public ProgramInfo(DateTime date, string machine, string shift, string @operator, string partName, int setup, string status)
        {
            Date = date;
            Machine = machine;
            Shift = shift;
            Operator = @operator;
            PartName = partName;
            Setup = setup;
            Status = status;
        }

        public DateTime Date { get; set; }
        public string Machine { get; set; }
        public string Shift { get; set; }
        public string Operator { get; set; }
        public string PartName { get; set; }
        public int Setup { get; set; }
        public string Status { get; set; }
    }
}
