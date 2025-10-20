using Application.Features.ReportJobFeature.BuildReport;
using Application.Features.ReportJobFeature.CreateReportJobDaily;
using Application.Features.ReportJobFeature.CreateReportJobMonthly;
using Application.Features.ReportJobFeature.CreateReportJobRuntime;
using Application.Features.ReportJobFeature.CreateReportJobTimeValidate;
using Application.Features.ReportJobFeature.CreateReportJobWeekly;
using Application.Features.ReportJobFeature.DeleteReportJob;
using Application.Features.ReportJobFeature.GetAllReportJob;
using Application.Features.ReportJobFeature.GetSessionList;
using Application.Features.ReportJobFeature.KillSession;
using Application.Features.ReportJobFeature.LoadReportJob;
using Application.Features.ReportJobFeature.TestReportJob;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{

    public interface IJobExecuteServiceRepository : IBaseRepository<ReportJob>
    {
        Task<ExecuteDataResponse> ExecuteData(int Id);


        Task<TestReportJobResponse> TestJobSchedule(TestReportJobRequest request, CancellationToken cancellationToken);

        Task<BuildReportResponse> CreateBuildCommand(BuildReportRequest request, CancellationToken cancellationToken);




        Task<List<GetSessionListResponse>> GetCurrentSessions(GetSessionListRequest request, CancellationToken cancellationToken);

        Task KillSessionForce(KillSessionRequest request, CancellationToken cancellationToken);

    }
}
