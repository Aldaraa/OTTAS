using Application.Common.Exceptions;
using Application.Features.ReportJobFeature.BuildReport;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Packaging.Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {
        #region ExecuteData

        public async Task<ExecuteDataResponse> ExecuteData(int jobId)
        {
            var returnData =new ExecuteDataResponse();
            var attachments = new List<string>();
            var currentJob = await GetReportJob(jobId);
            if (currentJob == null)
                return returnData;

            var currentReportTemplate = await GetReportTemplate(currentJob.ReportTemplateId.Value);
            if (currentReportTemplate == null) return returnData;

            var reportColumns = await GetReportColumns(currentJob.Id);
            var reportParameters = await GetReportParameters(currentJob.Id);
            string reportCode = $"{currentJob.Code}";
            string templateName = $"{currentReportTemplate.Description}";
            string reportName = $"{templateName}-{reportCode}";
            string? currentJobSubscriptionMail = currentJob.SubscriptionMail;

            try
            {
                if (currentReportTemplate.Code == "TASREPORT_109")
                {
                    var attachment = await Profilemaster(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName,  reportCode, currentJob.SubscriptionMail);
                }
                else if (currentReportTemplate.Code == "TASREPORT_111")
                {
                    attachments = await TransportManifestQueryModfy(reportColumns, currentJob.Code, reportParameters);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }

                else if (currentReportTemplate.Code == "TASREPORT_112")
                {
                    var attachment = await WorkflowQueryModfy(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment); 
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }
                else if (currentReportTemplate.Code == "TASREPORT_105")
                {
                    var attachment = await GetMobilisationData(reportColumns, currentJob.Code, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_101")
                {
                    var attachment = await AccommodationArrivals(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }
                else if (currentReportTemplate.Code == "TASREPORT_115")
                {
                    var attachment = await AccommodationDepartures(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }
                else if (currentReportTemplate.Code == "TASREPORT_106")
                {
                    var attachment = await OffsiteNoFutureBooking(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }


                else if (currentReportTemplate.Code == "TASREPORT_108")
                {
                    var attachment = await Roster(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_104")
                {
                    var attachment = await ManDays(reportColumns, reportName, reportParameters);
                    if (attachment == string.Empty || string.IsNullOrWhiteSpace(attachment))
                    {
                        await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                    }
                    else {
                        attachments.Add(attachment);
                        await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                    }

                    

                }
                else if (currentReportTemplate.Code == "TASREPORT_102")
                {
                    var attachment = await WorkflowCompletedQueryModfy(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }
                else if (currentReportTemplate.Code == "TASREPORT_110")
                {
                    var attachment = await TransportDetailsQueryModfy(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }

                else if (currentReportTemplate.Code == "TASREPORT_107")
                {
                    var attachment = await RoomOccupancy(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);
                }

                else if (currentReportTemplate.Code == "TASREPORT_113")
                {
                    var attachment = await NonSiteTravelQueryModfy(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_114")
                {
                    var attachment = await NonSiteHotelQueryModfy(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_116")
                {
                    var attachment = await RoomDateUtilization(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_117")
                {
                    var attachment = await TransportDetailsQuerySMS(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_118")
                {
                    var attachment = await FlightUtilization(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_119")
                {
                    var attachment = await SeatBlock(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_120")
                {
                    var attachment = await ProfileAudit(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                else if (currentReportTemplate.Code == "TASREPORT_121")
                {
                    var attachment = await ActionsTrends(reportColumns, reportName, reportParameters);
                    attachments.Add(attachment);
                    await SendReport(jobId, attachments, templateName, reportCode, currentJob.SubscriptionMail);

                }
                //TransportDetailsQuerySMS
                //RoomDateUtilization



                await CreateJobLogSuccess(jobId, currentJob.SubscriptionMail);
            }
            catch (Exception ex)
            {
                await CreateJobLogFailed(jobId, ex.Message);
            }


            returnData.Files = attachments;
            returnData.ReportName = reportName;
            return returnData;
        }

        #endregion



        private async Task<ReportJob> GetReportJob(int jobId)
        {
            return await Context.ReportJob.FirstOrDefaultAsync(x => x.Id == jobId);
        }

        private async Task<ReportTemplate> GetReportTemplate(int templateId)
        {
            return await Context.ReportTemplate.AsNoTracking().FirstOrDefaultAsync(x => x.Id == templateId);
        }



        private async Task<List<ReportCol>> GetReportColumns(int jobId)
        {
            return await (from jobColumn in Context.ReportJobColumn.AsNoTracking().Where(x => x.ReportJobId == jobId)
                          join col in Context.ReportTemplateColumn.AsNoTracking() on jobColumn.ColumnId equals col.Id into ColData
                          from col in ColData.DefaultIfEmpty()
                          select new ReportCol
                          {
                              FieldName = col.FieldName,
                              FieldCaption = col.Caption
                          }).ToListAsync();
        }


        private async Task<List<ReportParam>> GetReportParameters(int jobId)
        {
            return await (from jobParam in Context.ReportJobParameter.AsNoTracking().Where(x => x.ReportJobId == jobId)
                          join param in Context.ReportTemplateParameter.AsNoTracking() on jobParam.ParameterId equals param.Id into ParamData
                          from param in ParamData.DefaultIfEmpty()
                          select new ReportParam
                          {
                              FieldName = param.FieldName,
                              FieldValue = jobParam.ParameterValue,
                              FieldCaption = param.Caption,
                              Days = jobParam.Days
                          }).ToListAsync();
        }


        private EmailReportModel CreateEmailReportModel(string templatename, string code, List<string> attachments, string subscriptionMail)
        {
            return new EmailReportModel
            {
                Attachments = attachments,
                TemplateName = templatename,
                JobCode = code,
                Subject = $"Scheduled Report: - {templatename}-{code} {DateTime.Now}",
                To = subscriptionMail.Split(" ")[0],
                Cc = subscriptionMail.Split(" ").ToList(),
                Body = code,
                ReportDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
            };
        }


        private async Task SendReport(int jobId, List<string> attachments, string templatename, string code, string subscriptionMail)
        {
            var model = CreateEmailReportModel(templatename, code, attachments, subscriptionMail);
            model.JobId = jobId;
            await   SendMailReport(model);
        }


        #region SendMail

        public async Task<int> SendMailReport(EmailReportModel request)
        {

            var mailData = await Context.MailSmtpConfig.FirstOrDefaultAsync();

            if (mailData != null)
            {
                if (string.IsNullOrWhiteSpace(mailData.password))
                {

                    try
                    {
                        var smtpClient = new SmtpClient(mailData.smtpServer)
                        {
                            Port = mailData.smtpPort,
                        };
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress("tas_noreply@ot.mn");
                        mail.To.Add(request.To);

                        mail.IsBodyHtml = true;

                        if (request.Attachments.Count > 0)
                        {
                            mail.Subject = request.Subject;
                            mail.Body = GlobalConstants.BASIC_MAIL_HTML_REPORT_TEMPLATE.Replace("{reporttemplate}", request.TemplateName).Replace("{date}", request.ReportDate).Replace("{ReportJobCode}", request.JobCode);

                            foreach (string ccRecipient in request.Cc)
                            {
                                mail.Bcc.Add(ccRecipient);
                            }

                            foreach (var attachmentItem in request.Attachments)
                            {

                                var item2 = $"{Directory.GetCurrentDirectory()}{attachmentItem}";
                                var item = Path.Combine(Directory.GetCurrentDirectory(), attachmentItem);
                                Attachment attachment = new Attachment(item2);
                                mail.Attachments.Add(attachment);

                            }
                            await smtpClient.SendMailAsync(mail);
                            return 1;


                        }
                        else {
                            mail.Subject = $"No Data Available for {request.TemplateName}";
                            mail.Body = $"The report job '{request
                                .JobCode}' executed on {request.ReportDate} did not return any data.";

                            await smtpClient.SendMailAsync(mail);
                            return 1;

                        }






                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }

                }
                else {


                    string smtpServer = mailData.smtpServer;
                    int smtpPort = mailData.smtpPort; // Use the correct port for your email provider
                    string email = mailData.email;
                    string password = mailData.password;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(email);
                    mail.To.Add(request.To);
                    mail.Subject = request.Subject;



                    if (request.Attachments.Count > 0)
                    {
                        mail.IsBodyHtml = true;
                        mail.Body = GlobalConstants.BASIC_MAIL_HTML_REPORT_TEMPLATE.Replace("{reporttemplate}", request.TemplateName).Replace("{date}", request.ReportDate).Replace("{ReportJobCode}", request.JobCode);
                        foreach (string ccRecipient in request.Cc)
                        {
                            mail.CC.Add(ccRecipient);
                        }


                        foreach (var attachmentItem in request.Attachments)
                        {

                            var item2 = $"{Directory.GetCurrentDirectory()}{attachmentItem}";
                            var item = Path.Combine(Directory.GetCurrentDirectory(), attachmentItem);
                            Attachment attachment = new Attachment(item2);
                            mail.Attachments.Add(attachment);

                        }
                    }
                    else
                    {
                        mail.Subject = $"No Data Available for {request.TemplateName}";
                        mail.Body = $"The report job '{request
                            .JobCode}' executed on {request.ReportDate} did not return any data.";


                    }

                    // mail.Body = request.mailModel.Body;

                    SmtpClient smtpClient = new SmtpClient(smtpServer);
                    smtpClient.Port = smtpPort;
                    smtpClient.Credentials = new NetworkCredential(email, password);
                    smtpClient.EnableSsl = true;

                    ServicePointManager.ServerCertificateValidationCallback =
                        (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                    try
                    {
                        smtpClient.Send(mail);
                        return 1;
                    }
                    catch (Exception ex)
                    {

                        return 0;
                    }
                }
            }
            else
            {
                throw new BadRequestException(" SMTP configuration error");
            }


        }

        #endregion



    }


}
