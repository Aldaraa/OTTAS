using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OtinfoFeature.ManualSent
{

    public sealed class ManualSentHandler : IRequestHandler<ManualSentRequest>
    {
        private readonly ITransportInfoRepository _TransportInfoRepository;
        private readonly IMapper _mapper;

        public ManualSentHandler(ITransportInfoRepository TransportInfoRepository, IMapper mapper)
        {
            _TransportInfoRepository = TransportInfoRepository;
            _mapper = mapper;
        }

        public async Task Handle(ManualSentRequest request, CancellationToken cancellationToken)
        {
             await _TransportInfoRepository.ManualSentData(request, cancellationToken);
            return;

        }
    }
}
