using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.ActiveEmployeeDirectRequest;
using tas.Application.Features.EmployeeFeature.BulkDownloadEmployee;
using tas.Application.Features.EmployeeFeature.BulkDownloadGroupEmployee;
using tas.Application.Features.EmployeeFeature.BulkUploadEmployee;
using tas.Application.Features.EmployeeFeature.BulkUploadEmployeeGroup;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployee;
using tas.Application.Features.EmployeeFeature.BulkUploadPreviewEmployeeGroup;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeData;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeDataGroup;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeLocation;
using tas.Application.Features.EmployeeFeature.CheckADAccountEmployee;
using tas.Application.Features.EmployeeFeature.CreateEmployee;
using tas.Application.Features.EmployeeFeature.CreateEmployeeRequest;
using tas.Application.Features.EmployeeFeature.DeActiveEmployee;
using tas.Application.Features.EmployeeFeature.DeleteEmployeeTransport;
using tas.Application.Features.EmployeeFeature.DeleteEmployeeTransportBulk;
using tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheck;
using tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheckMultiple;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.EmployeeFeature.GetEmployeeAccountHistory;
using tas.Application.Features.EmployeeFeature.GetProfileTransport;
using tas.Application.Features.EmployeeFeature.ReActiveEmployee;
using tas.Application.Features.EmployeeFeature.RemovePassportImageEmployee;
using tas.Application.Features.EmployeeFeature.RosterExecuteEmployee;
using tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployeeAccommodation;
using tas.Application.Features.EmployeeFeature.SearchShortEmployee;
using tas.Application.Features.EmployeeFeature.StatusEmployee;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<SearchEmployeeResponse> SearchAdmin(SearchEmployeeRequest request, CancellationToken cancellationToken);
        Task EmployeeActiveCheck(int EmployeeId);

        Task<SearchShortEmployeeResponse> SearchShortAdmin(SearchShortEmployeeRequest request, CancellationToken cancellationToken);

        Task<GetEmployeeResponse> GetProfileAdmin(int Id, CancellationToken cancellationToken);

        Task<List<RosterExecuteEmployeeResponse>> RosterExecute(RosterExecuteEmployeeRequest request, CancellationToken cancellationToken);

        Task<RosterExecutePreviewEmployeeResponse> RosterExecuteRequest(RosterExecutePreviewEmployeeRequest request, CancellationToken cancellationToken);

        Task<StatusEmployeeResponse> GetStatusDates(StatusEmployeeRequest request, CancellationToken cancellationToken);

        Task CreateEmployeeValidateDB(CreateEmployeeRequest request, CancellationToken cancellationToken);

        Task CreateEmployeeRequestValidateDB(CreateEmployeeRequestRequest request, CancellationToken cancellationToken);

        Task UpdateEmployeeValidateDB(UpdateEmployeeRequest request, CancellationToken cancellationToken);

        Task<CheckADAccountEmployeeResponse> CheckADAccount(CheckADAccountEmployeeRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadEmployeeResponse> BulkRequestDownload(BulkDownloadEmployeeRequest request, CancellationToken cancellationToken);

        Task<BulkDownloadGroupEmployeeResponse> BulkRequestGroupDownload(BulkDownloadGroupEmployeeRequest request, CancellationToken cancellationToken);



        Task<List<int>> BulkRequestUpload(BulkUploadEmployeeRequest request, CancellationToken cancellationToken);

        Task ActiveEmployeeRequestDirect(ActiveEmployeeDirectRequest request, CancellationToken cancellationToken);


        Task BulkRequestGroupUpload(BulkUploadEmployeeGroupRequest request, CancellationToken cancellationToken);

        Task<BulkUploadPreviewEmployeeResponse> BulkRequestUploadPreview(BulkUploadPreviewEmployeeRequest request, CancellationToken cancellationToken);
        Task<BulkUploadPreviewEmployeeGroupResponse> BulkRequestUploadGroupPreview(BulkUploadPreviewEmployeeGroupRequest request, CancellationToken cancellationToken);




        public Task ChangeEmployeeData(ChangeEmployeeDataRequest request, CancellationToken cancellationToken);


        public Task UpdateProfileChangeData(Employee employee);


        Task<List<DeActiveEmployeeResponse>> DeActiveEmployee(DeActiveEmployeeRequest request, CancellationToken cancellationToken);


        Task ReActiveEmployee(ReActiveEmployeeRequest request, CancellationToken cancellationToken);

         Task DeleteTransport(DeleteEmployeeTransportRequest request, CancellationToken cancellationToken);

         Task DeleteTransportBulk(DeleteEmployeeTransportBulkRequest request, CancellationToken cancellationToken);



         Task<EmployeeDeActiveDateCheckResponse> EmployeeDeActiveDateCheck(EmployeeDeActiveDateCheckRequest request, CancellationToken cancellationToken);

         Task<List<EmployeeDeActiveDateCheckMultipleResponse>> EmployeeDeActiveDateCheckMultiple(EmployeeDeActiveDateCheckMultipleRequest request, CancellationToken cancellationToken);

         Task ChangeEmployeeDataGroup(ChangeEmployeeDataGroupRequest request, CancellationToken cancellationToken);


        Task ChangeEmployeeLocation(ChangeEmployeeLocationRequest request, CancellationToken cancellationToken);

        Task DeActiveEmployeeDelete(int EmployeeId, CancellationToken cancellationToken);


        Task<List<GetEmployeeAccountHistoryResponse>> GetAccountHistory(GetEmployeeAccountHistoryRequest request, CancellationToken cancellationToken);

        Task<List<GetProfileTransportResponse>> GetProfileTransport(GetProfileTransportRequest request, CancellationToken cancellationToken);

        Task<SearchEmployeeAccommodationResponse> SearchAccommodation(SearchEmployeeAccommodationRequest request, CancellationToken cancellationToken);

        Task RemovePassportImage(RemovePassportImageEmployeeRequest request, CancellationToken cancellationToken);



        Task<bool> OnsiteCheckEmployeeByRoster(int employeeId, DateTime RosterStartDate);


    }


}
