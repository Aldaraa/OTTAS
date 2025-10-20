using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterFeature.GetActiveTransportCluster
{
    public sealed record GetActiveTransportClusterResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }

        public string? DayNum { get; set; }

        public string? Direction { get; set; }


        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
