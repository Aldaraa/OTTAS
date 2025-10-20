using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.RoomAssignmentEmployeeInfo
{

    public sealed class RoomAssignmentEmployeeInfoHandler : IRequestHandler<RoomAssignmentEmployeeInfoRequest, RoomAssignmentEmployeeInfoResponse>
    {
        private readonly IRoomAssignmentRepository _RoomAssignmentRepository;
        private readonly IMapper _mapper;

        public RoomAssignmentEmployeeInfoHandler(IRoomAssignmentRepository RoomAssignmentRepository, IMapper mapper)
        {
            _RoomAssignmentRepository = RoomAssignmentRepository;
            _mapper = mapper;
        }

        public async Task<RoomAssignmentEmployeeInfoResponse> Handle(RoomAssignmentEmployeeInfoRequest request, CancellationToken cancellationToken)
        {
            return await _RoomAssignmentRepository.EmployeeInfo(request, cancellationToken);
        }
    }
}
