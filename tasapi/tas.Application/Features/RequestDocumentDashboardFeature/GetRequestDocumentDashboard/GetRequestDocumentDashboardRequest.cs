using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentDashboardFeature.GetRequestDocumentDashboard
{
    public sealed record GetRequestDocumentDashboardRequest(DateTime? DashboarDate) : IRequest<GetRequestDocumentDashboardResponse>;
}
