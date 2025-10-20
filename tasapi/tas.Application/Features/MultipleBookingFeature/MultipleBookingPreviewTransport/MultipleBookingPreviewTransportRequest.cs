using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MultipleBookingFeature.MultipleBookingPreviewTransport
{
    public sealed record MultipleBookingPreviewTransportRequest(
            List<int> EmployeeIds,
            int firsScheduleId,
            int? lastSheduleId



        ) : IRequest<List<MultipleBookingPreviewTransportResponse>>;
}
