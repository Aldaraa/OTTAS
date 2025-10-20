using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.FindAvailableByDates
{

    public sealed class FindAvailableByDatesHandler : IRequestHandler<FindAvailableByDatesRequest, List<FindAvailableByDatesResponse>>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public FindAvailableByDatesHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<List<FindAvailableByDatesResponse>> Handle(FindAvailableByDatesRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.FindAvailableByDates(request, cancellationToken);
        }
    }
}
