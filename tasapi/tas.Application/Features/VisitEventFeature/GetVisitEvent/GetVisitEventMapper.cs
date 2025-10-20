using AutoMapper;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.GetVisitEvent
{
    public sealed class GetVisitEventMapper : Profile
    {
        public GetVisitEventMapper()
        {
            CreateMap<VisitEvent, GetVisitEventResponse>();
        }
    }

}

