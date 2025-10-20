using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetReportTemplateData
{
    public sealed record GetReportTemplateDataResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        public string? Description { get; set; }
        public int? Active { get; set; }

        public string? Summary { get; set; }
        public DateTime? DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public List<GetReportTemplateDataColumn> Columns { get; set; }

        public List<GetReportTemplateDataParameter> Parameters { get; set; }
    }

    public sealed record GetReportTemplateDataColumn
    { 
        public int Id { get; set; }

        public string FieldName { get; set; }

        public string Caption { get; set; }

    }


    public sealed record GetReportTemplateDataParameter
    {
        public int Id { get; set; }

        public string FielName { get; set; }

        public string? Descr { get; set; }

        public string Caption { get; set; }

        public string Component { get; set; }
    }
}
