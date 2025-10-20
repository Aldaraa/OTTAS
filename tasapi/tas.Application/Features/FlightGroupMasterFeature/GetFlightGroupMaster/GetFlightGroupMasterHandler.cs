using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster
{

    public sealed class GetFlightGroupMasterHandler : IRequestHandler<GetFlightGroupMasterRequest, GetFlightGroupMasterResponse>
    {
        private readonly IFlightGroupMasterRepository _FlightGroupMasterRepository;
        private readonly IMapper _mapper;

        public GetFlightGroupMasterHandler(IFlightGroupMasterRepository FlightGroupMasterRepository, IMapper mapper)
        {
            _FlightGroupMasterRepository = FlightGroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<GetFlightGroupMasterResponse> Handle(GetFlightGroupMasterRequest request, CancellationToken cancellationToken)
        {
            return await _FlightGroupMasterRepository.GetProfile(request, cancellationToken);
            

        }
    }
}
