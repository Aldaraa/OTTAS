using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NationalityFeature.UpdateNationality;
using tas.Domain.Entities;

namespace tas.Application.Features.NationalityFeature.DeleteNationality
{

    public sealed class DeleteNationalityMapper : Profile
    {
        public DeleteNationalityMapper()
        {
            CreateMap<DeleteNationalityRequest, Nationality>();
        }
    }
}
