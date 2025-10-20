using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RosterDetailFeature.UpdateRosterDetail
{
    public sealed record UpdateRosterDetailRequest(int Id, int ShiftId, int DaysOn, int Rosterid, int Active) : IRequest;
}
