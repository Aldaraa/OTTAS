using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public sealed class Camp : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        public string Description { get; set; }


    }
}
