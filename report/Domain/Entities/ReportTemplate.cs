using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class  ReportTemplate : BaseEntity
    {
        public string? Code { get; set; }

        public string? Description { get; set; }

        public string? TemplateFile { get; set; }

        public string? Summary { get; set; }
    }
}
