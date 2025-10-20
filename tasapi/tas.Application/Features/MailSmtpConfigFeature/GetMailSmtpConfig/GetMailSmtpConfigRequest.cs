using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.MailSmtpConfigFeature.GetMailSmtpConfig
{
    public sealed record GetMailSmtpConfigRequest : IRequest<GetMailSmtpConfigResponse>;
}
