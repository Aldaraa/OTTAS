using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupById
{
    
    public sealed record RequestDocumentGroupByIdResponse
    {
        public int id { get; set; }
        public string? GroupName { get; set; }

        public int OrderIndex { get; set; }

        public string? GroupTag { get; set; }

        public int? GroupId { get; set; }
        public List<RequestDocumentGroupByIdEmployees> groupMembers { get; set; }


    }

    public sealed record RequestDocumentGroupByIdEmployees
    {
        public int id { get; set; }

        public int? employeeId { get; set; }

        public string? displayName { get; set; }

        public string? fullName { get; set; }


    }
}
