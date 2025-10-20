using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule
{ 
    public sealed class CreateRequestDocumentSiteTravelRescheduleHandler : IRequestHandler<CreateRequestDocumentSiteTravelRescheduleRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRescheduleRepository _RequestSiteTravelRescheduleRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;

        public CreateRequestDocumentSiteTravelRescheduleHandler(IUnitOfWork unitOfWork, IRequestSiteTravelRescheduleRepository requestSiteTravelRescheduleRepository, IRequestDocumentRepository requestDocumentRepository, IMapper mapper, IEmployeeRepository employeeRepository, ITransportCheckerRepository transportCheckerRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRescheduleRepository = requestSiteTravelRescheduleRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _transportCheckerRepository = transportCheckerRepository;   
            _requestDocumentRepository = requestDocumentRepository;
        }


        public async Task<int> Handle(CreateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {

            await _transportCheckerRepository.TransportUpdateValidDirectionSequenceCheckForRequest(request.flightData.existingScheduleId, request.flightData.reScheduleId, request.documentData.EmployeeId);


            await _employeeRepository.EmployeeActiveCheck(request.documentData.EmployeeId);
          var Id =  await _RequestSiteTravelRescheduleRepository.CreateRequestDocumentSiteTravelReschedule(request, cancellationToken);
            await _requestDocumentRepository.GenerateDescription(Id, cancellationToken);
            await _requestDocumentRepository.GenerateUpdateDocumentInfo(Id, cancellationToken);
            return Id;

        }
    }
}
