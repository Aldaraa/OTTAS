using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RosterDetailFeature.UpdateRosterDetail;
using tas.Domain.Entities;

namespace tas.Application.Features.RosterDetailFeature.DeleteRosterDetail
{

    public sealed class DeleteRosterDetailMapper : Profile
    {
        public DeleteRosterDetailMapper()
        {
            CreateMap<DeleteRosterDetailRequest, RosterDetail>();
        }
    }
}
