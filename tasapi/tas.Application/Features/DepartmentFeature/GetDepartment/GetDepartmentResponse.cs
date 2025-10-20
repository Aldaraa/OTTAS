using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.GetDepartment
{
    public sealed record GetDepartmentResponse
    {
        //public int Id { get; set; }
        //public string? Name { get; set; }
        //public string? parentdepartmentname { get; set; }

        //public int? EmployeeCount { get; set; }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentDepartmentId { get; set; }
        public int Active { get; set; }

        public int? CostCodeId { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int EmployeeCount { get; set; }

        public List<GetDepartmentResponseAdmin> Admins { get; set; }

        public List<GetDepartmentResponseManager> Managers { get; set; }

        public List<GetDepartmentResponseSupervisor> Supervisors { get; set; }

        public List<GetDepartmentResponseCostCode> CostCodes { get; set; }




    }

    public sealed record GetDepartmentResponseAdmin
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }

        public int? SAPID { get; set; }

        public string? ADAccount { get; set; }

        public int? Main { get; set; }
    }


    public sealed record GetDepartmentResponseManager
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }

        public int? SAPID { get; set; }
        public string? ADAccount { get; set; }

        public int? Main { get; set; }



    }



    public sealed record GetDepartmentResponseSupervisor
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? FullName { get; set; }

        public int? SAPID { get; set; }

        public string? ADAccount { get; set; }

        public int? Main { get; set; }

    }



    public sealed record GetDepartmentResponseCostCode
    {
        public int Id { get; set; }
        public string? CostCode { get; set; }

    }




}
