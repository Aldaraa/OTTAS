using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment
{

    public sealed class FindAvailableByDatesAssignmentHandler : IRequestHandler<FindAvailableByDatesAssignmentRequest, List<FindAvailableByDatesAssignmentResponse>>
    {
        private readonly IRoomAssignmentRepository _RoomAssignmentRepository;
        private readonly IMapper _mapper;

        public FindAvailableByDatesAssignmentHandler(IRoomAssignmentRepository RoomAssignmentRepository, IMapper mapper)
        {
            _RoomAssignmentRepository = RoomAssignmentRepository;
            _mapper = mapper;
        }

        public async Task<List<FindAvailableByDatesAssignmentResponse>> Handle(FindAvailableByDatesAssignmentRequest request, CancellationToken cancellationToken)
        {
            return await _RoomAssignmentRepository.FindAvailableByDatesAssignment(request, cancellationToken);
        }
    }
}
