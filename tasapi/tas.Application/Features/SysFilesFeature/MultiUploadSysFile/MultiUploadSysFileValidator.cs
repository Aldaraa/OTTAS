using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysFilesFeature.MultiUploadSysFile;

namespace tas.Application.Features.SysFilesFeature.MultiMultiUploadSysFile
{

    public sealed class MultiMultiUploadSysFileValidator : AbstractValidator<MultiUploadSysFileRequest>
    {
        public MultiMultiUploadSysFileValidator()
        {
            RuleFor(request => request.files)
                   .NotNull()
                   .WithMessage("Files list cannot be null.")
                   .Must(files => files.Count > 0)
                   .WithMessage("Files list cannot be empty.")
                   .ForEach(file =>
                   {
                       file.NotNull().WithMessage("File cannot be null.");
                       // You can add more specific file validations here
                   });
        }

    }
}
