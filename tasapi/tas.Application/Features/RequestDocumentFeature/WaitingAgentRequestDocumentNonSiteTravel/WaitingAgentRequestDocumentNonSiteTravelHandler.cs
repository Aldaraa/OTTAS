using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.WaitingAgentRequestDocumentNonSiteTravel;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.WaitingAgentRequestDocumentNonSiteTravel
{ 
    public sealed class WaitingAgentRequestDocumentNonSiteTravelHandler : IRequestHandler<WaitingAgentRequestDocumentNonSiteTravelRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentNonSiteTravelRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;
        private readonly IRequestDocumentRepository _RequestDocRepository;

        public WaitingAgentRequestDocumentNonSiteTravelHandler(IUnitOfWork unitOfWork, IRequestDocumentNonSiteTravelRepository requestDocumentRepository, IMapper mapper, IRequestDocumentRepository requestDocRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
            _RequestDocRepository = requestDocRepository;
        }

        public async Task<Unit> Handle(WaitingAgentRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
            await _RequestDocumentRepository.WaitingAgentRequestDocumentNonSiteTravel(request, cancellationToken);
             await _unitOfWork.Save(cancellationToken);
            await _RequestDocRepository.GenerateUpdateDocumentInfo(request.documentId, cancellationToken);
            return Unit.Value;

        }
    }
}
