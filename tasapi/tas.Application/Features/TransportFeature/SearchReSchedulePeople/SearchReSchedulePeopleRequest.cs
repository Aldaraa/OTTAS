using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.TransportFeature.SearchReSchedulePeople
{
    public sealed record SearchReSchedulePeopleRequest(
        DateTime? fromDate,
        DateTime toDate, 
        int ScheduleId,
        string direction
        ) : IRequest<SearchReSchedulePeopleResponse>;
}
