using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public static class DateTimeExtensionMethods
    {
        public static string ShortDate(this DateTime date)
        {
            return date.ToString("ddMMMyyyy");
        }

        public static string ShortDateDash(this DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }

        public static string DateTimeForFile(this DateTime date)
        {
            return date.ToString("ddMMMyyyy") + " (" + DateTime.Now.ToString("hh.mm.ss tt") + ")";
        }

        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static DateTime PreviousWorkDay(this DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            }
            while (date.IsWeekend());

            return date;
        }
    }
}
