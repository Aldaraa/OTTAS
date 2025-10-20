using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.StateFeature.GetAllState
{
    public sealed class GetAllStateMapper : Profile
    {
        public GetAllStateMapper()
        {
            CreateMap<State, GetAllStateResponse>();
        }
    }

}

