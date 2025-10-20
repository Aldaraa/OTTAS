using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Service
{
    public static class DateHelper
    {
        public static (DateTime PreviousMonday, DateTime NextSunday) GetWeekBoundaries(DateTime? inputDate = null)
        {
            // Set the current date to either the provided date or DateTime.Now if not provided
            DateTime currentDate = inputDate ?? DateTime.Now;

            // Calculate the previous Monday
            DateTime previousMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);

            // Adjust for Sunday case
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                previousMonday = previousMonday.AddDays(-7);
            }

            DateTime nextSunday = currentDate.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
            if (currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                nextSunday = currentDate.AddDays(7 - (int)currentDate.DayOfWeek);
            }

            return (previousMonday, nextSunday);
        }



        public static (DateTime FirstDay, DateTime LastDay) GetMonthBoundaries(DateTime? inputDate = null)
        {
            DateTime currentDate = inputDate ?? DateTime.Now;
            DateTime firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1); // Last day of the month
            return (firstDay, lastDay);
        }

        public static (DateTime FirstDay, DateTime LastDay) GetYearBoundaries(DateTime? inputDate = null)
        {
            DateTime currentDate = inputDate ?? DateTime.Now;
            DateTime firstDay = new DateTime(currentDate.Year, 1, 1); // First day of the year
            DateTime lastDay = new DateTime(currentDate.Year, 12, 31); // Last day of the year
            return (firstDay, lastDay);
        }


        public static (DateTime FirstDay, DateTime LastDay) GetQuarterlyBoundaries(DateTime? inputDate = null)
        {
            DateTime currentDate = inputDate ?? DateTime.Now;
            int quarter = (currentDate.Month - 1) / 3 + 1; // Calculate the current quarter
            DateTime firstDay = new DateTime(currentDate.Year, (quarter - 1) * 3 + 1, 1); // First day of the quarter
            DateTime lastDay = firstDay.AddMonths(3).AddDays(-1); // Last day of the quarter
            return (firstDay, lastDay);
        }



    }
}
