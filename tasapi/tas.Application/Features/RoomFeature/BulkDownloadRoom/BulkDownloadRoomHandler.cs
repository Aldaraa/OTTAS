using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RoomFeature.BulkDownloadRoom
{
    public sealed class BulkDownloadRoomHandler : IRequestHandler<BulkDownloadRoomRequest, BulkDownloadRoomResponse>
    {
        private readonly IRoomRepository _RoomRepository;

        public BulkDownloadRoomHandler(IRoomRepository RoomRepository)
        {
            _RoomRepository = RoomRepository;
        }

        public async Task<BulkDownloadRoomResponse>  Handle(BulkDownloadRoomRequest request, CancellationToken cancellationToken)
        {
          return await _RoomRepository.BulkRequestDownload(request, cancellationToken);
        }
    }
}
