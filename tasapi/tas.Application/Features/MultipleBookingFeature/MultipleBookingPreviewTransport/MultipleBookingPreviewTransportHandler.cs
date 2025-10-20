using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.MultipleBookingFeature.MultipleBookingPreviewTransport
{
    public sealed class MultipleBookingPreviewTransportHandler : IRequestHandler<MultipleBookingPreviewTransportRequest, List<MultipleBookingPreviewTransportResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransportRepository _TransportRepository;
        private readonly IMapper _mapper;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;


        public MultipleBookingPreviewTransportHandler(IUnitOfWork unitOfWork, ITransportRepository TransportRepository, IMapper mapper, ITransportCheckerRepository transportCheckerRepository, ITransportScheduleCalculateRepository transportScheduleCalculateRepository)
        {
            _unitOfWork = unitOfWork;
            _TransportRepository = TransportRepository;
            _mapper = mapper;
            _transportCheckerRepository = transportCheckerRepository;   
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
        }

        public async Task<List<MultipleBookingPreviewTransportResponse>>  Handle(MultipleBookingPreviewTransportRequest request, CancellationToken cancellationToken)
        {

            var data = await _TransportRepository.MultipleBookingPreviewTransport(request, cancellationToken);
            return data;
        }
    }
}
