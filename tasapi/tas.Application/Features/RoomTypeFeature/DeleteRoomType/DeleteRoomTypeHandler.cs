using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomTypeFeature.DeleteRoomType
{

    public sealed class DeleteRoomTypeHandler : IRequestHandler<DeleteRoomTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomTypeRepository _RoomTypeRepository;
        private readonly IMapper _mapper;

        public DeleteRoomTypeHandler(IUnitOfWork unitOfWork, IRoomTypeRepository RoomTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomTypeRepository = RoomTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRoomTypeRequest request, CancellationToken cancellationToken)
        {
            var RoomType = _mapper.Map<RoomType>(request);
            _RoomTypeRepository.Delete(RoomType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
