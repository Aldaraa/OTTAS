
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportJobFeature.LoadReportJob
{

    public sealed class LoadReportJobHandler : IRequestHandler<LoadReportJobRequest, Unit>
    {
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public LoadReportJobHandler(IReportJobRepository ReportJobRepository, IMapper mapper)
        {
            _ReportJobRepository = ReportJobRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(LoadReportJobRequest request, CancellationToken cancellationToken)
        {
             await _ReportJobRepository.LoadData(request);
           return Unit.Value;
        }
    }
}
