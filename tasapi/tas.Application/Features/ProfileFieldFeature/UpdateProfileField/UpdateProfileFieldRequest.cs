using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ProfileFieldFeature.UpdateProfileField
{
    public sealed record UpdateProfileFieldRequest(int Id, string  Label, int? FieldRequired, int? FielReadOnly, int? RequestRequired, int? RequestVisible, int? FieldVisible, int? FieldReadOnly) : IRequest;
}
