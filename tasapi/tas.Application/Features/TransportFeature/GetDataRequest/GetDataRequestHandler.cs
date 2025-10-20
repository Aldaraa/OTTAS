using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportFeature.GetDataRequest
{

    public sealed class GetDataRequestHandler : IRequestHandler<GetDataRequestRequest, GetDataRequestResponse>
    {
        private readonly ITransportRepository _ITransportRepository;
        private readonly IMapper _mapper;

        public GetDataRequestHandler(ITransportRepository TransportRepository, IMapper mapper)
        {
            _ITransportRepository = TransportRepository;
            _mapper = mapper;
        }

        public async Task<GetDataRequestResponse> Handle(GetDataRequestRequest request, CancellationToken cancellationToken)
        {

            return await _ITransportRepository.GetDataRequestRequestChange(request, cancellationToken);




        }
    }
}
