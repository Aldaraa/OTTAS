using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportTemplateFeature.GetReportTemplateMaster
{

    public sealed class GetReportTemplateMasterHandler : IRequestHandler<GetReportTemplateMasterRequest, GetReportTemplateMasterResponse>
    {
        private readonly IReportTemplateRepository _ReportTemplateRepository;
        private readonly IMapper _mapper;

        public GetReportTemplateMasterHandler(IReportTemplateRepository ReportTemplateRepository, IMapper mapper)
        {
            _ReportTemplateRepository = ReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<GetReportTemplateMasterResponse> Handle(GetReportTemplateMasterRequest request, CancellationToken cancellationToken)
        {

            var data =  await _ReportTemplateRepository.GetMaster(request, cancellationToken);
            return data;


        }
    }
}
