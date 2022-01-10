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
        public string Login { get; set; }
        public string Pass { get; set; }

        public Settings(string dinnerName = "Обед", string workName = "1.04. Общепроизводственая", int dinnerTime = 60, bool autoConfim = false, string login = "", string pass = "")
        {
            DinnerName = dinnerName;
            WorkName = workName;
            DinnerTime = dinnerTime;
            AutoConfim = autoConfim;
            Login = login;
            Pass = pass;
        }
    }
}
