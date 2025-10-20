using MediatR;
using tas.Application.Features.PositionFeature.GetAllPosition;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusGetEmployee
{
    public sealed record VisualStatusGetEmployeeRequest(int EmployeeId, DateTime StartDate) : IRequest<List<VisualStatusGetEmployeeResponse>>;
}
