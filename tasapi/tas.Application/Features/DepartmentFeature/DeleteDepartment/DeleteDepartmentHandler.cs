using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.DepartmentFeature.DeleteDepartment
{

    public sealed class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IMapper _mapper;

        public DeleteDepartmentHandler(IUnitOfWork unitOfWork, IDepartmentRepository DepartmentRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _DepartmentRepository = DepartmentRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteDepartmentRequest request, CancellationToken cancellationToken)
        {
            var Department = _mapper.Map<Department>(request);
            _DepartmentRepository.Delete(Department);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
