using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Domain.Entities
{
    public class ProfileField : BaseEntity
    {
        public string ColumnName { get; set; }
        public string Label { get; set; }
        public int? FieldRequired { get; set; }
        public int? FieldVisible { get; set; }
        public int? FieldReadOnly { get; set; }
        public int? RequestRequired { get; set; }
        public int? RequestVisible { get; set; }
    }
}
