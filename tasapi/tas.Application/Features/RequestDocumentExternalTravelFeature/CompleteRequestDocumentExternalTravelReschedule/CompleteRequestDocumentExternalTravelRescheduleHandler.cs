using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelReschedule
{ 
    public sealed class CompleteRequestDocumentExternalTravelRescheduleHandler : IRequestHandler<CompleteRequestDocumentExternalTravelRescheduleRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelReScheduleRepository _RequestExternalTravelRescheduleRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        private readonly IMapper _mapper;


        public CompleteRequestDocumentExternalTravelRescheduleHandler(IUnitOfWork unitOfWork, IRequestExternalTravelReScheduleRepository requestExternalTravelRescheduleRepository, IMapper mapper, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelRescheduleRepository = requestExternalTravelRescheduleRepository;
            _mapper = mapper;
            _requestDocumentRepository = requestDocumentRepository;
        }


        public async Task<Unit> Handle(CompleteRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            await _requestDocumentRepository.DocumentEmployeeActiveCheck(request.documentId);
            await _RequestExternalTravelRescheduleRepository.CompleteRequestDocumentExternalTravelReschedule(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _requestDocumentRepository.GenerateDescription(request.documentId, cancellationToken);

            return Unit.Value;

        }
    }
}
