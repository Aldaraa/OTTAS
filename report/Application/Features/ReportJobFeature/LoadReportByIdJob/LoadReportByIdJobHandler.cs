
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportJobFeature.LoadReportByIdJob
{

    public sealed class LoadReportJobHandler : IRequestHandler<LoadReportByIdJobRequest, Unit>
    {
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public LoadReportJobHandler(IReportJobRepository ReportJobRepository, IMapper mapper)
        {
            _ReportJobRepository = ReportJobRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(LoadReportByIdJobRequest request, CancellationToken cancellationToken)
        {
             await _ReportJobRepository.LoadDataById(request);
           return Unit.Value;
        }
    }
}
