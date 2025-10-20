using AutoMapper;
using MediatR;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Application.Features.TransportScheduleFeature.GetScheduleBusstop
{
    public sealed class GetScheduleBusstopHandler : IRequestHandler<GetScheduleBusstopRequest, List<GetScheduleBusstopResponse>>
    {
        private readonly ITransportScheduleRepository _TransportScheduleRepository;

        public GetScheduleBusstopHandler(ITransportScheduleRepository TransportScheduleRepository)
        {
            _TransportScheduleRepository = TransportScheduleRepository;
        }

        public async Task<List<GetScheduleBusstopResponse>>Handle(GetScheduleBusstopRequest request, CancellationToken cancellationToken)
        {
           return await _TransportScheduleRepository.GetScheduleBusstop(request, cancellationToken);

        }
    }
}
