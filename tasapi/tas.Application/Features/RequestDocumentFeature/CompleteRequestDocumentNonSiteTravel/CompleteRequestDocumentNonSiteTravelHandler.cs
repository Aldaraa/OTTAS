using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentNonSiteTravel;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentNonSiteTravel
{ 
    public sealed class CompleteRequestDocumentNonSiteTravelHandler : IRequestHandler<CompleteRequestDocumentNonSiteTravelRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentNonSiteTravelRepository _requestDocumentNonSiteTravelRepository;
        private readonly IMapper _mapper;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        public CompleteRequestDocumentNonSiteTravelHandler(IUnitOfWork unitOfWork, IRequestDocumentNonSiteTravelRepository requestDocumentNonSiteTravelRepository, IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _requestDocumentNonSiteTravelRepository = requestDocumentNonSiteTravelRepository;
            _mapper = mapper;
            _requestDocumentRepository = requestDocumentRepository;
        }

        public async Task<Unit> Handle(CompleteRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
        //    await _requestDocumentRepository.DocumentEmployeeActiveCheck(request.documentId);
            await _requestDocumentNonSiteTravelRepository.CompleteRequestDocumentNonSiteTravel(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _requestDocumentRepository.GenerateUpdateDocumentInfo(request.documentId, cancellationToken);
            return Unit.Value;

        }
    }
}
