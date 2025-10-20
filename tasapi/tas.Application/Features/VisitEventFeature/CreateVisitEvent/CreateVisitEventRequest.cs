using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.CreateVisitEvent
{
    public sealed record CreateVisitEventRequest(string Name, DateTime startDate, DateTime endDate, int HeadCount, int InScheduleId, int OutScheduleId, List<CreateVisitEventRequestPeoples> people) : IRequest;

    public sealed record CreateVisitEventRequestPeoples
    (
        string? first,
        string? last,
        int? Id
    );
}
