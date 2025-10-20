using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardSystemAdminFeature.GetPeopleTypeAndDepartment
{

    public sealed record GetPeopleTypeAndDepartmentResponse
    {
        public List<PeopleTypeEmployees> PeopleTypeEmployees { get; set; }


        public List<DepartmentEmployees> DepartmentEmployees { get; set; }



    }



    public sealed record PeopleTypeEmployees
    {
        public int? Count { get; set; }


        public string? Description { get; set; }

    }



    public sealed record DepartmentEmployees
    {
        public int Id { get; set; }
        public int? Count { get; set; }


        public string? Description { get; set; }

        public List<DepartmentEmployeesPeopleTypes> PeopleTypeData { get; set; }







    }

    public sealed record DepartmentEmployeesPeopleTypes
    {
        public int? Count { get; set; }

        public string? Description { get; set; }

        public int DepartmentId { get; set; }

    }

}
