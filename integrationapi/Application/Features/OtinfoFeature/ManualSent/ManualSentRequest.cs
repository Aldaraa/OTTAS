using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OtinfoFeature.ManualSent
{
    public sealed record ManualSentRequest(int batchsize = 10000, bool testmode = false) :  IRequest;

}
