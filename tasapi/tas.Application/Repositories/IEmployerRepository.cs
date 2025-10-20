using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCode;
using tas.Application.Features.EmployerFeature.BulkDownloadEmployer;
using tas.Application.Features.EmployerFeature.BulkDownloadEmployerEmployees;
using tas.Application.Features.EmployerFeature.BulkUploadEmployer;
using tas.Application.Features.EmployerFeature.BulkUploadEmployerEmployees;
using tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployer;
using tas.Application.Features.EmployerFeature.BulkUploadPreviewEmployerEmployees;
using tas.Application.Features.EmployerFeature.GetAllEmployer;
using tas.Application.Features.EmployerFeature.GetAllReportEmployer;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IEmployerRepository : IBaseRepository<Employer>
    {
        Task<Employer> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetAllEmployerResponse>> GetAllData(GetAllEmployerRequest request, CancellationToken cancellationToken);
        Task<List<GetAllReportEmployerResponse>> GetAllReportData(GetAllReportEmployerRequest request, CancellationToken cancellationToken);

        Task<BulkEmployerUploadPreviewResponse> BulkRequestPreview(BulkEmployerUploadPreviewRequest request, CancellationToken cancellationToken);

        Task BulkRequestUpload(BulkUploadEmployerRequest request, CancellationToken cancellationToken);


        Task BulkRequestEmployeesUpload(BulkUploadEmployerEmployeesRequest request, CancellationToken cancellationToken);


        Task<BulkDownloadEmployerResponse> BulkRequestDownload(BulkDownloadEmployerRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadEmployerEmployeesResponse> BulkRequestEmployeesDownload(BulkDownloadEmployerEmployeesRequest request, CancellationToken cancellationToken);
        Task<BulkEmployerUploadPreviewEmployeesResponse> BulkRequestPreviewEmployees(BulkEmployerUploadPreviewEmployeesRequest request, CancellationToken cancellationToken);

    }
}
