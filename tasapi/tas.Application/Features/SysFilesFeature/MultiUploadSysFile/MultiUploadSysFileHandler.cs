using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysFilesFeature.MultiUploadSysFile;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.SysFilesFeature.MultiUploadSysFiles
{

    public sealed class MultiUploadSysFileHandler : IRequestHandler<MultiUploadSysFileRequest, List<MultiUploadSysFileResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISysFileRepository _SysFileRepository;
        private readonly IMapper _mapper;

        public MultiUploadSysFileHandler(IUnitOfWork unitOfWork, ISysFileRepository SysFileRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _SysFileRepository = SysFileRepository;
            _mapper = mapper;
        }

        public async Task<List<MultiUploadSysFileResponse>> Handle(MultiUploadSysFileRequest request, CancellationToken cancellationToken)
        {
           var data =  await _SysFileRepository.MultiUploadSysFile(request, cancellationToken);
            return data;
        }
    }
}
