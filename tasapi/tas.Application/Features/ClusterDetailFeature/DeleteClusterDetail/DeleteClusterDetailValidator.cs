using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.ClusterDetailFeature.DeleteClusterDetail
{
    public sealed class DeleteClusterDetailValidator : AbstractValidator<DeleteClusterDetailRequest>
    {
        public DeleteClusterDetailValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
