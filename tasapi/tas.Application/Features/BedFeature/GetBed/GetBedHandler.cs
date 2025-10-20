using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Repositories;

namespace tas.Application.Features.BedFeature.GetBed
{

    public sealed class GetSysTeamHandler : IRequestHandler<GetBedRequest, GetBedResponse>
    {
        private readonly IBedRepository _BedRepository;
        private readonly IMapper _mapper;

        public GetSysTeamHandler(IBedRepository BedRepository, IMapper mapper)
        {
            _BedRepository = BedRepository;
            _mapper = mapper;
        }

        public async Task<GetBedResponse> Handle(GetBedRequest request, CancellationToken cancellationToken)
        {
            var Beds = await _BedRepository.Get(request.Id, cancellationToken);
            return _mapper.Map<GetBedResponse>(Beds);

        }
    }
}
