using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentAttachmentFeature.CreateRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.DeleteRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.DownloadRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.UpdateRequestDocumentAttachment;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestDocumentAttachmentRepository : BaseRepository<RequestDocumentAttachment>, IRequestDocumentAttachmentRepository
    {
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestDocumentAttachmentRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _HTTPUserRepository = hTTPUserRepository;
        }

        public async Task<List<GetRequestDocumentAttachmentResponse>> GetRequestDocumentAttachment(GetRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            var result = from attachments in Context.RequestDocumentAttachment
                         join emp in Context.Employee on attachments.UserIdCreated equals emp.Id into empData
                         from emp in empData.DefaultIfEmpty()
                         where attachments.DocumentId == request.DocumentId


                         select new GetRequestDocumentAttachmentResponse
                         {
                             Id = attachments.Id,
                             Description = attachments.Description,
                             FileAddress = attachments.FileAddress,
                             IncludeEmail = attachments.IncludeEmail,
                             DocumentId = attachments.DocumentId,
                             FullName = emp != null ? $"{emp.Firstname} {emp.Lastname}" : string.Empty,
                             CreatedDate = attachments.DateCreated
                         };


            var returnData = await result.ToListAsync();
            var currentEmployeeDoc =await Context.RequestDocument.Where(x => x.Id == request.DocumentId && x.DocumentType == RequestDocumentType.NonSiteTravel).Select(x => new { x.EmployeeId }).FirstOrDefaultAsync();
            if (currentEmployeeDoc != null)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == currentEmployeeDoc.EmployeeId && x.PassportImage != null).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    var item = new GetRequestDocumentAttachmentResponse {
                        Description = "Password Image",
                        DocumentId = request.DocumentId,
                        FileAddress = currentEmployee.PassportImage,
                        Id = 0,
                        IncludeEmail = 0,
                        CreatedDate = DateTime.Now,
                        FullName = "TAS SYSTEM"


                    };
                    returnData.Add(item);
                }
                
            }


            return returnData;

        }


        public async Task CreateRequestDocumentAttachment(CreateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {


                    foreach (var item in request.FileAddressIds)
                    {
                        var currentFile = await Context.SysFile.Where(x => x.Id == item).FirstOrDefaultAsync();

                        if (currentFile != null)
                        {
                            var newRecord = new RequestDocumentAttachment
                            {
                                DocumentId = request.DocumentId,
                                IncludeEmail = request.IncludeEmail,
                                FileAddress = currentFile.FileAddress,
                                Description = request.Description,
                                Active = 1,
                                DateCreated = DateTime.Now,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                            };

                            Context.RequestDocumentAttachment.Add(newRecord);

                            currentDocument.DateUpdated = DateTime.Now;
                            currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            Context.RequestDocument.Update(currentDocument);

                            var history = new RequestDocumentHistory
                            {
                                CurrentAction = RequestDocumentAction.Saved,
                                ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                                Active = 1,
                                DateCreated = DateTime.Now,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                DocumentId = currentDocument.Id,
                                Comment = "Added Attachment"

                            };

                            Context.RequestDocumentHistory.Add(history);


                        }
                    }
                
                }
                else
                {
                    throw new BadRequestException("This task cannot be modified.");
                }
            }
            else
            {
                throw new BadRequestException("Task not found.");
            }
            await Task.CompletedTask;
        }


        public async Task<int> UpdateRequestDocumentAttachment(UpdateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {

             var currentFile = await Context.RequestDocumentAttachment.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

            if (currentFile != null)
            {
                currentFile.IncludeEmail = request.IncludeEmail;
                currentFile.Description = request.Description;
                currentFile.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentFile.DateUpdated = DateTime.Now;
                Context.RequestDocumentAttachment.Update(currentFile);
                return currentFile.DocumentId;

            }
            else
            {
                throw new BadRequestException("Attachment not found.");
            }
        }

        public async Task<DownloadRequestDocumentAttachmentResponse> DownloadRequestDocumentAttachment(DownloadRequestDocumentAttachmentRequest request, CancellationToken cancellationToken) 
        {

            var currentDoc =await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
            if (currentDoc != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDoc.EmployeeId).FirstOrDefaultAsync();
                
                var returnData = new DownloadRequestDocumentAttachmentResponse();
                returnData.Attachments = new List<string>();
                if (currentEmployee != null)
                {
                    if(!string.IsNullOrWhiteSpace(currentEmployee.PassportImage))
                    {
                        returnData.Attachments.Add(currentEmployee.PassportImage);
                    }
                }

                var attachments = await Context.RequestDocumentAttachment.AsNoTracking().Where(x => x.DocumentId == request.DocumentId && x.IncludeEmail == 1).ToListAsync(cancellationToken);
                foreach (var item in attachments)
                {
                    returnData.Attachments.Add(item.FileAddress);
                }

                return returnData;
            }
            else {
                throw new BadRequestException("Document not found");
            }


        }





        public async Task<int> DeleteRequestDocumentAttachment(DeleteRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {

            var currentRecord = await Context.RequestDocumentAttachment.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == currentRecord.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {

                    if (currentRecord != null)
                    {
                        Context.RequestDocumentAttachment.Remove(currentRecord);

                        currentDocument.DateUpdated = DateTime.Now;
                        currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        Context.RequestDocument.Update(currentDocument);

                        var history = new RequestDocumentHistory
                        {
                            CurrentAction = RequestDocumentAction.Saved,
                            ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                            Active = 1,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            DocumentId = currentDocument.Id,
                            Comment = "Deleted Attachment"

                        };

                        Context.RequestDocumentHistory.Add(history);
                    }
                    else
                    {
                        throw new BadRequestException("Record not found.");
                    }

                }
                else
                {
                    throw new BadRequestException("This task cannot be modified.");
                }

                return currentDocument.Id;
            }

            return 0;

        }
    }
}
