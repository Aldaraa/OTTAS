using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChangeTemp
{ 
    public sealed class CreateRequestDocumentProfileChangeTempHandler : IRequestHandler<CreateRequestDocumentProfileChangeTempRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentProfileChangeEmployeeRepository _RequestDocumentProfileChangeTempEmployeeRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;
        public CreateRequestDocumentProfileChangeTempHandler(IUnitOfWork unitOfWork, IRequestDocumentProfileChangeEmployeeRepository requestDocumentProfileChangeTempEmployeeRepository, IMapper mapper, IEmployeeRepository employeeRepository, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestDocumentProfileChangeTempEmployeeRepository = requestDocumentProfileChangeTempEmployeeRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _RequestDocumentRepository = requestDocumentRepository;
        }

        public async Task<int> Handle(CreateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
            await _employeeRepository.EmployeeActiveCheck(request.Employee.EmployeeId);
            var Id =  await _RequestDocumentProfileChangeTempEmployeeRepository.CreateRequestDocumentProfileChangeTemp(request, cancellationToken);
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);
            await _RequestDocumentRepository.GenerateUpdateDocumentInfo(Id, cancellationToken);
            return Id;

        }
    }
}
