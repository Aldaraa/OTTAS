using Application.Features.ReportTemplateFeature.GetAllReportTemplate;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Microsoft.AspNetCore.Hosting;
using Application.Features.ReportJobFeature.CreateJob;
using Persistence.HostedService;
using Application.Common.Exceptions;

namespace Persistence.Repositories
{

    public class MailSmtpConfigRepository : BaseRepository<MailSmtpConfig>, IMailSmtpConfigRepository
    {
        IWebHostEnvironment _env;

        public MailSmtpConfigRepository(DataContext context, IConfiguration configuration, IWebHostEnvironment env) : base(context, configuration)
        {
            _env = env;
        }

        public async Task<int> SendMail(SendMailRequest request, CancellationToken cancellationToken)
        {
            var mailData = await Context.MailSmtpConfig.AsNoTracking().FirstOrDefaultAsync();
            if (mailData != null)
            {
                if (string.IsNullOrWhiteSpace(mailData.password))
                {
                    //corprate enviroment

                    var smtpClient = new SmtpClient(mailData.smtpServer)
                    {
                        Port = mailData.smtpPort,
                    };
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(mailData.email);
                    mail.To.Add(request.mailModel.To);
                    mail.Subject = request.mailModel.Subject;
                    mail.IsBodyHtml = true;

                    mail.Body = GlobalConstants.BASIC_MAIL_HTML_TEMPLATE.Replace("{value}", request.mailModel.Subject);
                    foreach (string ccRecipient in request.mailModel.Cc)
                    {
                        mail.CC.Add(ccRecipient);
                    }


                    foreach (var attachmentItem in request.mailModel.Attachments)
                    {

                        var item2 = $"{Directory.GetCurrentDirectory()}{attachmentItem}";
                        var item = Path.Combine(Directory.GetCurrentDirectory(), attachmentItem);
                        Attachment attachment = new Attachment(item2);
                        mail.Attachments.Add(attachment);

                    }

                    ServicePointManager.ServerCertificateValidationCallback =
                        (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                    try
                    {
                        smtpClient.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        var error = ex.ToString();

                    }


                    return 0;

                }
                else {

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(mailData.email);
                    mail.To.Add(request.mailModel.To);
                    mail.Subject = request.mailModel.Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = GlobalConstants.BASIC_MAIL_HTML_TEMPLATE.Replace("{value}", request.mailModel.Subject);
                    foreach (string ccRecipient in request.mailModel.Cc)
                    {
                        mail.CC.Add(ccRecipient);
                    }


                    foreach (var attachmentItem in request.mailModel.Attachments)
                    {

                        var item2 = $"{Directory.GetCurrentDirectory()}{attachmentItem}";
                        var item = Path.Combine(Directory.GetCurrentDirectory(), attachmentItem);
                        Attachment attachment = new Attachment(item2);
                        mail.Attachments.Add(attachment);

                    }
                    SmtpClient smtpClient = new SmtpClient(mailData.smtpServer);
                    smtpClient.Port = mailData.smtpPort;
                    smtpClient.Credentials = new NetworkCredential(mailData.email, mailData.password);
                    smtpClient.EnableSsl = true;

                    ServicePointManager.ServerCertificateValidationCallback =
                        (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                    try
                    {
                        smtpClient.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        var error = ex.ToString();

                    }


                    return 0;
                }

            }
            else {
                throw new BadRequestException("Please register mail configuration");
            }

        }









        public async Task<int> CreateTestJob(CreateJobRequest request, CancellationToken cancellationToken)
        {


            return 0;
        }
    }
}
