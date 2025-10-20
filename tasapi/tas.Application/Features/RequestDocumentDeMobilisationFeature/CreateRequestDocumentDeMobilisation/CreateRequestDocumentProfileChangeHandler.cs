using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.CreateRequestDocumentDeMobilisation
{ 
    public sealed class CreateRequestDocumentDeMobilisationHandler : IRequestHandler<CreateRequestDocumentDeMobilisationRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDeMobilisationRepository _RequestDeMobilisationRepository;
        private readonly IMapper _mapper;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;

        public CreateRequestDocumentDeMobilisationHandler(IUnitOfWork unitOfWork, IRequestDeMobilisationRepository RequestDeMobilisationRepository, IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestDeMobilisationRepository = RequestDeMobilisationRepository;
            _mapper = mapper;
            _RequestDocumentRepository = requestDocumentRepository;
        }

        public async Task<int> Handle(CreateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
            int Id = await _RequestDeMobilisationRepository.CreateRequestDocumentDeMobilisation(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);
            await _RequestDocumentRepository.GenerateUpdateDocumentInfo(Id, cancellationToken);
            return Id;

        }
    }
}
