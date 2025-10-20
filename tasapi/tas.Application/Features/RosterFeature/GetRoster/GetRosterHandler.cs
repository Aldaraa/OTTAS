using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RosterFeature.GetRoster
{

    public sealed class GetRosterHandler : IRequestHandler<GetRosterRequest, GetRosterResponse>
    {
        private readonly IRosterRepository _RosterRepository;
        private readonly IMapper _mapper;

        public GetRosterHandler(IRosterRepository RosterRepository, IMapper mapper)
        {
            _RosterRepository = RosterRepository;
            _mapper = mapper;
        }

        public async Task<GetRosterResponse> Handle(GetRosterRequest request, CancellationToken cancellationToken)
        {
            var Roster = await _RosterRepository.Get(request.Id, cancellationToken);
            return _mapper.Map<GetRosterResponse>(Roster);
        }
    }
}
