using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.RemoveRequestLineManagerEmployee 
{ 
    public sealed class RemoveRequestLineManagerEmployeeValidator : AbstractValidator<RemoveRequestLineManagerEmployeeRequest>
    {
        public RemoveRequestLineManagerEmployeeValidator()
        {
            RuleFor(request => request.Id).NotEmpty();

        }
    }
}
