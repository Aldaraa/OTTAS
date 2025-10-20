using AutoMapper;
using MediatR;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Application.Repositories;

namespace tas.Application.Features.RoomTypeFeature.GetAllRoomType
{

    public sealed class GetAllRoomTypeHandler : IRequestHandler<GetAllRoomTypeRequest, List<GetAllRoomTypeResponse>>
    {
        private readonly IRoomTypeRepository _RoomTypeRepository;
        private readonly IMapper _mapper;

        public GetAllRoomTypeHandler(IRoomTypeRepository RoomTypeRepository, IMapper mapper)
        {
            _RoomTypeRepository = RoomTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRoomTypeResponse>> Handle(GetAllRoomTypeRequest request, CancellationToken cancellationToken)
        {
            var data = await _RoomTypeRepository.GetAllData(request, cancellationToken);
            return _mapper.Map<List<GetAllRoomTypeResponse>>(data);

            //if (request.status.HasValue)
            //{
            //    var RoomTypes = await _RoomTypeRepository.GetAllActiveFilter((int)request.status, cancellationToken);
            //    return _mapper.Map<List<GetAllRoomTypeResponse>>(RoomTypes);
            //}
            //else {

            //}

        }
    }
}
