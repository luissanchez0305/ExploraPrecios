using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Core.Helper
{
    public static class DecimalExtensions
    {
        public static int RoundToInteger(this float n)
        { return RoundNumber(n); }

        public static int RoundToInteger(this decimal n)
        { return RoundNumber(n); }

        private static int RoundNumber(object n)
        {
            var intValue = Convert.ToInt32(n).ToString();
            var firstValue = Convert.ToInt32(n).ToString().ToCharArray()[0];

            var stringValue = (intValue.ToString().Length > 1 ? firstValue.ToString().PadRight(intValue.Length, '0') : firstValue.ToString());
            return Convert.ToInt32(stringValue);
        }
    }
}
