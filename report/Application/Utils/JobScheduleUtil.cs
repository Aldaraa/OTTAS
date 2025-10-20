using Application.Common.Exceptions;
using Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public static class JobScheduleUtil
    {

        #region DailyJob



        public static string DailyJobExpressionGenerate(string scheduleType, DateTime startDateTime,  int recureEvery )
        {
            if (scheduleType == ReportScheduleTypes.Daily)
            {
                int hour = startDateTime.Hour;
                int minute = startDateTime.Minute;
                int second = 0;
                string everyday = "*";
                if (recureEvery > 0)
                {
                    everyday = $"1/{recureEvery}";
                }
                string expression = $"{second} {minute} {hour} {everyday} * ? *";


                return expression;

            }
            else {
                throw new BadRequestException("Invalid schedule type");
            }



        }

        public static string DailyJobCommandGenerate(string scheduleType, DateTime startDateTime, DateTime? endDateTime,  int recureEvery)
        {
            if (scheduleType == ReportScheduleTypes.Daily)
            {
                var jobCommand = new
                {
                    scheduleType = scheduleType,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    recureEvery = recureEvery
                };

                string json = JsonConvert.SerializeObject(jobCommand);
                return json;
            }
            else
            {
                throw new BadRequestException("Invalid schedule type");
            }



        }

        #endregion


        #region WeeklyJob


        public static string WeeklyJobCommandGenerate(string scheduleType, DateTime startDateTime, DateTime? endDateTime, int recureEvery, List<string> days)
        {
            if (scheduleType == ReportScheduleTypes.Weekly)
            {

                List<string> upperCaseDays = days.Select(day => day.ToUpper()).ToList();
                string result = string.Join(", ", upperCaseDays);

                var jobCommand = new
                {
                    scheduleType = scheduleType,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    recureEvery = recureEvery,
                    days = upperCaseDays
                };

                // Serialize the object to JSON
                string json = JsonConvert.SerializeObject(jobCommand);
                return json;
            }
            else
            {
                throw new BadRequestException("Invalid schedule type");
            }



        }

        public static string WeeklyJobExpressionGenerate(string scheduleType, DateTime startDateTime, List<string> weekdays, int recureEvery)
        {
            if (scheduleType == ReportScheduleTypes.Weekly)
            {

                if (weekdays.Count > 0)
                {
                    int hour = startDateTime.Hour;
                    int minute = startDateTime.Minute;
                    int second = 0;

                    List<string> upperCaseDays = weekdays.Select(day => day.ToUpper()).ToList();
                    string result = string.Join(", ", upperCaseDays);

                    if (recureEvery > 0)
                    {
                        string expression = $"{second} {minute} {hour} ? * * {result} * {recureEvery}";
                        return expression;
                    }
                    else
                    {
                        string expression = $"{second} {minute} {hour} ? * * {result} *";
                        return expression;
                    }

                }
                else
                {
                    throw new BadRequestException("The 'weekdays' list is empty");
                }


            }
            else
            {
                throw new BadRequestException("Invalid schedule type");
            }

        }


        #endregion


        #region Monthly


        public static string MonthlyJobCommandGenerate(string scheduleType, DateTime startDateTime, DateTime? endDateTime,  List<string> months, List<int> days)
        {
            if (scheduleType == ReportScheduleTypes.Monthly)
            {
                var jobCommand = new
                {
                    scheduleType = scheduleType,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    months = months,
                    days = days
                };

                // Serialize the object to JSON
                string json = JsonConvert.SerializeObject(jobCommand);
                return json;
            }
            else
            {
                throw new BadRequestException("Invalid schedule type");
            }



        }

    



        public static string MonthlyJobExpressionGenerate(string scheduleType, DateTime startDateTime, List<string> months,  List<int> days)
        {
            if (scheduleType == ReportScheduleTypes.Monthly)
            {

                if (months.Count > 0)
                {

                    if (days.Count > 0)
                    {
                        int hour = startDateTime.Hour;
                        int minute = startDateTime.Minute;
                        int second = 0;


                        string monthValues = string.Join(",", months);

                        string dayValues = string.Join(",", days);

                        string cronExpression = $"{second} {minute} {hour} {dayValues} {monthValues} ? *";

                        return cronExpression;
                    }
                    else {
                        throw new BadRequestException("The 'dates' list is empty");
                    }
                }
                else
                {
                    throw new BadRequestException("The 'months' list is empty");
                }


            }
            else
            {
                throw new BadRequestException("Invalid schedule type");
            }
        }


        #endregion


        #region RuntimeJob
        public static string RunTimeJobCommandGenerate(string scheduleType, DateTime executeDateTime)
        {
            if (scheduleType == ReportScheduleTypes.RunTime)
            {
                var jobCommand = new
                {
                    scheduleType = scheduleType,
                    executeDateTime = executeDateTime
                };

                // Serialize the object to JSON
                string json = JsonConvert.SerializeObject(jobCommand);
                return json;
            }
            else
            {
                throw new BadRequestException("Invalid schedule type");
            }



        }
        #endregion


    }
}
