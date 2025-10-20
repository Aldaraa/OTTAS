using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.StatusChangesEmployeeRequestFeature.GetStatusChangesEmployeeRequestDeActive
{
    //Request
    public sealed record GetStatusChangesEmployeeRequestDeActiveResponse
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string? FullName { get; set; }
        public DateTime? EventDate { get; set; }

        public string? Comment { get; set; }

        public string? TerminationTypeName { get; set; }
        public string? CreatedEmployeeName { get; set; }

        public DateTime? CreatedDate { get; set; }

    }

 
}