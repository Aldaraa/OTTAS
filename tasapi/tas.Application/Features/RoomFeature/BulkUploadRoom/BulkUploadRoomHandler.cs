using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.BulkUploadRoom
{
    public sealed class BulkUploadRoomHandler : IRequestHandler<BulkUploadRoomRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public BulkUploadRoomHandler(IUnitOfWork unitOfWork, IRoomRepository RoomRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(BulkUploadRoomRequest request, CancellationToken cancellationToken)
        {
             await _RoomRepository.BulkRequestUpload(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
