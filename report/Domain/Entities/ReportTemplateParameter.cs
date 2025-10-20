using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class ReportTemplateParameter : BaseEntity
    {
        public string? FieldName { get; set; }

        public string? Caption  { get; set; }

        public string? Descr  { get; set; }



        public string? Component { get; set; }

        public int ReportTemplateId { get; set; }
    }
}
