using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelAdd
{ 
    public sealed class CompleteRequestDocumentExternalTravelAddHandler : IRequestHandler<CompleteRequestDocumentExternalTravelAddRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelAddRepository _RequestExternalTravelAddRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;

        private readonly ITransportRepository _TransportRepository;
        private readonly IMapper _mapper;

        public CompleteRequestDocumentExternalTravelAddHandler(IUnitOfWork unitOfWork, IRequestExternalTravelAddRepository requestExternalTravelAddRepository, IMapper mapper, ITransportRepository transportRepository, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelAddRepository = requestExternalTravelAddRepository;
            _mapper = mapper;
            _TransportRepository = transportRepository;
            _RequestDocumentRepository = requestDocumentRepository;
        }


        public async Task<Unit> Handle(CompleteRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken)
        {
            await _RequestDocumentRepository.DocumentEmployeeActiveCompleteExternalAddTravelCheck(request.documentId);
           
            await _RequestExternalTravelAddRepository.CompleteRequestDocumentExternalTravelAdd(request, cancellationToken);
            return Unit.Value;



        }
    }
}
