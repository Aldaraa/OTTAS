using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ClusterFeature.UpdateCluster;

namespace tas.Application.Features.ClustereFeature.UpdateClustere
{
    public sealed class UpdateClusterValidator : AbstractValidator<UpdateClusterRequest>
    {
        public UpdateClusterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Description).NotEmpty();
        }

    }
}
