using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterDetailFeature.CreateClusterDetail
{
    public sealed record CreateClusterDetailRequest(int ActiveTransportId, int ClusterId, int SeqNumber,  int Active) : IRequest;
}
