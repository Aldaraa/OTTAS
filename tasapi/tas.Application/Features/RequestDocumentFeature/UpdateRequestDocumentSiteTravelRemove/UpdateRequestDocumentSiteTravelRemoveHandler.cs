using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelRemove
{ 
    public sealed class UpdateRequestDocumentSiteTravelRemoveHandler : IRequestHandler<UpdateRequestDocumentSiteTravelRemoveRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRemoveRepository _RequestSiteTravelRemoveRepository;
        private  readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDocumentSiteTravelRemoveHandler(IUnitOfWork unitOfWork, IRequestSiteTravelRemoveRepository requestSiteTravelRemoveRepository, IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRemoveRepository = requestSiteTravelRemoveRepository;
            _mapper = mapper;
            _RequestDocumentRepository = requestDocumentRepository;
        }


        public async Task<Unit> Handle(UpdateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
           var docId = await _RequestSiteTravelRemoveRepository.UpdateRequestDocumentSiteTravelRemove(request, cancellationToken);
            await  _unitOfWork.Save(cancellationToken);
            if (docId > 0) {
              await  _RequestDocumentRepository.GenerateDescription(docId, cancellationToken);
            }
            return Unit.Value;

        }
    }
}
