using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportTemplateFeature.GetReportTemplateData
{

    public sealed class GetReportTemplateDataHandler : IRequestHandler<GetReportTemplateDataRequest, GetReportTemplateDataResponse>
    {
        private readonly IReportTemplateRepository _ReportTemplateRepository;
        private readonly IMapper _mapper;

        public GetReportTemplateDataHandler(IReportTemplateRepository ReportTemplateRepository, IMapper mapper)
        {
            _ReportTemplateRepository = ReportTemplateRepository;
            _mapper = mapper;
        }

        public async Task<GetReportTemplateDataResponse> Handle(GetReportTemplateDataRequest request, CancellationToken cancellationToken)
        {

            var data =  await _ReportTemplateRepository.GetData(request, cancellationToken);
            return data;


        }
    }
}
