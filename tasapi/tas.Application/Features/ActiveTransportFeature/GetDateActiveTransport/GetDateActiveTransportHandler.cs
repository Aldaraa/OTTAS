using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.ActiveTransportFeature.GetDateActiveTransport
{

    public sealed class GetDateActiveTransportHandler : IRequestHandler<GetDateActiveTransportRequest, List<GetDateActiveTransportResponse>>
    {
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public GetDateActiveTransportHandler(IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<List<GetDateActiveTransportResponse>> Handle(GetDateActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var data = await _ActiveTransportRepository.GetDateData(request, cancellationToken);
            return data;

        }
    }
}
