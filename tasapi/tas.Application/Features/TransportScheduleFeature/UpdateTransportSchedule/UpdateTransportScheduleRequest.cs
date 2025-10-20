using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateTransportSchedule
{
    public sealed record UpdateTransportScheduleRequest(
        int Id,
        string ETD,
        string ETA,
        int Seats,
        string TransportCode, 
        int CarrierId,
        int TransportModeId

        ) : IRequest;
}
