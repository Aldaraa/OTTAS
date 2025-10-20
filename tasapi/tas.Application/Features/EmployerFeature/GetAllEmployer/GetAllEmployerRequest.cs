using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.EmployerFeature.GetAllEmployer
{
    public sealed record GetAllEmployerRequest(int? status) : IRequest<List<GetAllEmployerResponse>>;
}
