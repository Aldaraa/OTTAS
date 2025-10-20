using MediatR;

namespace tas.Application.Features.EmployeeStatusFeature.DateLastEmployeeStatus
{
    public sealed record DateLastEmployeeStatusRequest(int EmployeeId, DateTime EventDate, int Onsite, bool laststatus= true) : IRequest<DateLastEmployeeStatusResponse>;
}
