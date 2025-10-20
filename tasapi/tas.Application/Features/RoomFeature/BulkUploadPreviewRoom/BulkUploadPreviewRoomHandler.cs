using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.BulkUploadPreviewRoom
{
    public sealed class BulkUploadPreviewRoomHandler : IRequestHandler<BulkUploadPreviewRoomRequest, BulkUploadPreviewRoomResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public BulkUploadPreviewRoomHandler(IUnitOfWork unitOfWork, IRoomRepository RoomRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<BulkUploadPreviewRoomResponse>  Handle(BulkUploadPreviewRoomRequest request, CancellationToken cancellationToken)
        {
          var returnData =  await _RoomRepository.BulkRequestUploadPreview(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return returnData;
        }
    }
}
