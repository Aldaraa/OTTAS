using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.FlightGroupDetailFeature.SetClusterFlightGroupDetail
{ 
    public sealed class ReOrderFlightGroupDetailValidator : AbstractValidator<SetClusterFlightGroupDetailRequest>
    {
        public ReOrderFlightGroupDetailValidator()
        {
            //RuleForEach(request => request)
            //  .ChildRules(record => 
            //  {
            //      record.RuleFor(x => x.data.FlightGroupDetailId)
            //          .GreaterThan(0)
            //          .WithMessage("FlightGroupDetailId must be greater than 0.");

            //      record.RuleFor(x => x.ClusterId)
            //          .Cascade(CascadeMode.StopOnFirstFailure)
            //          .Must(clusterId => clusterId == null || clusterId > 0)
            //          .WithMessage("If ClusterId is specified, it must be greater than 0.");
            //  });
        }
    }
}
