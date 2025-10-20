using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterFeature.GetAllCluster
{
    public sealed class GetAllClusterMapper : Profile
    {
        public GetAllClusterMapper()
        {
            CreateMap<Cluster, GetAllClusterResponse>();
        }
    }

}

