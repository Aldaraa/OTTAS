using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.EmployerFeature.BulkUploadEmployer
{
    public sealed class BulkUploadEmployerHandler : IRequestHandler<BulkUploadEmployerRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployerRepository _EmployerRepository;
        private readonly IMapper _mapper;

        public BulkUploadEmployerHandler(IUnitOfWork unitOfWork, IEmployerRepository EmployerRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _EmployerRepository = EmployerRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(BulkUploadEmployerRequest request, CancellationToken cancellationToken)
        {
            await _EmployerRepository.BulkRequestUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
