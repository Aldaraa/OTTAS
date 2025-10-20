using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestLineManagerEmployeeFeature.CreateRequestLineManagerEmployee 
{ 
    public sealed class CreateRequestLineManagerEmployeeValidator : AbstractValidator<CreateRequestLineManagerEmployeeRequest>
    {
        public CreateRequestLineManagerEmployeeValidator()
        {
            RuleFor(request => request.LineManagerEmployeeId).NotEmpty();
            RuleFor(request => request.EmployeeId).NotEmpty();


        }
    }
}
