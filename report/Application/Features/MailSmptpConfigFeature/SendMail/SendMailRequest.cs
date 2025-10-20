using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.GetAllReportTemplate
{
    public sealed record SendMailRequest(EmailModel mailModel) : IRequest<int>;

    public class EmailModel
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Cc { get; set; } // List of email addresses for CC
        public List<string> Bcc { get; set; } // List of email addresses for BCC
        public List<string> Attachments { get; set; } // List of file paths for attachments
    }
}
