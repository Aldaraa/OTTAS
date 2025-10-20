using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.VisitEventFeature.ReplaceProfile
{
    public sealed record ReplaceProfileRequest(int oldEmployeeId, int newEmployeeId,  int EventId) : IRequest;

}
