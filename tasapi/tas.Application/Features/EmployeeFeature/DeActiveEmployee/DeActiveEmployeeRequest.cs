using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.DeActiveEmployee
{
    public sealed record DeActiveEmployeeRequest(List<DeActiveEmployee> Employees
        ) : IRequest<List<DeActiveEmployeeResponse>>;


    public sealed record DeActiveEmployee
    {
        public int EmployeeId { get; set; }
        public DateTime EventDate { get; set; }

        public string? Comment { get; set; }

        public int? DemobTypeTypeId { get; set; }

    }
}