
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.DeleteReportJob
{

    public sealed class DeleteReportJobHandler : IRequestHandler<DeleteReportJobRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportJobRepository _ReportJobRepository;
        private readonly IMapper _mapper;

        public DeleteReportJobHandler(IReportJobRepository reportJobRepository, IUnitOfWork unitOfWork)
        {
            _ReportJobRepository = reportJobRepository;
            _unitOfWork = unitOfWork;

        }

        public async Task<Unit> Handle(DeleteReportJobRequest request, CancellationToken cancellationToken)
        {
            await _ReportJobRepository.DeleteJobSchedule(request, cancellationToken);
            return Unit.Value;
        }
    }
}
