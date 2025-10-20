using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.EmployeeTransportNoShow
{

    public sealed record EmployeeTransportNoShowResponse
    {
        public int Id { get; set; }

        public DateTime? EventDate { get; set; }
        
        public DateTime? EventDateTime { get; set; }

        public string? Direction { get; set; }

        public string? Description { get; set; }



        public string? Reason { get; set; }




            

    }
}
