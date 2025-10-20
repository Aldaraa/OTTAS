using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.CheckDataRequest
{

    public sealed class CheckDataRequestHandler : IRequestHandler<CheckDataRequestRequest, Unit>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public CheckDataRequestHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CheckDataRequestRequest request, CancellationToken cancellationToken)
        {
            await _ITransportRepository.CheckDataRequestRequestChange(request, cancellationToken);
            return Unit.Value;
        }
    }
}
