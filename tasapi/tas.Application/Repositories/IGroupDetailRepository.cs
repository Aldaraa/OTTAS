using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.GetAllGroupDetail;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IGroupDetailRepository : IBaseRepository<GroupDetail>
    {
        Task<GetAllGroupDetailResponse> GetAllByGroupMasterId(int? active, int Id, CancellationToken cancellationToken);
    }
}
