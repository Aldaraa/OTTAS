using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.AuthenticationFeature.AgreementCheck
{
    public sealed record AgreementCheckRequest(int check) : IRequest;
}
