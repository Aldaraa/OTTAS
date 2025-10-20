using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.ActiveTransportFeature.GetExtendActiveTransport
{

    public sealed class GetExtendActiveTransportHandler : IRequestHandler<GetExtendActiveTransportRequest, GetExtendActiveTransportResponse>
    {
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public GetExtendActiveTransportHandler(IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<GetExtendActiveTransportResponse> Handle(GetExtendActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var data = await _ActiveTransportRepository.GetExtendActiveTransport(request, cancellationToken);
            return data;

        }
    }
}
