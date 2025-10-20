using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.ClusterFeature.GetCluster
{

    public sealed class GetClusterHandler : IRequestHandler<GetClusterRequest, GetClusterResponse>
    {
        private readonly IClusterRepository _ClusterRepository;
        private readonly IMapper _mapper;

        public GetClusterHandler(IClusterRepository ClusterRepository, IMapper mapper)
        {
            _ClusterRepository = ClusterRepository;
            _mapper = mapper;
        }

        public async Task<GetClusterResponse> Handle(GetClusterRequest request, CancellationToken cancellationToken)
        {
            return await _ClusterRepository.GetCluster(request, cancellationToken);
   
        }
    }
}
