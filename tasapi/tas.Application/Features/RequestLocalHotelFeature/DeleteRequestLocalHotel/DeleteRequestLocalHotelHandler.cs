using AutoMapper;
using MediatR;
using tas.Application.Features.RequestLocalHotelFeature.DeleteRequestLocalHotel;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLocalHotelFeature.DeletRequestLocalHotel
{

    public sealed class DeletRequestLocalHotelHandler : IRequestHandler<DeleteRequestLocalHotelRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestLocalHotelRepository _RequestLocalHotelRepository;
        private readonly IMapper _mapper;

        public DeletRequestLocalHotelHandler(IUnitOfWork unitOfWork, IRequestLocalHotelRepository RequestLocalHotelRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestLocalHotelRepository = RequestLocalHotelRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteRequestLocalHotelRequest request, CancellationToken cancellationToken)
        {
            var RequestLocalHotel = _mapper.Map<RequestLocalHotel>(request);
            _RequestLocalHotelRepository.Delete(RequestLocalHotel);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
