using Application.Features.ReportJobFeature.LoadReportByIdJob;
using Application.Features.ReportJobFeature.LoadReportJob;
using Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence.Context;
using Persistence.Repositories;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Quartz.Logging.OperationName;

namespace Persistence.HostedService
{
    public class JobHostedService : IHostedService
    {
        #region Constructor
        private readonly JobScopedService _jobScopedService;
        private readonly IScheduler _scheduler;


        private const string JOBKEY_PREFIX = "TASREPORT_JOB_";
        private const string JOBTRIGGER_PREFIX = "TASREPORT_TRIGGER_";



        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JobHostedService(JobScopedService jobScopedService, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider)
        {
            _jobScopedService = jobScopedService;
            // Create a Quartz scheduler
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;

            _serviceScopeFactory = serviceScopeFactory;
            _scheduler.JobFactory = serviceProvider.GetRequiredService<IJobFactory>();



        }
        #endregion


        #region JobStart&&Stop

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await LoadSavedJobExecute();
        //    await _scheduler.Start();
            await _scheduler.Start();
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _scheduler.Shutdown().Wait();

            return Task.CompletedTask;
        }
        #endregion


        #region JobServiceInfo
        public async Task<DateTime?> JobNextExecuteDate(int jobId)
        {
            if (jobId == 5131) {
                var aa = 0; 
            }
            JobKey jobKey = new JobKey("TASREPORT_JOB_" + jobId);
            var jobDetail = await _scheduler.GetJobDetail(jobKey);

            if (jobDetail != null)
            {
                var triggers = await _scheduler.GetTriggersOfJob(jobKey);

                if (triggers != null && triggers.Count > 0)
                {

                    var nextTrigger = triggers.First(); // You may need to implement your own logic for selecting a trigger if there are multiple

                    var nextFireTimeUtc = nextTrigger.GetNextFireTimeUtc();
                    if (nextFireTimeUtc.HasValue)
                    {
                        return nextFireTimeUtc.Value.LocalDateTime;
                    }


                //    return nextTrigger.GetNextFireTimeUtc().Value.LocalDateTime;

                //    if (nextExecuteDate.HasValue)
                //    {
                // Convert to local time if needed
                //       nextExecuteDate = nextExecuteDate.Value;// TimeZoneInfo.ConvertTimeFromUtc(nextExecuteDate.Value.DateTime, TimeZoneInfo.Local);
                //         return nextExecuteDate.Value.DateTime;
                //  }
                }
            }
            return null;
        }

        #endregion


        #region LoadJobStartService

        private async Task LoadSavedJobExecute()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetRequiredService<IReportJobRepository>();

                await scopedService.LoadData(new LoadReportJobRequest());

