using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCode;
using tas.Application.Features.CostCodeFeature.BulkDownloadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCode;
using tas.Application.Features.CostCodeFeature.BulkUploadCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCode;
using tas.Application.Features.CostCodeFeature.BulkUploadPreviewCostCodeEmployees;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ICostCodeRepository : IBaseRepository<CostCode>
    {
        Task<CostCode> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetAllCostCodeResponse>> GetAllData(GetAllCostCodeRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadCostCodeResponse> BulkRequestDownload(BulkDownloadCostCodeRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadCostCodeEmployeesResponse> BulkRequestEmployeeDownload(BulkDownloadCostCodeEmployeesRequest request, CancellationToken cancellationToken);

        Task BulkRequestUpload(BulkUploadCostCodeRequest request, CancellationToken cancellationToken);
        Task BulkRequestEmployeesUpload(BulkUploadCostCodeEmployeesRequest request, CancellationToken cancellationToken);



        Task<BulkUploadPreviewCostCodeResponse> BulkRequestPreview(BulkUploadPreviewCostCodeRequest request, CancellationToken cancellationToken);

        Task<BulkUploadPreviewCostCodeEmployeesResponse> BulkRequestEmployeesPreview(BulkUploadPreviewCostCodeEmployeesRequest request, CancellationToken cancellationToken);





    }
}
