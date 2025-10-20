using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeTransportFeature.CreateTransport
{
    public sealed class CreateTransportHandler : IRequestHandler<CreateTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISafeModeTransportRepository _safeModeTransportRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public CreateTransportHandler(IUnitOfWork unitOfWork, ISafeModeTransportRepository safeModeTransportRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _safeModeTransportRepository = safeModeTransportRepository;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<Unit>  Handle(CreateTransportRequest request, CancellationToken cancellationToken)
        {
            await _safeModeTransportRepository.CreateTransport(request, cancellationToken);
            await _transportScheduleCalculateRepository.CalculateByScheduleId(request.ScheduleId, cancellationToken);
            return Unit.Value;
        }
    }
}
