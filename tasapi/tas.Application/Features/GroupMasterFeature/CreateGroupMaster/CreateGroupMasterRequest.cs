using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.CreateGroupMaster
{
    public sealed record CreateGroupMasterRequest(string description, int? ShowOnProfile, int? OrderBy, int? CreateLog, int? Required) : IRequest;
}
