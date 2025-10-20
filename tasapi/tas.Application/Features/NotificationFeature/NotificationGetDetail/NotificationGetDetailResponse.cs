using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.NotificationFeature.NotificationGetDetail
{
    public sealed record NotificationGetDetailResponse
    {
        public string? link { get; set; }
    }
}
