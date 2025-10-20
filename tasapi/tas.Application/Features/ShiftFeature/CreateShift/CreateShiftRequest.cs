using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ShiftFeature.CreateShift
{
    public sealed record CreateShiftRequest(string Code, string Description, int OnSite, int isDefault, int Active, int? ColorId, int TransportGroup) : IRequest;
}
