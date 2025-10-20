using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument
{ 
    public sealed class ApproveRequestDocumentHandler : IRequestHandler<ApproveRequestDocumentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public ApproveRequestDocumentHandler(IUnitOfWork unitOfWork, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(ApproveRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            // await _RequestDocumentRepository.DocumentEmployeeActiveCheck(request.Id);
             await _RequestDocumentRepository.ApproveRequestDocument(request, cancellationToken);
             await _unitOfWork.Save(cancellationToken);
             await _RequestDocumentRepository.GenerateUpdateDocumentInfo(request.Id, cancellationToken);
             return Unit.Value;

        }
    }
}
