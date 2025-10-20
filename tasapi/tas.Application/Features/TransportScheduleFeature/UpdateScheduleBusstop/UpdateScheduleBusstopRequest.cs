using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateScheduleBusstop
{
    public sealed record UpdateScheduleBusstopRequest(
        int Id,
       List<UpdateScheduleBusstopRequestBussttops> Busstops
        ) : IRequest;


    public sealed record UpdateScheduleBusstopRequestBussttops
    {
        public string Description { get; set; }
        public string ETD { get; set; }

    }
}
