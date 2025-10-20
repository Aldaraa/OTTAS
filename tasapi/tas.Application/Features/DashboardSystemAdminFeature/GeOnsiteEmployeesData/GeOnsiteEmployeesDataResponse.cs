using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Features.DashboardSystemAdminFeature.GeOnsiteEmployeesData
{

    public sealed record GeOnsiteEmployeesDataResponse
    {
        public List<GeOnsiteEmployeesDataGender> GenderData { get; set; }


        public List<GeOnsiteEmployeesDataDepartment> DepartmentData { get; set; }


        public List<GeOnsiteEmployeesDataShift> ShiftData { get; set; }


        public List<GeOnsiteEmployeesDataCamp> CampData { get; set; }


        public List<GeOnsiteEmployeesDataPeopletype> Peopletype { get; set; }
  

    }



    public sealed record GeOnsiteEmployeesDataGender 
    {
        public string? Date { get; set; }

        public string? Gender { get; set; }

        public int? NumberOfEmployees { get; set; }


    }


    public sealed record GeOnsiteEmployeesDataDepartment
    {
        public string? Date { get; set; }
        public string? Department { get; set; }

        public int? NumberOfEmployees { get; set; }


    }


    public sealed record GeOnsiteEmployeesDataShift
    {
        public string? Date { get; set; }

        public string? Code { get; set; }

        public int? NumberOfEmployees { get; set; }


    }


    public sealed record GeOnsiteEmployeesDataCamp
    {
        public string? Date { get; set; }

        public string? Camp { get; set; }

        public int? NumberOfEmployees { get; set; }


    }



    public sealed record GeOnsiteEmployeesDataPeopletype
    {
        public string? Date { get; set; }

        public string? Code { get; set; }

        public int? NumberOfEmployees { get; set; }


    }
}
