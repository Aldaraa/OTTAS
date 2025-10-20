using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.GetDateDriveTransportSchedule
{
    public sealed class GetDateDriveTransportScheduleHandler : IRequestHandler<GetDateDriveTransportScheduleRequest, GetDateDriveTransportScheduleResponse>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;

        public GetDateDriveTransportScheduleHandler(ITransportScheduleRepository TransportScheduleRepository)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
        }

        public async Task<GetDateDriveTransportScheduleResponse>Handle(GetDateDriveTransportScheduleRequest request, CancellationToken cancellationToken)
        {
           return await _TransportScheduleRepository.GetDateDriveTransportSchedule(request, cancellationToken);
        }
    }
}
