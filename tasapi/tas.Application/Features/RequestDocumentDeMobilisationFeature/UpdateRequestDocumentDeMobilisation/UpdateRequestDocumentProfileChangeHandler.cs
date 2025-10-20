using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentDeMobilisationFeature.UpdateRequestDocumentDeMobilisation
{ 
    public sealed class UpdateRequestDocumentDeMobilisationHandler : IRequestHandler<UpdateRequestDocumentDeMobilisationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDeMobilisationRepository _RequestDeMobilisationRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDocumentDeMobilisationHandler(IUnitOfWork unitOfWork, IRequestDeMobilisationRepository RequestDeMobilisationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDeMobilisationRepository = RequestDeMobilisationRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
            await _RequestDeMobilisationRepository.UpdateRequestDocumentDeMobilisation(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
