using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CheckDemobRequest;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd
{ 
    public sealed class CreateRequestDocumentSiteTravelHandler : IRequestHandler<CreateRequestDocumentSiteTravelAddRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelAddRepository _RequestSiteTravelAddRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;

        private readonly IMapper _mapper;
        private readonly ITransportCheckerRepository _transportCheckerRepository;

        public CreateRequestDocumentSiteTravelHandler(IUnitOfWork unitOfWork, IRequestSiteTravelAddRepository requestSiteTravelAddRepository, IRequestDocumentRepository requestDocumentRepository, IMapper mapper, ITransportCheckerRepository transportCheckerRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelAddRepository = requestSiteTravelAddRepository;
            _mapper = mapper;
            _transportCheckerRepository = transportCheckerRepository;   
            _RequestDocumentRepository = requestDocumentRepository;
        }


        public async Task<int> Handle(CreateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            await _RequestDocumentRepository.CheckDemobRequest(new CheckDemobRequestRequest(request.documentData.EmployeeId), cancellationToken);
            await _transportCheckerRepository.TransportAddValidDirectionSequenceCheck(request.documentData.EmployeeId, request.flightData.inScheduleId, request.flightData.outScheduleId);
            int Id =   await _RequestSiteTravelAddRepository.CreateRequestDocumentSiteTravelAdd(request, cancellationToken);
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);
            await _RequestDocumentRepository.GenerateUpdateDocumentInfo(Id, cancellationToken);
            return Id;
        }
    }
}
