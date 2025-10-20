using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestLocalHotelFeature.GetAllRequestLocalHotel
{

    public sealed class GetAllRequestLocalHotelHandler : IRequestHandler<GetAllRequestLocalHotelRequest, List<GetAllRequestLocalHotelResponse>>
    {
        private readonly IRequestLocalHotelRepository _RequestLocalHotelRepository;
        private readonly IMapper _mapper;

        public GetAllRequestLocalHotelHandler(IRequestLocalHotelRepository RequestLocalHotelRepository, IMapper mapper)
        {
            _RequestLocalHotelRepository = RequestLocalHotelRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestLocalHotelResponse>> Handle(GetAllRequestLocalHotelRequest request, CancellationToken cancellationToken)
        {
            if (request.status.HasValue)
            {
                var RequestLocalHotels = await _RequestLocalHotelRepository.GetAllActiveFilter((int)request.status, cancellationToken);
                return _mapper.Map<List<GetAllRequestLocalHotelResponse>>(RequestLocalHotels);
            }
            else {
                var users = await _RequestLocalHotelRepository.GetAll(cancellationToken);
                return _mapper.Map<List<GetAllRequestLocalHotelResponse>>(users);
            }

        }
    }
}
