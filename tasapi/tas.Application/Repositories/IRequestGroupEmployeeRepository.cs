using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestGroupEmployeeFeature.AddRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetAllRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupInpersonateUsers;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerAdminEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.OrderRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.RequestGroupActiveEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.SetPrimaryRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.UpdateRequestGroupEmployees;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface  IRequestGroupEmployeeRepository :  IBaseRepository<RequestGroupEmployee>
    {
        Task<List<RequestGroupActiveEmployeesResponse>> GetActiveEmployees(RequestGroupActiveEmployeesRequest request, CancellationToken cancellationToken);

        Task<GetRequestGroupEmployeesResponse> GetGroupEmployees(GetRequestGroupEmployeesRequest request, CancellationToken cancellationToken);
        Task<GetRequestLineManagerEmployeesResponse> GetLineManagerEmployees(GetRequestLineManagerEmployeesRequest request, CancellationToken cancellationToken);


        Task<List<GetRequestLineManagerAdminEmployeesResponse>> GetLineManagerAdminEmployees(GetRequestLineManagerAdminEmployeesRequest request, CancellationToken cancellationToken);
      
        
        Task AddGroupEmployees(AddRequestGroupEmployeesRequest request, CancellationToken cancellationToken);

        Task RemoveGroupEmployees(RemoveRequestGroupEmployeesRequest request, CancellationToken cancellationToken);

        Task UpdateGroupEmployees(UpdateRequestGroupEmployeesRequest request, CancellationToken cancellationToken);

        Task SetPrimaryGroupEmployees(SetPrimaryRequestGroupEmployeesRequest request, CancellationToken cancellationToken);


        Task OrderGroupEmployees(OrderRequestGroupEmployeesRequest request, CancellationToken cancellationToken);

        Task<List<GetAllRequestGroupEmployeesResponse>> GetAllGroupEmployees(GetAllRequestGroupEmployeesRequest request, CancellationToken cancellationToken);



        Task<List<GetRequestGroupInpersonateUsersResponse>> GetRequestGroupInpersonateUsers(GetRequestGroupInpersonateUsersRequest request, CancellationToken cancellationToken);


    }
}
