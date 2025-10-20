using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportTemplateFeature.GetDashboard
{

    public sealed class GetDashboardHandler : IRequestHandler<GetDashboardRequest, GetDashboardResponse>
    {
        private readonly IReportTemplateRepository _ReportTemplateRepository;
        private readonly IMapper _mapper;

        public GetDashboardHandler(IReportTemplateRepository ReportTemplateRepository, IMapper mapper)
        {
            _ReportTemplateRepository = ReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<GetDashboardResponse> Handle(GetDashboardRequest request, CancellationToken cancellationToken)
        {

            var data =  await _ReportTemplateRepository.GetDashboardData(request, cancellationToken);
            return data;


        }
    }
}
