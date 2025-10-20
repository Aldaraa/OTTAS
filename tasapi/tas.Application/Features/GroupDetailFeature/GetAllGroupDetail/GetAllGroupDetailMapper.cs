using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.GroupDetailFeature.GetAllGroupDetail
{
    public sealed class GetAllGroupDetailMapper : Profile
    {
        public GetAllGroupDetailMapper()
        {
            CreateMap<GroupDetail, GetAllGroupDetailResponse>();
        }
    }

}

