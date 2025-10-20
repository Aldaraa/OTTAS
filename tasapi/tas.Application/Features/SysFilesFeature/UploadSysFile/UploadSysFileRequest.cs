using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysFilesFeature.UploadSysFile;
using tas.Application.Features.SysRoleFeature.GetAllSysRole;

namespace tas.Application.Features.SysFilesFeature.UploadSysFiles
{

    public sealed record UploadSysFileRequest(IFormFile file) : IRequest<UploadSysFileResponse>;
}
