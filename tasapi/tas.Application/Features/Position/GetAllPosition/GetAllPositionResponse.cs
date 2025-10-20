using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Domain.Common;

namespace tas.Application.Features.PositionFeature.GetAllPosition
{
    public sealed record GetAllPositionResponse : BasePaginationResponse<GetAllPositionResult>
    {

    }


    public sealed record GetAllPositionResult
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }

        public int? EmployeeCount { get; set; }
        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
