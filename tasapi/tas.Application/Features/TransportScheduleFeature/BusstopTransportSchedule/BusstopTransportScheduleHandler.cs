using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.BusstopTransportSchedule
{
    public sealed class BusstopTransportScheduleHandler : IRequestHandler<BusstopTransportScheduleRequest, BusstopTransportScheduleResponse>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;

        public BusstopTransportScheduleHandler(ITransportScheduleRepository TransportScheduleRepository)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
        }

        public async Task<BusstopTransportScheduleResponse>Handle(BusstopTransportScheduleRequest request, CancellationToken cancellationToken)
        {
           return await _TransportScheduleRepository.BusstopTransportSchedule(request, cancellationToken);
        }
    }
}
