using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportScheduleFeature.CreateScheduleDriveTransport
{

    public sealed record CreateScheduleDriveTransportRequest(
        string Code, 
        int fromLocationId,
        int toLocationId,
        int FrequencyWeeks,
        DateTime StartDate,
        DateTime  EndDate,
        string? ETD,
        string? outETD,
        string[] dayNums,
        string? AircraftCode


    ) : IRequest;
}
