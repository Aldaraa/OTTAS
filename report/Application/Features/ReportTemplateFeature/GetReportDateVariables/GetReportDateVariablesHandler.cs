using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportTemplateFeature.GetReportDateVariables
{

    public sealed class GetReportDateVariablesHandler : IRequestHandler<GetReportDateVariablesRequest, GetReportDateVariablesResponse>
    {
        private readonly IReportTemplateRepository _ReportTemplateRepository;
        private readonly IMapper _mapper;

        public GetReportDateVariablesHandler(IReportTemplateRepository ReportTemplateRepository, IMapper mapper)
        {
            _ReportTemplateRepository = ReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<GetReportDateVariablesResponse> Handle(GetReportDateVariablesRequest request, CancellationToken cancellationToken)
        {

            var data =  await _ReportTemplateRepository.GetDateTypes(request, cancellationToken);
            return data;


        }
    }
}
