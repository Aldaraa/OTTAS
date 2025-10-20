using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChangeTemp
{
    public sealed record GetRequestDocumentProfileChangeTempResponse
    {
        public int Id { get; set; }
        public int? EmployerId { get; set; }
        public int? CostCodeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Permanent { get; set; }

    }

}
