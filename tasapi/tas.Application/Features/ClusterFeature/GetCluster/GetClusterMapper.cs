using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterFeature.GetCluster
{
    public sealed class GetClusterMapper : Profile
    {
        public GetClusterMapper()
        {
            CreateMap<Cluster, GetClusterResponse>();
        }
    }

}

