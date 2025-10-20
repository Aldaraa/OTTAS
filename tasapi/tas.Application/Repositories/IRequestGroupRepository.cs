using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Features.RequestGroupFeature.CreateRequestGroup;
using tas.Application.Features.RequestGroupFeature.GetAllRequestGroup;
using tas.Application.Features.RequestGroupFeature.UpdateRequestGroup;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestGroupRepository : IBaseRepository<RequestGroup>
    {

        Task<List<GetAllRequestGroupResponse>> GetAllData(GetAllRequestGroupRequest request, CancellationToken cancellationToken);

        Task CreateData(CreateRequestGroupRequest request, CancellationToken cancellationToken);

        Task UpdateData(UpdateRequestGroupRequest request, CancellationToken cancellationToken);

    }
}
