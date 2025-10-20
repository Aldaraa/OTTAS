using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterDetailFeature.CreateClusterDetail
{
    public sealed class ReOrderClusterDetailHandler : IRequestHandler<CreateClusterDetailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClusterDetailRepository _ClusterDetailRepository;
        private readonly IMapper _mapper;

        public ReOrderClusterDetailHandler(IUnitOfWork unitOfWork, IClusterDetailRepository ClusterDetailRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ClusterDetailRepository = ClusterDetailRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateClusterDetailRequest request, CancellationToken cancellationToken)
        {
            await _ClusterDetailRepository.Create(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
