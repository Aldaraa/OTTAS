using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.UpdateTemplateParameter
{
    public sealed record UpdateTemplateParameterRequest(List<UpdateTemplateParameterData> data
        ) : IRequest;

    public sealed record UpdateTemplateParameterData
    {
       public int Id { get; set;}
       public string Descr { get; set; }
    }
}
