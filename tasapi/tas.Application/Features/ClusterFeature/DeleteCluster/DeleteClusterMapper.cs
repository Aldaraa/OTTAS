using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ClusterFeature.UpdateCluster;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterFeature.DeleteCluster
{

    public sealed class DeleteClusterMapper : Profile
    {
        public DeleteClusterMapper()
        {
            CreateMap<DeleteClusterRequest, Cluster>();
        }
    }
}
