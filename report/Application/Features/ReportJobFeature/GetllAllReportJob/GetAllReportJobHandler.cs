
using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportJobFeature.GetAllReportJob
{

    public sealed class GetAllReportJobHandler : IRequestHandler<GetAllReportJobRequest, List<GetAllReportJobResponse>>
    {
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public GetAllReportJobHandler(IReportJobRepository ReportJobRepository, IMapper mapper)
        {
            _ReportJobRepository = ReportJobRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllReportJobResponse>> Handle(GetAllReportJobRequest request, CancellationToken cancellationToken)
        {

            return await _ReportJobRepository.GetAllData(request, cancellationToken);


        }
    }
}
