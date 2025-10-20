using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.ClusterFeature.GetActiveTransportCluster
{

    public sealed class GetActiveTransportClusterHandler : IRequestHandler<GetActiveTransportClusterRequest, List<GetActiveTransportClusterResponse>>
    {
        private readonly IClusterRepository _ClusterRepository;
        private readonly IMapper _mapper;

        public GetActiveTransportClusterHandler(IClusterRepository ClusterRepository, IMapper mapper)
        {
            _ClusterRepository = ClusterRepository;
            _mapper = mapper;
        }

        public async Task<List<GetActiveTransportClusterResponse>> Handle(GetActiveTransportClusterRequest request, CancellationToken cancellationToken)
        {
            var Transports = await _ClusterRepository.GetActiveTranports(request, cancellationToken);
            return _mapper.Map<List<GetActiveTransportClusterResponse>>(Transports);

        }
    }
}
