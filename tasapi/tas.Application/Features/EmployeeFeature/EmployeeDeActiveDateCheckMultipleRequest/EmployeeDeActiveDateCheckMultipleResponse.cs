using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetEmployee;

namespace tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheckMultiple
{
    public sealed record EmployeeDeActiveDateCheckMultipleResponse
    {
        public bool DateValidationStatus { get; set; }

        public bool FutureTransportValidationStatus { get; set; }

        public int? Index { get; set; }

        public int? EmployeeId { get; set; }

        

    }

   


}
