using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Entities;

namespace tas.Application.Features.VisitEventFeature.CreateVisitEvent
{
    public sealed class CreateVisitEventMapper : Profile
    {
        public CreateVisitEventMapper()
        {
            CreateMap<CreateVisitEventRequest, VisitEvent>();
        }
    }
}
