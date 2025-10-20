using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChangeTemp
{ 
    public sealed class CompleteRequestDocumentProfileChangeTempHandler : IRequestHandler<CompleteRequestDocumentProfileChangeTempRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentProfileChangeEmployeeRepository _RequestDocumentProfileChangeTempEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IRequestDocumentRepository _requestDocumentRepository;

        public CompleteRequestDocumentProfileChangeTempHandler(IUnitOfWork unitOfWork, IRequestDocumentProfileChangeEmployeeRepository requestDocumentProfileChangeTempEmployeeRepository, IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentProfileChangeTempEmployeeRepository = requestDocumentProfileChangeTempEmployeeRepository;
            _mapper = mapper;
            _requestDocumentRepository = requestDocumentRepository;
        }

        public async Task<Unit> Handle(CompleteRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {

            await _requestDocumentRepository.DocumentEmployeeActiveCheck(request.documentId);
            await _RequestDocumentProfileChangeTempEmployeeRepository.CompleteRequestDocumentProfileChangeTemp(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
