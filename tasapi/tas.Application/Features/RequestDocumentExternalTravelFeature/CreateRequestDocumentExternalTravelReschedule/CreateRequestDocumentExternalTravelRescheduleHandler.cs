using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelReschedule
{ 
    public sealed class CreateRequestDocumentExternalTravelRescheduleHandler : IRequestHandler<CreateRequestDocumentExternalTravelRescheduleRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelReScheduleRepository _RequestExternalTravelRescheduleRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;

        public CreateRequestDocumentExternalTravelRescheduleHandler(IUnitOfWork unitOfWork, IRequestExternalTravelReScheduleRepository requestExternalTravelRescheduleRepository, IRequestDocumentRepository requestDocumentRepository, IMapper mapper, IEmployeeRepository employeeRepository, ITransportCheckerRepository transportCheckerRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelRescheduleRepository = requestExternalTravelRescheduleRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _transportCheckerRepository = transportCheckerRepository;   
            _requestDocumentRepository = requestDocumentRepository;
        }


        public async Task<int> Handle(CreateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            await _employeeRepository.EmployeeActiveCheck(request.documentData.EmployeeId);
            await _transportCheckerRepository.TransportExternalRescheduleValidCheck(request.flightData.oldTransportId, request.flightData.ScheduleId);
            var Id =  await _RequestExternalTravelRescheduleRepository.CreateRequestDocumentExternalTravelReschedule(request, cancellationToken);
            await _requestDocumentRepository.GenerateDescription(Id, cancellationToken);
            return Id;

        }
    }
}
