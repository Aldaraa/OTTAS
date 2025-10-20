using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ShiftFeature.UpdateShift
{
    public sealed record UpdateShiftRequest(int Id, string? Code, string? Description, int OnSite, int isDefault, int? ColorId, int? TransportGroup) : IRequest;
}
