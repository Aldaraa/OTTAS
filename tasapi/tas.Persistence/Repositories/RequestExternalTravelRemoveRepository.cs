using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public  class RequestExternalTravelRemoveRepository : BaseRepository<RequestExternalTravelRemove>, IRequestExternalTravelRemoveRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly IMemoryCache _memoryCache;
        public RequestExternalTravelRemoveRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, IMemoryCache memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
        }


        #region Complete
        public async Task<int> CompleteRequestDocumentExternalTravelRemove(CompleteRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
                    if (currentDocument != null)
                    {
                        var oldDocumentAssignedGrouId = currentDocument.AssignedRouteConfigId;
                        if (currentDocument.DocumentType == RequestDocumentType.ExternalTravel)
                        {
                            if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                            {
                                throw new BadRequestException("This task already Completed");
                            }
                            else
                            {
                                await RemoveTransport(request.documentId, currentDocument.EmployeeId.Value, cancellationToken);
                                var assignedOldGroupId = await Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();
                                currentDocument.CurrentAction = RequestDocumentAction.Completed;
                                currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                                Context.RequestDocument.Update(currentDocument);
                                var newHistoryRecord = new RequestDocumentHistory
                                {
                                    Comment = RequestDocumentAction.Completed + " " + request.comment,
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    CurrentAction = RequestDocumentAction.Completed,
                                    ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    DocumentId = request.documentId,
                                    AssignedGroupId = assignedOldGroupId?.GroupId
                                };
                                Context.RequestDocumentHistory.Add(newHistoryRecord);

                                if (currentDocument.EmployeeId.HasValue)
                                {
                                    string cacheEntityName = $"Employee_{currentDocument.EmployeeId}";
                                    _memoryCache.Remove($"API::{cacheEntityName}");
                                }


                                await Context.SaveChangesAsync();
                                await transaction.CommitAsync(cancellationToken);

                                _HTTPUserRepository.ClearAllEmployeeCache();

                                return currentDocument.Id;

                            }
                        }
                        else
                        {
                            throw new BadRequestException("Document is not of type 'SiteTravel'.");
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Document not found.");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new BadRequestException(ex.Message);
                }
            }

        }


        public async Task RemoveTransport(int documentId, int employeeId, CancellationToken cancellationToken)
        {
            //  await Context.RequestDocument.Where(x => x.Id == documentId).FirstOrDefaultAsync();
            var currentRemoveScheduleData = await Context.RequestExternalTravelRemove.Where(x => x.DocumentId == documentId).Select(x => new { x.TransportId }).FirstOrDefaultAsync(cancellationToken);

            if (currentRemoveScheduleData != null)
            {
                var currentTransport = await Context.Transport.Where(x => x.Id == currentRemoveScheduleData.TransportId && x.EmployeeId == employeeId).FirstOrDefaultAsync(cancellationToken);

                if (currentTransport != null)
                {
                    currentTransport.ChangeRoute = "Remove exteral";
                    Context.Transport.Remove(currentTransport);
                }
            }
        }


        #endregion


        #region GetRomoveData
        public async Task<GetRequestDocumentExternalTravelRemoveResponse> GetRequestDocumentExternalTravelRemove(GetRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetRequestDocumentExternalTravelRemoveResponse();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync(cancellationToken);
                var assignEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync(cancellationToken);
                var updatedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdUpdated).FirstOrDefaultAsync(cancellationToken);
                var createdEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdCreated).FirstOrDefaultAsync(cancellationToken);


                var deleagationEmployee = await Context.RequestDelegates.AsNoTracking().Where(x => x.FromEmployeeId == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync();

                var userId = _HTTPUserRepository.LogCurrentUser()?.Id;


                returnData.Id = currentDocument.Id;
                returnData.RequestedDate = currentDocument.DateCreated;
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.RequestUserId = createdEmployee?.Id;
                returnData.EmployeeFullName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                returnData.EmployeeActive = currentEmployee?.Active;
                returnData.AssignedEmployeeFullName = $"{assignEmployee?.Firstname} {assignEmployee?.Lastname}";
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.DocumentType = currentDocument.DocumentType;
                returnData.UpdatedInfo = $"{currentDocument.DateUpdated} {updatedEmployee?.Firstname} {updatedEmployee?.Lastname}";
                returnData.RequesterFullName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}";
                returnData.AssignedEmployeeId = currentDocument.AssignedEmployeeId;
                returnData.AssignedRouteConfigId = currentDocument.AssignedRouteConfigId;
                returnData.EmployeeId = currentDocument?.EmployeeId;
                returnData.DelegateEmployeeId = deleagationEmployee?.ToEmployeeId;

                try
                {

                    returnData.DaysAway = currentDocument.DaysAwayDate.HasValue ? (DateTime.Now.Subtract(currentDocument.DaysAwayDate.Value).Days) * (-1) : (DateTime.Now.Subtract(currentDocument.DateCreated.Value).Days) * (-1);
                }
                catch (Exception)
                {
                    returnData.DaysAway = 0;
                }



                var currentTravelData = await Context.RequestExternalTravelRemove.Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                if (currentTravelData != null)
                {
                    var travelData = new GetRequestDocumentExternalTravelRemoveTravel();

                    var currentTransport =await Context.Transport.AsNoTracking().Where(x => x.Id == currentTravelData.TransportId).FirstOrDefaultAsync();

                    if (currentTransport != null)
                    {
                        var scheduleData = await Context.TransportSchedule.Where(x => x.Id == currentTransport.ScheduleId).FirstOrDefaultAsync();
                        if (scheduleData != null)
                        {
                            travelData.ScheduleId = scheduleData.Id;
                            travelData.ScheduleDate = scheduleData.EventDate;

                            var firtscheduleTransport = await Context.ActiveTransport.Where(x => x.Id == currentTransport.ActiveTransportId).FirstOrDefaultAsync();
                            if (firtscheduleTransport != null)
                            {
                                travelData.ScheduleDirection = firtscheduleTransport.Direction;
                                travelData.ScheduleDescription =  firtscheduleTransport.Description;
                            }


                        }

                    }


                    travelData.Id = currentTravelData.Id;



                    returnData.TravelData = travelData;
                }



            }
            return returnData;

        }

        #endregion



        #region CreateRemoveTravel

        public async Task<int> CreateRequestDocumentExternalTravelRemove(CreateRequestDocumentExternalTravelRemoveRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {
                    var TransportId = request.flightData.TransportId;

                    var currenttransport = await Context.Transport.AsNoTracking().Where(x => x.Direction == "EXTERNAL" && x.Id == TransportId).FirstOrDefaultAsync(cancellationToken);

                    if (currenttransport == null)
                    {
                        throw new BadRequestException("Transport data not found");
                    }

                    var reqDocument = new RequestDocument();
                    reqDocument.CurrentAction = request.documentData.Action;
                    reqDocument.Active = 1;
                    reqDocument.DateCreated = DateTime.Now;
                    reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqDocument.Description = reqDocument.Description;
                    reqDocument.EmployeeId = request.documentData.EmployeeId;
                    reqDocument.DocumentType = RequestDocumentType.ExternalTravel;
                    reqDocument.DocumentTag = "REMOVE";
                    reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
                    reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;

                    Context.RequestDocument.Add(reqDocument);
                    await Context.SaveChangesAsync();
                    await CreateSaveAttachment(request.Files, reqDocument.Id);

                    await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id);
                    await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                    await Context.SaveChangesAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return reqDocument.Id;
                }
                catch (Exception ex)
                {
                    // Roll back the transaction if an exception occurs
                    await transaction.RollbackAsync(cancellationToken);

                    throw new BadRequestException(ex.Message);
                }
            }
        }


        private async Task CreateSaveFlightData(CreateRequestDocumentExternalTravelRemoveData travelData, int employeeId, int documentId)
        {

            var newRecord = new RequestExternalTravelRemove
            {
                DocumentId = documentId,
                EmployeeId = employeeId,
                Active = 1,
                TransportId = travelData.TransportId,
                DateCreated = DateTime.Now,
                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
            };
            Context.RequestExternalTravelRemove.Add(newRecord);
            await Task.CompletedTask;
        }
        //CreateRequestDocumentExternalTravelAttachment
        private async Task CreateSaveAttachment(List<CreateRequestDocumentExternalTravelRemoveAttachment> attachments, int documentId)
        {
            foreach (var item in attachments)
            {
                var currentFile = await Context.SysFile.Where(x => x.Id == item.FileAddressId).FirstOrDefaultAsync();
                if (currentFile != null)
                {
                    var reqAttach = new RequestDocumentAttachment();
                    reqAttach.Active = 1;
                    reqAttach.DateCreated = DateTime.Now;
                    reqAttach.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqAttach.DocumentId = documentId;
                    reqAttach.Description = item.Comment;
                    reqAttach.FileAddress = currentFile.FileAddress;
                    reqAttach.IncludeEmail = item.IncludeEmail;
                    Context.RequestDocumentAttachment.Add(reqAttach);
                }
            }
        }

        private async Task SaveDocumentHistory(CreateRequestDocumentExternalTravelRemoveRequest request, int DocumentId, CancellationToken cancellationToken)
        {

            var reqHist = new RequestDocumentHistory();
            reqHist.Comment = request.documentData.Comment;
            reqHist.Active = 1;
            reqHist.DateCreated = DateTime.Now;
            reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.DocumentId = DocumentId;
            reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.CurrentAction = request.documentData.Action;
            Context.RequestDocumentHistory.Add(reqHist);
            await Task.CompletedTask;

        }

        #endregion
    }






}