using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;
using tas.Domain.CustomModel;

namespace tas.Domain.Entities
{
    [Audit]
    public sealed class Employer : BaseEntity
    {

        public string Code { get; set; }

        public string? Description { get; set; }


    }
}
