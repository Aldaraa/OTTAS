using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.RequestAirportFeature.GetAllRequestAirport
{

    public sealed class GetAllRequestAirportHandler : IRequestHandler<GetAllRequestAirportRequest, List<GetAllRequestAirportResponse>>
    {
        private readonly IRequestAirportRepository _RequestAirportRepository;
        private readonly IMapper _mapper;

        public GetAllRequestAirportHandler(IRequestAirportRepository RequestAirportRepository, IMapper mapper)
        {
            _RequestAirportRepository = RequestAirportRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllRequestAirportResponse>> Handle(GetAllRequestAirportRequest request, CancellationToken cancellationToken)
        {
            var returnData = await _RequestAirportRepository.GetAll(request, cancellationToken);
            return returnData;
           

        }
    }
}
