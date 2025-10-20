using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterDetailFeature.DeleteClusterDetail
{
    public sealed record DeleteClusterDetailRequest(int Id) : IRequest;
}
