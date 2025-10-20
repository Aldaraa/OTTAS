using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.MailSmtpConfigFeature.CreateMailSmtpConfig;
using tas.Application.Features.MailSmtpConfigFeature.GetMailSmtpConfig;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;
using tas.Application.Common.Exceptions;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;
using tas.Domain.Enums;
using MediatR;
using tas.Application.Features.RequestDocumentFeature.GetDocumentList;
using tas.Application.Utils;
using Microsoft.AspNetCore.Html;
using OfficeOpenXml.Drawing;
using tas.Application.Service;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using tas.Application.Features.RequestDocumentAttachmentFeature.DownloadRequestDocumentAttachment;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestNonsiteDocument;

namespace tas.Persistence.Repositories
{
    public partial class MailSmtpConfigRepository : BaseRepository<MailSmtpConfig>, IMailSmtpConfigRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _userRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly EmailSender _emailSender;
        private readonly ILogger<MailSmtpConfigRepository> _logger;
        private readonly SignalrHub _signalrhub;
        private readonly IRequestDocumentAttachmentRepository _requestDocumentAttachmentRepository;



        public MailSmtpConfigRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, IMemoryCache memoryCache, EmailSender emailSender,
            IRequestDocumentAttachmentRepository requestDocumentAttachmentRepository,
            ILogger<MailSmtpConfigRepository> logger, SignalrHub signalrhub) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _userRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
            _emailSender = emailSender;
            _logger = logger;
            _signalrhub = signalrhub;
            _requestDocumentAttachmentRepository = requestDocumentAttachmentRepository;
            
        }



        public async Task CreateData(CreateMailSmtpConfigRequest request, CancellationToken cancellationToken)
        {
            var data = await Context.MailSmtpConfig.FirstOrDefaultAsync();
            if (data != null)
            {
                data.email = request.email;
                data.smtpServer = request.smtpServer;
                data.password = request.password;
                data.smtpPort = request.smtpPort;

                bool isValidSmtp = TestSmtpConfiguration(request.email, request.smtpServer, request.password, request.smtpPort);

                if (isValidSmtp)
                {

                    data.DateUpdated = DateTime.Now;
                    data.UserIdUpdated = _userRepository.LogCurrentUser()?.Id;
                    Context.MailSmtpConfig.Update(data);
                }



            }
            else
            {

                var newRecord = new MailSmtpConfig();
                newRecord.email = request.email;
                newRecord.smtpServer = request.smtpServer;
                newRecord.password = request.password;
                newRecord.smtpPort = request.smtpPort;

                newRecord.Active = 1;
                newRecord.DateCreated = DateTime.Now;
                newRecord.UserIdCreated = _userRepository.LogCurrentUser()?.Id;



                Context.MailSmtpConfig.Add(newRecord);

                bool isValidSmtp = TestSmtpConfiguration(request.email, request.smtpServer, request.password, request.smtpPort);

                if (isValidSmtp)
                {
                    Context.MailSmtpConfig.Add(newRecord);
                }


            }
        }
        private bool TestSmtpConfiguration(string? email, string smtpServer, string? password, int smtpPort)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(email))
                {
                    var smtpClient = new SmtpClient(smtpServer)
                    {
                        Port = smtpPort
                    };
                    // Sending a test email to the same address as a simple validation
                    smtpClient.Send("mdavkharbayar@riotinto.com", "mdavkharbayar@gmail.com", "Test Email", "This is a test email for SMTP configuration validation.");

                    return true;
                }
                else {
                    var smtpClient = new SmtpClient(smtpServer)
                    {
                        Port = smtpPort,
                        Credentials = new NetworkCredential(email, password),
                        EnableSsl = true,
                    };
                    smtpClient.Send(email, email, "Test Email", "This is a test email for SMTP configuration validation.");

                    return true;
                }
               
            }
            catch (SmtpException ex)
            {
                if (ex.StatusCode == SmtpStatusCode.GeneralFailure && ex.Message.Contains("does not support secure connections"))
                {
                    throw new BadRequestException(ex.ToString() + "------------ SSL not supported, return false or handle accordingly");
                }
                else
                {
                    throw new BadRequestException(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.ToString());
            }
        }
        public async Task<GetMailSmtpConfigResponse> GetData(GetMailSmtpConfigRequest request, CancellationToken cancellationToken)
        {
            var data = await Context.MailSmtpConfig.FirstOrDefaultAsync();
            var returnData = new GetMailSmtpConfigResponse();

            if (data != null)
            {
                returnData.email = data.email;
                returnData.smtpServer = data.smtpServer;
                returnData.smtpPort = data.smtpPort;
                returnData.password = data.password;
                
                returnData.Id = data.Id;

            }

            return returnData;



        }



        #region RequestDocumentSendNotifcation



        public async Task SendMailRequestDocumentLineManageNotification(int documentId, int AssignedEmployeeId, int ApprovedEmployeeId,  CancellationToken cancellationToken)
        {

            var currentDocument = await (from document in Context.RequestDocument.AsNoTracking().Where(c => c.Id == documentId)
                                         join requestedEmployee in Context.Employee.AsNoTracking() on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                                         from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                                         join documentEmployee in Context.Employee on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                                         from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                                         select new GetDocumentMailInfo
                                         {
                                             Id = document.Id,
                                             DocumentTag = document.DocumentTag,
                                             MailDescription = document.MailDescription,
                                             DocumentType = document.DocumentType,
                                             RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname} ",
                                             RequestEmployeeId = document.UserIdCreated,
                                             RequestEmail = $"{requestedEmployee.Email}",
                                             Mobile = requestedEmployee.Mobile,
                                             RequestedDate = document.DateCreated,
                                             EmployeeFullName = $"{documentEmployee.Firstname} {documentEmployee.Lastname}",
                                             CurrentStatus = document.CurrentAction,
                                             CreatedDate = document.DateCreated,
                                             AssignedEmployeeId = document.AssignedEmployeeId,

                                             AssignedRouteConfigId = document.AssignedRouteConfigId
                                         }).FirstOrDefaultAsync();


            var attachments = new List<string>();

            var attachmentResponse = await _requestDocumentAttachmentRepository.DownloadRequestDocumentAttachment(new DownloadRequestDocumentAttachmentRequest(documentId), cancellationToken);

            if (attachmentResponse != null)
            {
                if (attachmentResponse.Attachments.Count > 0)
                {
                    attachments = attachmentResponse.Attachments;
                }
            }

            var assignedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == AssignedEmployeeId).FirstOrDefaultAsync();
            var approvedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == ApprovedEmployeeId).FirstOrDefaultAsync();
            if (assignedEmployee == null || approvedEmployee == null) {
                return;
            }

            var userId = _userRepository.LogCurrentUser()?.Id;
            if (currentDocument != null)
            {
                string frontDomain = _configuration.GetSection("AppSettings:Fronturl").Value;
                string LinkURL = string.Empty;
                var emails = new List<string?>();
                var linkUrl = await GenerateLinkUrl(currentDocument.Id);


                //var currentDoc = await GetDocumentDescription(currentDocument);

                var UpdatedInfo = await (from history in Context.RequestDocumentHistory.AsNoTracking().Where(x => x.DocumentId == currentDocument.Id)
                                         join employee in Context.Employee.AsNoTracking() on history.UserIdCreated equals employee.Id into employeeData
                                         from employee in employeeData.DefaultIfEmpty()
                                         select new
                                         {
                                             FirstName = employee.Firstname,
                                             LastName = employee.Lastname,
                                             ActionDate = history.DateCreated,
                                             Comment = history.Comment,


                                         }).OrderByDescending(x => x.ActionDate).ToListAsync();

                currentDocument.UpdatedInfo = "<ul>";
                foreach (var item in UpdatedInfo)
                {
                    currentDocument.UpdatedInfo += $"<li> By {item.FirstName} {item.LastName} at {item.ActionDate.Value.ToString("dd/MM/yyyy h:mm:ss tt")} {item.Comment} </li>";

                }
                currentDocument.UpdatedInfo += "</ul>";

                var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);


                if (currentEmployee != null)
                {
                    if (linkUrl != null)
                    {

                        var templateService = new EmailTemplateService();
                        var shtmlString = templateService.LoadTemplate("RequestDocument.html");
                        if (!string.IsNullOrWhiteSpace(shtmlString))
                        {
                            // otworkflow_noreply@ot.mn



                            var requestModel = new EmailRequestDocumentModel();
                 
                         //     requestModel.Subject = $"Approved {currentDocument.DocumentType}#{currentDocument.Id} >> {currentDocument.EmployeeFullName}";

                            requestModel.Subject = $"Info {currentDocument.CurrentStatus} {currentDocument.DocumentType}#{currentDocument.Id} >> {currentDocument.EmployeeFullName}";


                            shtmlString = shtmlString.Replace("{url}", $"{frontDomain}{linkUrl}");

                            string? inTitle = $"Request : On belhalf of {assignedEmployee?.Firstname} {assignedEmployee?.Lastname} <br> approved by {approvedEmployee?.Firstname} {approvedEmployee?.Lastname} ";
                            string? docNumber = $"{currentDocument.Id}";
                            string? descr = currentDocument.MailDescription;
                            string? requester = currentDocument.RequesterFullName;
                            string? requesterEmail = currentDocument.RequestEmail;
                            string? RequestMobile = currentDocument.Mobile;

                            if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                            {
                                var selectedOption = await Context.RequestNonSiteTravelOption.AsNoTracking().Where(c => c.DocumentId == currentDocument.Id && c.Selected == 1).FirstOrDefaultAsync();
                                descr += "</br>";
                                descr += $"<pre>{selectedOption?.OptionData} </pre>";

                            }
                            string? document = $"{currentDocument.DocumentType} {currentDocument.DocumentTag}  {currentDocument.EmployeeFullName}";
                            string? currentAction = currentDocument.CurrentStatus;
                            string? comments = currentDocument.UpdatedInfo;

                            string notifdescr = $"{descr} by  {currentEmployee.Firstname}{currentEmployee.Lastname}";
                            shtmlString = shtmlString.Replace("{inTitle}", $"<b>{inTitle}</b>");
                            shtmlString = shtmlString.Replace("{docNumber}", $"<b>{docNumber}</b>");
                            shtmlString = shtmlString.Replace("{descr}", $"<b>{descr}</b>");
                            shtmlString = shtmlString.Replace("{requester}", $"<b>{requester}</b> <br>" +
                                $" <a href=mailto:{requesterEmail}?subject=Request#{docNumber}&body={currentEmployee.Firstname}> {requesterEmail} </a> <br>" +
                                $"(P) <a href=\"tel:{RequestMobile}\">{RequestMobile}</a> ");
                            shtmlString = shtmlString.Replace("{document}", $"<b>{document}</b>");
                            shtmlString = shtmlString.Replace("{currentAction}", $"<b>{currentAction}</b>");
                            shtmlString = shtmlString.Replace("{comments}", $"{comments}");
                            requestModel.Attachments = attachments;


                            requestModel.Body = shtmlString;

                            requestModel.To = assignedEmployee.Email;
                            

                            EmailAuthModel? rmu = null;
                            if (_memoryCache.TryGetValue($"TAS_MAIL::SMTP", out rmu))
                            {
                                requestModel.SenderMail = rmu.email;
                                await _emailSender.SendEmailAsync(requestModel, rmu);

                            }
                            else
                            {

                                var data = await Context.MailSmtpConfig.AsNoTracking().FirstOrDefaultAsync();
                                if (data != null)
                                {
                                    rmu = new EmailAuthModel();
                                    rmu.email = data.email;
                                    rmu.password = data.password;
                                    rmu.smtpServer = data.smtpServer;
                                    rmu.smtpPort = data.smtpPort;
                                    _memoryCache.Set($"TAS_MAIL::SMTP", rmu, TimeSpan.FromDays(30));
                                    requestModel.SenderMail = data.email;
                                    await _emailSender.SendEmailAsync(requestModel, rmu);
                                }
                                else
                                {
                                    throw new BadRequestException("Please register smtp config");

                                }

                            }

                        }


                    }
                }
                else
                {
                    _logger.LogWarning("CurrentEmployee not found =======================================");
                }
            }
            else
            {
                throw new BadRequestException("DocumentNotfound");
            }
        }

        public async Task SendMailRequestDocumentNotification(SendMailRequestDocumentRequest request, CancellationToken cancellationToken)
        {

            var currentDocument = await(from document in Context.RequestDocument.AsNoTracking().Where(c=> c.Id == request.documentId)
                         join requestedEmployee in Context.Employee.AsNoTracking() on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                         from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                         join documentEmployee in Context.Employee on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                         from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                         select new GetDocumentMailInfo
                         {
                             Id = document.Id,
                             DocumentTag = document.DocumentTag,
                             MailDescription = document.MailDescription,
                             DocumentType = document.DocumentType,
                             RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname} ",
                             RequestEmployeeId = document.UserIdCreated,
                             RequestEmail = $"{requestedEmployee.Email}",
                             Mobile = requestedEmployee.Mobile,
                             RequestedDate = document.DateCreated,
                             EmployeeFullName = $"{documentEmployee.Firstname} {documentEmployee.Lastname}",
                             CurrentStatus = document.CurrentAction,
                             CreatedDate = document.DateCreated,
                             AssignedEmployeeId = document.AssignedEmployeeId,
                             
                             AssignedRouteConfigId = document.AssignedRouteConfigId
                         }).FirstOrDefaultAsync();


            var attachments = new List<string>();

            var attachmentResponse  = await _requestDocumentAttachmentRepository.DownloadRequestDocumentAttachment(new DownloadRequestDocumentAttachmentRequest(request.documentId), cancellationToken);

            if (attachmentResponse != null)
            {
                if (attachmentResponse.Attachments.Count > 0)
                {
                    attachments = attachmentResponse.Attachments;
                }
            }


            var userId = _userRepository.LogCurrentUser()?.Id;
            if (currentDocument != null)
            {
                string frontDomain = _configuration.GetSection("AppSettings:Fronturl").Value;
                string LinkURL = string.Empty;
                var emails = new List<string?>();
                var linkUrl =await GenerateLinkUrl(currentDocument.Id);


                //var currentDoc = await GetDocumentDescription(currentDocument);

                var UpdatedInfo = await (from history in Context.RequestDocumentHistory.AsNoTracking().Where(x => x.DocumentId == currentDocument.Id)
                                         join employee in Context.Employee.AsNoTracking() on history.UserIdCreated equals employee.Id into employeeData
                                         from employee in employeeData.DefaultIfEmpty()
                                         select new {
                                             FirstName = employee.Firstname,
                                             LastName = employee.Lastname,
                                             ActionDate = history.DateCreated,
                                             Comment = history.Comment,
                                             

                                         }).OrderByDescending(x=> x.ActionDate).ToListAsync();

                currentDocument.UpdatedInfo = "<ul>";
                foreach (var item in UpdatedInfo)
                {
                    currentDocument.UpdatedInfo += $"<li> By {item.FirstName} {item.LastName} at {item.ActionDate.Value.ToString("dd/MM/yyyy h:mm:ss tt")} {item.Comment} </li>";

                }
                currentDocument.UpdatedInfo += "</ul>";


                emails = await GetGroupEmails(currentDocument.Id);
               string? resourceMail = await GetResourceMail(currentDocument.Id);


                var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);


                if (currentEmployee != null)
                {
                    if (linkUrl != null)
                    {

                        var templateService = new EmailTemplateService();
                        var shtmlString = templateService.LoadTemplate("RequestDocument.html");
                        if (!string.IsNullOrWhiteSpace(shtmlString))
                        {
                            // otworkflow_noreply@ot.mn



                            var requestModel = new EmailRequestDocumentModel();
                            if (currentDocument.CurrentStatus =="Completed" || currentDocument.CurrentStatus == "Declined")
                            {
                                requestModel.Subject = $"Request info {currentDocument.CurrentStatus} {currentDocument.DocumentType}#{currentDocument.Id} >> {currentDocument.EmployeeFullName}";
                            }
                            else
                            {
                                requestModel.Subject = $"Action required {currentDocument.CurrentStatus} {currentDocument.DocumentType}#{currentDocument.Id} >> {currentDocument.EmployeeFullName}";
                            }
                            

                            shtmlString = shtmlString.Replace("{url}", $"{frontDomain}{linkUrl}");

                            string? inTitle = $"Request : {currentDocument.CurrentStatus}";

                            if (currentDocument.CurrentStatus == "Completed" || currentDocument.CurrentStatus == "Declined")
                            {
                                inTitle = $"Request info {currentDocument.CurrentStatus}";
                            }
                            else
                            {
                                inTitle = $"Action required {currentDocument.DocumentType}#{currentDocument.Id} >> {currentDocument.EmployeeFullName}";
                            }


                            string? docNumber =$"{currentDocument.Id}";
                            string? descr = currentDocument.MailDescription;
                            string? requester = currentDocument.RequesterFullName;
                            string? requesterEmail = currentDocument.RequestEmail;
                            string? RequestMobile = currentDocument.Mobile;

                            if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                            {
                                var selectedOption = await Context.RequestNonSiteTravelOption.AsNoTracking().Where(c => c.DocumentId == currentDocument.Id && c.Selected == 1).FirstOrDefaultAsync();
                                descr += "</br>";
                                descr += $"<pre>{selectedOption?.OptionData} </pre>";

                            }


                            string? document = $"{currentDocument.DocumentType} {currentDocument.DocumentTag} {currentDocument.EmployeeFullName}";
                            if (currentDocument.DocumentType == RequestDocumentType.ProfileChanges)
                            { 
                                document = $"{currentDocument.DocumentType} {currentDocument.EmployeeFullName}";
                            }
                            string? currentAction = currentDocument.CurrentStatus;
                            string? comments = currentDocument.UpdatedInfo;

                            string notifdescr = $"{descr} by  {currentEmployee.Firstname}{currentEmployee.Lastname}";
                            shtmlString = shtmlString.Replace("{inTitle}", $"<b>{inTitle}</b>");
                            shtmlString = shtmlString.Replace("{docNumber}", $"<b>{docNumber}</b>");
                            shtmlString = shtmlString.Replace("{descr}", $"<b>{descr}</b>");
                            shtmlString = shtmlString.Replace("{requester}", $"<b>{requester}</b> <br>" +
                                $" <a href=mailto:{requesterEmail}?subject=Request#{docNumber}&body={currentEmployee.Firstname}> {requesterEmail} </a> <br>" +
                                $"(P) <a href=\"tel:{RequestMobile}\">{RequestMobile}</a> ");
                            shtmlString = shtmlString.Replace("{document}", $"<b>{document}</b>");
                            shtmlString = shtmlString.Replace("{currentAction}", $"<b>{currentAction}</b>");
                            shtmlString = shtmlString.Replace("{comments}", $"{comments}");
                            requestModel.Attachments = attachments;


                                requestModel.Body = shtmlString;
                            if (emails != null && emails.Any())
                            {
                                requestModel.To = emails.First();
                            }
   
                        //    requestModel.Cc = emails;

                            EmailAuthModel? rmu = null;
                            if (_memoryCache.TryGetValue($"TAS_MAIL::SMTP", out rmu))
                            {
                                requestModel.SenderMail = rmu.email;
                                await _emailSender.SendEmailAsync(requestModel, rmu);
                                if (resourceMail != null) 
                                {
                                    var requestModelResource = requestModel;

                                    requestModelResource.To = resourceMail;
                                    requestModelResource.Subject = requestModelResource.Subject.Replace("Action required", "");
                                    requestModel.Body = requestModel.Body.Replace("Action required", "");

                                    await _emailSender.SendEmailAsync(requestModelResource, rmu);
                                }

                            }
                            else
                            {

                                var data = await Context.MailSmtpConfig.AsNoTracking().FirstOrDefaultAsync();
                                if (data != null)
                                {
                                    rmu = new EmailAuthModel();
                                    rmu.email = data.email;
                                    rmu.password = data.password;
                                    rmu.smtpServer = data.smtpServer;
                                    rmu.smtpPort = data.smtpPort;
                                    _memoryCache.Set($"TAS_MAIL::SMTP", rmu, TimeSpan.FromDays(30));
                                    requestModel.SenderMail = data.email;
                                    await _emailSender.SendEmailAsync(requestModel, rmu);

                                    if (resourceMail != null)
                                    {
                                        var requestModelResource = requestModel;

                                        requestModelResource.SenderMail = resourceMail;
                                        requestModelResource.Subject = requestModelResource.Subject.Replace("Action required", "");
                                        await _emailSender.SendEmailAsync(requestModelResource, rmu);
                                    }
                                }
                                else
                                {
                                    throw new BadRequestException("Please register smtp config");

                                }

                            }

                        }


                    }
                }
                else {
                    _logger.LogWarning("CurrentEmployee not found =======================================");
                }
            }
            else {
                throw new BadRequestException("DocumentNotfound");
            }
        }



        public async Task SendMailNonSiteDocumentNotification(SendMailRequestNonsiteDocumentRequest request, CancellationToken cancellationToken)
        {

            var currentDocument = await (from document in Context.RequestDocument.AsNoTracking().Where(c => c.Id == request.documentId && c.DocumentType == RequestDocumentType.NonSiteTravel)
                                         join requestedEmployee in Context.Employee.AsNoTracking() on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                                         from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                                         join documentEmployee in Context.Employee on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                                         from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                                         select new GetDocumentMailInfo
                                         {
                                             Id = document.Id,
                                             DocumentTag = document.DocumentTag,
                                             MailDescription = document.MailDescription,
                                             DocumentType = document.DocumentType,
                                             RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname} ",
                                             RequestEmployeeId = document.UserIdCreated,
                                             RequestEmail = $"{requestedEmployee.Email}",
                                             Mobile = requestedEmployee.Mobile,
                                             RequestedDate = document.DateCreated,
                                             EmployeeFullName = $"{documentEmployee.Firstname} {documentEmployee.Lastname}",
                                             CurrentStatus = document.CurrentAction,
                                             CreatedDate = document.DateCreated,
                                             AssignedEmployeeId = document.AssignedEmployeeId,

                                             AssignedRouteConfigId = document.AssignedRouteConfigId
                                         }).FirstOrDefaultAsync();


            var attachments = new List<string>();

            var attachmentResponse = await _requestDocumentAttachmentRepository.DownloadRequestDocumentAttachment(new DownloadRequestDocumentAttachmentRequest(request.documentId), cancellationToken);

            if (attachmentResponse != null)
            {
                if (attachmentResponse.Attachments.Count > 0)
                {
                    attachments = attachmentResponse.Attachments;
                }
            }


            var userId = _userRepository.LogCurrentUser()?.Id;
            if (currentDocument != null)
            {
                string frontDomain = _configuration.GetSection("AppSettings:Fronturl").Value;
                string LinkURL = string.Empty;
                var emails = new List<string?>();
                var linkUrl = await GenerateLinkUrl(currentDocument.Id);




                //var currentDoc = await GetDocumentDescription(currentDocument);

                var UpdatedInfo = await (from history in Context.RequestDocumentHistory.AsNoTracking().Where(x => x.DocumentId == currentDocument.Id)
                                         join employee in Context.Employee.AsNoTracking() on history.UserIdCreated equals employee.Id into employeeData
                                         from employee in employeeData.DefaultIfEmpty()
                                         select new
                                         {
                                             FirstName = employee.Firstname,
                                             LastName = employee.Lastname,
                                             ActionDate = history.DateCreated,
                                             Comment = history.Comment,


                                         }).OrderByDescending(x => x.ActionDate).ToListAsync();

                currentDocument.UpdatedInfo = "<ul>";
                foreach (var item in UpdatedInfo)
                {
                    currentDocument.UpdatedInfo += $"<li> By {item.FirstName} {item.LastName} at {item.ActionDate.Value.ToString("dd/MM/yyyy h:mm:ss tt")} {item.Comment} </li>";

                }
                currentDocument.UpdatedInfo += "</ul>";


            //    emails = await GetGroupEmails(currentDocument.Id);
                string? requesterMail = await GetRequesterMail(currentDocument.Id);


                var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);


                if (currentEmployee != null)
                {
                    if (linkUrl != null)
                    {

                        var templateService = new EmailTemplateService();
                        var shtmlString = templateService.LoadTemplate("RequestDocument.html");
                        if (!string.IsNullOrWhiteSpace(shtmlString))
                        {
                            // otworkflow_noreply@ot.mn



                            var requestModel = new EmailRequestDocumentModel();


                            requestModel.Subject = $"Request info {currentDocument.CurrentStatus} {currentDocument.DocumentType}#{currentDocument.Id} >> {currentDocument.EmployeeFullName}";
                            

                            shtmlString = shtmlString.Replace("{url}", $"{frontDomain}{linkUrl}");


                            string  inTitle = $"Update info : {request.Reason}  {currentDocument.DocumentType}#{currentDocument.Id} >> {currentDocument.EmployeeFullName}";


                            string? docNumber = $"{currentDocument.Id}";
                            string? descr = currentDocument.MailDescription;
                            string? requester = currentDocument.RequesterFullName;
                            string? requesterEmail = currentDocument.RequestEmail;
                            string? RequestMobile = currentDocument.Mobile;

                            if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                            {
                                var selectedOption = await Context.RequestNonSiteTravelOption.AsNoTracking().Where(c => c.DocumentId == currentDocument.Id && c.Selected == 1).FirstOrDefaultAsync();
                                descr += "</br>";
                                descr += $"<pre>{selectedOption?.OptionData} </pre>";

                            }


                            string? document = $"{currentDocument.DocumentType}  {currentDocument.EmployeeFullName}";
      
                            string? currentAction = "#";
                            string? comments = currentDocument.UpdatedInfo;

                            string notifdescr = $"{descr} by  {currentEmployee.Firstname}{currentEmployee.Lastname}";
                            shtmlString = shtmlString.Replace("{inTitle}", $"<b>{inTitle}</b>");
                            shtmlString = shtmlString.Replace("{docNumber}", $"<b>{docNumber}</b>");
                            shtmlString = shtmlString.Replace("{descr}", $"<b>{descr}</b>");
                            shtmlString = shtmlString.Replace("{requester}", $"<b>{requester}</b> <br>" +
                                $" <a href=mailto:{requesterEmail}?subject=Request#{docNumber}&body={currentEmployee.Firstname}> {requesterEmail} </a> <br>" +
                                $"(P) <a href=\"tel:{RequestMobile}\">{RequestMobile}</a> ");
                            shtmlString = shtmlString.Replace("{document}", $"<b>{document}</b>");
                            shtmlString = shtmlString.Replace("{currentAction}", $"<b>{currentAction}</b>");
                            shtmlString = shtmlString.Replace("{comments}", $"{comments}");
                            requestModel.Attachments = attachments;


                            requestModel.Body = shtmlString;
                            if (emails != null && emails.Any())
                            {
                                requestModel.To = emails.First();
                            }

                            //    requestModel.Cc = emails;

                            EmailAuthModel? rmu = null;
                            if (_memoryCache.TryGetValue($"TAS_MAIL::SMTP", out rmu))
                            {
                                if (requesterEmail != null)
                                {
                                    requestModel.SenderMail = rmu.email;
                                    var requestModelResource = requestModel;

                                    requestModelResource.To = requesterEmail;

                                    await _emailSender.SendEmailAsync(requestModelResource, rmu);
                                }

                            }
                            else
                            {

                                var data = await Context.MailSmtpConfig.AsNoTracking().FirstOrDefaultAsync();
                                if (data != null)
                                {
                                    rmu = new EmailAuthModel();
                                    rmu.email = data.email;
                                    rmu.password = data.password;
                                    rmu.smtpServer = data.smtpServer;
                                    rmu.smtpPort = data.smtpPort;
                                    _memoryCache.Set($"TAS_MAIL::SMTP", rmu, TimeSpan.FromDays(30));

                                    if (requesterEmail != null)
                                    {
                                        var requestModelResource = requestModel;

                                        requestModelResource.SenderMail = requesterEmail;
                                        await _emailSender.SendEmailAsync(requestModelResource, rmu);
                                    }
                                }
                                else
                                {
                                    _logger.LogError("Please register smtp config");

                                }

                            }

                        }


                    }
                }
                else
                {
                    _logger.LogWarning("CurrentEmployee not found =======================================");
                }
            }
            else
            {
                _logger.LogWarning("DocumentNotfound not found =======================================" + request.documentId);
            }
        }




        #region GetDocumentURL



        private async Task<string?> GenerateLinkUrl(int documentId)
        {

            var document =await Context.RequestDocument.AsNoTracking().Where(x => x.Id == documentId).FirstOrDefaultAsync();

            if (document != null)
            {
                if (document.DocumentType == RequestDocumentType.SiteTravel)
                {
                    return BuildSiteTravelUrl(document.DocumentTag, document.Id);
                }
                else if (document.DocumentType == RequestDocumentType.NonSiteTravel)
                {
                    return $"/request/task/nonsitetravel/{document.Id}";
                }
                else if (document.DocumentType == RequestDocumentType.ProfileChanges)
                {
                    return $"/request/task/profilechanges/{document.Id}";
                }
                else if (document.DocumentType == RequestDocumentType.DeMobilisation)
                {
                    return $"/request/task/de-mobilisation/{document.Id}";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private string BuildSiteTravelUrl(string? documentTag, int documentId)
        {
            switch (documentTag)
            {
                case "ADD":
                    return $"/request/task/sitetravel/addtravel/{documentId}";
                case "REMOVE":
                    return $"/request/task/sitetravel/remove/{documentId}";
                case "RESCHEDULE":
                    return $"/request/task/sitetravel/reschedule/{documentId}";
                default:
                    return null;
            }
        }


        #endregion


        #endregion


    }


    public sealed record GetDocumentMailInfo
    {
        public int Id { get; set; }

        public string? CurrentStatus { get; set; }

        public string? CurrentStatusGroup { get; set; }
        public string? DocumentType { get; set; }

        public string? MailDescription { get; set; }

        public string? RequesterFullName { get; set; }
        public string? RequestEmail { get; set; }

        public string? Mobile { get; set; }

        public int? AssignedEmployeeId { get; set; }


        public DateTime? RequestedDate { get; set; }

        public string? EmployeeFullName { get; set; }

        public string? UpdatedInfo { get; set; }

        public int? EmployeeId { get; set; }

        public string? DocumentTag { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? AssignedRouteConfigId { get; set; }

        public int? RequestEmployeeId { get; set; }




    }

}
