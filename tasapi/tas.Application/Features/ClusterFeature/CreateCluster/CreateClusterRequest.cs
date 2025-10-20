using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterFeature.CreateCluster
{
    public sealed record CreateClusterRequest(string Code, string Description,  int Active, string DayNum, string Direction) : IRequest;
}
