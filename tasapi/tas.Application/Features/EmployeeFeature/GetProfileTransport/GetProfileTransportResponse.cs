using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.EmployeeFeature.GetProfileTransport
{
    public sealed record GetProfileTransportResponse
    {
        public DateTime? InEventDate { get; set; }

        public DateTime? InEventDateTime { get; set; }

        public string? InDirection { get; set; }

        public string? InDescription { get; set; }

        public string? InTransportMode { get; set; }

        public string? InTransportCode { get; set; }

        public DateTime? OutEventDate { get; set; }

        public DateTime? OutEventDateTime { get; set; }

        public string? OutDirection { get; set; }

        public string? OutDescription { get; set; }

        public string? OutTransportMode { get; set; }

        public string? OutTransportCode { get; set; }
    }

  

}
