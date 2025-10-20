using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.LocationFeature.GetAllLocation;

namespace tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChange
{
    public sealed record VisualStatusDateChangeRequest(int EmployeeId, List<StatusDate> StatusDates) : IRequest;


    public sealed record StatusDate
    { 
        public DateTime EventDate { get; set; }

        public int  ShiftId { get; set; }


    }




}
