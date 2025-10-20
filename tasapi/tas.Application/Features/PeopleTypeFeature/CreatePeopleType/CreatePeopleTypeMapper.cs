using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.PeopleTypeFeature.CreatePeopleType
{
    public sealed class CreatePeopleTypeMapper : Profile
    {
        public CreatePeopleTypeMapper()
        {
            CreateMap<CreatePeopleTypeRequest, PeopleType>();
        }
    }
}
