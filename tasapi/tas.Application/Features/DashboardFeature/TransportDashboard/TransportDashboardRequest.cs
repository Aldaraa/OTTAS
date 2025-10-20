using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DashboardFeature.TransportDashboard;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.Application.Features.DashboardFeature.TransportDashboard
{
    public sealed record TransportDashboardRequest(int? status) : IRequest <TransportDashboardResponse>;
}
