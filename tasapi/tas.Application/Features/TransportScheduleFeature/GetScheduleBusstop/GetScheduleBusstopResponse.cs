using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportScheduleFeature.GetScheduleBusstop
{

    public sealed record GetScheduleBusstopResponse
    {
        public int Id { get; set; }

        public string? Description { get; set; }


        public string? ETD { get; set; }



    }
}
