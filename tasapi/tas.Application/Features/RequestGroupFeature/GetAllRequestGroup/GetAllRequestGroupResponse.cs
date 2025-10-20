using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupFeature.GetAllRequestGroup
{
    public sealed record GetAllRequestGroupResponse
    {
        public int Id { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }

        public int ReadOnly { get; set; }
        public DateTime? DateCreated { get; set; }

        public int? EmployeeCount { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
