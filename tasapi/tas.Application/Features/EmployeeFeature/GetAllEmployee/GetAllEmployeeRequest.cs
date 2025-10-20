using MediatR;

namespace tas.Application.Features.EmployeeFeature.GetAllEmployee
{
    public sealed record GetAllEmployeeRequest(int? status) : IRequest<List<GetAllEmployeeResponse>>;
}
