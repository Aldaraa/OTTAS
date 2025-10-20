using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{

    public class DepartmentCostCode : BaseEntity
    {
        public int DepartmentId { get; set; }

        public int CostCodeId { get; set; }



    }
}
