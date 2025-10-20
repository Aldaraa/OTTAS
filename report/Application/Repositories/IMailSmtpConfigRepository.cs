using Application.Features.ReportJobFeature.CreateJob;
using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{


    public interface IMailSmtpConfigRepository : IBaseRepository<MailSmtpConfig>
    {
        Task<int> SendMail(SendMailRequest request, CancellationToken cancellationToken);


        Task<int> CreateTestJob(CreateJobRequest request, CancellationToken cancellationToken);
    }
}
