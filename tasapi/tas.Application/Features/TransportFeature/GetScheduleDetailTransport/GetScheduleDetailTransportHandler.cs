using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.GetScheduleDetailTransport
{

    public sealed class GetScheduleDetailTransportHandler : IRequestHandler<GetScheduleDetailTransportRequest, List<GetScheduleDetailTransportResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly ITransportScheduleCalculateRepository _ITransportScheduleCalculateRepository;
        private readonly IMapper _mapper;

        public GetScheduleDetailTransportHandler(ITransportRepository TransportRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _ITransportScheduleCalculateRepository = transportScheduleCalculateRepository;
            _mapper = mapper;
        }

        public async Task<List<GetScheduleDetailTransportResponse>> Handle(GetScheduleDetailTransportRequest request, CancellationToken cancellationToken)
        {
            await _ITransportScheduleCalculateRepository.CalculateByScheduleId(request.ScheduleId, cancellationToken);
            return await _ITransportRepository.ScheduleDetail(request, cancellationToken);




        }
    }
}
