using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.EmployerFeature.GetAllReportEmployer
{
    public sealed record GetAllReportEmployerRequest: IRequest<List<GetAllReportEmployerResponse>>;
}
