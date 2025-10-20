using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Features.RosterFeature.GetAllRoster;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRosterRepository : IBaseRepository<Roster>
    {
        Task<List<GetAllRosterResponse>> GetAllData(GetAllRosterRequest request, CancellationToken cancellationToken);
    }
}
