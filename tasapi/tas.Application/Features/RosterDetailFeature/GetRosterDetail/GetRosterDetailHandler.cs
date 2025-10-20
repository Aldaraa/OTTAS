using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;

namespace tas.Application.Features.RosterDetailFeature.GetRosterDetail
{

    public sealed class GetRosterDetailHandler : IRequestHandler<GetRosterDetailRequest, List<GetRosterDetailResponse>>
    {
        private readonly IRosterDetailRepository _RosterDetailRepository;
        private readonly IMapper _mapper;

        public GetRosterDetailHandler(IRosterDetailRepository RosterDetailRepository, IMapper mapper)
        {
            _RosterDetailRepository = RosterDetailRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRosterDetailResponse>> Handle(GetRosterDetailRequest request, CancellationToken cancellationToken)
        {
            var RosterDetails = await _RosterDetailRepository.GetbyRosterId(request.RosterId, cancellationToken);
            return RosterDetails;

        }
    }
}
