using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.FlightGroupMasterFeature.GetAllFlightGroupMaster
{

    public sealed class GetFlightGroupMasterHandler : IRequestHandler<GetAllFlightGroupMasterRequest, List<GetAllFlightGroupMasterResponse>>
    {
        private readonly IFlightGroupMasterRepository _FlightGroupMasterRepository;
        private readonly IMapper _mapper;

        public GetFlightGroupMasterHandler(IFlightGroupMasterRepository FlightGroupMasterRepository, IMapper mapper)
        {
            _FlightGroupMasterRepository = FlightGroupMasterRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllFlightGroupMasterResponse>> Handle(GetAllFlightGroupMasterRequest request, CancellationToken cancellationToken)
        {
           return await _FlightGroupMasterRepository.GetAllData(request, cancellationToken);

            //if (request.status.HasValue)
            //{
            //    var FlightGroupMasters = await _FlightGroupMasterRepository.GetAllActiveFilter((int)request.status, cancellationToken);
            //    return _mapper.Map<List<GetAllFlightGroupMasterResponse>>(FlightGroupMasters);
            //}
            //else {
            //    var users = await _FlightGroupMasterRepository.GetAllData(request, cancellationToken);
            //    return _mapper.Map<List<GetAllFlightGroupMasterResponse>>(users);
            //}

        }
    }
}
