using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.UpdateDepartment
{
    public class UpdateDepartmentHandler : IRequestHandler<UpdateDepartmentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public UpdateDepartmentHandler(IUnitOfWork unitOfWork, IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            await _DepartmentRepository.UpdateDepartment(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
