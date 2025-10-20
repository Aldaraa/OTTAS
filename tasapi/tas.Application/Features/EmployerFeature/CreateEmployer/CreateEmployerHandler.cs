using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.CreateEmployer
{
    public sealed class CreateEmployerHandler : IRequestHandler<CreateEmployerRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public CreateEmployerHandler(IUnitOfWork unitOfWork, IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateEmployerRequest request, CancellationToken cancellationToken)
        {
            var Employer = _mapper.Map<Employer>(request);
           
            await _EmployerRepository.CheckDuplicateData(Employer, c => c.Code, c=> c.Description);
            _EmployerRepository.Create(Employer);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
