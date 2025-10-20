using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.TransportBookingInfo
{

    public sealed record TransportBookingInfoResponse
    {
        public int TransportId { get; set; }
        public string? Fullname { get; set; }

        public int? EmployeeId { get; set; }



        public string? Direction { get; set; }

        public string? EventDate { get; set; }

        public string? Description { get; set; }


        public DateTime? CreatedDate { get; set; }

        public string? TransportCode { get; set; }




    }
}
