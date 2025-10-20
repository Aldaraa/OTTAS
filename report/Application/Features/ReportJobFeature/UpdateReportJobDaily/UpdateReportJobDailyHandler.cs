
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobDaily
{

    public sealed class UpdateReportJobDailyHandler : IRequestHandler<UpdateReportJobDailyRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public UpdateReportJobDailyHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReportJobRepository = reportJobRepository;
        }

        public async Task<Unit> Handle(UpdateReportJobDailyRequest request, CancellationToken cancellationToken)
        {
            await _ReportJobRepository.UpdateJobScheduleDaily(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
