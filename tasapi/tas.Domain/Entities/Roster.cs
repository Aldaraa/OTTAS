using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class Roster : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int RosterGroupId { get; set; }
    }
}
