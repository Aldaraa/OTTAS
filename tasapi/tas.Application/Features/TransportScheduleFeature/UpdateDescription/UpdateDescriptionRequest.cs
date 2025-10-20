using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.UpdateDescription
{
    public sealed record UpdateDescriptionRequest(
        int Id,
       string description
            
        ) : IRequest;
}
