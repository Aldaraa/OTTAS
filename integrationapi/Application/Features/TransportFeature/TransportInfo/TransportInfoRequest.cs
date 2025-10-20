using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TransportFeature.TransportInfo
{
    public sealed record TransportInfoRequest(int batchsize = 10000) :  IRequest<List<TransportInfoResponse>>;

}
