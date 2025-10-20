using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ShiftFeature.DeleteShift
{
    public sealed record DeleteShiftRequest(int Id) : IRequest;
}
