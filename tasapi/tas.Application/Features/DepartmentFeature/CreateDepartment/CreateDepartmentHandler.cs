using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.CreateUser;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.CreateDepartment
{
    public sealed class CreateDepartmentHandler : IRequestHandler<CreateDepartmentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public CreateDepartmentHandler(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
            await  _departmentRepository.CreateDepartment(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
