using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.EmployeeRoomProfile
{

    public sealed class EmployeeRoomProfileHandler : IRequestHandler<EmployeeRoomProfileRequest, EmployeeRoomProfileResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public EmployeeRoomProfileHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeRoomProfileResponse> Handle(EmployeeRoomProfileRequest request, CancellationToken cancellationToken)
        {
            var room = await _RoomRepository.GetEmployeeRoomProfile(request, cancellationToken);
            return _mapper.Map<EmployeeRoomProfileResponse>(room);
        }
    }
}
