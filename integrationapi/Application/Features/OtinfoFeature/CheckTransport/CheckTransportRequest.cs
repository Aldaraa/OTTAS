using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OtinfoFeature.CheckTransport
{
    public sealed record CheckTransportRequest(string SAPID) :  IRequest<string>;

}
