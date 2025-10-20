using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestLocalHotelFeature.CreateRequestLocalHotel
{
    public sealed class CreateRequestLocalHotelHandler : IRequestHandler<CreateRequestLocalHotelRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestLocalHotelRepository _RequestLocalHotelRepository;
        private readonly IMapper _mapper;

        public CreateRequestLocalHotelHandler(IUnitOfWork unitOfWork, IRequestLocalHotelRepository RequestLocalHotelRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestLocalHotelRepository = RequestLocalHotelRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateRequestLocalHotelRequest request, CancellationToken cancellationToken)
        {
            var RequestLocalHotel = _mapper.Map<RequestLocalHotel>(request);
            await _RequestLocalHotelRepository.CheckDuplicateData(RequestLocalHotel,  c => c.Description);
            _RequestLocalHotelRepository.Create(RequestLocalHotel);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
