using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.GetSessionList
{
    public sealed record GetSessionListResponse
    {
        public int KillId { get; set; }
        public int SessionId { get; set; }
        public string? SessionName { get; set; }

        public DateTime? CreatedDate { get; set; }


    }

}
