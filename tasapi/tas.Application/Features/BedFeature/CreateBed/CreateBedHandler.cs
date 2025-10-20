using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.BedFeature.CreateBed
{
    public sealed class CreateBedHandler : IRequestHandler<CreateBedRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBedRepository _BedRepository;
        private readonly IRoomRepository _RoomRepository;
        private readonly IMapper _mapper;

        public CreateBedHandler(IUnitOfWork unitOfWork, IBedRepository BedRepository, IRoomRepository roomRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _BedRepository = BedRepository;
            _mapper = mapper;
            _RoomRepository = roomRepository;
        }

        public async Task<Unit>  Handle(CreateBedRequest request, CancellationToken cancellationToken)
        {
            var Bed = _mapper.Map<Bed>(request);
            _BedRepository.Create(Bed);
            await _unitOfWork.Save(cancellationToken);
           // await _RoomRepository.UpdateRoomBedCount(request.RoomId);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
