using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.SearchTransportSchedule
{
    public sealed class SearchTransportScheduleHandler : IRequestHandler<SearchTransportScheduleRequest, List<SearchTransportScheduleResponse>>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;

        public SearchTransportScheduleHandler(ITransportScheduleRepository TransportScheduleRepository)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
        }

        public async Task<List<SearchTransportScheduleResponse>>Handle(SearchTransportScheduleRequest request, CancellationToken cancellationToken)
        {
           return await _TransportScheduleRepository.Search(request, cancellationToken);

        }
    }
}
