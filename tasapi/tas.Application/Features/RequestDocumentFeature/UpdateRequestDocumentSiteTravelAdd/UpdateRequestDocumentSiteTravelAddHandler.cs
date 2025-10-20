using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelAdd
{ 
    public sealed class UpdateRequestDocumentSiteTravelAddHandler : IRequestHandler<UpdateRequestDocumentSiteTravelAddRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelAddRepository _RequestSiteTravelAddRepository;
        private readonly IMapper _mapper;
        private readonly IRequestDocumentRepository _requestDocumentRepository;

        public UpdateRequestDocumentSiteTravelAddHandler(IUnitOfWork unitOfWork, IRequestSiteTravelAddRepository requestSiteTravelAddRepository, IRequestDocumentRepository requestDocumentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelAddRepository = requestSiteTravelAddRepository;
            _requestDocumentRepository = requestDocumentRepository;
            _mapper = mapper;
        }


        public async Task<Unit> Handle(UpdateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var docId =  await _RequestSiteTravelAddRepository.UpdateRequestDocumentSiteTravelAdd(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            if (docId > 0) {
                await _requestDocumentRepository.GenerateDescription(docId, cancellationToken);
            }
            
            return Unit.Value;

        }
    }
}
