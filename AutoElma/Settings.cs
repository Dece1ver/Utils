using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoElma
{
    public class Settings
    {
        public string DinnerName { get; set; }
        public string WorkName { get; set; }
        public int DinnerTime { get; set; }
        public bool AutoConfim { get; set; }

        public Settings(string dinnerName = "Обед", string workName = "Общепроизводственная деятельность", int dinnerTime = 30, bool autoConfim = false)
        {
            DinnerName = dinnerName;
            WorkName = workName;
            DinnerTime = dinnerTime;
            AutoConfim = autoConfim;
        }
    }
}
