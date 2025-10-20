using AutoMapper;
using MediatR;
using tas.Application.Features.PositionFeature.BulkUploadPositionEmployees;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.PositionFeature.BulkUploadPosition
{
    public sealed class BulkUploadPositionEmployeesHandler : IRequestHandler<BulkUploadPositionEmployeesRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPositionRepository _PositionRepository;
        private readonly IMapper _mapper;

        public BulkUploadPositionEmployeesHandler(IUnitOfWork unitOfWork, IPositionRepository PositionRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _PositionRepository = PositionRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(BulkUploadPositionEmployeesRequest request, CancellationToken cancellationToken)
        {
            await _PositionRepository.BulkRequestEmployeesUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
