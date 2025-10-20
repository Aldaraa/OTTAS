using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportTemplateFeature.GetAllReportTemplate
{

    public sealed class GetAllReportTemplateHandler : IRequestHandler<GetAllReportTemplateRequest, List<GetAllReportTemplateResponse>>
    {
        private readonly IReportTemplateRepository _ReportTemplateRepository;
        private readonly IMapper _mapper;

        public GetAllReportTemplateHandler(IReportTemplateRepository ReportTemplateRepository, IMapper mapper)
        {
            _ReportTemplateRepository = ReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<List<GetAllReportTemplateResponse>> Handle(GetAllReportTemplateRequest request, CancellationToken cancellationToken)
        {

            return await _ReportTemplateRepository.GetAllData(request, cancellationToken);


        }
    }
}
