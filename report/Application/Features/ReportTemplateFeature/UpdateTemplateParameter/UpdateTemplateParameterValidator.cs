using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Features.ReportTemplateFeature.UpdateTemplateParameter
{

    public sealed class UpdateTemplateParameterValidator : AbstractValidator<UpdateTemplateParameterRequest>
    {
        public UpdateTemplateParameterValidator()
        {
          //  RuleFor(x => x.Id).NotEmpty();
          //  RuleFor(x=> x.Descr).NotEmpty();

        }


    }
}
