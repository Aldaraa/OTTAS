using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardDataAdminFeature.GetOnsiteEmployeesData
{

    public sealed record GetOnsiteEmployeesDataResponse
    {
        public List<GetOnsiteEmployeesDataDepartments> Departments { get; set; }

        public List<GetOnsiteEmployeesDataPeopleType> PeopleTypes { get; set; }

    }

    public sealed record GetOnsiteEmployeesDataDepartments
    { 
        public string? Name { get; set; }

        public List<GetOnsiteEmployeesDataDepartmentsDate> DateData { get; set; }


    }


    public sealed record GetOnsiteEmployeesDataDepartmentsDate
    {
        public string? Date { get; set; }

        public int? Cnt { get; set; }

        public string? ChildKey { get; set; }
    }


    public sealed record GetOnsiteEmployeesDataPeopleType
    {
        public string? ParentKey { get; set; }

        public string? Name { get; set; }

        public List<GetOnsiteEmployeesDataPeopleTypeDate> DateData { get; set; }


    }


    public sealed record GetOnsiteEmployeesDataPeopleTypeDate
    {
        public string? PeopleTypeName { get; set; }

        public int? Cnt { get; set; }

    }

}
