using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ClusterDetailFeature.CreateClusterDetail;

namespace tas.Application.Features.ClusterDetailFeature.CreateClusterDetailDetail
{
    public sealed class CreateClusterDetailDetailValidator : AbstractValidator<CreateClusterDetailRequest>
    {
        public CreateClusterDetailDetailValidator()
        {
            RuleFor(x => x.ActiveTransportId)
                .GreaterThan(0)
                .WithMessage("The active transport ID must be greater than 0.");

            RuleFor(x => x.ClusterId)
                .GreaterThan(0)
                .WithMessage("The cluster ID must be greater than 0.");

            RuleFor(x => x.SeqNumber)
                .GreaterThan(0)
                .WithMessage("The sequence number must be greater than 0.");

        }


    }
}
