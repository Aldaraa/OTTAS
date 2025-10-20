using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class Cluster : BaseEntity
    {
        public string? Code { get; set; }

        public string? Description { get; set; }

        public string? DayNum { get; set; }

        public string? Direction { get; set; }


    }
}
