using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MailSmtpConfigFeature.GetMailSmtpConfig
{
    public sealed record GetMailSmtpConfigResponse
    {
        public int Id { get; set; }

        public string? smtpServer { get; set; }

        public int smtpPort { get; set; }
        public string? email { get; set; }

        public string? password { get; set; }

    }
}
