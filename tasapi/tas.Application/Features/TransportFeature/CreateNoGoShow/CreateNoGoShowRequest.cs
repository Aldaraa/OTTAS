using MediatR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.CreateNoGoShow
{
    public sealed record CreateNoGoShowRequest(
        int EmployeeId,
        DateTime EventDate,
        string? Direction,
        string? Reason,
        string? Description,
        int? ScheduleId,
        bool NoShow =true



        ) : IRequest;
}
