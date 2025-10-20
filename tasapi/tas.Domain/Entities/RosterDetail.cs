using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class RosterDetail : BaseEntity
    {

        public int? ShiftId { get; set; }

        public int? SeqNumber { get; set; }

        public int? DaysOn { get; set; }

        public int? RosterId { get; set; }


    }
}
