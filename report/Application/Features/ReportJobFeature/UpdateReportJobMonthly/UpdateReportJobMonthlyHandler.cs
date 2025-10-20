
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobMonthly
{

    public sealed class UpdateReportJobMonthlyHandler : IRequestHandler<UpdateReportJobMonthlyRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public UpdateReportJobMonthlyHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _ReportJobRepository = reportJobRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateReportJobMonthlyRequest request, CancellationToken cancellationToken)
        {
            await _ReportJobRepository.UpdateJobScheduleMonthly(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
