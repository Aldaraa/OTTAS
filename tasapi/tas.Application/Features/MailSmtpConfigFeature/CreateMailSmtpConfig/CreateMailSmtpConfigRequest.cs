using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MailSmtpConfigFeature.CreateMailSmtpConfig
{
    public sealed record CreateMailSmtpConfigRequest(string smtpServer, int smtpPort, string? email, string? password) : IRequest;
}
