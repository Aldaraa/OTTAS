using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel
{ 
    public sealed class CreateRequestDocumentNonSiteTravelHandler : IRequestHandler<CreateRequestDocumentNonSiteTravelRequest, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestDocumentNonSiteTravelRepository _RequestNonSiteDocumentRepository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRequestDocumentRepository _RequestDocumentRepository;

        public CreateRequestDocumentNonSiteTravelHandler(IUnitOfWork unitOfWork, IRequestDocumentNonSiteTravelRepository requestNonSiteDocumentRepository, IMapper mapper, IEmployeeRepository employeeRepository, IRequestDocumentRepository requestDocumentRepository)
        {
            _unitOfWork = unitOfWork;
            _RequestNonSiteDocumentRepository = requestNonSiteDocumentRepository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;   
            _RequestDocumentRepository = requestDocumentRepository;
        }

        public async Task<int> Handle(CreateRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
         //   await _employeeRepository.EmployeeActiveCheck(request.travelData.EmployeeId);
            _RequestNonSiteDocumentRepository.CreateRequestDocumentNonSiteTravelValidate(request);
            var Id =  await _RequestNonSiteDocumentRepository.CreateRequestDocumentNonSiteTravel(request, cancellationToken);
            await _RequestDocumentRepository.GenerateDescription(Id, cancellationToken);
            await _RequestDocumentRepository.GenerateUpdateDocumentInfo(Id, cancellationToken);


            return Id;

        }
    }
}
