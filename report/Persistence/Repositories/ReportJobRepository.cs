
using Application.Common.Exceptions;
using Application.Features.ReportJobFeature.BuildReport;
using Application.Features.ReportJobFeature.CreateReportJobDaily;
using Application.Features.ReportJobFeature.CreateReportJobMonthly;
using Application.Features.ReportJobFeature.CreateReportJobRuntime;
using Application.Features.ReportJobFeature.CreateReportJobTimeValidate;
using Application.Features.ReportJobFeature.CreateReportJobWeekly;
using Application.Features.ReportJobFeature.DeleteReportJob;
using Application.Features.ReportJobFeature.GetAllReportJob;
using Application.Features.ReportJobFeature.GetAvailableTime;
using Application.Features.ReportJobFeature.GetAvailableTimeSlots;
using Application.Features.ReportJobFeature.GetReportJob;
using Application.Features.ReportJobFeature.GetReportJobLog;
using Application.Features.ReportJobFeature.LoadReportByIdJob;
using Application.Features.ReportJobFeature.LoadReportJob;
using Application.Features.ReportJobFeature.UpdateReportJobDaily;
using Application.Features.ReportJobFeature.UpdateReportJobMonthly;
using Application.Features.ReportJobFeature.UpdateReportJobRuntime;
using Application.Features.ReportJobFeature.UpdateReportJobWeekly;
using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Repositories;
using Application.Service;
using Application.Utils;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using Persistence.HostedService;
using Quartz;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class  ReportJobRepository : BaseRepository<ReportJob>, IReportJobRepository
    {
        JobHostedService _hostedService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private CacheService _cacheService;
        private readonly int CACHE_MINUTE = 5;
        public  ReportJobRepository(IHttpContextAccessor httpContextAccessor, DataContext context, IConfiguration configuration, JobHostedService hostedService, CacheService cacheService) : base(context, configuration)
        {
            _hostedService = hostedService;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
        }

        #region ALL REPORT JOB

        [Authorize]
        public async Task<List<GetAllReportJobResponse>> GetAllData(GetAllReportJobRequest request, CancellationToken cancellationToken)
        {



            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {
                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {

                    var returnData = new List<GetAllReportJobResponse>();



                    IQueryable<ReportJob> jobData = Context.ReportJob.AsNoTracking().OrderByDescending(x => x.DateCreated);

                    if (request.templateId.HasValue)
                    {
                        jobData = jobData.Where(x => x.ReportTemplateId == request.templateId);
                    }

                    if (!string.IsNullOrWhiteSpace(request.keyword))
                    {
                        jobData = jobData.Where(x => x.Code.Contains(request.keyword) || (x.SubscriptionMail ?? "").Contains(request.keyword));
                    }


                    var jobList = await jobData.ToListAsync();


                    foreach (var item in jobList)
                    {
                        //    var subscriptionEmailList =  item.SubscriptionMail.Split(' ').ToList();

                        var subscriptionEmailList = item.SubscriptionMail.Split(' ')
                                .Select(email => email.ToLower())
                                .ToList();

                        if (roleData.RoleTag == "SystemAdmin" ||
                                item.UserIdCreated == roleData.EmployeeId ||
                                subscriptionEmailList.IndexOf(roleData?.Email.ToLower()) > -1)
                        {
                            var nextexecuteDate = await _hostedService.JobNextExecuteDate(item.Id);
                            var currentReportTemplate = await Context.ReportTemplate.AsNoTracking().Where(x => x.Id == item.ReportTemplateId).FirstOrDefaultAsync();
                            var lastExecuteLog = await Context.ReportJobLog.AsNoTracking().Where(x => x.ReportJobId == item.Id).OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
                            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.UserIdCreated).FirstOrDefaultAsync();
                            int? activeStatus = 0;
                            if (item.EndDate != null)
                            {
                                if (item.EndDate >= DateTime.Now)
                                {
                                    activeStatus = 1;
                                }
                                else
                                {
                                    activeStatus = 0;
                                }
                            }
                            var newData = new GetAllReportJobResponse
                            {
                                Id = item.Id,
                                Code = item.Code,
                                Description = item.Description,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                ScheduleType = item.ScheduleType,
                                ScheduleCommandParameter = item.ScheduleCommandParameter,
                                Active = lastExecuteLog?.ExecuteStatus == "Error" ? null : activeStatus,
                                nextExecuteDate = nextexecuteDate,
                                ReportTemplateName = currentReportTemplate?.Description,
                                lastExecuteDate = lastExecuteLog?.ExecuteDate,
                                lastExecuteStatus = lastExecuteLog?.ExecuteStatus,
                                CreatedUser = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}",
                                CreatedDate = item.DateCreated


                            };

                            returnData.Add(newData);
                        }


                    }
                    return returnData.OrderByDescending(x => x.lastExecuteDate).ToList();
                    //}
                }
                else
                {
                    throw new BadRequestException("Access denied");
                }
            }
            else
            {
                throw new BadRequestException("Access denied");
            }


        }

        #endregion


        #region UserRoleData

        public async Task<Userinfo> GetUserRoleData(string username)
        {
            var outData = new Userinfo();

            var cacheName =$"ReportJob_userinfo_{username}";

            if (_cacheService.TryGetValue(cacheName, out outData))
            {
                Console.WriteLine("--------------FROM CACHE------------------------");

                return outData;
            }
            else {
                string queryData = @$"SELECT sre.RoleId, sre.EmployeeId, e.email, sr.RoleTag from SysRoleEmployees sre
                            LEFT JOIN Employee e ON sre.EmployeeId = e.Id
                            LEFT JOIN SysRole sr ON sre.RoleId = sr.Id
                            WHERE sre.EmployeeId IN (SELECT Id FROM Employee e WHERE e.Active = 1 AND e.ADAccount = '{username}')";

                var roleInfo = await ExecuteRolDataQuery(queryData);
                if (roleInfo != null && roleInfo.Count > 0)
                {
                    var returnData = new Userinfo
                    {
                        RoleId = roleInfo[0].RoleId,
                        EmployeeId = roleInfo[0].EmployeeId,
                        Email = roleInfo[0].Email,
                        RoleTag = roleInfo[0].RoleTag
                    };

                    Console.WriteLine("--------------FROM DB------------------------");

                    _cacheService.Set(cacheName, returnData, TimeSpan.FromMinutes(CACHE_MINUTE));

                    return returnData;
                }

                return null;
            }


        }



        public async Task<List<Userinfo>> ExecuteRolDataQuery(string query)
        {
            var roleInfoList = new List<Userinfo>();

            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                await Context.Database.OpenConnectionAsync();

                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        var roleInfo = new Userinfo
                        {
                            RoleId = result.GetInt32(0),
                            EmployeeId = result.IsDBNull(1) ? (int?)null : result.GetInt32(1),
                            Email = result.IsDBNull(2) ? null : result.GetString(2),
                            RoleTag = result.IsDBNull(3) ? null : result.GetString(3)

                        };
                        roleInfoList.Add(roleInfo);
                    }
                }
            }
            return roleInfoList;
        }


        #endregion

        #region GETJOB 

        public async Task<GetReportJobResponse> GetJobData(GetReportJobRequest request, CancellationToken cancellationToken)
        {
            var currentData = await (from job in Context.ReportJob.AsNoTracking().Where(x => x.Id == request.Id)
                                   join template in Context.ReportTemplate.AsNoTracking() on job.ReportTemplateId equals template.Id into templateData
                                     from template in templateData.DefaultIfEmpty()
                                     select new
                                     {
                                         Id = job.Id,
                                         Active = job.Active,
                                         Description = job.Description,
                                         Name = job.Code,
                                         StartDate = job.StartDate,
                                         EndDate = job.EndDate,
                                         ScheduleType = job.ScheduleType,
                                         reportTemplateId = job.ReportTemplateId,
                                         SubscriptionMail = job.SubscriptionMail,
                                         TemplateCode = template.Code,
                                         Command = job.ScheduleCommandParameter
                                        
                                     }).FirstOrDefaultAsync();
            if (currentData != null)
            {
                var subscriptionMail = new List<string>();
                if (currentData.SubscriptionMail != null)
                {
                    subscriptionMail = currentData.SubscriptionMail.Split(" ").ToList();
                }



                var columnData = await (from jobCol in Context.ReportJobColumn.AsNoTracking().Where(x => x.ReportJobId == currentData.Id)
                                        join tempCol in Context.ReportTemplateColumn.AsNoTracking() on jobCol.ColumnId equals tempCol.Id into tempColData
                                        from tempCol in tempColData.DefaultIfEmpty()
                                        select new JobJobColumns
                                        {
                                            ColumnId = jobCol.ColumnId,
                                            Caption = tempCol.Caption,
                                            FieldName = tempCol.FieldName,
                                        }).ToListAsync();

                var paramData = await (from jobParam in Context.ReportJobParameter.AsNoTracking().Where(x => x.ReportJobId == currentData.Id)
                                       join tempParam in Context.ReportTemplateParameter.AsNoTracking() on jobParam.ParameterId equals tempParam.Id into tempParamData
                                       from tempParam in tempParamData.DefaultIfEmpty()
                                       select new JobParameter
                                       {
                                           Id = jobParam.Id,
                                           Caption = tempParam.Caption,
                                           FieldName = tempParam.FieldName,
                                           Descr = tempParam.Descr,
                                           Component = tempParam.Component,
                                           ParameterId = jobParam.ParameterId,
                                           ParameterValue = jobParam.ParameterValue,
                                           Days = jobParam.Days
                                       }).ToListAsync();


                var returnData = new GetReportJobResponse
                {
                    Id = currentData.Id,
                    Active = currentData.Active,
                    Name = currentData.Name,
                    StartDate = currentData.StartDate.Value,
                    
                    EndDate = currentData.EndDate,
                    ScheduleType = currentData.ScheduleType,
                    Description = currentData.Description,
                    reportTemplateId = currentData.reportTemplateId,
                    subscriptionMails = subscriptionMail,
                    reportTemplateCode = currentData.TemplateCode,
                    Command = currentData.Command,
                    Columns = columnData,
                    Parameters = paramData
                };

                return returnData;
            }
            else {
                return null;
            }

        }

        #endregion


        #region LoadData
        public async Task LoadData(LoadReportJobRequest request)
        {
            var dbJobData = await Context.ReportJob.AsNoTracking().Where(x => x.EndDate > DateTime.Today && x.Active == 1 || (x.ScheduleType == ReportScheduleTypes.RunTime && x.StartDate > DateTime.Now)).ToListAsync();

            foreach (var item in dbJobData)
            {
                if (item.ScheduleType == ReportScheduleTypes.Weekly)
                {
                    CronSchedule cronSchedule = new CronSchedule();
                    cronSchedule.StartDate = item.StartDate.Value;
                    cronSchedule.Id = item.Id;
                    cronSchedule.CronExpression = item.ScheduleCommand;

                    await _hostedService.AddNewJobWeekly(cronSchedule);
                }
                else
                {
                    if (item.EndDate.HasValue)
                    {
                        CronSchedule cronSchedule = new CronSchedule();
                        cronSchedule.StartDate = item.StartDate.Value;
                        cronSchedule.EndDate = item.EndDate;
                        cronSchedule.Id = item.Id;
                        cronSchedule.CronExpression = item.ScheduleCommand;

                        await _hostedService.AddNewJobWithEndDate(cronSchedule);
                    }
                    else
                    {
                        if (item.ScheduleType == ReportScheduleTypes.RunTime)
                        {
                            CronSchedule cronSchedule = new CronSchedule();
                            cronSchedule.StartDate = item.StartDate.Value;
                            cronSchedule.Id = item.Id;
                            cronSchedule.CronExpression = item.ScheduleCommand;

                            await _hostedService.AddNewJobRunTime(cronSchedule);
                        }
                        else
                        {
                            CronSchedule cronSchedule = new CronSchedule();
                            cronSchedule.StartDate = item.StartDate.Value;
                            cronSchedule.Id = item.Id;
                            cronSchedule.CronExpression = item.ScheduleCommand;

                            await _hostedService.AddNewJob(cronSchedule);
                        }
                    }
                }


                
            }
        }

        #endregion


        public async Task LoadDataById(LoadReportByIdJobRequest request)
        {
            var item = await Context.ReportJob
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Where(x => x.Active == 1)
                .Where(x => x.EndDate == null || x.EndDate > DateTime.Today) // null = хугацаагүй идэвхтэй
                .FirstOrDefaultAsync();
            if (item != null)
            {
                if (item.ScheduleType == ReportScheduleTypes.Weekly)
                {
                    if (item.ScheduleCommand != null) {
                        CronSchedule cronSchedule = new CronSchedule();
                        cronSchedule.StartDate = item.StartDate.Value;
                        cronSchedule.EndDate = item.EndDate;
                        cronSchedule.Id = item.Id;
                        cronSchedule.CronExpression = item.ScheduleCommand;
                        await _hostedService.AddNewJobWeekly(cronSchedule);
                    }

                }
            }
        }


        #region DeleteReport


        public async Task DeleteJobSchedule(DeleteReportJobRequest request, CancellationToken cancellationToken)
        {
            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {
                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {


                    using (var transaction = Context.Database.BeginTransaction())
                    {
                        try
                        {
                            var currentJob = await Context.ReportJob
                                                          .Where(x => x.Id == request.Id)
                                                          .FirstOrDefaultAsync();

                            if (currentJob != null)
                            {
                                if (roleData.RoleTag == "SystemAdmin" ||  currentJob.UserIdCreated == roleData.EmployeeId)
                                {

                                    await _hostedService.DeleteJob(currentJob.Id);
                                    var jobParameters = await Context.ReportJobParameter
                                                                     .Where(x => x.ReportJobId == request.Id)
                                                                     .ToListAsync();

                                    var jobColumns = await Context.ReportJobColumn
                                                                  .Where(x => x.ReportJobId == request.Id)
                                                                  .ToListAsync();
                                    Context.ReportJobParameter.RemoveRange(jobParameters);
                                    Context.ReportJobColumn.RemoveRange(jobColumns);
                                    await Context.SaveChangesAsync();
                                    Context.ReportJob.Remove(currentJob);
                                    await Context.SaveChangesAsync();
                                    transaction.Commit();
                                }
                                else {
                                    transaction.Rollback();
                                    throw new BadRequestException("You can only delete schedules that you have created or System administrator");
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new BadRequestException(ex.Message);
                        }
                    }

                }
                else {
                    throw new BadRequestException("Access denied");
                }

            }



        }

        #endregion


        #region ValidateTimeReport


        public async Task<bool> ReportJobTimeValidate(CreateReportJobTimeValidateRequest request, CancellationToken cancellationToken)
        {
            if (request.Id.HasValue)
            {
                // Calculate the time window for validation
                var startDate = request.scheduleStartDate.AddMinutes(-2);
                var endDate = request.scheduleStartDate.AddMinutes(2);

                // Convert to TimeSpan for time of day comparison
                var startTime = startDate.TimeOfDay;
                var endTime = endDate.TimeOfDay;

                // Query the database to count jobs within the time window
                var reportJobs = await Context.ReportJob.AsNoTracking()
                    .Where(rj => rj.StartDate.HasValue
                                 && rj.StartDate.Value.TimeOfDay >= startTime
                                 && rj.StartDate.Value.TimeOfDay <= endTime
                                 && rj.Id != request.Id)
                    .CountAsync(cancellationToken);

                // Return true if no jobs are found, otherwise false
                return reportJobs == 0;

            }
            else {
                // Calculate the time window for validation
                var startDate = request.scheduleStartDate.AddMinutes(-2);
                var endDate = request.scheduleStartDate.AddMinutes(2);

                // Convert to TimeSpan for time of day comparison
                var startTime = startDate.TimeOfDay;
                var endTime = endDate.TimeOfDay;

                // Query the database to count jobs within the time window
                var reportJobs = await Context.ReportJob.AsNoTracking()
                    .Where(rj => rj.StartDate.HasValue
                                 && rj.StartDate.Value.TimeOfDay >= startTime
                                 && rj.StartDate.Value.TimeOfDay <= endTime)
                    .CountAsync(cancellationToken);

                // Return true if no jobs are found, otherwise false
                return reportJobs == 0;

            }




        }


        public async Task<List<int>> GetAvailableTime(GetAvailableTimeRequest request, CancellationToken cancellationToken)
        {
            var startDate = new DateTime(request.scheduleStartDate.Year, request.scheduleStartDate.Month, request.scheduleStartDate.Day, request.scheduleStartDate.Hour, 0, 0);
            var endDate = new DateTime(request.scheduleStartDate.Year, request.scheduleStartDate.Month, request.scheduleStartDate.Day, request.scheduleStartDate.Hour, 59, 59);

            var availableTimeSlots = new List<int>();


            for (DateTime currentMinute = startDate; currentMinute <= endDate; currentMinute = currentMinute.AddMinutes(1))
            {
                var ssDate = currentMinute.AddMinutes(-2);
                var eeDate = currentMinute.AddMinutes(2);

                var startTime = new TimeSpan(ssDate.Hour, ssDate.Minute, ssDate.Second);
                var endTime = new TimeSpan(eeDate.Hour, eeDate.Minute, eeDate.Second);

                var reportJob = await Context.ReportJob.AsNoTracking()
                    .Where(rj => rj.StartDate.Value.TimeOfDay >= startTime && rj.StartDate.Value.TimeOfDay <= endTime)
                    .FirstOrDefaultAsync(cancellationToken);
                if (reportJob != null)
                {
                    var currentDate = reportJob.StartDate;
                    if (currentDate != null)
                    {
                        var currentDateBefore2Minute = currentDate.Value.AddMinutes(-2);
                        var currentDateBefore1Minute = currentDate.Value.AddMinutes(-1);
                        var currentDateAfter2Minute = currentDate.Value.AddMinutes(2);
                        var currentDateAfter1Minute = currentDate.Value.AddMinutes(1);

                        availableTimeSlots.Add(currentDateBefore2Minute.Minute);
                        availableTimeSlots.Add(currentDateBefore1Minute.Minute);
                        availableTimeSlots.Add(currentDate.Value.Minute);
                        availableTimeSlots.Add(currentDateAfter2Minute.Minute);
                        availableTimeSlots.Add(currentDateAfter1Minute.Minute);

                    }

                }

            
            }

            availableTimeSlots.Sort();
            return availableTimeSlots.Distinct().ToList();
        }

        #endregion



        #region JobLogDetail

        public async Task<List<GetReportJobLogResponse>> GetJobLogData(GetReportJobLogRequest request, CancellationToken cancellationToken)
        {
              return await Context.ReportJobLog.AsNoTracking().Where(c=> c.ReportJobId == request.Id).Select(x => new GetReportJobLogResponse { 
                    Id = x.Id,
                    ExecuteDate = x.ExecuteDate,
                    ExecuteStatus = x.ExecuteStatus,
                    Descr= x.Descr

                }).OrderByDescending(x=> x.ExecuteDate).ToListAsync();

        }

        #endregion


        #region  GetAvailableTimeSlots
        public async Task<List<DateTime>> GetAvailableTimeSlots(GetAvailableTimeSlotsRequest requestdata, CancellationToken cancellationToken)
        {
            var availableTimeSlots = new List<DateTime>();

            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            var reportJobs = await Context.ReportJob.AsNoTracking()
                .Where(rj => rj.StartDate.HasValue && rj.EndDate > DateTime.Now)
                .Select(rj => new { rj.StartDate })
                .ToListAsync(cancellationToken);

            // Calculate deactivated intervals from ReportJob StartDate
            var deactivatedIntervals = reportJobs.Select(rj => new
            {
                StartTime = rj.StartDate.Value.TimeOfDay.Add(new TimeSpan(0, -1, 0)), // Example: 1 minute before StartTime
                EndTime = rj.StartDate.Value.TimeOfDay.Add(new TimeSpan(0, 1, 0))     // Example: 1 minute after StartTime
                                                                                      // Adjust as per your business logic for calculating deactivated intervals
            }).ToList();

            // Iterate through the date range in 4-minute intervals
            var currentTime = startDate;
            while (currentTime <= endDate)
            {
                bool isDeactivated = deactivatedIntervals.Any(interval =>
                {
                    TimeSpan currentTimeOfDay = currentTime.TimeOfDay;
                    return currentTimeOfDay >= interval.StartTime && currentTimeOfDay <= interval.EndTime;
                });

                if (!isDeactivated)
                {
                    availableTimeSlots.Add(currentTime); // Add available time slot
                }
                currentTime = currentTime.AddMinutes(4);
            }

            return availableTimeSlots;
        }

        #endregion

    }



}
