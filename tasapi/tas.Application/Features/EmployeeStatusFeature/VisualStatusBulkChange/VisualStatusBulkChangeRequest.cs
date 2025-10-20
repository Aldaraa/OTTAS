using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.LocationFeature.GetAllLocation;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusBulkChange
{
    public sealed record VisualStatusBulkChangeRequest(int EmployeeId, DateTime StartDate, DateTime EndDate, int ShiftId) : IRequest;






}
