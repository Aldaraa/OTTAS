using MediatR;

namespace tas.Application.Features.StateFeature.GetAllState
{
    public sealed record GetAllStateRequest(int? status) : IRequest<List<GetAllStateResponse>>;
}
