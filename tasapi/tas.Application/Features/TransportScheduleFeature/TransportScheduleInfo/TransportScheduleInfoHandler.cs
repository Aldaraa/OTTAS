using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo
{

    public sealed class TransportScheduleInfoHandler : IRequestHandler<TransportScheduleInfoRequest, TransportScheduleInfoResponse>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;
        private readonly IMapper _mapper;

        public TransportScheduleInfoHandler(ITransportScheduleRepository TransportScheduleRepository, IMapper mapper)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
            _mapper = mapper;
        }

        public async Task<TransportScheduleInfoResponse> Handle(TransportScheduleInfoRequest request, CancellationToken cancellationToken)
        {

            return await _TransportScheduleRepository.GetTransportScheduleInfo(request, cancellationToken);




        }
    }
}
