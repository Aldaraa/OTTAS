using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.SeatInfoTransportSchedule
{
    public sealed class SeatInfoTransportScheduleHandler : IRequestHandler<SeatInfoTransportScheduleRequest, SeatInfoTransportScheduleResponse>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;

        public SeatInfoTransportScheduleHandler(ITransportScheduleRepository TransportScheduleRepository)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
        }

        public async Task<SeatInfoTransportScheduleResponse>Handle(SeatInfoTransportScheduleRequest request, CancellationToken cancellationToken)
        {
           return await _TransportScheduleRepository.SeatInfoTransportSchedule(request, cancellationToken);
        }
    }
}
