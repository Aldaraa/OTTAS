using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.GetAllVisitEvent
{
    public sealed class GetAllVisitEventMapper : Profile
    {
        public GetAllVisitEventMapper()
        {
            CreateMap<VisitEvent, GetAllVisitEventResponse>();
        }
    }

}

