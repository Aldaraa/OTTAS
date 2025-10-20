
using Application.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ReportJobFeature.TestReportJob
{

    public sealed class TestReportJobHandler : IRequestHandler<TestReportJobRequest, TestReportJobResponse>
    {
        private readonly IJobExecuteServiceRepository _JobExecuteServiceRepository;
        public TestReportJobHandler(IJobExecuteServiceRepository jobExecuteServiceRepository)
        {
            _JobExecuteServiceRepository = jobExecuteServiceRepository;
        }

        public async Task<TestReportJobResponse> Handle(TestReportJobRequest request, CancellationToken cancellationToken)
        {
           var returnData = await _JobExecuteServiceRepository.TestJobSchedule(request, cancellationToken);
            return returnData;
        }
    }
}
