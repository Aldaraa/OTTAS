using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentHistoryFeature.GetRequestDocumentHistory
{
    public sealed record GetRequestDocumentHistoryResponse
    {
            public int? Id { get; set; }
            public string? Comment { get; set; }

            public string? CurrentAction { get; set; }

            public int? ActionEmployeeId { get; set; }

            public string? ActionEmployeeFullName { get; set; }

            public string? AssignedGroupName { get; set; }


        public DateTime? CreateDate { get; set; }
       

    }

 




}
