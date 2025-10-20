using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CampFeature.UpdateCamp;
using tas.Domain.Entities;

namespace tas.Application.Features.CampFeature.DeleteCamp
{

    public sealed class DeleteCampMapper : Profile
    {
        public DeleteCampMapper()
        {
            CreateMap<DeleteCampRequest, Camp>();
        }
    }
}
