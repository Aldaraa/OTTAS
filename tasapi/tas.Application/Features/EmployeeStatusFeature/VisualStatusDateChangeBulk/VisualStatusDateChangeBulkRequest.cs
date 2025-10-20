using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.LocationFeature.GetAllLocation;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChangeBulk
{
    public sealed record VisualStatusDateChangeBulkRequest(List<int> EmployeeIds, int ShiftId,  DateTime startDate, DateTime endDate) : IRequest<List<VisualStatusDateChangeBulkResponse>>;


}
