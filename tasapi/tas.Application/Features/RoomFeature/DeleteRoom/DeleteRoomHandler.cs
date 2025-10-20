using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.DeleteRoom
{

    public sealed class DeleteRoomHandler : IRequestHandler<DeleteRoomRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public DeleteRoomHandler(IUnitOfWork unitOfWork, IRoomRepository RoomRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRoomRequest request, CancellationToken cancellationToken)
        {
            var Room = _mapper.Map<Room>(request);



            _RoomRepository.Delete(Room);
            await _RoomRepository.DeActiveRoomOwnersRemove(request.Id, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
