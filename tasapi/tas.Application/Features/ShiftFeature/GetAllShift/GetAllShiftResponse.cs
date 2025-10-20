using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ShiftFeature.GetAllShift
{
    public sealed record GetAllShiftResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }

        public string? ColorCode { get; set; }

        public int? ColorId { get; set; }
    
        public int isDefault { get; set; }

        public int OnSite { get; set; }

        public int Active { get; set; }

        public int? TransportGroup { get; set; }
        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
