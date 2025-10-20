using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CancelRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CancelRequestDocument
{ 
    public sealed class CancelRequestDocumentHandler : IRequestHandler<CancelRequestDocumentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public CancelRequestDocumentHandler(IUnitOfWork unitOfWork, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CancelRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            await _RequestDocumentRepository.CancelRequestDocument(request, cancellationToken);

            await _unitOfWork.Save(cancellationToken);
            await _RequestDocumentRepository.GenerateUpdateDocumentInfo(request.Id, cancellationToken);
            return Unit.Value;

        }
    }
}
