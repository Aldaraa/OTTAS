using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ProfileFieldFeature.GetAllProfileField
{
    public sealed record GetAllProfileFieldResponse
    {
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public string Label { get; set; }
        public int? FieldRequired { get; set; }
        public int? FieldVisible { get; set; }
        public int? FieldReadOnly { get; set; }
        public int? RequestRequired { get; set; }
        public int? RequestVisible { get; set; }
        public int Active { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
