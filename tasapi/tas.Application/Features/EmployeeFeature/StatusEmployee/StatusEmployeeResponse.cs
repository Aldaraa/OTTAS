using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;

namespace tas.Application.Features.EmployeeFeature.StatusEmployee
{

    public sealed record StatusEmployeeResponse 
    {
        public List<EmployeeStatusDate> employeeStatusDates { get; set; }

        public List<EmployeeTransport> employeeTransports { get; set; }
    }
}
