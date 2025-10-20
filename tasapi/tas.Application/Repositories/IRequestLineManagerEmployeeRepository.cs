using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestLineManagerEmployeeFeature.CreateRequestLineManagerEmployee;
using tas.Application.Features.RequestLineManagerEmployeeFeature.GetRequestLineManagerEmployee;
using tas.Application.Features.RequestLineManagerEmployeeFeature.RemoveRequestLineManagerEmployee;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


    public interface IRequestLineManagerEmployeeRepository : IBaseRepository<RequestLineManagerEmployee>
    {
        Task CreateRequestLineManagerEmployee(CreateRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken);

        Task<List<GetRequestLineManagerEmployeeResponse>> GetRequestLineManagerEmployee(GetRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken);



        Task RemoveRequestLineManagerEmployee(RemoveRequestLineManagerEmployeeRequest request, CancellationToken cancellationToken);


    }

}
