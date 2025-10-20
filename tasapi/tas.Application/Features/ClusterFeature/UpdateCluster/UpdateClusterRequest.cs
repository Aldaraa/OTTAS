using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterFeature.UpdateCluster
{
    public sealed record UpdateClusterRequest(int Id, string Code, string Description, int Active, string DayNum, string Direction) : IRequest;
}
