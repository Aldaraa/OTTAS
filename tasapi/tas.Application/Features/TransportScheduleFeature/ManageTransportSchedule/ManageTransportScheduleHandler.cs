using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.ManageTransportSchedule
{
    public sealed class ManageTransportScheduleHandler : IRequestHandler<ManageTransportScheduleRequest, ManageTransportScheduleResponse>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;

        public ManageTransportScheduleHandler(ITransportScheduleRepository TransportScheduleRepository)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
        }

        public async Task<ManageTransportScheduleResponse>Handle(ManageTransportScheduleRequest request, CancellationToken cancellationToken)
        {
           return await _TransportScheduleRepository.ManageTransportSchedule(request, cancellationToken);
        }
    }
}
