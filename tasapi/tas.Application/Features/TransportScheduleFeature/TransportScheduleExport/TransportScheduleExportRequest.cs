using MediatR;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleExport;
using tas.Domain.Common;

namespace tas.Application.Features.TransportScheduleFeature.TransportScheduleExport
{
    public sealed record TransportScheduleExportRequest(

            DateTime StartDate,
            DateTime EndDate,
            int? TransportModeId,
            string? Direction

        ) : IRequest<TransportScheduleExportResponse>;


}
