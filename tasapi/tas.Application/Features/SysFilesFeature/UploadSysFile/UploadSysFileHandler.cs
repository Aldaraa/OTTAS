using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysFilesFeature.UploadSysFile;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.SysFilesFeature.UploadSysFiles
{

    public sealed class UploadSysFileHandler : IRequestHandler<UploadSysFileRequest, UploadSysFileResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysFileRepository _SysFileRepository;
        private readonly IMapper _mapper;

        public UploadSysFileHandler(IUnitOfWork unitOfWork, ISysFileRepository SysFileRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysFileRepository = SysFileRepository;
            _mapper = mapper;
        }

        public async Task<UploadSysFileResponse> Handle(UploadSysFileRequest request, CancellationToken cancellationToken)
        {
           var data =  await _SysFileRepository.UploadSysFile(request, cancellationToken);
            return data;
        }
    }
}
