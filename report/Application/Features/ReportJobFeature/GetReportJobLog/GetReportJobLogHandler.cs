
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportJobFeature.GetReportJobLog
{

    public sealed class GetReportJobLogHandler : IRequestHandler<GetReportJobLogRequest, List<GetReportJobLogResponse>>
    {
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public GetReportJobLogHandler(IReportJobRepository ReportJobRepository, IMapper mapper)
        {
            _ReportJobRepository = ReportJobRepository;
            _mapper = mapper;
        }

        public async Task<List<GetReportJobLogResponse>> Handle(GetReportJobLogRequest request, CancellationToken cancellationToken)
        {

            return await _ReportJobRepository.GetJobLogData(request, cancellationToken);


        }
    }
}
