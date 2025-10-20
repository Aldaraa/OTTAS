using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleEmployeeMenuFeature.GetAllSysRoleEmployeeMenu
{
    public sealed record GetAllSysRoleEmployeeMenuResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }


        public string? Route { get; set; }
        public int? OrderIndex { get; set; }

        public int? Head_ID { get; set; }

        public int? Permission { get; set; }
    


    }


}