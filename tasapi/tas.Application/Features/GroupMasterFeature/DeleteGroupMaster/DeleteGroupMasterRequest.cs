using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupMasterFeature.DeleteGroupMaster
{
    public sealed record DeleteGroupMasterRequest(int Id) : IRequest;
}
