using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterFeature.UpdateCluster
{

    public sealed class UpdateClusterMapper : Profile
    {
        public UpdateClusterMapper()
        {
            CreateMap<UpdateClusterRequest, Cluster>();
        }
    }
}
