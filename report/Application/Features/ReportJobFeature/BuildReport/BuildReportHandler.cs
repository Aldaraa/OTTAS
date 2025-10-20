
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.BuildReport
{

    public sealed class BuildReportHandler : IRequestHandler<BuildReportRequest, BuildReportResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobExecuteServiceRepository _jobExecuteServiceRepository;
        private readonly IMapper _mapper;

        public BuildReportHandler(IJobExecuteServiceRepository jobExecuteServiceRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _jobExecuteServiceRepository = jobExecuteServiceRepository;
        }

        public async Task<BuildReportResponse> Handle(BuildReportRequest request, CancellationToken cancellationToken)
        {
           return await _jobExecuteServiceRepository.CreateBuildCommand(request, cancellationToken);
      
        }
    }
}
