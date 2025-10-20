using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuthenticationFeature.DepartmentRole
{
    public sealed record DepartmentRoleResponse
    {
        public List<int> EmployeeIds { get; set; }
       
        public List<int> DepartmentsIds { get; set; }


    }


}
