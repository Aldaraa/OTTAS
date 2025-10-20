using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelRemove
{ 
    public sealed class CreateRequestDocumentExternalTravelRemoveHandler : IRequestHandler<CreateRequestDocumentExternalTravelRemoveRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestExternalTravelRemoveRepository _RequestExternalTravelRemoveRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateRequestDocumentExternalTravelRemoveHandler(IUnitOfWork unitOfWork, IRequestExternalTravelRemoveRepository requestExternalTravelRemoveRepository,  IMapper mapper, IEmployeeRepository employeeRepository, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestExternalTravelRemoveRepository = requestExternalTravelRemoveRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _RequestDocumentRepository = requestDocumentRepository;
        }


        public async Task<int> Handle(CreateRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            await _employeeRepository.EmployeeActiveCheck(request.documentData.EmployeeId);
            var Id =  await _RequestExternalTravelRemoveRepository.CreateRequestDocumentExternalTravelRemove(request, cancellationToken);
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);

            return Id; 

        }
    }
}
