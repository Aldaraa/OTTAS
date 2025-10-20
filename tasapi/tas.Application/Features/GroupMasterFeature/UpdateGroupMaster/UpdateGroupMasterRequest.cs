using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.UpdateGroupMaster
{
    public sealed record UpdateGroupMasterRequest(int Id,  string Description, int? ShowOnProfile, int? OrderBy, int? CreateLog, int? Required) : IRequest;
}
