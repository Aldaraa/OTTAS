using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelRemove
{ 
    public sealed class CreateRequestDocumentSiteTravelRemoveHandler : IRequestHandler<CreateRequestDocumentSiteTravelRemoveRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestSiteTravelRemoveRepository _RequestSiteTravelRemoveRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateRequestDocumentSiteTravelRemoveHandler(IUnitOfWork unitOfWork, IRequestSiteTravelRemoveRepository requestSiteTravelRemoveRepository,  IMapper mapper, IEmployeeRepository employeeRepository, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestSiteTravelRemoveRepository = requestSiteTravelRemoveRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _RequestDocumentRepository = requestDocumentRepository;
        }


        public async Task<int> Handle(CreateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            await _employeeRepository.EmployeeActiveCheck(request.documentData.EmployeeId);
            var Id =  await _RequestSiteTravelRemoveRepository.CreateRequestDocumentSiteTravelRemove(request, cancellationToken);
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);
            await _RequestDocumentRepository.GenerateUpdateDocumentInfo(Id, cancellationToken);

            return Id; 

        }
    }
}
