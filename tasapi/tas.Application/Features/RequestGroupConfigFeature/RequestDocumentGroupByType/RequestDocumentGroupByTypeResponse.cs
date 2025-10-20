using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupByType
{
    
    public sealed record RequestDocumentGroupByTypeResponse
    {
        public int? PendingRequests { get; set; }

        public List<RequestDocumentGroupByTypeApprovalGroups> groups { get; set; }


    }

    public sealed record RequestDocumentGroupByTypeApprovalGroups
    {

        public int id { get; set; }
        public string? GroupName { get; set; }

        public int OrderIndex { get; set; }

        public string? GroupTag { get; set; }

        public int? GroupId { get; set; }

        public List<RequestDocumentGroupByTypeEmployees> groupMembers { get; set; }
    }

    public sealed record RequestDocumentGroupByTypeEmployees
    {
        public int id { get; set; }

        public int? employeeId { get; set; }

        public string? displayName { get; set; }

        public string? fullName { get; set; }


    }
}
