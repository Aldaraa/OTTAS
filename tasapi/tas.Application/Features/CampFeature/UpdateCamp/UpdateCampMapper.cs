using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.CampFeature.UpdateCamp
{

    public sealed class UpdateCampMapper : Profile
    {
        public UpdateCampMapper()
        {
            CreateMap<UpdateCampRequest, Camp>();
        }
    }
}
