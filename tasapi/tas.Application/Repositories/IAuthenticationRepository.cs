using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.AuthenticationFeature.AgreementCheck;
using tas.Application.Features.AuthenticationFeature.ClearADCache;
using tas.Application.Features.AuthenticationFeature.DepartmentRole;
using tas.Application.Features.AuthenticationFeature.ImpersoniteUser;
using tas.Application.Features.AuthenticationFeature.LoginUser;
using tas.Application.Features.AuthenticationFeature.RemoveUserCache;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IAuthenticationRepository : IBaseRepository<Employee>
    {
        Task<LoginUserResponse> LoginUser(LoginUserRequest request, CancellationToken cancellationToken);
        Task<ImpersoniteUserResponse> ImpersoniteUserData(ImpersoniteUserRequest request, CancellationToken cancellationToken);

        Task<LoginUserResponse> LoginUserMiddleware(LoginUserRequest request);


        Task<DepartmentRoleResponse> GetDepartmentRoleData(DepartmentRoleRequest request);

        Task AgreementCheck(AgreementCheckRequest request, CancellationToken cancellationToken);

        Task ClearADCache(ClearADCacheRequest request, CancellationToken cancellationToken);


        Task RemoveUserCache(RemoveUserCacheRequest request, CancellationToken cancellationToken);









    }
}
