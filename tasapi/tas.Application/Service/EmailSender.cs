using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace tas.Application.Service
{
    public class EmailSender 
    {
        private readonly ILogger<EmailSender> _logger;



        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailRequestDocumentModel model, EmailAuthModel autModel)
        {
            try
            {
               // _logger.LogInformation("MAIL ENTERED FUNCTION===============*********==");
                if (string.IsNullOrWhiteSpace(autModel.email))
                {
                    if (!string.IsNullOrWhiteSpace(model.To))
                    {

                        var smtpClient = new SmtpClient(autModel.smtpServer)
                        {
                            Port = autModel.smtpPort,
                        };
                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress("tas_noreply@ot.mn");
                        mail.To.Add(model.To);
                        mail.Subject = model.Subject;
                        mail.IsBodyHtml = true;
                        mail.Body = model.Body;
                        //if (model.Cc != null)
                        //{
                        //    foreach (string ccRecipient in model.Cc)
                        //    {
                        //        if (ccRecipient != null)
                        //        {
                        //            mail.Bcc.Add(ccRecipient);
                        //        }
                        //    }
                        //}

                        if (model.Attachments.Count > 0)
                        {
                            foreach (var attchmentItem in model.Attachments)
                            {
                                string fullPath = Directory.GetCurrentDirectory() + attchmentItem;

                                if (System.IO.File.Exists(fullPath))
                                {
                                    var item = Path.Combine(fullPath);
                                    Attachment attachment = new Attachment(item);
                                    mail.Attachments.Add(attachment);

                                }
                            }
                        }
                        await smtpClient.SendMailAsync(mail);
                  //      _logger.LogInformation("MAIL SUCCESS SEND ===============*********==");
                    }

                }
                else
                {

                    if (!string.IsNullOrWhiteSpace(model.To))
                    {
                        var smtpClient = new SmtpClient(autModel.smtpServer)
                        {
                            Port = autModel.smtpPort,
                            Credentials = new NetworkCredential(autModel.email, autModel.password),
                            EnableSsl = true,
                        };


                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(model.SenderMail);
                        mail.To.Add(model.To);
                        mail.Subject = model.Subject;
                        mail.IsBodyHtml = true;
                        mail.Body = model.Body;
                        //if (model.Cc != null)
                        //{
                        //    foreach (string ccRecipient in model.Cc)
                        //    {
                        //        if (ccRecipient != null)
                        //        {
                        //            mail.Bcc.Add(ccRecipient);
                        //        }

                        //    }

                        //}`

                        if (model.Attachments.Count > 0)
                        {
                            foreach (var attchmentItem in model.Attachments)
                            {
                                string fullPath = Directory.GetCurrentDirectory() + attchmentItem;

                                if (System.IO.File.Exists(fullPath))
                                {
                                    var item = Path.Combine(fullPath);
                                    Attachment attachment = new Attachment(item);
                                    mail.Attachments.Add(attachment);

                                }
                            }
                        }
                        //mail.To.Add("mdavkharbayar@gmail.com");
                        //mail.To.Add("ch.soyloo.edu@gmail.com");


                        //_logger.LogInformation($"Port ==={autModel.smtpPort}");
                        //_logger.LogInformation($"Email ==={autModel.email}");
                        //_logger.LogInformation($"SmtpServer ==={autModel.smtpServer}");



                        try
                        {
                            await smtpClient.SendMailAsync(mail);
                 //           _logger.LogInformation("MAIL SUCCESS SEND ===============*********==");
                        }
                        catch (Exception ex)
                        {

                      //      _logger.LogWarning("mail send error ::======================" + ex.ToString());

                            throw;
                        }
                    }


                }

            }
            catch (SmtpException ex)
            {
                if (ex.StatusCode == SmtpStatusCode.GeneralFailure && ex.Message.Contains("does not support secure connections"))
                {
          //          _logger.LogError(ex.ToString() + "------------ SSL not supported, return false or handle accordingly");

                }
                else
                {

                    _logger.LogError("Email sent successfully." + ex.ToString());
                }
            }
            catch (Exception ex)
            {

            //    _logger.LogError("Email sent unsuccessfully." + ex.ToString());



            }
        }

    }


    public class EmailRequestDocumentModel
    {
        public string? To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public string SenderMail { get; set; }

        public List<string> Cc { get; set; }

        public List<string> Attachments { get; set; }
    }


    public class EmailAuthModel
    {
        public string smtpServer { get; set; }
        public int smtpPort { get; set; }

        public string? email { get; set; }
        public string? password { get; set; }
    }


}
