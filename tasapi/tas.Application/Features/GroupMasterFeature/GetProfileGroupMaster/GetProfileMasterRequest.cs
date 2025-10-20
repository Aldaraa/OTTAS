using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.GetProfileGroupMaster
{
    public sealed record GetProfileGroupMasterRequest : IRequest<List<GetProfileGroupMasterResponse>>;
}
