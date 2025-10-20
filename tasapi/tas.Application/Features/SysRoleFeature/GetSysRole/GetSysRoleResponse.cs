using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleFeature.GetSysRole
{
    public sealed record GetSysRoleResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public string? RoleTag { get; set; }


        public int? DataPermission { get; set; }

        public int? EmloyeeCount { get; set; }

        public List<RoleEmployee> RoleUsers { get; set; }
        
    }

    public sealed record RoleEmployee
    {
        public int? Id { get; set; }
        public int? EmployeeId { get; set; }

        public string? Lastname { get; set; }

        public string? Firstname { get; set; }

        public string? AdAccount { get; set; }

        public int? ReadonlyAccess { get; set; }

        public bool? HasMenu { get; set; }

        public bool? HasReport { get; set; }

        public bool? HasReportData { get; set; }





        public DateTime? LastLoginDate { get; set; }



    }




}