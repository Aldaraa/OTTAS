using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.MultipleBookingFeature.MultipleBookingAddTransport
{
    public sealed class MultipleBookingAddTransportHandler : IRequestHandler<MultipleBookingAddTransportRequest, List<MultipleBookingAddTransportResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportRepository _TransportRepository;
        private readonly IMapper _mapper;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public MultipleBookingAddTransportHandler(IUnitOfWork unitOfWork, ITransportRepository TransportRepository, IMapper mapper, ITransportCheckerRepository transportCheckerRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _TransportRepository = TransportRepository;
            _mapper = mapper;
            _transportCheckerRepository = transportCheckerRepository;   
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<List<MultipleBookingAddTransportResponse>>  Handle(MultipleBookingAddTransportRequest request, CancellationToken cancellationToken)
        {
            // await _transportCheckerRepository.TransportAddValidDirectionSequenceCheck(request.EmployeeId, request.inScheduleId, request.outScheduleId);
            // await _TransportRepository.ValidateAddTravel(request, cancellationToken);
            //await _TransportRepository.AddTravel(request, cancellationToken);

            var data = await _TransportRepository.MultipleBookingAddTransport(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            await _transportScheduleCalculateRepository.CalculateByScheduleId(request.firsScheduleId, cancellationToken);
            if (request.lastSheduleId.HasValue)
            {
                await _transportScheduleCalculateRepository.CalculateByScheduleId(request.lastSheduleId.Value, cancellationToken);
            }


            return data;
        }
    }
}
