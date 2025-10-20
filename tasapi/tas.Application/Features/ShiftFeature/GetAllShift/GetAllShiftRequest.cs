using MediatR;

namespace tas.Application.Features.ShiftFeature.GetAllShift
{
    public sealed record GetAllShiftRequest(int? status) : IRequest<List<GetAllShiftResponse>>;
}
