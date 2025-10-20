using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChange
{ 
    public sealed class CompleteRequestDocumentProfileChangeHandler : IRequestHandler<CompleteRequestDocumentProfileChangeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentProfileChangeEmployeeRepository _RequestDocumentProfileChangeEmployeeRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        private readonly IMapper _mapper;

        public CompleteRequestDocumentProfileChangeHandler(IUnitOfWork unitOfWork, IRequestDocumentProfileChangeEmployeeRepository requestDocumentProfileChangeEmployeeRepository, IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentProfileChangeEmployeeRepository = requestDocumentProfileChangeEmployeeRepository;
            _mapper = mapper;
            _requestDocumentRepository = requestDocumentRepository;
        }

        public async Task<Unit> Handle(CompleteRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {
          //    await _requestDocumentRepository.DocumentEmployeeActiveCheck(request.documentId);
              await _RequestDocumentProfileChangeEmployeeRepository.CompleteRequestDocumentProfileChange(request, cancellationToken);
              await _unitOfWork.Save(cancellationToken);
              return Unit.Value;

        }
    }
}
