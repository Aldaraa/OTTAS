using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentMyInfo;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.GetRequestDocumentMyInfo
{ 
    public sealed class GetRequestDocumentMyInfoHandler : IRequestHandler<GetRequestDocumentMyInfoRequest, GetRequestDocumentMyInfoResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public GetRequestDocumentMyInfoHandler(IUnitOfWork unitOfWork, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<GetRequestDocumentMyInfoResponse> Handle(GetRequestDocumentMyInfoRequest request, CancellationToken cancellationToken)
        {
            return await _RequestDocumentRepository.GetRequestDocumentMyInfo(request, cancellationToken);

              

        }
    }
}
