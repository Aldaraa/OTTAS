using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport
{

    public sealed class ScheduleListActiveTransportHandler : IRequestHandler<ScheduleListActiveTransportRequest, ScheduleListActiveTransportResponse>
    {
        private readonly IActiveTransportRepository _ActiveTransportRepository;

        public ScheduleListActiveTransportHandler(IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _ActiveTransportRepository = ActiveTransportRepository;
        }

        public async Task<ScheduleListActiveTransportResponse> Handle(ScheduleListActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var data = await _ActiveTransportRepository.ScheduleList(request, cancellationToken);
            return data;

        }
    }
}
