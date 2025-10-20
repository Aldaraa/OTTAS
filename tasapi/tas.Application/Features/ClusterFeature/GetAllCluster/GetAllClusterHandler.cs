using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.ClusterFeature.GetAllCluster
{

    public sealed class GetAllClusterHandler : IRequestHandler<GetAllClusterRequest, List<GetAllClusterResponse>>
    {
        private readonly IClusterRepository _ClusterRepository;
        private readonly IMapper _mapper;

        public GetAllClusterHandler(IClusterRepository ClusterRepository, IMapper mapper)
        {
            _ClusterRepository = ClusterRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllClusterResponse>> Handle(GetAllClusterRequest request, CancellationToken cancellationToken)
        {

            return await _ClusterRepository.GetAllData(request, cancellationToken);

            //if (request.status.HasValue)
            //{
            //    var Clusters = await _ClusterRepository.GetAllActiveFilter((int)request.status, cancellationToken);
            //    return _mapper.Map<List<GetAllClusterResponse>>(Clusters);
            //}
            //else {

            //}

        }
    }
}
