
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
using Application.Features.ReportJobFeature.GetSessionList;
using Application.Features.ReportJobFeature.KillSession;
using Application.Features.ReportJobFeature.LoadReportByIdJob;
using Application.Features.ReportJobFeature.LoadReportJob;
using Application.Features.ReportJobFeature.TestReportJob;
using Application.Features.ReportJobFeature.UpdateReportJobDaily;
using Application.Features.ReportJobFeature.UpdateReportJobMonthly;
using Application.Features.ReportJobFeature.UpdateReportJobRuntime;
using Application.Features.ReportJobFeature.UpdateReportJobWeekly;
using Application.Features.ReportTemplateFeature.UpdateTemplateParameter;
using Domain.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{

    public interface IReportJobRepository : IBaseRepository<ReportJob>
    {
        Task<List<GetAllReportJobResponse>> GetAllData(GetAllReportJobRequest request, CancellationToken cancellationToken);
        Task<GetReportJobResponse> GetJobData(GetReportJobRequest request, CancellationToken cancellationToken);


        Task LoadDataById(LoadReportByIdJobRequest request);


        Task CreateJobScheduleDaily(CreateReportJobDailyRequest request, CancellationToken cancellationToken);

        Task UpdateJobScheduleDaily(UpdateReportJobDailyRequest request, CancellationToken cancellationToken);



        Task CreateJobScheduleWeekly(CreateReportJobWeeklyRequest request, CancellationToken cancellationToken);

        Task UpdateJobScheduleWeekly(UpdateReportJobWeeklyRequest request, CancellationToken cancellationToken);


        Task CreateJobScheduleMonthly(CreateReportJobMonthlyRequest request, CancellationToken cancellationToken);


        Task UpdateJobScheduleMonthly(UpdateReportJobMonthlyRequest request, CancellationToken cancellationToken);



        Task CreateJobScheduleRuntime(CreateReportJobRuntimeRequest request, CancellationToken cancellationToken);


        Task UpdateJobScheduleRuntime(UpdateReportJobRuntimeRequest request, CancellationToken cancellationToken);


        Task LoadData(LoadReportJobRequest request);

        Task DeleteJobSchedule(DeleteReportJobRequest request, CancellationToken cancellationToken);


        Task<bool> ReportJobTimeValidate(CreateReportJobTimeValidateRequest request, CancellationToken cancellationToken);


        Task<List<int>> GetAvailableTime(GetAvailableTimeRequest request, CancellationToken cancellationToken);


        Task<List<GetReportJobLogResponse>> GetJobLogData(GetReportJobLogRequest request, CancellationToken cancellationToken);


        Task<List<DateTime>> GetAvailableTimeSlots(GetAvailableTimeSlotsRequest request, CancellationToken cancellationToken);


        Task<Userinfo> GetUserRoleData(string username);


    }

}
