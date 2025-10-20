using Application.Common.Exceptions;
using Application.Common.Utils;
using Application.Features.ReportJobFeature.GetAllReportJob;
using Application.Features.ReportTemplateFeature.DateSimulation;
using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Features.ReportTemplateFeature.GetDashboard;
using Application.Features.ReportTemplateFeature.GetReportDateVariables;
using Application.Features.ReportTemplateFeature.GetReportTemplateData;
using Application.Features.ReportTemplateFeature.GetReportTemplateMaster;
using Application.Features.ReportTemplateFeature.UpdateTemplateParameter;
using Application.Repositories;
using Application.Service;
using Azure;
using Domain.Common;
using Domain.CustomModel;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using Persistence.HostedService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ReportTemplateRepository : BaseRepository<ReportTemplate>, IReportTemplateRepository
    {
        JobHostedService _hostedService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private CacheService _cacheService;
        private readonly int CACHE_MINUTE = 5;
        public ReportTemplateRepository(IHttpContextAccessor httpContextAccessor, DataContext context, IConfiguration configuration, JobHostedService jobHostedService, CacheService cacheService ) : base(context, configuration)
        {
            _hostedService = jobHostedService;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
        }


        [Authorize]
        public async Task<List<GetAllReportTemplateResponse>> GetAllData(GetAllReportTemplateRequest request, CancellationToken cancellationToken)
        {

            string? username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ForBiddenException("Access denied: User is not authenticated.");
            }

            var roleData = await GetUserRoleData(username);
            if (roleData == null)
            {
                throw new ForBiddenException($"Access denied: No role data found for user '{username}'.");
            }

            List<int> templateIds = new List<int>();

            // Get templates by employee role or by user role if employee templates are unavailable
            var employeeTemplateIds = await Context.SysRoleEmployeeReportTemplate
                .AsNoTracking()
                .Where(x => x.EmployeeId == roleData.EmployeeId)
                .Select(x => x.ReportTemplateId)
                .ToListAsync(cancellationToken);

            if (employeeTemplateIds.Any())
            {
                templateIds = employeeTemplateIds;
            }
            else
            {
                var roleTemplateIds = await Context.SysRoleReportTemplate
                    .AsNoTracking()
                    .Where(x => x.RoleId == roleData.RoleId)
                    .Select(c => c.ReportTemplateId)
                    .ToListAsync(cancellationToken);

                if (roleTemplateIds.Any())
                {
                    templateIds = roleTemplateIds;
                }
            }

            if (!templateIds.Any())
            {
                return new List<GetAllReportTemplateResponse>();
            }

            var data = await Context.ReportTemplate
                .AsNoTracking()
                .Where(x => templateIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var returnData = new List<GetAllReportTemplateResponse>();

            foreach (var item in data)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var jobCount = await Context.ReportJob
                    .AsNoTracking()
                    .Where(x => x.ReportTemplateId == item.Id)
                    .CountAsync(cancellationToken);

                var newRecord = new GetAllReportTemplateResponse
                {
                    Id = item.Id,
                    Active = item.Active,
                    Description = item.Description,
                    Code = item.Code,
                    ScheduleCount = jobCount
                };

                returnData.Add(newRecord);
            }

            return returnData;



        }


        public async Task<GetReportTemplateDataResponse> GetData(GetReportTemplateDataRequest request, CancellationToken cancellationToken)
        {
            var reportTemplate =await Context.ReportTemplate.AsNoTracking().Where(x => x.Id == request.templateId).FirstOrDefaultAsync();
            if (reportTemplate != null)
            {
                var reportColumns = await Context.ReportTemplateColumn.AsNoTracking().ToListAsync();
                var reportParameters = await Context.ReportTemplateParameter.AsNoTracking().Where(x => x.ReportTemplateId == request.templateId).ToListAsync();

                var columns = new List<GetReportTemplateDataColumn>();
                var parameters = new List<GetReportTemplateDataParameter>();

                foreach (var item in reportColumns)
                {
                    var newRecord = new GetReportTemplateDataColumn
                    {
                        Id = item.Id,
                        Caption = item.Caption,
                        FieldName = item.FieldName
                    };
                    columns.Add(newRecord);
                }

                foreach (var item in reportParameters)
                {
                    var newRecord = new GetReportTemplateDataParameter
                    {
                        Id = item.Id,
                        Caption = item.Caption,
                        FielName = item.FieldName,
                        Descr = item.Descr,
                        Component = item.Component
                    };
                    parameters.Add(newRecord);
                }


                var returnData = new GetReportTemplateDataResponse
                {
                    Id = reportTemplate.Id,
                    Active = reportTemplate.Active,
                    Code = reportTemplate.Code,
                    Summary = reportTemplate.Summary,
                    Description = reportTemplate.Description,
                    DateCreated = reportTemplate.DateCreated,
                    DateUpdated = reportTemplate.DateUpdated,
                    Columns = columns,
                    Parameters = parameters
                };


                return returnData;
            }
            else {
                return new GetReportTemplateDataResponse();
            }

        }


        #region UserRoleData

        public async Task<Userinfo> GetUserRoleData(string username)
        {

            string queryData = @$"SELECT sre.RoleId, sre.EmployeeId from SysRoleEmployees sre WHERE sre.EmployeeId IN (SELECT Id FROM Employee e WHERE e.Active = 1 AND e.ADAccount = '{username}')";

            var outData = new Userinfo();

            var cacheName = $"Report_userinfo_{username}";

            if (_cacheService.TryGetValue(cacheName, out outData))
            {
                return outData;
            }
            else {
                var roleInfo = await ExecuteRolDataQuery(queryData);
                if (roleInfo != null)
                {
                    if (roleInfo.Count > 0)
                    {
                        var returndata = new Userinfo { RoleId = roleInfo[0].RoleId, EmployeeId = roleInfo[0].EmployeeId };
                        _cacheService.Set(cacheName, returndata, TimeSpan.FromMinutes(CACHE_MINUTE));
                        return returndata;

                    }
                    else
                    {
                        return null;
                    }

                }

                return null;


            }


        }



        private async Task<List<dynamic>> ExecuteRolDataQuery(string queryData)
        {

            try
            {
                using (var command = Context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = queryData;
                    command.CommandTimeout = 300;
                    await Context.Database.OpenConnectionAsync();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        var dynamicList = new List<dynamic>();
                        while (await result.ReadAsync())
                        {
                            dynamic d = new ExpandoObject();
                            for (int i = 0; i < result.FieldCount; i++)
                            {
                                ((IDictionary<string, object>)d).Add(result.GetName(i), result[i]);
                            }
                            dynamicList.Add(d);
                        }

                        return dynamicList;
                    }
                }
            }
            catch (Exception ec)
            {
                return null;
            }


        }


        #endregion



        #region Master
        public async Task<GetReportTemplateMasterResponse> GetMaster(GetReportTemplateMasterRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetReportTemplateMasterResponse();

            var jobTypes = new List<string>();
            jobTypes.Add(ReportScheduleTypes.RunTime);
            jobTypes.Add(ReportScheduleTypes.Daily);
            jobTypes.Add(ReportScheduleTypes.Weekly);
            jobTypes.Add(ReportScheduleTypes.Monthly);


            returnData.Months = new List<string>() { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            returnData.Weekdays = new List<string>() { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };






            returnData.JobType = jobTypes;


            return returnData;
        }



        #endregion

        #region GetDateTypes
        public async Task<GetReportDateVariablesResponse> GetDateTypes(GetReportDateVariablesRequest request, CancellationToken cancellationToken)
        {
            string[] dateTypes = DateFunctions.GetDateTypes();

            var returnData = new GetReportDateVariablesResponse();

            var newRecord = new List<string>();

            foreach (string dateType in dateTypes)
            {
               newRecord.Add("{" + dateType + "}");
            }
            returnData.DateVariables = newRecord;

            return returnData;
           
        }


        #endregion


        #region DateSimulator
        public async Task<DateTime> GetDateTypeSimulator(DateSimulationRequest request, CancellationToken cancellationToken)
        {

            var modifiedDateType = request.reportDateType.Replace("{", "").Replace("}", "");

            if (Enum.TryParse<ReportDateTypes>(modifiedDateType, true, out ReportDateTypes dateType))
            {
                var currentDate = DateFunctions.GetDate(dateType);
                return currentDate.AddDays(request.days);
            }
            else {
                DateTime currentDate;

                if (DateTime.TryParseExact(request.reportDateType, "yyyy-MM-dd",
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None,
                                           out currentDate))
                {

                    return currentDate.AddDays(request.days);
                }
                else
                {
                    throw new BadRequestException("Invalid report Date types");
                }


            }

        }


        #endregion



        #region DashboardData

        public async Task<GetDashboardResponse> GetDashboardData(GetDashboardRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetDashboardResponse();

            int allReportTemplateCount =await Context.ReportTemplate.AsNoTracking().CountAsync();
            int allReportScheduleCount = await Context.ReportJob.AsNoTracking().CountAsync();
            int activeReportTemplateCount = await Context.ReportTemplate.AsNoTracking().Where(x=> x.Active == 1).CountAsync();
            int activeReportScheduleCount = 0;
            var schedules = await Context.ReportJob.AsNoTracking().Select(x=> x.Id).ToListAsync();
            int  todayReportScheduleCount = 0;

            int uTodayCount = 0;
            int uTomorrowCount = 0;
            int uThisWeekCount = 0;
            int uThisMonthCount = 0;
            int uThisYearCount = 0;
            int uNextYearCount = 0;

            DateTime today = DateTime.Today;
            DateTime todayAfterNextWeek = today.AddDays(7);
            DateTime firstDayOfNextMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1);
            DateTime lastDayOfCurrentMonth = firstDayOfNextMonth.AddDays(-1);

            foreach (var item in schedules)
            {
                var nextExecuteData = await _hostedService.JobNextExecuteDate(item);
                if (nextExecuteData.HasValue)
                {
                    if (nextExecuteData.Value.Date == DateTime.Today)
                    { 
                        todayReportScheduleCount++;
                        uTodayCount++;
                    }
                    if (nextExecuteData.Value.Date == DateTime.Today.AddDays(-1))
                    {
                        uTomorrowCount++;

                    }
                    if (nextExecuteData.Value.Date >= today && nextExecuteData.Value.Date <= todayAfterNextWeek)
                    { 
                        uThisWeekCount++;   
                    
                    }
                    if (nextExecuteData.Value.Date >= today && nextExecuteData.Value.Date <= lastDayOfCurrentMonth)
                    {
                        uThisMonthCount++;

                    }
                    if (nextExecuteData.Value.Date.Year == today.Year)
                    {
                        uThisYearCount++;

                    }
                    if (nextExecuteData.Value.Date.Year == today.Year + 1 )
                    {
                        uNextYearCount++;

                    }


                    activeReportScheduleCount++;
                }
            }


            var schedulesTypeData = new List<GetDashboardScheduleTypes>();
            schedulesTypeData.Add(new GetDashboardScheduleTypes { ScheduleType = ReportScheduleTypes.Daily, ActiveCount = 0, AllCount = 0 });
            schedulesTypeData.Add(new GetDashboardScheduleTypes { ScheduleType = ReportScheduleTypes.Weekly, ActiveCount = 0, AllCount = 0 });
            schedulesTypeData.Add(new GetDashboardScheduleTypes { ScheduleType = ReportScheduleTypes.Monthly, ActiveCount = 0, AllCount = 0 });
            schedulesTypeData.Add(new GetDashboardScheduleTypes { ScheduleType = ReportScheduleTypes.RunTime, ActiveCount = 0, AllCount = 0 });

            foreach (var item in schedulesTypeData)
            {
               var scheData  =await  Context.ReportJob.AsNoTracking().Where(x => x.ScheduleType == item.ScheduleType).ToListAsync();
                item.AllCount = scheData.Count;
                foreach (var itemSchedule in scheData)
                {
                    var nextExecuteData = await _hostedService.JobNextExecuteDate(itemSchedule.Id);
                    if (nextExecuteData.HasValue)
                    { 
                        item.ActiveCount++; 
                    }
                }

                
            }

            var templateTypesData = await(from template in Context.ReportTemplate
                         join job in Context.ReportJob on template.Id equals job.ReportTemplateId
                         group job by new { Id = template.Id, name = template.Description } into groupedJobs
                         select new GetDashboardTemplateTypes
                         {
                             TemplateName = groupedJobs.Key.name,
                             AllCount = groupedJobs.Count()
                         }).ToListAsync();

            var upComingData = new List<UpComningSchedule>();

            upComingData.Add(new UpComningSchedule { UpcomingTypes = "TODAY", ScheduleCount = uTodayCount });
            upComingData.Add(new UpComningSchedule { UpcomingTypes = "TOMORROW", ScheduleCount = uTomorrowCount });
            upComingData.Add(new UpComningSchedule { UpcomingTypes = "THIS WEEK", ScheduleCount = uThisWeekCount });
            upComingData.Add(new UpComningSchedule { UpcomingTypes = "THIS MONTH", ScheduleCount = uThisMonthCount });
            upComingData.Add(new UpComningSchedule { UpcomingTypes = "THIS YEAR", ScheduleCount = uThisYearCount });
            upComingData.Add(new UpComningSchedule { UpcomingTypes = "NEXT YEAR", ScheduleCount = uNextYearCount });

            return new GetDashboardResponse
            {
                ActiveReportScheduleCount = activeReportScheduleCount,
                ActiveReportTemplateCount = activeReportTemplateCount,
                ALLReportScheduleCount = allReportScheduleCount,
                ALLReportTemplateCount = allReportTemplateCount,
                ScheduleTypesData = schedulesTypeData,
                TemplateTypeData = templateTypesData,
                TodayReportScheduleCount = todayReportScheduleCount,
                UpComningScheduleData = upComingData,
            };



        }

        #endregion



        #region UpdateParameterDesc


        public async Task UpdateTemplateParameter(UpdateTemplateParameterRequest request, CancellationToken cancellationToken)
        {

            foreach (var item in request.data)
            {
                var currentData = await Context.ReportTemplateParameter.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                if (currentData != null)
                {
                    currentData.Descr = item.Descr;
                    Context.ReportTemplateParameter.Update(currentData);
                }
            }


          await  Task.CompletedTask;
        }

        #endregion
    }
}
