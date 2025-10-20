using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PositioneFeature.BulkUploadPosition;
using tas.Application.Features.PositioneFeature.BulkUploadPreviewPosition;
using tas.Application.Features.PositionFeature.AllPosition;
using tas.Application.Features.PositionFeature.BulkDownloadPosition;
using tas.Application.Features.PositionFeature.BulkDownloadPositionEmployees;
using tas.Application.Features.PositionFeature.BulkUploadPositionEmployees;
using tas.Application.Features.PositionFeature.BulkUploadPreviewPositionEmployees;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IPositionRepository : IBaseRepository<Position>
    {
        Task<Position> GetbyId(int Id, CancellationToken cancellationToken);

        Task<GetAllPositionResponse> GetAllData(GetAllPositionRequest request, CancellationToken cancellationToken);

        Task<List<AllPositionResponse>> AllData(AllPositionRequest request, CancellationToken cancellationToken);

        Task BulkRequestUpload(BulkUploadPositionRequest request, CancellationToken cancellationToken);

        Task BulkRequestEmployeesUpload(BulkUploadPositionEmployeesRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadPositionResponse> BulkRequestDownload(BulkDownloadPositionRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadPositionEmployeesResponse> BulkRequestEmployeeDownload(BulkDownloadPositionEmployeesRequest request, CancellationToken cancellationToken);



        Task<BulkRequestUploadPreviewPositionResponse> BulkRequestUploadPreview(BulkRequestUploadPreviewPositionRequest request, CancellationToken cancellationToken);


        Task<BulkUploadPreviewPositionEmployeesResponse> BulkRequestEmployeesPreview(BulkUploadPreviewPositionEmployeesRequest request, CancellationToken cancellationToken);

        

    }

}
