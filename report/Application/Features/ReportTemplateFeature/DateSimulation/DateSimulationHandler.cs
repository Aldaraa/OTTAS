using Application.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.ReportTemplateFeature.DateSimulation
{

    public sealed class DateSimulationHandler : IRequestHandler<DateSimulationRequest, DateTime>
    {
        private readonly IReportTemplateRepository _ReportTemplateRepository;

        public DateSimulationHandler(IReportTemplateRepository ReportTemplateRepository)
        {
            _ReportTemplateRepository = ReportTemplateRepository;
        }

        public  async Task<DateTime> Handle(DateSimulationRequest request, CancellationToken cancellationToken)
        {
            var data = await _ReportTemplateRepository.GetDateTypeSimulator(request ,cancellationToken);
            return data;


        }
    }
}
