using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CheckDemobRequest;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CheckDemobRequest
{ 
    public sealed class CheckDemobRequestHandler : IRequestHandler<CheckDemobRequestRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public CheckDemobRequestHandler(IUnitOfWork unitOfWork, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CheckDemobRequestRequest request, CancellationToken cancellationToken)
        {
            await _RequestDocumentRepository.CheckDemobRequest(request, cancellationToken);
            return Unit.Value;

        }
    }
}
