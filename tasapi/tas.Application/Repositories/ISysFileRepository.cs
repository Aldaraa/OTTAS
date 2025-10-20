using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysFilesFeature.MultiUploadSysFile;
using tas.Application.Features.SysFilesFeature.UploadSysFile;
using tas.Application.Features.SysFilesFeature.UploadSysFiles;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ISysFileRepository : IBaseRepository<SysFile>
    {
        Task<UploadSysFileResponse> UploadSysFile(UploadSysFileRequest request, CancellationToken cancellationToken);


        Task<List<MultiUploadSysFileResponse>> MultiUploadSysFile(MultiUploadSysFileRequest request, CancellationToken cancellationToken);

    }
}
