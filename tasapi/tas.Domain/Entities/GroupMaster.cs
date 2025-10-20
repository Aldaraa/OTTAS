using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class GroupMaster : BaseEntity
    {
        public string? Description { get; set; }


        public int? ShowOnProfile { get; set; } = 1;

        public int? OrderBy { get; set; }

        public int? CreateLog { get; set; }

        public int? Required { get; set; }



    }
}
