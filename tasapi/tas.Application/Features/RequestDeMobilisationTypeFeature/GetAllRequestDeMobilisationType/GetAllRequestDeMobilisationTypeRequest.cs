using MediatR;

namespace tas.Application.Features.RequestDeMobilisationTypeFeature.GetAllRequestDeMobilisationType
{
    public sealed record GetAllRequestDeMobilisationTypeRequest(int? status) : IRequest<List<GetAllRequestDeMobilisationTypeResponse>>;
}
