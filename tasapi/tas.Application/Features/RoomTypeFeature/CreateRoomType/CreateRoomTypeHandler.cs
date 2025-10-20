using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomTypeFeature.CreateRoomType
{
    public sealed class CreateRoomTypeHandler : IRequestHandler<CreateRoomTypeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomTypeRepository _RoomTypeRepository;
        private readonly IMapper _mapper;

        public CreateRoomTypeHandler(IUnitOfWork unitOfWork, IRoomTypeRepository RoomTypeRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomTypeRepository = RoomTypeRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRoomTypeRequest request, CancellationToken cancellationToken)
        {
            var RoomType = _mapper.Map<RoomType>(request);
            await _RoomTypeRepository.CheckDuplicateData(RoomType, c => c.Description);
            _RoomTypeRepository.Create(RoomType);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
