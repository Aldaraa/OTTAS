using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.CompleteRequestDocumentDeMobilisation
{ 
    public sealed class CompleteRequestDocumentDeMobilisationHandler : IRequestHandler<CompleteRequestDocumentDeMobilisationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDeMobilisationRepository _RequestDeMobilisationRepository;
        private readonly IMapper _mapper;

        public CompleteRequestDocumentDeMobilisationHandler(IUnitOfWork unitOfWork, IRequestDeMobilisationRepository RequestDeMobilisationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDeMobilisationRepository = RequestDeMobilisationRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CompleteRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
              await _RequestDeMobilisationRepository.CompleteRequestDocumentDeMobilisation(request, cancellationToken);
              await _unitOfWork.Save(cancellationToken);
              return Unit.Value;

        }
    }
}
