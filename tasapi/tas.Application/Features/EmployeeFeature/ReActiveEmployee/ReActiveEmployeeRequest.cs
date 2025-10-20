using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.ReActiveEmployee
{
    public sealed record ReActiveEmployeeRequest(List<ReActiveEmployee> Employees
        ) : IRequest<Unit>;


    public sealed record ReActiveEmployee
    {
        public int EmployeeId { get; set; }
        public DateTime EventDate { get; set; }

        public int? EmployerId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CostCodeId { get; set; }



    }
}