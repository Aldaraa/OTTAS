using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfileUndo
{
    public sealed record ReplaceProfileUndoRequest(int EmployeeId,  int EventId) : IRequest;

}
