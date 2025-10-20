using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.DashboardFeature.EmployeeDashboard
{
    public sealed record EmployeeDashboardResponse
    {

        public int? TotalEmployees { get; set; }

        public int? TotalActiveEmployees { get; set; }

        public int? TotalDeActiveEmployees { get; set; }



        public int? EmployeeMN { get; set; }
        public int? EmployeeOtherCountries { get; set; }

        public int? OnsiteEmployees { get; set; }

        public int? OffSiteEmployees { get; set; }


        public List<SiteStatusEmployee> WeekData { get; set; }

    }

    public sealed record SiteStatusEmployee
    { 
        public DateTime date { get; set; }

        public int OnsiteEmployee { get; set; }

        public int OffsiteEmployee { get; set; }

    }

}
