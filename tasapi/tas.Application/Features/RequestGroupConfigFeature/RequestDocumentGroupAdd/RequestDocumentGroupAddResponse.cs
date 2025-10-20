using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupAdd
{
    
    public sealed record RequestDocumentGroupAddResponse
    {
    
        public int Id { get; set; }

        public string Document { get; set; }

        public int OrderIndex { get; set; }

    }
}
