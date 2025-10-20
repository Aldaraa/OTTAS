
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportJobFeature.GetReportJob
{

    public sealed class GetReportJobHandler : IRequestHandler<GetReportJobRequest, GetReportJobResponse>
    {
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public GetReportJobHandler(IReportJobRepository ReportJobRepository, IMapper mapper)
        {
            _ReportJobRepository = ReportJobRepository;
            _mapper = mapper;
        }

        public async Task<GetReportJobResponse> Handle(GetReportJobRequest request, CancellationToken cancellationToken)
        {

            return await _ReportJobRepository.GetJobData(request, cancellationToken);


        }
    }
}
