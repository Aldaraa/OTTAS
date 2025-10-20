using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RosterFeature.GetAllRoster
{

    public sealed class GetAllRosterHandler : IRequestHandler<GetAllRosterRequest, List<GetAllRosterResponse>>
    {
        private readonly IRosterRepository _RosterRepository;
        private readonly IMapper _mapper;

        public GetAllRosterHandler(IRosterRepository RosterRepository, IMapper mapper)
        {
            _RosterRepository = RosterRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRosterResponse>> Handle(GetAllRosterRequest request, CancellationToken cancellationToken)
        {
            return await _RosterRepository.GetAllData(request, cancellationToken);

        }
    }
}
