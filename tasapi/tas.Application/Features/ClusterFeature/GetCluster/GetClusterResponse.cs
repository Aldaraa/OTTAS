using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterFeature.GetCluster
{
    public sealed record GetClusterResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public int Active { get; set; }

        public string? DayNum { get; set; }

        public string? Direction { get; set; }


        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public List<GetClusterResponseDetail> data { get; set; }
    }


    public sealed record GetClusterResponseDetail
    { 
        public int Id { get; set; }

        public int SeqNumber    { get; set; }

        public int ActiveTransportId { get; set; }

        public string? ActiveTransportDescription { get; set; }


        public string? ActiveTransportCode { get; set; }

    }


}
