using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.SetSeqGroupMaster
{
    public sealed record SetSeqGroupMasterRequest(List<int> GroupMasterIds) : IRequest;
}
