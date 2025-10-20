using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMembersFeature.CreateGroupMembers
{
    public sealed record CreateGroupMembersRequest(int EmployeeId, List<GroupMembersGroup> GroupData ) : IRequest;

    public sealed record GroupMembersGroup
    {
        public int? GroupDetailId { get; set; }

        public int? GroupMasterId { get; set; }
    }
}
