using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.GetEmployeeAccountHistory
{
    public sealed record GetEmployeeAccountHistoryResponse
    {
        public int? Id { get; set; }

        public DateTime? EventDate { get; set; }

        public string? Comment { get; set; }

        public string TerminationTypeName { get; set; }

        public string? Action { get; set; }

        public int EmployeeId { get; set; }
    }

  

}
