using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetailsList.Infrastructure
{
    public static class DetailsInfo
    {
        public static readonly string[] numberSigns = new[] { 
            "АР", "АРМ", "АРКП", "АРПГА", "АРКО", "АРНП", "АТГ", "НМГ", "АМГ", "АРН", "АРС", "АРФС", "М8Л", "ТОС", "AP", // наше
            "ТОМ3", "ИН0", "ИНО", // кооперация
            "10-", "15-", "25-", "32-", "40-", "50-", // фланцы гост
        };
    }
}
