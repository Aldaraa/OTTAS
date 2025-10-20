using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class ReportJobParameter : BaseEntity
    {
        public int ParameterId { get; set; }

        public int ReportJobId { get; set; }

        public string? ParameterValue { get; set; }
        
        public int? Days { get; set; }



    }
}

