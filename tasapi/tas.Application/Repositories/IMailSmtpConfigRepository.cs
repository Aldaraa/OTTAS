using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.MailSmtpConfigFeature.CreateMailSmtpConfig;
using tas.Application.Features.MailSmtpConfigFeature.GetMailSmtpConfig;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestNonsiteDocument;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IMailSmtpConfigRepository : IBaseRepository<MailSmtpConfig>
    {
        Task CreateData(CreateMailSmtpConfigRequest request, CancellationToken cancellationToken);

        Task<GetMailSmtpConfigResponse> GetData(GetMailSmtpConfigRequest request, CancellationToken cancellationToken);

        Task SendMailRequestDocumentNotification(SendMailRequestDocumentRequest request, CancellationToken cancellationToken);

        Task SendMailRequestDocumentLineManageNotification(int documentId, int AssignedEmployeeId, int ApprovedEmployeeId, CancellationToken cancellationToken);


        Task SendMailNonSiteDocumentNotification(SendMailRequestNonsiteDocumentRequest request,  CancellationToken cancellationToken);





    }
}
