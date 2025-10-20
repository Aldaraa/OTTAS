using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.GetDataRequest
{
 
    public sealed record GetDataRequestResponse
    {
       public  List<dynamic> Data { get; set; }

    }
}
