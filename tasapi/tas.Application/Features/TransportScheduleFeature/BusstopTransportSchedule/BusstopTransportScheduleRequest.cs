using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee;
using tas.Domain.Common;

namespace tas.Application.Features.TransportScheduleFeature.BusstopTransportSchedule
{
    public sealed record BusstopTransportScheduleRequest(BusstopRequestModel model) : BasePagenationRequest, IRequest<BusstopTransportScheduleResponse>;

    public record BusstopRequestModel(
        int? transportModeId, 
        int? departLocationId,
        int? arriveLocationId,
        bool? external,
        DateTime? startDate, 
        DateTime? endDate,
        string? Code
    );
}



