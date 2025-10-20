using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.GroupDetailFeature.GetAllGroupDetail
{

    public sealed record GetAllGroupDetailResponse
    { 
        public int Id { get; set; }

        public string? Name { get; set; }

        public List<GetAllGroupDetail> details { get; set; }


    }


    public sealed record GetAllGroupDetail
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? isDefault { get; set; }

        public int? GroupMasterId { get; set; }


        public int? EmployeeCount { get; set; }

        public int Active { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
