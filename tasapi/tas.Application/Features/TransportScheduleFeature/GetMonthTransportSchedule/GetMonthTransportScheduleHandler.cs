using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.GetMonthTransportSchedule
{
    public sealed class GetMonthTransportScheduleHandler : IRequestHandler<GetMonthTransportScheduleRequest, List<GetMonthTransportScheduleResponse>>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;

        public GetMonthTransportScheduleHandler(ITransportScheduleRepository TransportScheduleRepository)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
        }

        public async Task<List<GetMonthTransportScheduleResponse>>Handle(GetMonthTransportScheduleRequest request, CancellationToken cancellationToken)
        {
           return await _TransportScheduleRepository.GetMonthTransportSchedule(request, cancellationToken);

        }
    }
}
