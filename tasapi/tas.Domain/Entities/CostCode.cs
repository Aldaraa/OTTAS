using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;
using tas.Domain.CustomModel;

namespace tas.Domain.Entities
{
    [Audit]
    public sealed class CostCode : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        public string Number { get; set; }

        public string Description { get; set; }


    }
}
