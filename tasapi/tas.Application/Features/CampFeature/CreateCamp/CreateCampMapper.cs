using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.CampFeature.CreateCamp
{
    public sealed class CreateCampMapper : Profile
    {
        public CreateCampMapper()
        {
            CreateMap<CreateCampRequest, Camp>();
        }
    }
}
