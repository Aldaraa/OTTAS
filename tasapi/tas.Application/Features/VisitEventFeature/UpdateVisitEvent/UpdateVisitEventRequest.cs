using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.UpdateVisitEvent
{
    public sealed record UpdateVisitEventRequest(int Id, string Name, int HeadCount, int InScheduleId, int OutScheduleId) : IRequest;

}
