using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterFeature.DeleteCluster
{
    public sealed class DeleteClusterValidator : AbstractValidator<DeleteClusterRequest>
    {
        public DeleteClusterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
