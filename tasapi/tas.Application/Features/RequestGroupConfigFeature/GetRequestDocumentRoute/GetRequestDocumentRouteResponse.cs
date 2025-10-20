using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentRoute
{
    
    public sealed record RequestDocumentRouteResponse
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string? GroupName { get; set; }

        public int CurrentPosition { get; set; }

        public int OrderIndex { get; set; }

        public string? GroupTag { get; set; }


    }
}
