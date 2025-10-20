using AutoMapper;
using MediatR;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.RequestAirportFeature.SearchRequestAirport
{
    public sealed class SearchRequestAirportHandler : IRequestHandler<SearchRequestAirportRequest, List<SearchRequestAirportResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestAirportRepository _RequestAirportRepository;
        private readonly IMapper _mapper;

        public SearchRequestAirportHandler(IUnitOfWork unitOfWork, IRequestAirportRepository RequestAirportRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _RequestAirportRepository = RequestAirportRepository;
            _mapper = mapper;
        }

        public async Task<List<SearchRequestAirportResponse>> Handle(SearchRequestAirportRequest request, CancellationToken cancellationToken)
        {
            var returnData = await _RequestAirportRepository.SearchData(request, cancellationToken);
            return returnData;


        }
    }
}
