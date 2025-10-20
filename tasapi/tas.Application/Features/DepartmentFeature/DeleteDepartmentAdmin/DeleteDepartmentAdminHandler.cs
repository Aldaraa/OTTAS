using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartmentAdmin
{

    public sealed class DeleteDepartmentAdminHandler : IRequestHandler<DeleteDepartmentAdminRequest,Unit>
    {
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentAdminHandler(IDepartmentRepository DepartmentRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteDepartmentAdminRequest request, CancellationToken cancellationToken)
        {
            await _DepartmentRepository.DeleteDepartmentAdmin(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;


        }
    }
}
