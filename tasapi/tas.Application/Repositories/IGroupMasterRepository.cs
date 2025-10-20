using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.GetAllGroupDetail;
using tas.Application.Features.GroupMasterFeature.GetAllGroupMaster;
using tas.Application.Features.GroupMasterFeature.GetProfileGroupMaster;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IGroupMasterRepository : IBaseRepository<GroupMaster>
    {
        Task<GroupMaster> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetProfileGroupMasterResponse>> ProfileData(GetProfileGroupMasterRequest request, CancellationToken cancellationToken);

        Task<List<GetAllGroupMasterResponse>> GetAllData(GetAllGroupMasterRequest request, CancellationToken cancellationToken);

        Task ChangeOrderBy(List<int> GroupMasterIds, CancellationToken cancellationToken);

    }
}
