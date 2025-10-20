using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.AddExternalTravel
{
    public sealed record AddExternalTravelRequest(
        int EmployeeId,
        int FirstSheduleId,
        int? LastSheduleId,

        int DepartmentId,
        int PositionId, 
        int EmployerId, 
        int CostCodeId
        ) : IRequest;
}
