using MediatR;

namespace tas.Application.Features.RequestTravelPurposeFeature.GetAllRequestTravelPurpose
{
    public sealed record GetAllRequestTravelPurposeRequest(int? status) : IRequest<List<GetAllRequestTravelPurposeResponse>>;
}
