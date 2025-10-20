using MediatR;

namespace tas.Application.Features.RequestTravelAgentFeature.GetAllRequestTravelAgent
{
    public sealed record GetAllRequestTravelAgentRequest(int? status) : IRequest<List<GetAllRequestTravelAgentResponse>>;
}
