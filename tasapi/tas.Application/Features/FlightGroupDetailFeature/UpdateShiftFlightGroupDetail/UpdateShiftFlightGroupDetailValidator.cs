using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupDetailFeature.UpdateShiftFlightGroupDetail
{ 
    public sealed class ReOrderFlightGroupDetailValidator : AbstractValidator<UpdateShiftFlightGroupDetailRequest>
    {
        public ReOrderFlightGroupDetailValidator()
        {
            
        }
    }
}
