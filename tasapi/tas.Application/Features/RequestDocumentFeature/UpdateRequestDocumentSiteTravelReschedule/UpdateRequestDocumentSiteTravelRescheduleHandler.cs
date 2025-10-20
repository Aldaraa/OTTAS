using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelReschedule
{ 
    public sealed class UpdateRequestDocumentSiteTravelRescheduleHandler : IRequestHandler<UpdateRequestDocumentSiteTravelRescheduleRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRescheduleRepository _RequestSiteTravelRescheduleRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        private readonly IMapper _mapper;

        public UpdateRequestDocumentSiteTravelRescheduleHandler(IUnitOfWork unitOfWork, IRequestSiteTravelRescheduleRepository requestSiteTravelRescheduleRepository,  IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRescheduleRepository = requestSiteTravelRescheduleRepository;
            _mapper = mapper;
            _requestDocumentRepository = requestDocumentRepository;
        }


        public async Task<Unit> Handle(UpdateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var docId =  await _RequestSiteTravelRescheduleRepository.UpdateRequestDocumentSiteTravelReschedule(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            if (docId > 0) {
                await _requestDocumentRepository.GenerateDescription(docId, cancellationToken);
            }
            
            return Unit.Value;

        }
    }
}
