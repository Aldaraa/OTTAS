using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeTransportFeature.UpdateTransport
{
    public sealed class UpdateTransportHandler : IRequestHandler<UpdateTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISafeModeTransportRepository _safeModeTransportRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public UpdateTransportHandler(IUnitOfWork unitOfWork, ISafeModeTransportRepository safeModeTransportRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _safeModeTransportRepository = safeModeTransportRepository;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<Unit>  Handle(UpdateTransportRequest request, CancellationToken cancellationToken)
        {
            var newScheduleId =  await _safeModeTransportRepository.UpdateTransport(request, cancellationToken);
            if (newScheduleId.HasValue) {
                await _transportScheduleCalculateRepository.CalculateByScheduleId(newScheduleId.Value, cancellationToken);
            }
            await _transportScheduleCalculateRepository.CalculateByScheduleId(request.ScheduleId, cancellationToken);
            return Unit.Value;
        }
    }
}
