using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportFeature.AddTravelTransport
{
    public sealed class AddTravelTransportHandler : IRequestHandler<AddTravelTransportRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportRepository _TransportRepository;
        private readonly IMapper _mapper;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public AddTravelTransportHandler(IUnitOfWork unitOfWork, ITransportRepository TransportRepository, IMapper mapper, ITransportCheckerRepository transportCheckerRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _TransportRepository = TransportRepository;
            _mapper = mapper;
            _transportCheckerRepository = transportCheckerRepository;   
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<Unit>  Handle(AddTravelTransportRequest request, CancellationToken cancellationToken)
        {
            await _transportCheckerRepository.TransportAddValidDirectionSequenceCheck(request.EmployeeId, request.inScheduleId, request.outScheduleId);
            await _TransportRepository.ValidateAddTravel(request, cancellationToken);
           await _TransportRepository.AddTravel(request, cancellationToken);
            await _transportScheduleCalculateRepository.CalculateByScheduleId(request.inScheduleId, cancellationToken);
            await _transportScheduleCalculateRepository.CalculateByScheduleId(request.outScheduleId, cancellationToken);


            return Unit.Value;
        }
    }
}
