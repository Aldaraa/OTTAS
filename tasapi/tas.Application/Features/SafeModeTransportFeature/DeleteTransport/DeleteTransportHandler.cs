using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.SafeModeTransportFeature.DeleteTransport
{
    public sealed class DeleteTransportHandler : IRequestHandler<DeleteTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISafeModeTransportRepository _safeModeTransportRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public DeleteTransportHandler(IUnitOfWork unitOfWork, ISafeModeTransportRepository safeModeTransportRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _safeModeTransportRepository = safeModeTransportRepository;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<Unit> Handle(DeleteTransportRequest request, CancellationToken cancellationToken)
        {
           var currentScheduleId =  await _safeModeTransportRepository.DeleteTransport(request, cancellationToken);
            if (currentScheduleId.HasValue) {
                await _transportScheduleCalculateRepository.CalculateByScheduleId(currentScheduleId.Value, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
