using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.UserFeatures.GetAllUser;
using tas.Application.Repositories;

namespace tas.Application.Features.BedFeature.GetAllBed
{

    public sealed class GetAllBedHandler : IRequestHandler<GetAllBedRequest, List<GetAllBedResponse>>
    {
        private readonly IBedRepository _BedRepository;
        private readonly IMapper _mapper;

        public GetAllBedHandler(IBedRepository BedRepository, IMapper mapper)
        {
            _BedRepository = BedRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllBedResponse>> Handle(GetAllBedRequest request, CancellationToken cancellationToken)
        {
            if (request.status.HasValue)
            {
                var Beds = await _BedRepository.GetAllBedFilter((int)request.status, request.roomId, cancellationToken);
                return _mapper.Map<List<GetAllBedResponse>>(Beds);
            }
            else {
                var users = await _BedRepository.GetAllBed(request.roomId, cancellationToken);
                return _mapper.Map<List<GetAllBedResponse>>(users);
            }

        }
    }
}
