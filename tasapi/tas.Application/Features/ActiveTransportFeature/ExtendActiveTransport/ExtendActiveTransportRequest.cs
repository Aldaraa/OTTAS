using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.SearchShortEmployee;

namespace tas.Application.Features.ActiveTransportFeature.ExtendActiveTransport
{
    public sealed record ExtendActiveTransportRequest(
        int ActiveTransportId, 
        DateTime StartDate, 
        DateTime EndDate, 
        int? Seats, 
        string ETA,
        string ETD
    ) : IRequest;





}
