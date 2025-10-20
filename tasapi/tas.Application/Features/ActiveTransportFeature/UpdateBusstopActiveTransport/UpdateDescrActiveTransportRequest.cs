using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ActiveTransportFeature.UpdateBusstopActiveTransport
{
    public sealed record UpdateBusstopActiveTransportRequest(
        int Id,
        DateTime startDate, DateTime endDate,
        List<UpdateBusstopActiveTransportRequestBussttops> Busstops
            
        ) : IRequest;

    public sealed record UpdateBusstopActiveTransportRequestBussttops
    { 
        public string Description { get; set; }
        public string ETD { get; set; }

    }
}
