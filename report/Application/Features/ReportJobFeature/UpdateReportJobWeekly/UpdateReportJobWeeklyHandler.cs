
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobWeekly
{

    public sealed class UpdateReportJobWeeklyHandler : IRequestHandler<UpdateReportJobWeeklyRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public UpdateReportJobWeeklyHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _ReportJobRepository = reportJobRepository;
            _unitOfWork = unitOfWork;

        }

        public async Task<Unit> Handle(UpdateReportJobWeeklyRequest request, CancellationToken cancellationToken)
        {
            await _ReportJobRepository.UpdateJobScheduleWeekly(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
