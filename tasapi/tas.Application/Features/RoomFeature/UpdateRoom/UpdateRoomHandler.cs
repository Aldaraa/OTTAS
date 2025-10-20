using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.UpdateRoom
{
    public class UpdateRoomHandler : IRequestHandler<UpdateRoomRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public UpdateRoomHandler(IUnitOfWork unitOfWork, IRoomRepository RoomRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRoomRequest request, CancellationToken cancellationToken)
        {
            await _RoomRepository.UpdateValidateRoom(request, cancellationToken);

          var Room = _mapper.Map<Room>(request);
            await _RoomRepository.CheckDuplicateData(Room, c => c.Number);
            _RoomRepository.Update(Room);
                    await _unitOfWork.Save(cancellationToken);
        //  await  _RoomRepository.GenerateBed(Room.Id);
                    return Unit.Value;
        }
    }
}
