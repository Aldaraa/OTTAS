using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportFeature.AddExternalTravel
{
    public sealed class AddExternalTravelHandler : IRequestHandler<AddExternalTravelRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportRepository _TransportRepository;
        private readonly IMapper _mapper;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;

        public AddExternalTravelHandler(IUnitOfWork unitOfWork, ITransportRepository TransportRepository, IMapper mapper, ITransportCheckerRepository transportCheckerRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _TransportRepository = TransportRepository;
            _mapper = mapper;
            _transportCheckerRepository = transportCheckerRepository;   
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<Unit> Handle(AddExternalTravelRequest request, CancellationToken cancellationToken)
        {
            await _transportCheckerRepository.TransportExternalAddValidCheck(request.EmployeeId, request.FirstSheduleId, request.LastSheduleId);
           await _TransportRepository.AddExternalTravel(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _transportScheduleCalculateRepository.CalculateByScheduleId(request.FirstSheduleId, cancellationToken);
            if (request.LastSheduleId.HasValue) { 
                await _transportScheduleCalculateRepository.CalculateByScheduleId(request.LastSheduleId.Value, cancellationToken);
            }


            return Unit.Value;
        }
    }
}
