using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.AddDepartmentSupervisor
{

    public sealed class AddDepartmentSupervisorHandler : IRequestHandler<AddDepartmentSupervisorRequest,Unit>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddDepartmentSupervisorHandler(IDepartmentRepository DepartmentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddDepartmentSupervisorRequest request, CancellationToken cancellationToken)
        {
            await _DepartmentRepository.AddDepartmentSupervisor(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
