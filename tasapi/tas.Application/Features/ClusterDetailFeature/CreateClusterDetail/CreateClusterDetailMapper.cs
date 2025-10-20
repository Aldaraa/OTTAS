using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.ClusterDetailFeature.CreateClusterDetail
{
    public sealed class CreateFlightGroupShiftMapper : Profile
    {
        public CreateFlightGroupShiftMapper()
        {
            CreateMap<CreateClusterDetailRequest, Cluster>();
        }
    }
}
