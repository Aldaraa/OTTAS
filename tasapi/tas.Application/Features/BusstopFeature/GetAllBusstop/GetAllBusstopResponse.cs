using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BusstopFeature.GetAllBusstop
{
    public sealed record GetAllBusstopResponse
    {
        public int Id { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
