using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.CreateScheduleDriveTransport
{ 
    public sealed class CreateDriveTransportHandler : IRequestHandler<CreateScheduleDriveTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportScheduleRepository _TransportScheduleRepository;
        private readonly IMapper _mapper;

        public CreateDriveTransportHandler(IUnitOfWork unitOfWork, ITransportScheduleRepository TransportScheduleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportScheduleRepository = TransportScheduleRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateScheduleDriveTransportRequest request, CancellationToken cancellationToken)
        {
            await _TransportScheduleRepository.CreateScheduleDrive(request);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
