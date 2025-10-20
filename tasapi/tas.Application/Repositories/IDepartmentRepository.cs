using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmenteFeature.BulkUploadPreviewDepartmentEmployees;
using tas.Application.Features.DepartmentFeature.AddDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.AddDepartmentManager;
using tas.Application.Features.DepartmentFeature.AddDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.BulkDownloadDepartment;
using tas.Application.Features.DepartmentFeature.BulkDownloadDepartmentEmployees;
using tas.Application.Features.DepartmentFeature.BulkUploadDepartment;
using tas.Application.Features.DepartmentFeature.BulkUploadDepartmentEmployees;
using tas.Application.Features.DepartmentFeature.CreateDepartment;
using tas.Application.Features.DepartmentFeature.CustomListDepartmment;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentManager;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.GetAdminsDepartment;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.DepartmentFeature.GetAllDepartmentAdmins;
using tas.Application.Features.DepartmentFeature.GetAllDepartmentManagers;
using tas.Application.Features.DepartmentFeature.GetAllReportDepartment;
using tas.Application.Features.DepartmentFeature.GetDepartment;
using tas.Application.Features.DepartmentFeature.GetManagersDepartment;
using tas.Application.Features.DepartmentFeature.GetParentDepartments;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentManager;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.UpdateDepartment;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<GetDepartmentResponse> GetbyId(GetDepartmentRequest request, CancellationToken cancellationToken);

        //     Task<List<Department>> GetAllDeparments(CancellationToken cancellationToken);


        Task<List<GetAllDepartmentResponse>> GetAllDepartmentsWithChildren(GetAllDepartmentRequest request, CancellationToken cancellationToken);

        Task<List<GetAllReportDepartmentResponse>> GetAllReportDepartmentsWithChildren(GetAllReportDepartmentRequest request, CancellationToken cancellationToken);



        Task<List<CustomListDepartmentResponse>> GetMinimumList(CancellationToken cancellationToken);

        Task<BulkDownloadDepartmentResponse> BulkRequestDownload(BulkDownloadDepartmentRequest request, CancellationToken cancellationToken);


        Task<BulkUploadPreviewDepartmentEmployeesResponse> BulkRequestEmployeesPreview(BulkUploadPreviewDepartmentEmployeesRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadDepartmentEmployeesResponse> BulkRequestDownloadEmployees(BulkDownloadDepartmentEmployeesRequest request, CancellationToken cancellationToken);

        Task  BulkRequestEmployeesUpload(BulkUploadDepartmentEmployeesRequest request, CancellationToken cancellationToken);


        Task<BulkUploadDepartmentResponse> BulkRequestUpload(BulkUploadDepartmentRequest request, CancellationToken cancellationToken);


        Task CreateDepartment(CreateDepartmentRequest request, CancellationToken cancellationToken);


        Task UpdateDepartment(UpdateDepartmentRequest request, CancellationToken cancellationToken);


        Task<List<GetAllDepartmentAdminsResponse>> GetAllDepartmentAdmins(GetAllDepartmentAdminsRequest request, CancellationToken cancellationToken);

        Task<List<GetAllDepartmentManagersResponse>> GetAllDepartmentManagers(GetAllDepartmentManagersRequest request, CancellationToken cancellationToken);

        Task AddDepartmentManager(AddDepartmentManagerRequest request, CancellationToken cancellationToken);

        Task AddDepartmentAdmin(AddDepartmentAdminRequest request, CancellationToken cancellationToken);

        Task DeleteDepartmentAdmin(DeleteDepartmentAdminRequest request, CancellationToken cancellationToken);


        Task DeleteDepartmentManager(DeleteDepartmentManagerRequest request, CancellationToken cancellationToken);

        Task DeleteDepartmentSupervisor(DeleteDepartmentSupervisorRequest request, CancellationToken cancellationToken);

        Task AddDepartmentSupervisor(AddDepartmentSupervisorRequest request, CancellationToken cancellationToken);

        Task<List<GetAdminsDepartmentResponse>> GetAdminsDepartment(GetAdminsDepartmentRequest request, CancellationToken cancellationToken);

        Task<List<GetManagersDepartmentResponse>> GetManagersDepartment(GetManagersDepartmentRequest request, CancellationToken cancellationToken);

        Task<List<GetParentDepartmentsResponse>> GetParentDepartments(GetParentDepartmentsRequest request, CancellationToken cancellationToken);

        Task SetMainDepartmentManager(SetMainDepartmentManagerRequest request, CancellationToken cancellationToken);

        Task SetMainDepartmentAdmin(SetMainDepartmentAdminRequest request, CancellationToken cancellationToken);

        Task SetMainDepartmentSupervisor(SetMainDepartmentSupervisorRequest request, CancellationToken cancellationToken);






    }
}
