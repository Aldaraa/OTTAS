using Application.Features.ReportTemplateFeature.DateSimulation;
using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Features.ReportTemplateFeature.GetDashboard;
using Application.Features.ReportTemplateFeature.GetReportDateVariables;
using Application.Features.ReportTemplateFeature.GetReportTemplateData;
using Application.Features.ReportTemplateFeature.GetReportTemplateMaster;
using Application.Features.ReportTemplateFeature.UpdateTemplateParameter;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{


    public interface IReportTemplateRepository : IBaseRepository<ReportTemplate>
    {
        Task<List<GetAllReportTemplateResponse>> GetAllData(GetAllReportTemplateRequest request, CancellationToken cancellationToken);

        Task<GetReportTemplateDataResponse> GetData(GetReportTemplateDataRequest request, CancellationToken cancellationToken);
        Task<GetReportTemplateMasterResponse> GetMaster(GetReportTemplateMasterRequest request, CancellationToken cancellationToken);

        Task<GetReportDateVariablesResponse> GetDateTypes(GetReportDateVariablesRequest request, CancellationToken cancellationToken);

        Task<DateTime> GetDateTypeSimulator(DateSimulationRequest request, CancellationToken cancellationToken);



        Task<GetDashboardResponse> GetDashboardData(GetDashboardRequest request, CancellationToken cancellationToken);

        Task UpdateTemplateParameter(UpdateTemplateParameterRequest request, CancellationToken cancellationToken);


    }


}