using MediatR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.ActiveTransportFeature.CreateSpecialActiveTransport
{

    public sealed record CreateSpecialActiveTransportRequest(
        string Code, 
        int CarrierId,
        int TransportModeId,
        int Seats,
        int? OutSeats,
        int fromLocationId,
        int toLocationId,
        DateTime EventDate,
        string ETA,
        string ETD,
        int CostCodeId,
        string? OUTETA,
        string? OUTETD,
        string? AircraftCode
    ) : IRequest;
}



