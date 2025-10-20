using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.DeleteEmployer
{

    public sealed class DeleteEmployerHandler : IRequestHandler<DeleteEmployerRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public DeleteEmployerHandler(IUnitOfWork unitOfWork, IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteEmployerRequest request, CancellationToken cancellationToken)
        {
            var Employer = _mapper.Map<Employer>(request);
            _EmployerRepository.Delete(Employer);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
