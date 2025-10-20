
using Application.Common.Exceptions;
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.UpdateReportJobRuntime
{

    public sealed class UpdateReportJobRuntimeHandler : IRequestHandler<UpdateReportJobRuntimeRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public UpdateReportJobRuntimeHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReportJobRepository = reportJobRepository;
        }

        public async Task<Unit> Handle(UpdateReportJobRuntimeRequest request, CancellationToken cancellationToken)
        {
            await _ReportJobRepository.UpdateJobScheduleRuntime(request, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            return Unit.Value;
        }
    }
}
