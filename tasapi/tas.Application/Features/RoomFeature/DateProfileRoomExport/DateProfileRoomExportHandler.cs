using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomFeature.DateProfileRoomExport
{

    public sealed class DateProfileRoomExportHandler : IRequestHandler<DateProfileRoomExportRequest, DateProfileRoomExportResponse>
    {
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public DateProfileRoomExportHandler(IRoomRepository RoomRepository, IMapper mapper)
        {
            _RoomRepository = RoomRepository;
            _mapper = mapper;
        }

        public async Task<DateProfileRoomExportResponse> Handle(DateProfileRoomExportRequest request, CancellationToken cancellationToken)
        {
            return await _RoomRepository.GetRoomDateProfileExport(request, cancellationToken);

        }
    }
}
