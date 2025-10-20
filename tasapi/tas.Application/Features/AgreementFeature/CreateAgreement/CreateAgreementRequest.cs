using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AgreementFeature.CreateAgreement
{
    public sealed record CreateAgreementRequest(string AgreementText) : IRequest;
}
