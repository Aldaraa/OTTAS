using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.ActiveTransportFeature.GetCalendarActiveTransport
{

    public sealed class GetCalendarActiveTransportHandler : IRequestHandler<GetCalendarActiveTransportRequest, List<GetCalendarActiveTransportResponse>>
    {
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public GetCalendarActiveTransportHandler(IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<List<GetCalendarActiveTransportResponse>> Handle(GetCalendarActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var data = await _ActiveTransportRepository.GetCalendarData(request, cancellationToken);
            return data;

        }
    }
}
