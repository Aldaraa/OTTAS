using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.TransportFeature.TransportInfo
{

    public sealed class TransportInfoHandler : IRequestHandler<TransportInfoRequest, List<TransportInfoResponse>>
    {
        private readonly ITransportInfoRepository _transportInfoRepository;
        private readonly IMapper _mapper;

        public TransportInfoHandler(ITransportInfoRepository transportInfoRepository, IMapper mapper)
        {
            _transportInfoRepository = transportInfoRepository;
            _mapper = mapper;
        }

        public async Task<List<TransportInfoResponse>> Handle(TransportInfoRequest request, CancellationToken cancellationToken)
        {
            var data = await _transportInfoRepository.EmployeeTransportInfo();
            return data;

        }
    }
}
