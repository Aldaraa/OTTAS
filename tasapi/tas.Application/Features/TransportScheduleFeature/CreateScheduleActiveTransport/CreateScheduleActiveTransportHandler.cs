using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.CreateScheduleActiveTransport
{ 
    public sealed class CreateActiveTransportHandler : IRequestHandler<CreateScheduleActiveTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportScheduleRepository _TransportScheduleRepository;
        private readonly IMapper _mapper;

        public CreateActiveTransportHandler(IUnitOfWork unitOfWork, ITransportScheduleRepository TransportScheduleRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _TransportScheduleRepository = TransportScheduleRepository;
            _mapper = mapper;
        }

        public async Task<Unit>  Handle(CreateScheduleActiveTransportRequest request, CancellationToken cancellationToken)
        {
            await _TransportScheduleRepository.CreateSchedule(request);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;

        }
    }
}
