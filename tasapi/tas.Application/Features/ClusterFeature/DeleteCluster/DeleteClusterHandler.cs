using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterFeature.DeleteCluster
{

    public sealed class DeleteClusterHandler : IRequestHandler<DeleteClusterRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClusterRepository _ClusterRepository;
        private readonly IMapper _mapper;

        public DeleteClusterHandler(IUnitOfWork unitOfWork, IClusterRepository ClusterRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _ClusterRepository = ClusterRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteClusterRequest request, CancellationToken cancellationToken)
        {
            var Cluster = _mapper.Map<Cluster>(request);
            _ClusterRepository.Delete(Cluster);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
