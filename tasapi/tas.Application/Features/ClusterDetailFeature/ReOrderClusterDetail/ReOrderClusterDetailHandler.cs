using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterDetailFeature.ReOrderClusterDetail
{
    public sealed class ReOrderClusterDetailHandler : IRequestHandler<ReOrderClusterDetailRequest, Unit>
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

        public async Task<Unit>  Handle(ReOrderClusterDetailRequest request, CancellationToken cancellationToken)
        {
            await _ClusterDetailRepository.ReOrder(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
