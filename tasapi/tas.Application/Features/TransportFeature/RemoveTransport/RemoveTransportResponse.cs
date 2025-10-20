using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.RemoveTransport
{
    public class RemoveTransportResponse
    {
        public int? StartScheduleId { get; set; }

        public int? EndScheduleId { get; set; }
    }
}
