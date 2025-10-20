using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.EmployeeDateTransport
{

    public sealed record EmployeeDateTransportResponse
    {
        public int Id { get; set; }


        public string? ScheduleCode { get; set; }

        public int ScheduleId { get; set; }
            

    }
}
