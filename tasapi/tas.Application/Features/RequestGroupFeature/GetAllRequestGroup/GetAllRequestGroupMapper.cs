using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestGroupFeature.GetAllRequestGroup
{
    public sealed class GetAllRequestGroupMapper : Profile
    {
        public GetAllRequestGroupMapper()
        {
            CreateMap<RequestGroup, GetAllRequestGroupResponse>();
        }
    }

}

