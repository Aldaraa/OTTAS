using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PeopleTypeFeature.UpdatePeopleType;
using tas.Domain.Entities;

namespace tas.Application.Features.PeopleTypeFeature.DeletePeopleType
{

    public sealed class DeletePeopleTypeMapper : Profile
    {
        public DeletePeopleTypeMapper()
        {
            CreateMap<DeletePeopleTypeRequest, PeopleType>();
        }
    }
}
