using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.CreateUser;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployeeFeature.ChangeEmployeeLocation
{
    public sealed class ChangeEmployeeLocationHandler : IRequestHandler<ChangeEmployeeLocationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeRepository _EmployeeRepository;
        private readonly IMapper _mapper;

        public ChangeEmployeeLocationHandler(IUnitOfWork unitOfWork, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(ChangeEmployeeLocationRequest request, CancellationToken cancellationToken)
        {
            await  _EmployeeRepository.ChangeEmployeeLocation(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
