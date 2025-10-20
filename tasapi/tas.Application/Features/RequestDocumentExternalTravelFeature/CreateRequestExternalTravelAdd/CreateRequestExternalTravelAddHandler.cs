using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestExternalTravelAdd
{ 
    public sealed class CreateRequestDocumentSiteTravelHandler : IRequestHandler<CreateRequestExternalTravelAddRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelAddRepository _RequestExternalTravelAddRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;

        private readonly IMapper _mapper;
        private readonly ITransportCheckerRepository _transportCheckerRepository;

        public CreateRequestDocumentSiteTravelHandler(IUnitOfWork unitOfWork, IRequestExternalTravelAddRepository requestExternalTravelAddRepository, IRequestDocumentRepository requestDocumentRepository, IMapper mapper, ITransportCheckerRepository transportCheckerRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelAddRepository = requestExternalTravelAddRepository;
            _mapper = mapper;
            _transportCheckerRepository = transportCheckerRepository;   
            _RequestDocumentRepository = requestDocumentRepository;
        }


        public async Task<int> Handle(CreateRequestExternalTravelAddRequest request, CancellationToken cancellationToken)
        {
            await _transportCheckerRepository.TransportExternalAddValidCheck(request.documentData.EmployeeId, request.flightData.FirstScheduleId, request.flightData.LastScheduleId);
            int Id =   await _RequestExternalTravelAddRepository.CreateRequestExternalTravelAdd(request, cancellationToken);
            
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);
            return Id;
        }
    }
}
