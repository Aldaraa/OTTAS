using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysRoleFeature.GetEmployeeRoleInfo
{
    public sealed record GetEmployeeRoleInfoResponse
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public List<GetEmployeeGroupInfo> GroupInfo { get; set; }

        public List<GetEmployeeDepartmentAdminInfo> DepartmentAdminInfo { get; set; }


        public List<GetEmployeeDepartmentSupervisorInfo> DepartmentSupervisorInfo { get; set; }

        public List<GetEmployeeDepartmentManagerInfo> DepartmentManagerInfo { get; set; }

        public List<EmployeeMenu> Menu { get; set; }





    }


    public sealed record GetEmployeeGroupInfo
    {
        public int Id { get; set; }
        public string? GroupName { get; set; }  
    
    }


    public sealed record GetEmployeeDepartmentAdminInfo
    {
        public int Id { get; set; }
        public string? DepartmentName { get; set; }

    }


    public sealed record GetEmployeeDepartmentSupervisorInfo
    {
        public int Id { get; set; }
        public string? DepartmentName { get; set; }

    }


    public sealed record GetEmployeeDepartmentManagerInfo
    {
        public int Id { get; set; }
        public string? DepartmentName { get; set; }

    }


    public sealed record EmployeeMenu
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