using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport
{

    public sealed class GetAllActiveTransportHandler : IRequestHandler<GetAllActiveTransportRequest, GetAllActiveTransportResponse>
    {
        private readonly IActiveTransportRepository _ActiveTransportRepository;
        private readonly IMapper _mapper;

        public GetAllActiveTransportHandler(IActiveTransportRepository ActiveTransportRepository, IMapper mapper)
        {
            _ActiveTransportRepository = ActiveTransportRepository;
            _mapper = mapper;
        }

        public async Task<GetAllActiveTransportResponse> Handle(GetAllActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var data = await _ActiveTransportRepository.GetAllData(request, cancellationToken);
            return data;

        }
    }
}
