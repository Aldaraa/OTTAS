using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportScheduleFeature.CreateScheduleActiveTransport
{

    public sealed record CreateScheduleActiveTransportRequest(
        string Code, 
        int CarrierId,
        int TransportModeId,
        int inSeats,
        int OutSeats,

        int fromLocationId,
        int toLocationId,
        int FrequencyWeeks,
        DateTime StartDate,
        DateTime  EndDate,
        string? ETA,
        string? ETD,
        string? outETA,
        string? outETD,
        string[] dayNums,
        string? AircraftCode


    ) : IRequest;
}
