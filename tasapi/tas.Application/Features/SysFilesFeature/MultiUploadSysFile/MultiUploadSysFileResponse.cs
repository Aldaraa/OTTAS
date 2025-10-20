using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.SysFilesFeature.MultiUploadSysFile
{
    public sealed record MultiUploadSysFileResponse
    {
        public int Id { get; set; }
        public string? FileAddress { get; set; }
    }
    
}
