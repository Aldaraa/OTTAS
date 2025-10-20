using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;

namespace tas.Application.Features.SysFilesFeature.UploadSysFiles
{

    public sealed class CreateRequestDocumentValidator : AbstractValidator<UploadSysFileRequest>
    {
        public CreateRequestDocumentValidator()
        {
            RuleFor(request => request.file)
                .NotNull() // Ensure the file is not null
                .WithMessage("File is required");

            RuleFor(request => request.file)
                .Must(file => file.Length > 0)
                .WithMessage("File must not be empty");
        }

    }
}