                await Task.CompletedTask;
            }


        }
        #endregion


        #region AddNewJobRuntime
        public async Task AddNewJob(CronSchedule data)
        {
            JobKey jobKey = new JobKey(JOBKEY_PREFIX + data.Id);
            bool jobExists =await _scheduler.CheckExists(jobKey);

            if (!jobExists)
            {
                try
                {

                    DateTimeOffset startDateTimeOffsetWithTimeZone = new DateTimeOffset(data.StartDate);
                    
                    IJobDetail newJob = JobBuilder.Create<MyJob>()
                    .WithIdentity(JOBKEY_PREFIX + data.Id.ToString())
                    .Build();

                    ITrigger newTrigger = TriggerBuilder.Create()
                        .WithIdentity(JOBTRIGGER_PREFIX + data.Id.ToString())
                        .WithDescription("job---> " + data.Id)
                        .WithCronSchedule(data.CronExpression)
                        .StartAt(startDateTimeOffsetWithTimeZone)
                        .Build();

                    await _scheduler.ScheduleJob(newJob, newTrigger);

                    Console.WriteLine("job added --------------------------->" + data.Id);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                }

            }
        }


        public async Task AddNewJobWithEndDate(CronSchedule data)
        {

            JobKey jobKey = new JobKey(JOBKEY_PREFIX + data.Id);
            bool jobExists =await _scheduler.CheckExists(jobKey);

            if (!jobExists)
            {
                try
                {
                    
                    DateTime todayScheduleTime = DateTime.Today.Date.AddHours(data.StartDate.Hour).AddMinutes(data.StartDate.Minute);
                    if (todayScheduleTime < DateTime.Now)
                    {
                        todayScheduleTime = todayScheduleTime.AddDays(1);
                    }

                    DateTime correctStartDate = DateTime.SpecifyKind(data.StartDate < DateTime.Now ? todayScheduleTime : data.StartDate, DateTimeKind.Local);
                    DateTimeOffset localDateTimeOffsetWithTimeZoneStart = new DateTimeOffset(correctStartDate);

                    DateTime correctEndDate = DateTime.SpecifyKind(data.EndDate.Value, DateTimeKind.Local);
                    DateTimeOffset localDateTimeOffsetWithTimeZoneEnd = new DateTimeOffset(correctEndDate);



                    IJobDetail newJob = JobBuilder.Create<MyJob>()
                    .WithIdentity(JOBKEY_PREFIX+ data.Id.ToString())
                    .Build();

                    ITrigger newTrigger = TriggerBuilder.Create()
                        .WithIdentity(JOBTRIGGER_PREFIX + data.Id.ToString())
                        .WithDescription("job---> " + data.Id)
                        .WithCronSchedule(data.CronExpression)
                        .StartAt(correctStartDate)
                        .EndAt(localDateTimeOffsetWithTimeZoneEnd)
                        .Build();

                    await _scheduler.ScheduleJob(newJob, newTrigger);

                    Console.WriteLine($"job added ---------------------------> {data.Id}------------ {data.CronExpression}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding job: {ex.Message}");
                }

            }


        }



        public async Task AddNewJobRunTime(CronSchedule data)
        {

            JobKey jobKey = new JobKey(JOBKEY_PREFIX + data.Id);
            bool jobExists = _scheduler.CheckExists(jobKey).Result;

            if (!jobExists)
            {
                try
                {

                    DateTime correctStartDate = DateTime.SpecifyKind(data.StartDate, DateTimeKind.Local);
                    DateTimeOffset localDateTimeOffsetWithTimeZone = new DateTimeOffset(correctStartDate);
                    // If you specifically want to set the time to the system's local timezone
                    // DateTimeOffset localDateTimeOffsetWithTimeZone = new DateTimeOffset(data.StartDate);



                    IJobDetail newJob = JobBuilder.Create<MyJob>()
                    .WithIdentity(JOBKEY_PREFIX + data.Id.ToString())
                    .Build();

                    ITrigger newTrigger = TriggerBuilder.Create()
                        .WithIdentity(JOBTRIGGER_PREFIX + data.Id.ToString())
                        .WithDescription("job---> " + data.Id)
                        .StartAt(localDateTimeOffsetWithTimeZone)
                        .Build();

                 var aa =   await _scheduler.ScheduleJob(newJob, newTrigger);

                    Console.WriteLine($"job added ---------------------------> {data.Id}");
                }
                catch (Exception)
                {
                }

            }


        }


        public async Task DeleteJob(int Id)
        {

            JobKey jobKey = new JobKey(JOBKEY_PREFIX + Id);
            bool jobExists = _scheduler.CheckExists(jobKey).Result;

            if (jobExists)
            {
                try
                {
                  await  _scheduler.DeleteJob(jobKey);

                 Console.WriteLine("job deleted --------------------------->" + JOBKEY_PREFIX + Id);
                }
                catch (Exception ex)
                {
                    throw;
                }

            }

           await Task.CompletedTask;
        }

        #endregion

        #region WeeklyJob

        public async Task AddNewJobWeekly(CronSchedule data)
        {

            JobKey jobKey = new JobKey(JOBKEY_PREFIX + data.Id);
            bool jobExists =await _scheduler.CheckExists(jobKey);


            if (await _scheduler.CheckExists(jobKey))
            {
                bool deleted = await _scheduler.DeleteJob(jobKey);
                string str;

                if (!deleted)
                    str = $"Failed to delete job for ID={data.Id}.";
                else
                    str = $"Deleted existing job and triggers for ID={data.Id}.";
            }


            try
            {

                if (data.Id == 1013) {
                    var aa = 0;
                }

                    MatchCollection matches = Regex.Matches(data.CronExpression, "MON|TUE|WED|THU|FRI|SAT|SUN");
                    var dayNames = new List<string>();
                    foreach (Match match in matches)
                    {
                        dayNames.Add(match.Value);
                    }
                    int repeatTimes = GetWeeklyRecuveryTimes(data.CronExpression);

                    List<DayOfWeek> daysOfWeek = ConvertToDayOfWeek(dayNames);

                    var scheduleDate =  WeekFindNextDate(data.StartDate, repeatTimes, daysOfWeek);
                    var scheduleExecuteDateTime = scheduleDate.Date.AddHours(data.StartDate.Hour).AddMinutes(data.StartDate.Minute);


                    DateTime todayScheduleTime = DateTime.Today.Date.AddHours(scheduleExecuteDateTime.Hour).AddMinutes(scheduleExecuteDateTime.Minute);
                    if (todayScheduleTime < DateTime.Now)
                    {
                        todayScheduleTime = todayScheduleTime.AddDays(1);
                    }

                    DateTime correctStartDate = DateTime.SpecifyKind(scheduleExecuteDateTime < DateTime.Now ? todayScheduleTime : scheduleExecuteDateTime, DateTimeKind.Local);
                    DateTimeOffset localDateTimeOffsetWithTimeZoneStart = new DateTimeOffset(correctStartDate);

                    DateTime correctEndDate = DateTime.SpecifyKind(correctStartDate.AddMinutes(3), DateTimeKind.Local);
                    DateTimeOffset localDateTimeOffsetWithTimeZoneEnd = new DateTimeOffset(correctEndDate);


                    IJobDetail newJob = JobBuilder.Create<MyJob>()
                  .WithIdentity(JOBKEY_PREFIX + data.Id.ToString())
                  .Build();

                    ITrigger newTrigger = TriggerBuilder.Create()
                        .WithIdentity(JOBTRIGGER_PREFIX + data.Id.ToString())
                        .WithDescription("job---> " + data.Id)
                        .StartAt(correctStartDate)
                        .EndAt(correctEndDate)
                        .WithSimpleSchedule(x =>
                            x.WithIntervalInHours(168) // 7 хоног
                             .RepeatForever()
                        )
                        .Build();

                    await _scheduler.ScheduleJob(newJob, newTrigger);

                    Console.WriteLine($"job added ---------------------------> {data.Id}");


                }
                catch (Exception)
                {
                }

            


        }

        private static DateTime WeekFindNextDate(DateTime startDate, int recurTimes, List<DayOfWeek> weekdays)
        {
            DateTime now = DateTime.Now;
            TimeSpan scheduleTime = startDate.TimeOfDay;

            // 0. Эхлэх суурь цэг = хамгийн эртдээ "өнөөдөр", цаггүй
            DateTime firstDate = now.Date;

            // 1. "өнөөдөр"-өөс эхлэн хэдэн 7 хоног өнгөрсөн бэ?

    
            int weeksPassed = (int)Math.Ceiling((firstDate - startDate.Date).TotalDays / 7.0);

            if (startDate < DateTime.Now || weeksPassed < 0)
            {
                weeksPassed = 0;
            }
          //  if (weeksPassed < 0) weeksPassed = 0;

            // 2. Давтамжийг үндэслэн дараагийн recurrence cycle руу шилжүүлнэ
            int recurrenceSteps = (int)Math.Ceiling(weeksPassed / (double)recurTimes) * recurTimes;

            DateTime baseDate = startDate.Date.AddDays(recurrenceSteps * 7);

            // 3. Тэр долоо хоногийн дотор тохирох weekday + цагийг олно
            DateTime candidate = FindNextWeekday(baseDate, weekdays, scheduleTime);

            // 4. Хэрвээ яг өнөөдөр мөртлөө цаг нь өнгөрсөн бол дараагийн recurrence рүү шилжүүлнэ
            while (candidate <= now)
            {
                baseDate = baseDate.AddDays(recurTimes * 7);
                candidate = FindNextWeekday(baseDate, weekdays, scheduleTime);
            }

            return candidate;
        }

        //private static DateTime WeekFindNextDate(DateTime startDate, int recurTimes, List<DayOfWeek> weekdays)
        //{
        //    DateTime now = DateTime.Now;
        //    TimeSpan scheduleTime = startDate.TimeOfDay;

        //    // 1. Давтамжийг үндэслэж эхлэх өдрийг өнөөдрөөс ирээдүйд шилжүүлнэ
        //    int weeksPassed = (int)Math.Ceiling((now.Date - startDate.Date).TotalDays / 7.0);

        //    // 2. Давтамжийн үржвэрт хүргэж ойролцоо долоо хоногийг олох
        //    int recurrenceSteps = (int)Math.Ceiling(weeksPassed / (double)recurTimes) * recurTimes;

        //    DateTime baseDate = startDate.AddDays(recurrenceSteps * 7);

        //    // 3. BaseDate-ээс эхлэн, тухайн долоо хоногт тохирох weekday + цаг олно
        //    DateTime candidate = FindNextWeekday(baseDate, weekdays, scheduleTime);

        //    // 4. Хэрэв олдсон өдөр яг өнөөдөр ч гэсэн цаг нь өнгөрсөн бол, дараагийн recurrence-рүү шилжүүлнэ
        //    while (candidate <= now)
        //    {
        //        baseDate = baseDate.AddDays(recurTimes * 7);
        //        candidate = FindNextWeekday(baseDate, weekdays, scheduleTime);
        //    }

        //    return candidate;
        //}

        //private static DateTime WeekFindNextDate(DateTime startDate, int recurTimes, List<DayOfWeek> weekdays)
        //{
        //    DateTime today = DateTime.Now;
        //    DateTime nextDate = startDate;
        //    int weekMultiplier = 0;
        //    while (nextDate < today || !weekdays.Contains(nextDate.DayOfWeek))
        //    {
        //        nextDate = startDate.AddDays(weekMultiplier * 7);
        //        if (weekMultiplier == 0 || nextDate >= today)
        //        {
        //            nextDate = FindNextWeekday(nextDate, weekdays);
        //        }

        //        weekMultiplier++;
        //    }

        //    // Adjust for the recurrence
        //    if (recurTimes > 1)
        //    {
        //        int weeksToAdd = ((weekMultiplier - 1) / recurTimes) * recurTimes;
        //        nextDate = startDate.AddDays(weeksToAdd * 7);
        //        nextDate = FindNextWeekday(nextDate, weekdays);
        //    }

        //    return nextDate;
        //}

        //private static DateTime FindNextWeekday(DateTime start, List<DayOfWeek> weekdays)
        //{
        //    DateTime nextWeekday = start;
        //    while (!weekdays.Contains(nextWeekday.DayOfWeek))
        //    {
        //        nextWeekday = nextWeekday.AddDays(1);
        //    }
        //    return nextWeekday;
        //}


        private static DateTime FindNextWeekday(DateTime start, List<DayOfWeek> weekdays, TimeSpan scheduleTime)
        {
            DateTime now = DateTime.Now;
            DateTime date = start.Date;

            for (int i = 0; i < 7; i++)
            {
                if (weekdays.Contains(date.DayOfWeek))
                {
                    DateTime candidate = date + scheduleTime;
                    if (candidate > now)
                        return candidate;
                }

                date = date.AddDays(1);
            }

            // fallback: 7 хоногт таараагүй байвал startDate + scheduleTime буцаана
            return start.Date + scheduleTime;
        }


        private int GetWeeklyRecuveryTimes(string expression)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                    return 1;

                // Ар талаас тасалж задлаад хамгийн сүүлийн утгаас тоо авах
                string[] parts = expression.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 2)
                {
                    string maybeNumber = parts[^1]; // Сүүлчийн утга

                    if (int.TryParse(maybeNumber, out int recurTimes))
                        return recurTimes;
                }

                return 1;

            }
            catch (Exception)
            {

                  return 1;
            }


            //if (!string.IsNullOrEmpty(expression))
            //{
            //    try
            //    {
            //        return Convert.ToInt32(expression[expression.Length - 1]);

            //    }
            //    catch (Exception ex)
            //    {
            //        return 1;
            //    }
            //}
            //else
            //{
            //    return 1;
            //}
        }


        private static List<DayOfWeek> ConvertToDayOfWeek(List<string> daysAbbrev)
        {
            var daysOfWeek = new List<DayOfWeek>();

            foreach (string dayAbbrev in daysAbbrev)
            {
                switch (dayAbbrev.ToUpper())
                {
                    case "MON":
                        daysOfWeek.Add(DayOfWeek.Monday);
                        break;
                    case "TUE":
                        daysOfWeek.Add(DayOfWeek.Tuesday);
                        break;
                    case "WED":
                        daysOfWeek.Add(DayOfWeek.Wednesday);
                        break;
                    case "THU":
                        daysOfWeek.Add(DayOfWeek.Thursday);
                        break;
                    case "FRI":
                        daysOfWeek.Add(DayOfWeek.Friday);
                        break;
                    case "SAT":
                        daysOfWeek.Add(DayOfWeek.Saturday);
                        break;
                    case "SUN":
                        daysOfWeek.Add(DayOfWeek.Sunday);
                        break;
                    default:
                        throw new ArgumentException("Invalid day abbreviation: " + dayAbbrev);
                }
            }

            return daysOfWeek;
        }



        #endregion

    }







    #region OtherClasses


    public class MyJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MyJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        public async Task Execute(IJobExecutionContext context)
        {

            try
            {

                Console.WriteLine("CALLED FUNCTION" + DateTime.Now + "-----------------------------" + context.Trigger.JobKey);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedService = scope.ServiceProvider.GetRequiredService<IJobExecuteServiceRepository>();
                    int Id = Convert.ToInt32(Convert.ToString(context.Trigger.JobKey).Replace("DEFAULT.TASREPORT_JOB_", ""));
                    await scopedService.ExecuteData(Id);

                    IReportJobRepository scopedReportService = scope.ServiceProvider.GetRequiredService<IReportJobRepository>();
                    await scopedReportService.LoadDataById(new LoadReportByIdJobRequest(Id));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown from job execution: {ex.Message}");
                throw; 
            }

        }
    }


    public class CustomQuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomQuartzJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;
            return (IJob)_serviceProvider.GetRequiredService(jobType);
        }

        public void ReturnJob(IJob job)
        {
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public class CronSchedule
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Name { get; set; }

        public string CronExpression { get; set; }
    }


    #endregion


}
