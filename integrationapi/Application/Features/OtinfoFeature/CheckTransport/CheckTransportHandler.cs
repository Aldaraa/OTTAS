using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OtinfoFeature.CheckTransport
{

    public sealed class CheckTransportHandler : IRequestHandler<CheckTransportRequest, string>
    {
        private readonly ITransportInfoRepository _transportInfoRepository;
        private readonly IMapper _mapper;

        public CheckTransportHandler(ITransportInfoRepository TransportInfoRepository, IMapper mapper)
        {
            _transportInfoRepository= TransportInfoRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(CheckTransportRequest request, CancellationToken cancellationToken)
        {
            var data = await _transportInfoRepository.CheckTransport(request, cancellationToken);
            return data;

        }
    }
}
