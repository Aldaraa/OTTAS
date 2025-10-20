using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterGroupFeature.GetAllRosterGroup;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
   
    public interface IRosterGroupRepository : IBaseRepository<RosterGroup>
    {
        Task<List<GetAllRosterGroupResponse>> GetAllData(GetAllRosterGroupRequest request, CancellationToken cancellationToken);
    }
}
