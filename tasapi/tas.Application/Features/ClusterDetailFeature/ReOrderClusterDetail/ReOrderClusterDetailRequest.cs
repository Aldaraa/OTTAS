using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterDetailFeature.ReOrderClusterDetail
{
    public sealed record ReOrderClusterDetailRequest(List<int> ClusterDetailIds, int ClusterId) : IRequest;
}
