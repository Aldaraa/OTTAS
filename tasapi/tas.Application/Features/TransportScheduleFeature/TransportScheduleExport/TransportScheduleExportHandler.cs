using AutoMapper;
using MediatR;
using tas.Application.Repositories;

namespace tas.Application.Features.TransportScheduleFeature.TransportScheduleExport
{

    public sealed class TransportScheduleExportHandler : IRequestHandler<TransportScheduleExportRequest, TransportScheduleExportResponse>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;
        private readonly IMapper _mapper;

        public TransportScheduleExportHandler(ITransportScheduleRepository TransportScheduleRepository, IMapper mapper)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
            _mapper = mapper;
        }

        public async Task<TransportScheduleExportResponse> Handle(TransportScheduleExportRequest request, CancellationToken cancellationToken)
        {
            return await _TransportScheduleRepository.TransportScheduleExport(request, cancellationToken);

        }
    }
}
