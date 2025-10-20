using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.CreateRoom
{
    public sealed class CreateRoomHandler : IRequestHandler<CreateRoomRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public CreateRoomHandler(IUnitOfWork unitOfWork, IRoomRepository RoomRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRoomRequest request, CancellationToken cancellationToken)
        {
          await  _RoomRepository.CreateValidateRoom(request, cancellationToken);
            var Room = _mapper.Map<Room>(request);
            await _RoomRepository.CheckDuplicateData(Room, c => c.Number);
            _RoomRepository.Create(Room);
            await _unitOfWork.Save(cancellationToken);
            await  _RoomRepository.GenerateBed(Room.Id);
            return Unit.Value;
        }
    }
}
