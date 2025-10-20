using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ColorFeature.UpdateColor;
using tas.Domain.Entities;

namespace tas.Application.Features.ColorFeature.DeleteColor
{

    public sealed class DeleteColorMapper : Profile
    {
        public DeleteColorMapper()
        {
            CreateMap<DeleteColorRequest, Color>();
        }
    }
}
