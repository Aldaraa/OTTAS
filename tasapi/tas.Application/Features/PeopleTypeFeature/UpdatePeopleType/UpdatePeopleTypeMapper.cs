using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.PeopleTypeFeature.UpdatePeopleType
{

    public sealed class UpdatePeopleTypeMapper : Profile
    {
        public UpdatePeopleTypeMapper()
        {
            CreateMap<UpdatePeopleTypeRequest, PeopleType>();
        }
    }
}
