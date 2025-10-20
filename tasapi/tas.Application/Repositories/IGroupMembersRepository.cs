using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupMembersFeature.CreateGroupMembers;
using tas.Application.Features.PositionFeature.AllPosition;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IGroupMembersRepository : IBaseRepository<GroupMembers>
    {

        Task SaveData(CreateGroupMembersRequest request, CancellationToken cancellationToken);



    }
}
