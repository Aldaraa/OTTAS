using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupEmpLines
{
    
    public sealed record RequestDocumentGroupEmpLinesResponse
    {
        public int? id { get; set; }

        public string? fullName { get; set; }


    }

 
}
