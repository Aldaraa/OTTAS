using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeDashboardFeature.GetAllSysRoleEmployeeDashboard
{
    public sealed record GetAllSysRoleEmployeeDashboardResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }

        public int? Permission { get; set; }
    


    }


}