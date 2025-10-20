using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentMyInfo
{
    public sealed record GetRequestDocumentMyInfoResponse
    {
        public int EmployeeId { get; set; }
        public string Fullname { get; set; }

        public string? ApprovalGroupName { get; set; }
        public int? ApprovalGroupId { get; set; }

        public int? RoleId { get; set; }

        public string? Rolename { get; set; }

        public List<int> ApprovalGroupIds { get; set; }


        public List<int> LineManagerIds { get; set; }

        public List<int> GroupIds { get; set; }


    }


}
