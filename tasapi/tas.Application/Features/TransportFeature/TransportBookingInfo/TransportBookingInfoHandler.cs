using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.TransportBookingInfo
{

    public sealed class TransportBookingInfoHandler : IRequestHandler<TransportBookingInfoRequest, List<TransportBookingInfoResponse>>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public TransportBookingInfoHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<List<TransportBookingInfoResponse>> Handle(TransportBookingInfoRequest request, CancellationToken cancellationToken)
        {
            return await _ITransportRepository.TransportBookingInfo(request, cancellationToken);
        }
    }
}
