using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterFeature.CreateCluster
{
    public sealed class CreateClusterDetailValidator : AbstractValidator<CreateClusterRequest>
    {
        public CreateClusterDetailValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Description).NotEmpty();

        }


    }
}
