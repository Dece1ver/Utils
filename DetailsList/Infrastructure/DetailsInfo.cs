using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetailsList.Infrastructure
{
    public static class DetailsInfo
    {
        public static readonly string[] numberSignsAreopag = new[] {
            "АР", "АРМ", "АРКП", "АРПГА", "АРКО", "АРНП", "АТГ", "НМГ", "АМГ", "АРН", "АРС", "АРФС", "М8Л", "ТОС", "AP", // наше
        };

        public static readonly string[] numberSignsThird = new[] {
            "ТОМ3", "ИН0", "ИНО", // кооперация
        };

        public static readonly string[] numberSignsGost = new[] {
            "10-", "15-", "25-", "32-", "40-", "50-", // фланцы гост
        };

        public static readonly string[] numberSigns = Array.Empty<string>()
            .Concat(numberSignsAreopag)
            .Concat(numberSignsThird)
            .Concat(numberSignsGost).ToArray();

        public static readonly string[] trimmableEndings = new[] {
            "ZAG", "PRED", "PER", "POS", "POC", "TO", "OP", "SB", "DO FR", "STAR", "NOV", 
            "BAZ", "TIT", "FOA", "CHI", "CHERN", "VAR", "VAP", " L-", "SV", "UST", "ST",
            "BRONZA", "P FR", "PO FR", "TP", "KANA", "SHLIF", "TPZ", "TR", "POD", " V", 
            "1U", "1S", "1Y", "1C",
            "2U", "2S", "2Y", "2C",
            "3U", "3S", "3Y", "3C",
            "4U", "4S", "4Y", "4C",
            "5U", "5S", "5Y", "5C",
            "6U", "6S", "6Y", "6C", 
        };

        public static string TranslateFromEnNumber(this string name)
        {
            if (name.Contains("C 15-"))
            {

            }
            return name.ToUpper()
                .Replace("NAR", "")
                .Replace("NAP", "")
                .Replace("AR", "АР")
                .Replace("AP", "АР")
                .Replace("ARM", "АРМ")
                .Replace("APM", "АРМ")
                .Replace("ARS", "АРС")
                .Replace("APS", "АРС")
                .Replace("KP", "КП")
                .Replace("KO", "КО")
                .Replace("NP", "НП")
                .Replace("HP", "НП")
                .Replace("FS", "ФС")
                .Replace("FC", "ФС")
                .Replace("PGA", "ПГА")
                .Replace("PGA", "ПГА")
                .Replace("ATG", "АТГ")
                .Replace("AMG", "АМГ")
                .Replace("ARN", "АРН")
                .Replace("APN", "АРН")
                .Replace("NMG", "НМГ")
                .Replace("M8L", "М8Л")
                .Replace("M8", "М8")
                .Replace("L1", "L")
                .Replace("L2", "L")
                .Replace("АП", "АР")
                .Replace("АП", "АР");
        }

        public static string FindNumber(this string name)
        {
            
            bool fileOk = false;
            bool gost = true;
            for (int i = 0; i < 3; i++)
            {
                foreach (var ending in trimmableEndings)
                {
                    if (name.Contains(ending)) name = name.Remove(name.IndexOf(ending));
                    if (name.Trim().EndsWith(ending)) name = name.Remove(name.IndexOf(ending));
                }
                foreach (var endSetups in new string[] {" 1", " 2", " 3", " 4", " 5", " 6", ".1", ".2", ".3", ".4", ".5", ".6", ".FREZEROVKA", ".FREZ" })
                {
                    if (name.Trim().EndsWith(endSetups)) name = name.Remove(name.IndexOf(endSetups));
                }
            }

            for (int i = 0; i < 3; i++)
            {
                foreach (var sign in numberSignsAreopag.Concat(numberSignsThird))
                {
                    if (name.Contains(sign) && sign != "М8Л")
                    {
                        name = name[name.IndexOf(sign)..];
                        gost = false;
                        fileOk = true;
                    }
                }
            }

            if (gost)
            {
                for (int i = 0; i < 3; i++)
                {
                    foreach (var sign in numberSignsGost)
                    {
                        if (name.Contains(sign))
                        {
                            name = name[name.IndexOf(sign)..];
                            fileOk = true;
                        }
                    }
                }
            }

            if (fileOk) return name.Trim().Trim('.');
            return string.Empty;
        }
    }
}
