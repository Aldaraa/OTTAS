using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChange
{ 
    public sealed class CreateRequestDocumentProfileChangeHandler : IRequestHandler<CreateRequestDocumentProfileChangeRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentProfileChangeEmployeeRepository _RequestDocumentProfileChangeEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;

        public CreateRequestDocumentProfileChangeHandler(IUnitOfWork unitOfWork, IRequestDocumentProfileChangeEmployeeRepository requestDocumentProfileChangeEmployeeRepository, IMapper mapper, IEmployeeRepository employeeRepository, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentProfileChangeEmployeeRepository = requestDocumentProfileChangeEmployeeRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _RequestDocumentRepository = requestDocumentRepository;
        }

        public async Task<int> Handle(CreateRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {
        //    await _employeeRepository.EmployeeActiveCheck(request.Employee.EmployeeId);
            var Id =   await _RequestDocumentProfileChangeEmployeeRepository.CreateRequestDocumentProfileChange(request, cancellationToken);
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);
            await _RequestDocumentRepository.GenerateUpdateDocumentInfo(Id, cancellationToken);
            return Id;

        }
    }
}
