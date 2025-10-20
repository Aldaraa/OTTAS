using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupDetailFeature.CreateGroupDetail
{
    public sealed record CreateGroupDetailRequest(string Code, string Description,  int?  isDefault, int GroupMasterId) : IRequest;
}
