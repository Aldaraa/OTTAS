using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.NationalityFeature.UpdateNationality
{

    public sealed class UpdateNationalityMapper : Profile
    {
        public UpdateNationalityMapper()
        {
            CreateMap<UpdateNationalityRequest, Nationality>();
        }
    }
}
