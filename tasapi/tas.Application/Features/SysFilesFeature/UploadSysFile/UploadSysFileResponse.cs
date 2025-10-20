using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysFilesFeature.UploadSysFile
{
    public sealed record UploadSysFileResponse
    {
        public int Id { get; set; }
        public string? FileAddress { get; set; }

    }
}
