using MediatR;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.TransportBookingInfo
{
    public sealed record TransportBookingInfoRequest(string? sapids, string? empIds, DateTime startDate, DateTime endDate, int? DepartmentId, int? EmployerId) : IRequest<List<TransportBookingInfoResponse>>;
}
