using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.AddDepartmentAdmin
{

    public sealed class AddDepartmentAdminHandler : IRequestHandler<AddDepartmentAdminRequest,Unit>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddDepartmentAdminHandler(IDepartmentRepository DepartmentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddDepartmentAdminRequest request, CancellationToken cancellationToken)
        {
            await _DepartmentRepository.AddDepartmentAdmin(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
