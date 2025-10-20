using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.RemoveCancelRequestDocument;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.RemoveCancelRequestDocument
{ 
    public sealed class RemoveCancelRequestDocumentHandler : IRequestHandler<RemoveCancelRequestDocumentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public RemoveCancelRequestDocumentHandler(IUnitOfWork unitOfWork, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(RemoveCancelRequestDocumentRequest request, CancellationToken cancellationToken)
        {
             await _RequestDocumentRepository.RemoveCancelRequestDocument(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
