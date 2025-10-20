using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ClusterFeature.UpdateCluster;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterDetailFeature.DeleteClusterDetail
{

    public sealed class DeleteClusterDetailMapper : Profile
    {
        public DeleteClusterDetailMapper()
        {
            CreateMap<DeleteClusterDetailRequest, Cluster>();
        }
    }
}
