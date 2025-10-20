
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportJobFeature.KillSession
{

    public sealed class KillSessionHandler : IRequestHandler<KillSessionRequest, Unit>
    {
        private readonly IJobExecuteServiceRepository _JobExecuteServiceRepository;
        private readonly IMapper _mapper;

        public KillSessionHandler(IJobExecuteServiceRepository JobExecuteServiceRepository, IMapper mapper)
        {
            _JobExecuteServiceRepository = JobExecuteServiceRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(KillSessionRequest request, CancellationToken cancellationToken)
        {

             await _JobExecuteServiceRepository.KillSessionForce(request, cancellationToken);
            return Unit.Value;

        }
    }
}
