using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SafeModeEmployeeStatusFeature.CreateEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.GetEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.SetDSEmployeeStatus;
using tas.Application.Features.SafeModeEmployeeStatusFeature.SetRREmployeeStatus;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ISafeModeEmployeeStatusRepository : IBaseRepository<EmployeeStatus>
    {
        Task CreateEmployeeStatus(CreateEmployeeStatusRequest request, CancellationToken cancellationToken);

        Task<GetEmployeeStatusResponse> GetEmployeeStatus(GetEmployeeStatusRequest request, CancellationToken cancellationToken);

        Task<int> SetRREmployeeStatus(SetRREmployeeStatusRequest request, CancellationToken cancellationToken);
        Task<int> SetDSEmployeeStatus(SetDSEmployeeStatusRequest request, CancellationToken cancellationToken);
    }
}
