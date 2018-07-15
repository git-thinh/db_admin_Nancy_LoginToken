using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace host
{
    public class msgConverter
    {
        /// <summary>
        /// Convert date string array: 151019;151020 => 292;293
        /// </summary>
        /// <param name="array_date">151019;151020</param>
        /// <returns></returns>
        public static int[] f_DateKeyArrayToIndexCache(string array_date)
        {
            int[] a = array_date.Split(';')
                .Select(x => f_DateKeyToIndexCache(x.TryParseToInt()))
                .Where(x => x > -1)
                .ToArray();
            return a;
        }

        public static List<Tuple<byte, int>> f_DateKeyArrayToIndexCacheYY(string array_date)
        {
            var a = array_date.Split(';')
                .Select(x => new Tuple<byte, int>(x.Substring(0, 2).TryParseToByte(), f_DateKeyToIndexCache(x.TryParseToInt())))
                .Where(x => x.Item1 > 0 && x.Item2 > -1)
                .ToList();
            return a;
        }

        public static int f_DateKeyToIndexCache(int yyMMdd)
        {
            int k = -1;

            string date = yyMMdd.ToString();
            if (date.Length > 5)
            {
                int yy = ("20" + date.Substring(0, 2)).TryParseToInt(),
                    mm = date.Substring(2, 2).TryParseToInt(),
                    dd = date.Substring(4, 2).TryParseToInt();

                if (yy > 0 && mm > 0 && dd > 0)
                    k = new DateTime(yy, mm, dd).DayOfYear - 1;
            }

            return k;
        }

        /// <summary>
        /// Convert 292; 293; => 19-10-2015; 20-10-2015;
        /// </summary>
        /// <param name="int_DayOfYear"></param>
        /// <returns></returns>
        public static string f_DayOfYearToDate_dd_MM_yyyy(int int_DayOfYear)
        {
            if (int_DayOfYear > 0 && int_DayOfYear < 367)
                return f_DayOfYearToDateKey(int_DayOfYear).ToString("dd-MM-yyyy");

            return "";
        }

        /// <summary>
        /// Convert 292;293 => 151019;151020
        /// </summary>
        /// <param name="int_DayOfYear"></param>
        /// <returns></returns>
        public static int f_DayOfYearToDate_yyMMdd(int int_DayOfYear)
        {
            if (int_DayOfYear > 0 && int_DayOfYear < 367)
                return f_DayOfYearToDateKey(int_DayOfYear).ToString("yyMMdd").TryParseToInt();

            return 0;
        }

        public static DateTime f_DayOfYearToDateKey(int int_DayOfYear)
        {
            return JulianDayToDateTime(DateTime.Now.Year, int_DayOfYear).AddDays(1);
        }

        private static DateTime JulianDayToDateTime(int year, int julianDayNumber)
        {
            if (year < DateTime.MinValue.Year) throw new ArgumentOutOfRangeException("year");
            if (year > DateTime.MaxValue.Year) throw new ArgumentOutOfRangeException("year");

            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            if (julianDayNumber < 1) throw new ArgumentOutOfRangeException("julianDayNumber");
            if (julianDayNumber > daysInYear) throw new ArgumentOutOfRangeException("julianDayNumber");

            DateTime baseDate = new DateTime(year, 1, 1);
            DateTime instance = baseDate.AddDays(julianDayNumber - 1);
            return instance;
        }

    }
}
