using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.SearchShortEmployee;

namespace tas.Application.Features.ActiveTransportFeature.CreateActiveTransport
{
    public sealed record CreateActiveTransportRequest(
        string Code, 
        string Description, 
        string DayNum, 
        string Direction, 
        int CarrierId,
        int TransportModeId,
        int TransportAudit,
        int Seats,
        int fromLocationId,
        int toLocationId,
        int Special,
        int FrequencyWeeks
    ) : IRequest;





}
