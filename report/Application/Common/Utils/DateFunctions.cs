using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utils
{
    public static class DateFunctions
    {
        public static string[] GetDateTypes()
        {
            return Enum.GetNames(typeof(ReportDateTypes));
        }

        public static DateTime GetDate(ReportDateTypes dateType)
        {
            DateTime today = DateTime.Today;

            switch (dateType)
            {
                case ReportDateTypes.Today:
                    return today;
                case ReportDateTypes.Yesterday:
                    return today.AddDays(-1);
                case ReportDateTypes.Tomorrow:
                    return today.AddDays(1);
                case ReportDateTypes.FirstDayOfYear:
                    return new DateTime(today.Year, 1, 1);
                case ReportDateTypes.LastDayOfYear:
                    return new DateTime(today.Year, 12, 31);
                case ReportDateTypes.FirstDayOfMonth:
                    return new DateTime(today.Year, today.Month, 1);
                case ReportDateTypes.LastDayOfMonth:
                    return new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                case ReportDateTypes.FirstDayOfNextMonth:
                    return new DateTime(today.Year, today.Month, 1).AddMonths(1);
                case ReportDateTypes.LastDayOfNextMonth:
                    return new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)).AddMonths(1);
                case ReportDateTypes.LastMonday:
                    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                    return today.AddDays(-7).AddDays(daysUntilMonday);
                case ReportDateTypes.LastSunday:
                    int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
                    return today.AddDays(-7).AddDays(daysUntilSunday);
                default:
                    return today;
            }
        }


    }



    public enum ReportDateTypes
    {
        Today,
        Yesterday,
        Tomorrow,
        FirstDayOfYear,
        LastDayOfYear,
        FirstDayOfMonth,
        LastDayOfMonth,
        FirstDayOfNextMonth,
        LastDayOfNextMonth,
        LastMonday,
        LastSunday
    }


}
