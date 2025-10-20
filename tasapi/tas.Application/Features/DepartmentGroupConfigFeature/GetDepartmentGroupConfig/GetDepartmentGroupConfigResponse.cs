using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.DepartmentGroupConfigFeature.GetDepartmentGroupConfig
{
    public sealed record GetDepartmentGroupConfigResponse
    {
        public int Id { get; set; } 
        public string? GroupMasterName { get; set; }

        public int? GroupMasterId { get; set; }


        public string? GroupDetailName { get; set; }

        public int? GroupDetailId { get; set; }


        public string? EmployerName { get; set; } 

        public int? EmployerId { get; set; }


    }
}
