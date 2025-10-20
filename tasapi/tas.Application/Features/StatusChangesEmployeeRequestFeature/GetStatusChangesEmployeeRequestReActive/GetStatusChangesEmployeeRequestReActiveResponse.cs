using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestReActive
{
    //Request
    public sealed record GetStatusChangesEmployeeRequestReActiveResponse
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string? FullName { get; set; }
        public DateTime? EventDate { get; set; }

        public string? CreatedEmployeeName { get; set; }

        public DateTime? CreatedDate { get; set; }

    }

 
}