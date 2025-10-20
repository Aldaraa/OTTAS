using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.BedFeature.GetAllBed
{
    public sealed record GetAllBedResponse
    {
        public int Id { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }

        public int RoomId { get; set; }

        public string RoomNumber { get; set; }

        public string CampName { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
