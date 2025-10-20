using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.LocationFeature.GetAllLocation
{

    public sealed class GetAllLocationHandler : IRequestHandler<GetAllLocationRequest, List<GetAllLocationResponse>>
    {
        private readonly ILocationRepository _LocationRepository;
        private readonly IMapper _mapper;

        public GetAllLocationHandler(ILocationRepository LocationRepository, IMapper mapper)
        {
            _LocationRepository = LocationRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllLocationResponse>> Handle(GetAllLocationRequest request, CancellationToken cancellationToken)
        {
            if (request.status.HasValue)
            {
                var Locations = await _LocationRepository.GetAllActiveFilter((int)request.status, cancellationToken);
                return _mapper.Map<List<GetAllLocationResponse>>(Locations);
            }
            else {
                var users = await _LocationRepository.GetAll(cancellationToken);
                return _mapper.Map<List<GetAllLocationResponse>>(users);
            }

        }
    }
}
