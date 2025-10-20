using Application.Features.ReportJobFeature.CreateReportJobDaily;
using Application.Features.ReportJobFeature.CreateReportJobMonthly;
using Application.Features.ReportJobFeature.CreateReportJobRuntime;
using Application.Features.ReportJobFeature.CreateReportJobWeekly;
using Application.Features.ReportJobFeature.UpdateReportJobDaily;
using Application.Features.ReportJobFeature.UpdateReportJobMonthly;
using Application.Features.ReportJobFeature.UpdateReportJobRuntime;
using Application.Features.ReportJobFeature.UpdateReportJobWeekly;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.HostedService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class ReportJobRepository
    {


        #region DailyJobCreate


        public async Task CreateJobScheduleDaily(CreateReportJobDailyRequest request, CancellationToken cancellationToken)
        {

            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {

                    string cronexpression = JobScheduleUtil.DailyJobExpressionGenerate(ReportScheduleTypes.Daily, request.startDate, request.recureEvery);
                    string commandexpression = JobScheduleUtil.DailyJobCommandGenerate(ReportScheduleTypes.Daily, request.startDate, request.endDate, request.recureEvery);


                    var newData = new ReportJob
                    {
                        Description = request.Description,
                        Code = request.Name,
                        StartDate = request.startDate,
                        EndDate = request.endDate,
                        ScheduleType = ReportScheduleTypes.Daily,
                        ReportTemplateId = request.reportTemplateId,
                        SubscriptionMail = String.Join(" ", request.subscriptionMails),
                        Active = 1,
                        ScheduleCommand = cronexpression,
                        ScheduleCommandParameter = commandexpression,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        NextExecuteDate = DateTime.Now,
                        UserIdCreated = roleData.EmployeeId
                    };

                    Context.ReportJob.Add(newData);
                    await Context.SaveChangesAsync();


                    if (request.ColumnIds != null)
                    {
                        foreach (var item in request.ColumnIds)
                        {
                            var newColumn = new ReportJobColumn
                            {
                                Active = 1,
                                ColumnId = item,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                DateDeleted = DateTime.Now,
                                ReportJobId = newData.Id,
                                UserIdDeleted = 1
                            };

                            Context.ReportJobColumn.Add(newColumn);
                        }
                    }



                    foreach (var item in request.Parameters)
                    {
                        var newParameter = new ReportJobParameter
                        {
                            Active = 1,
                            ParameterId = item.ParameterId,
                            ParameterValue = item.ParameterValue,
                            Days = item.Days,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            DateDeleted = DateTime.Now,
                            ReportJobId = newData.Id,
                            UserIdDeleted = 1
                        };

                        Context.ReportJobParameter.Add(newParameter);
                    }

                    await Context.SaveChangesAsync();

                    if (request.endDate.HasValue)
                    {
                        var schedule = new CronSchedule
                        {
                            CronExpression = cronexpression,
                            EndDate = request.endDate.Value,
                            StartDate = request.startDate,
                            Name = request.Name,
                            Id = newData.Id
                        };


                        await _hostedService.AddNewJobWithEndDate(schedule);
                    }
                    if (!request.endDate.HasValue)
                    {
                        var schedule = new CronSchedule
                        {
                            CronExpression = cronexpression,
                            StartDate = request.startDate,
                            Name = request.Name,
                            Id = newData.Id
                        };


                        await _hostedService.AddNewJob(schedule);
                    }

                }

            }




            await Task.CompletedTask;
        }

        #endregion

        #region DailyJobUpdate


        public async Task UpdateJobScheduleDaily(UpdateReportJobDailyRequest request, CancellationToken cancellationToken)
        {

            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {
                    string cronexpression = JobScheduleUtil.DailyJobExpressionGenerate(ReportScheduleTypes.Daily, request.startDate, request.recureEvery);
                    string commandexpression = JobScheduleUtil.DailyJobCommandGenerate(ReportScheduleTypes.Daily, request.startDate, request.endDate, request.recureEvery);


                    var currentData = await Context.ReportJob.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (currentData != null)
                    {
                        currentData.Description = request.Description;
                        currentData.Code = request.Name;
                        currentData.StartDate = request.startDate;
                        currentData.EndDate = request.endDate;
                        currentData.ScheduleType = ReportScheduleTypes.Daily;
                        currentData.ReportTemplateId = request.reportTemplateId;
                        currentData.SubscriptionMail = String.Join(" ", request.subscriptionMails);
                        currentData.Active = 1;
                        currentData.ScheduleCommand = cronexpression;
                        currentData.ScheduleCommandParameter = commandexpression;
                        currentData.DateUpdated = DateTime.Now;
                        currentData.NextExecuteDate = DateTime.Now;
                        currentData.UserIdUpdated = roleData.EmployeeId;
                        Context.ReportJob.Update(currentData);


                        var oldReportJobColumns = await Context.ReportJobColumn.Where(x => x.ReportJobId == request.Id).ToListAsync();
                        var oldReportJobParameters = await Context.ReportJobParameter.Where(x => x.ReportJobId == request.Id).ToListAsync();

                        Context.ReportJobColumn.RemoveRange(oldReportJobColumns);
                        Context.ReportJobParameter.RemoveRange(oldReportJobParameters);

                        await _hostedService.DeleteJob(request.Id);


                        if (request.ColumnIds != null)
                        {
                            foreach (var item in request.ColumnIds)
                            {
                                var newColumn = new ReportJobColumn
                                {
                                    Active = 1,
                                    ColumnId = item,
                                    DateCreated = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    DateDeleted = DateTime.Now,
                                    ReportJobId = request.Id,
                                    UserIdDeleted = 1
                                };

                                Context.ReportJobColumn.Add(newColumn);
                            }
                        }



                        foreach (var item in request.Parameters)
                        {
                            var newParameter = new ReportJobParameter
                            {
                                Active = 1,
                                ParameterId = item.ParameterId,
                                ParameterValue = item.ParameterValue,
                                Days = item.Days,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                DateDeleted = DateTime.Now,
                                ReportJobId = request.Id,
                                UserIdDeleted = 1
                            };

                            Context.ReportJobParameter.Add(newParameter);
                        }

                        await Context.SaveChangesAsync();

                        if (request.endDate.HasValue)
                        {
                            var schedule = new CronSchedule
                            {
                                CronExpression = cronexpression,
                                EndDate = request.endDate.Value,
                                StartDate = request.startDate,
                                Name = request.Name,
                                Id = request.Id
                            };


                            await _hostedService.AddNewJobWithEndDate(schedule);
                        }
                        if (!request.endDate.HasValue)
                        {
                            var schedule = new CronSchedule
                            {
                                CronExpression = cronexpression,
                                StartDate = request.startDate,
                                Name = request.Name,
                                Id = request.Id
                            };


                            await _hostedService.AddNewJob(schedule);
                        }





                    }

                }

            }



            await Task.CompletedTask;
        }

        #endregion



        #region WeeklyJobCreate

        public async Task CreateJobScheduleWeekly(CreateReportJobWeeklyRequest request, CancellationToken cancellationToken)
        {


            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {

                    string cronexpression = JobScheduleUtil.WeeklyJobExpressionGenerate(ReportScheduleTypes.Weekly, request.startDate, request.days, request.recureEvery);
                    string commandexpression = JobScheduleUtil.WeeklyJobCommandGenerate(ReportScheduleTypes.Weekly, request.startDate, request.endDate, request.recureEvery, request.days);

                    var newData = new ReportJob
                    {
                        Description = request.Description,
                        Code = request.Name,
                        StartDate = request.startDate,
                        EndDate = request.endDate,
                        ScheduleType = ReportScheduleTypes.Weekly,
                        ReportTemplateId = request.reportTemplateId,
                        SubscriptionMail = String.Join(" ", request.subscriptionMails),
                        Active = 1,
                        NextExecuteDate = DateTime.Now,
                        ScheduleCommand = cronexpression,
                        ScheduleCommandParameter = commandexpression,
                        DateCreated = DateTime.Now,
                        UserIdCreated = roleData.EmployeeId,
                    };

                    Context.ReportJob.Add(newData);
                    await Context.SaveChangesAsync();

                    if (request.ColumnIds != null)
                    {
                        foreach (var item in request.ColumnIds)
                        {
                            var newColumn = new ReportJobColumn
                            {
                                Active = 1,
                                ColumnId = item,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                DateDeleted = DateTime.Now,
                                ReportJobId = newData.Id,
                                UserIdDeleted = 1
                            };

                            Context.ReportJobColumn.Add(newColumn);
                        }
                    }


                    foreach (var item in request.Parameters)
                    {
                        var newParameter = new ReportJobParameter
                        {
                            Active = 1,
                            ParameterId = item.ParameterId,
                            ParameterValue = item.ParameterValue,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            DateDeleted = DateTime.Now,
                            ReportJobId = newData.Id,
                            UserIdDeleted = 1
                        };

                        Context.ReportJobParameter.Add(newParameter);
                    }

                    var schedule = new CronSchedule
                    {
                        CronExpression = cronexpression,
                        StartDate = request.startDate,
                        Name = request.Name,
                        Id = newData.Id
                    };


                    await _hostedService.AddNewJobWeekly(schedule);


                }

            }
            await Task.CompletedTask;
        }

        #endregion

        #region WeeklyJobUpdate


        public async Task UpdateJobScheduleWeekly(UpdateReportJobWeeklyRequest request, CancellationToken cancellationToken)
        {
            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {

                    string cronexpression = JobScheduleUtil.WeeklyJobExpressionGenerate(ReportScheduleTypes.Weekly, request.startDate, request.days, request.recureEvery);
                    string commandexpression = JobScheduleUtil.WeeklyJobCommandGenerate(ReportScheduleTypes.Weekly, request.startDate, request.endDate, request.recureEvery, request.days);


                    var currentData = await Context.ReportJob.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (currentData != null)
                    {
                        currentData.Description = request.Description;
                        currentData.Code = request.Name;
                        currentData.StartDate = request.startDate;
                        currentData.EndDate = request.endDate;
                        currentData.ScheduleType = ReportScheduleTypes.Weekly;
                        currentData.ReportTemplateId = request.reportTemplateId;
                        currentData.SubscriptionMail = String.Join(" ", request.subscriptionMails);
                        currentData.Active = 1;
                        currentData.NextExecuteDate = DateTime.Now;
                        currentData.ScheduleCommand = cronexpression;
                        currentData.ScheduleCommandParameter = commandexpression;
                        currentData.DateUpdated = DateTime.Now;
                        currentData.UserIdUpdated = roleData.EmployeeId;

                        Context.ReportJob.Update(currentData);

                        var oldReportJobColumns = await Context.ReportJobColumn.Where(x => x.ReportJobId == request.Id).ToListAsync();
                        var oldReportJobParameters = await Context.ReportJobParameter.Where(x => x.ReportJobId == request.Id).ToListAsync();

                        Context.ReportJobColumn.RemoveRange(oldReportJobColumns);
                        Context.ReportJobParameter.RemoveRange(oldReportJobParameters);

                        await _hostedService.DeleteJob(request.Id);


                        if (request.ColumnIds != null)
                        {
                            foreach (var item in request.ColumnIds)
                            {
                                var newColumn = new ReportJobColumn
                                {
                                    Active = 1,
                                    ColumnId = item,
                                    DateCreated = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    DateDeleted = DateTime.Now,
                                    ReportJobId = request.Id,
                                    UserIdDeleted = 1
                                };

                                Context.ReportJobColumn.Add(newColumn);
                            }
                        }



                        foreach (var item in request.Parameters)
                        {
                            var newParameter = new ReportJobParameter
                            {
                                Active = 1,
                                ParameterId = item.ParameterId,
                                ParameterValue = item.ParameterValue,
                                Days = item.Days,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                DateDeleted = DateTime.Now,
                                ReportJobId = request.Id,
                                UserIdDeleted = 1
                            };

                            Context.ReportJobParameter.Add(newParameter);
                        }

                        var schedule = new CronSchedule
                        {
                            CronExpression = cronexpression,
                            StartDate = request.startDate,
                            Name = request.Name,
                            Id = request.Id
                        };


                        await _hostedService.AddNewJobWeekly(schedule);
                    }

                }

            }


            await Task.CompletedTask;
        }

        #endregion


        #region MonthlyJobCreate
        public async Task CreateJobScheduleMonthly(CreateReportJobMonthlyRequest request, CancellationToken cancellationToken)
        {

            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {

                    string cronexpression = JobScheduleUtil.MonthlyJobExpressionGenerate(
                ReportScheduleTypes.Monthly,
                request.startDate,
                request.months, request.days);
                    string commandexpression = JobScheduleUtil.MonthlyJobCommandGenerate(ReportScheduleTypes.Monthly,
                        request.startDate,
                        request.endDate,
                        request.months, request.days);

                    var newData = new ReportJob
                    {
                        Description = request.Description,
                        Code = request.Name,
                        StartDate = request.startDate,
                        EndDate = request.endDate,
                        ScheduleType = ReportScheduleTypes.Monthly,
                        ReportTemplateId = request.reportTemplateId,
                        SubscriptionMail = String.Join(" ", request.subscriptionMails),
                        Active = 1,
                        NextExecuteDate = DateTime.Now,
                        ScheduleCommand = cronexpression,
                        ScheduleCommandParameter = commandexpression,
                        DateCreated = DateTime.Now,
                        UserIdCreated = roleData.EmployeeId,
                    };

                    Context.ReportJob.Add(newData);
                    await Context.SaveChangesAsync();

                    if (request.ColumnIds != null)
                    {

                        if (request.ColumnIds != null)
                        {
                            foreach (var item in request.ColumnIds)
                            {
                                var newColumn = new ReportJobColumn
                                {
                                    Active = 1,
                                    ColumnId = item,
                                    DateCreated = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    DateDeleted = DateTime.Now,
                                    ReportJobId = newData.Id,
                                    UserIdDeleted = 1
                                };

                                Context.ReportJobColumn.Add(newColumn);
                            }
                        }

                    }
                    foreach (var item in request.ColumnIds)
                    {
                        var newColumn = new ReportJobColumn
                        {
                            Active = 1,
                            ColumnId = item,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            DateDeleted = DateTime.Now,
                            ReportJobId = newData.Id,
                            UserIdDeleted = 1
                        };

                        Context.ReportJobColumn.Add(newColumn);
                    }


                    foreach (var item in request.Parameters)
                    {
                        var newParameter = new ReportJobParameter
                        {
                            Active = 1,
                            ParameterId = item.ParameterId,
                            ParameterValue = item.ParameterValue,
                            Days = item.Days,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            DateDeleted = DateTime.Now,
                            ReportJobId = newData.Id,
                            UserIdDeleted = 1
                        };

                        Context.ReportJobParameter.Add(newParameter);
                    }

                    if (request.endDate.HasValue)
                    {
                        var schedule = new CronSchedule
                        {
                            CronExpression = cronexpression,
                            EndDate = request.endDate.Value,
                            StartDate = request.startDate,
                            Name = request.Name,
                            Id = newData.Id
                        };


                        await _hostedService.AddNewJobWithEndDate(schedule);
                    }
                    if (!request.endDate.HasValue)
                    {
                        var schedule = new CronSchedule
                        {
                            CronExpression = cronexpression,
                            StartDate = request.startDate,
                            Name = request.Name,
                            Id = newData.Id
                        };


                        await _hostedService.AddNewJob(schedule);
                    }


                }

            }


            await Task.CompletedTask;
        }


        #endregion

        #region MonthlyJobUpdate
        public async Task UpdateJobScheduleMonthly(UpdateReportJobMonthlyRequest request, CancellationToken cancellationToken)
        {

            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {

                    string cronexpression = JobScheduleUtil.MonthlyJobExpressionGenerate(
                ReportScheduleTypes.Monthly,
                request.startDate,
                request.months, request.days);
                    string commandexpression = JobScheduleUtil.MonthlyJobCommandGenerate(ReportScheduleTypes.Monthly,
                        request.startDate,
                        request.endDate,
                        request.months, request.days);


                    var currentData = await Context.ReportJob.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (currentData != null)
                    {
                        currentData.Description = request.Description;
                        currentData.Code = request.Name;
                        currentData.StartDate = request.startDate;
                        currentData.EndDate = request.endDate;
                        currentData.ScheduleType = ReportScheduleTypes.Monthly;
                        currentData.ReportTemplateId = request.reportTemplateId;
                        currentData.SubscriptionMail = String.Join(" ", request.subscriptionMails);
                        currentData.Active = 1;
                        currentData.NextExecuteDate = DateTime.Now;
                        currentData.ScheduleCommand = cronexpression;
                        currentData.ScheduleCommandParameter = commandexpression;
                        currentData.DateUpdated = DateTime.Now;
                        currentData.UserIdUpdated = roleData.EmployeeId;

                        var oldReportJobColumns = await Context.ReportJobColumn.Where(x => x.ReportJobId == request.Id).ToListAsync();
                        var oldReportJobParameters = await Context.ReportJobParameter.Where(x => x.ReportJobId == request.Id).ToListAsync();

                        Context.ReportJobColumn.RemoveRange(oldReportJobColumns);
                        Context.ReportJobParameter.RemoveRange(oldReportJobParameters);

                        await _hostedService.DeleteJob(request.Id);

                        if (request.ColumnIds != null)
                        {
                            foreach (var item in request.ColumnIds)
                            {
                                var newColumn = new ReportJobColumn
                                {
                                    Active = 1,
                                    ColumnId = item,
                                    DateCreated = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    DateDeleted = DateTime.Now,
                                    ReportJobId = request.Id,
                                    UserIdDeleted = 1
                                };

                                Context.ReportJobColumn.Add(newColumn);
                            }
                        }



                        foreach (var item in request.Parameters)
                        {
                            var newParameter = new ReportJobParameter
                            {
                                Active = 1,
                                ParameterId = item.ParameterId,
                                ParameterValue = item.ParameterValue,
                                Days = item.Days,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                DateDeleted = DateTime.Now,
                                ReportJobId = request.Id,
                                UserIdDeleted = 1
                            };

                            Context.ReportJobParameter.Add(newParameter);
                        }

                        if (request.endDate.HasValue)
                        {
                            var schedule = new CronSchedule
                            {
                                CronExpression = cronexpression,
                                EndDate = request.endDate.Value,
                                StartDate = request.startDate,
                                Name = request.Name,
                                Id = request.Id
                            };


                            await _hostedService.AddNewJobWithEndDate(schedule);
                        }
                        if (!request.endDate.HasValue)
                        {
                            var schedule = new CronSchedule
                            {
                                CronExpression = cronexpression,
                                StartDate = request.startDate,
                                Name = request.Name,
                                Id = request.Id
                            };


                            await _hostedService.AddNewJob(schedule);
                        }

                    }

                }

            }


            await Task.CompletedTask;
        }


        #endregion

        #region RunTimeReportCreate

        public async Task CreateJobScheduleRuntime(CreateReportJobRuntimeRequest request, CancellationToken cancellationToken)
        {

            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {


                    string commandexpression = JobScheduleUtil.RunTimeJobCommandGenerate(ReportScheduleTypes.RunTime,
              request.executeDate);

                    var newData = new ReportJob
                    {
                        Description = request.Description,
                        Code = request.Name,
                        StartDate = request.executeDate,
                        ScheduleType = ReportScheduleTypes.RunTime,
                        ReportTemplateId = request.reportTemplateId,
                        SubscriptionMail = String.Join(" ", request.subscriptionMails),
                        NextExecuteDate = DateTime.Now,
                        Active = 1,
                        ScheduleCommand = "* * * * *",
                        ScheduleCommandParameter = commandexpression,
                        DateCreated = DateTime.Now,
                        UserIdCreated = roleData.EmployeeId
                    };

                    Context.ReportJob.Add(newData);
                    await Context.SaveChangesAsync();

                    if (request.ColumnIds != null)
                    {
                        foreach (var item in request.ColumnIds)
                        {
                            var newColumn = new ReportJobColumn
                            {
                                Active = 1,
                                ColumnId = item,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                DateDeleted = DateTime.Now,
                                ReportJobId = newData.Id,
                                UserIdDeleted = 1
                            };

                            Context.ReportJobColumn.Add(newColumn);
                        }
                    }



                    foreach (var item in request.Parameters)
                    {
                        var newParameter = new ReportJobParameter
                        {
                            Active = 1,
                            ParameterId = item.ParameterId,
                            ParameterValue = item.ParameterValue,
                            Days = item.Days,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            DateDeleted = DateTime.Now,
                            ReportJobId = newData.Id,
                            UserIdDeleted = 1
                        };

                        Context.ReportJobParameter.Add(newParameter);
                    }

                    var schedule = new CronSchedule
                    {
                        CronExpression = "* * * * * *",
                        StartDate = request.executeDate,
                        Name = request.Name,
                        Id = newData.Id
                    };


                    await _hostedService.AddNewJobRunTime(schedule);

                }

            }


            await Task.CompletedTask;


        }

        #endregion


        #region RunTimeReportUpdate

        public async Task UpdateJobScheduleRuntime(UpdateReportJobRuntimeRequest request, CancellationToken cancellationToken)
        {

            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (!string.IsNullOrWhiteSpace(username))
            {

                var roleData = await GetUserRoleData(username);
                if (roleData != null)
                {
                    string commandexpression = JobScheduleUtil.RunTimeJobCommandGenerate(ReportScheduleTypes.RunTime,
              request.executeDate);

                    var currentData = await Context.ReportJob.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (currentData != null)
                    {

                        currentData.Description = request.Description;
                        currentData.Code = request.Name;
                        currentData.StartDate = request.executeDate;
                        currentData.ScheduleType = ReportScheduleTypes.RunTime;
                        currentData.ReportTemplateId = request.reportTemplateId;
                        currentData.SubscriptionMail = String.Join(" ", request.subscriptionMails);
                        currentData.NextExecuteDate = DateTime.Now;
                        currentData.Active = 1;
                        currentData.ScheduleCommand = "* * * * *";
                        currentData.ScheduleCommandParameter = commandexpression;
                        currentData.DateUpdated = DateTime.Now;
                        currentData.UserIdUpdated = roleData.EmployeeId;


                        Context.ReportJob.Update(currentData);

                        var oldReportJobColumns = await Context.ReportJobColumn.Where(x => x.ReportJobId == request.Id).ToListAsync();
                        var oldReportJobParameters = await Context.ReportJobParameter.Where(x => x.ReportJobId == request.Id).ToListAsync();

                        Context.ReportJobColumn.RemoveRange(oldReportJobColumns);
                        Context.ReportJobParameter.RemoveRange(oldReportJobParameters);

                        await _hostedService.DeleteJob(request.Id);

                        if (request.ColumnIds != null)
                        {
                            foreach (var item in request.ColumnIds)
                            {
                                var newColumn = new ReportJobColumn
                                {
                                    Active = 1,
                                    ColumnId = item,
                                    DateCreated = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    DateDeleted = DateTime.Now,
                                    ReportJobId = request.Id,
                                    UserIdDeleted = 1
                                };

                                Context.ReportJobColumn.Add(newColumn);
                            }

                        }


                        foreach (var item in request.Parameters)
                        {
                            var newParameter = new ReportJobParameter
                            {
                                Active = 1,
                                ParameterId = item.ParameterId,
                                ParameterValue = item.ParameterValue,
                                Days = item.Days,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                DateDeleted = DateTime.Now,
                                ReportJobId = request.Id,
                                UserIdDeleted = 1
                            };

                            Context.ReportJobParameter.Add(newParameter);
                        }

                        var schedule = new CronSchedule
                        {
                            CronExpression = "* * * * * *",
                            StartDate = request.executeDate,
                            Name = request.Name,
                            Id = request.Id
                        };


                        await _hostedService.AddNewJobRunTime(schedule);

                    }

                }

            }

            await Task.CompletedTask;


        }

        #endregion


    }
}
