using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeStatusFeature.GetDateRangeStatus
{
    public sealed record GetDateRangeStatusResponse
    {
        public int Id { get; set; }
        
        public string? ShiftCode { get; set; }


        public string? Description { get; set; }

        public string? Color { get; set; }

        public DateTime? EvenDate { get; set; }


    }
}
