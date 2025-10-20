using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterFeature.DeleteCluster
{
    public sealed record DeleteClusterRequest(int Id) : IRequest;
}
