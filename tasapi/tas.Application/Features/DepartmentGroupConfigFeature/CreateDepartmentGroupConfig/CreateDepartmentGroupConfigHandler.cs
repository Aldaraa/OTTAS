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

namespace tas.Application.Features.DepartmentGroupConfigFeature.CreateDepartmentGroupConfig
{
    public sealed class CreateDepartmentGroupConfigHandler : IRequestHandler<CreateDepartmentGroupConfigRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentGroupConfigRepository _DepartmentGroupConfigRepository;
        private readonly IMapper _mapper;

        public CreateDepartmentGroupConfigHandler(IUnitOfWork unitOfWork, IDepartmentGroupConfigRepository DepartmentGroupConfigRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentGroupConfigRepository = DepartmentGroupConfigRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateDepartmentGroupConfigRequest request, CancellationToken cancellationToken)
        {
            await  _DepartmentGroupConfigRepository.CreateDepartmentGroupConfig(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
