using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassCalc
{
    public static class Util
    {
        public enum GetNumberOption { Any, OnlyPositive }

        /// <summary>
        /// Получает число из строки
        /// </summary>
        /// <param name="stringNumber">Строка для получения</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="numberOption">Возвращаемое значение: только положительное или любое</param>
        /// <returns>Значение Double, при неудаче возвращает значение по умолчанию</returns>
        public static double GetDouble(this string stringNumber, double defaultValue = 0, GetNumberOption numberOption = GetNumberOption.OnlyPositive)
        {
            NumberFormatInfo numberFomat = new() { NumberDecimalSeparator = "," };
            if (Double.TryParse(stringNumber, NumberStyles.Any, numberFomat, out double result))
            {
                if (numberOption == GetNumberOption.OnlyPositive && result > 0)
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Получает число из строки
        /// </summary>
        /// <param name="stringNumber">Строка для получения</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="numberOption">Возвращаемое значение: только положительное или любое</param>
        /// <returns>Значение Int32, при неудаче возвращает значение по умолчанию</returns>
        public static int GetInt(this string stringNumber, int defaultValue = 0, GetNumberOption numberOption = GetNumberOption.OnlyPositive)
        {
            NumberFormatInfo numberFomat = new() { NumberDecimalSeparator = "," };
            if (Int32.TryParse(stringNumber, NumberStyles.Any, numberFomat, out int result))
            {
                if (numberOption == GetNumberOption.OnlyPositive && result > 0)
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

    }
}
