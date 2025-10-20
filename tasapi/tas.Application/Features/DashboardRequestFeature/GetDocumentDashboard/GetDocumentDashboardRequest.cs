using MediatR;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardRequestFeature.GetDocumentDashboard
{
    public sealed record GetDocumentDashboardRequest(DateTime? startdate, DateTime? endDate) : IRequest<GetDocumentDashboardResponse>;


}
