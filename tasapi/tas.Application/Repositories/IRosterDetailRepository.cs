using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterDetailFeature.GetRosterDetail;
using tas.Application.Features.RosterDetailFeature.UpdateRosterDetail;
using tas.Application.Features.RosterDetailFeature.UpdateSeqRosterDetail;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{


    public interface IRosterDetailRepository : IBaseRepository<RosterDetail>
    {
        Task<List<GetRosterDetailResponse>> GetbyRosterId(int RosterId, CancellationToken cancellationToken);
        Task UpdateReorderSequeensNumber(UpdateSeqRosterDetailRequest request, CancellationToken cancellationToken = default);

        Task UpdateData(UpdateRosterDetailRequest request, CancellationToken cancellationToken);
    }
}
