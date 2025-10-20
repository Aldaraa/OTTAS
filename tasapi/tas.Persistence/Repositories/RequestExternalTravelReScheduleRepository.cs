using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public sealed class RequestExternalTravelReScheduleRepository : BaseRepository<RequestExternalTravelReschedule>, IRequestExternalTravelReScheduleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        public RequestExternalTravelReScheduleRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, ICheckDataRepository checkDataRepository, IMemoryCache memoryCache, ITransportCheckerRepository transportCheckerRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _checkDataRepository = checkDataRepository;
            _memoryCache = memoryCache;
            _transportCheckerRepository = transportCheckerRepository;
        }


        #region GetData
        public async Task<GetRequestDocumentExternalTravelRescheduleResponse> GetRequestDocumentExternalTravelReschedule(GetRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetRequestDocumentExternalTravelRescheduleResponse();
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
                returnData.EmployeeFullName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                returnData.RequestUserId = createdEmployee?.Id;
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



                var currentTravelData = await Context.RequestExternalTravelReschedule.Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                if (currentTravelData != null)
                {

                    var travelData = new GetRequestDocumentExternalTravelRescheduleTravel();

                    var existingTransport = await Context.Transport.AsNoTracking().Where(c => c.Id == currentTravelData.oldTransportId).FirstOrDefaultAsync();
                    if (existingTransport != null)
                    {




                        var existingscheduleData = await Context.TransportSchedule.Where(x => x.Id == existingTransport.ScheduleId).FirstOrDefaultAsync();
                        if (existingscheduleData != null)
                        {
                            travelData.ExistingScheduleId = existingscheduleData.Id;
                            travelData.ExistingScheduleDate = existingscheduleData.EventDate;
                            var inscheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == existingscheduleData.ActiveTransportId).FirstOrDefaultAsync();
                            if (inscheduleTransport != null)
                            {
                                travelData.ExistingScheduleDirection = inscheduleTransport.Direction;
                                travelData.ExistingScheduleDescription = existingscheduleData.Description;
                            }


                        }

                        var ReScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentTravelData.ScheduleId).FirstOrDefaultAsync();
                        if (ReScheduleData != null)
                        {
                            travelData.ReScheduleDate = ReScheduleData.EventDate;
                            travelData.ReScheduleId = ReScheduleData.Id;
                            var ReScheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == ReScheduleData.ActiveTransportId).FirstOrDefaultAsync();
                            if (ReScheduleTransport != null)
                            {
                                travelData.ReScheduleDirection = ReScheduleTransport.Direction;
                                travelData.ReScheduleDescription = ReScheduleData.Description;
                            }


                        }

                        int ReScheduleAvailableSeatsCount = 0;

                        int tCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == currentTravelData.ScheduleId).CountAsync();
                        if (ReScheduleData != null)
                        {
                            if (ReScheduleData.Seats.HasValue)
                            {
                                ReScheduleAvailableSeatsCount = ReScheduleData.Seats.Value - tCount;
                            }
                            else
                            {
                                ReScheduleAvailableSeatsCount = 0;
                            }


                        }
                        else
                        {
                            ReScheduleAvailableSeatsCount = 0;
                        }


                        travelData.Id = currentTravelData.Id;
                        travelData.ReScheduleAvailableSeats = ReScheduleAvailableSeatsCount;

                        returnData.TravelData = travelData;
                    }
                    else {
                        throw new BadRequestException("Existing transport data not found");
                    }

                }



            }
            return returnData;

        }


        #endregion


        #region UpdateDta
        public async Task UpdateRequestDocumentExternalTravelReschedule(UpdateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {

            var currentData = await Context.RequestExternalTravelReschedule.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentData.ScheduleId = request.newScheduleId;
                Context.RequestExternalTravelReschedule.Update(currentData);
                await  Context.SaveChangesAsync();

            }

            await Task.CompletedTask;

        }




        #endregion


        #region CreateData



        public async Task<int> CreateRequestDocumentExternalTravelReschedule(CreateRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {
                    var reqDocument = new RequestDocument();
                    reqDocument.CurrentAction = request.documentData.Action;
                    reqDocument.Active = 1;
                    reqDocument.DateCreated = DateTime.Now;
                    reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqDocument.Description = reqDocument.Description;
                    reqDocument.EmployeeId = request.documentData.EmployeeId;
                    reqDocument.DocumentType = RequestDocumentType.ExternalTravel;
                    reqDocument.DocumentTag = "RESCHEDULE";
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
                    await transaction.RollbackAsync(cancellationToken);
                    throw new BadRequestException(ex.Message);
                }
            }


        }


        private async Task CreateSaveFlightData(CreateRequestDocumentExternalTravelRescheduleData travelData, int employeeId, int documentId)
        {

            var newRecord = new RequestExternalTravelReschedule
            {
                DocumentId = documentId,
                EmployeeId = employeeId,
                Active = 1,
                oldTransportId = travelData.oldTransportId,
                ScheduleId = travelData.ScheduleId,
                DateCreated = DateTime.Now,
                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
            };
            Context.RequestExternalTravelReschedule.Add(newRecord);
            await Task.CompletedTask;
        }
        //CreateRequestDocumentSiteTravelAttachment
        private async Task CreateSaveAttachment(List<CreateRequestDocumentExternalTravelRescheduleAttachment> attachments, int documentId)
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

        private async Task SaveDocumentHistory(CreateRequestDocumentExternalTravelRescheduleRequest request, int DocumentId, CancellationToken cancellationToken)
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


        #region Comlplete

        public async Task CompleteRequestDocumentExternalTravelReschedule(CompleteRequestDocumentExternalTravelRescheduleRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
                    if (currentDocument != null)
                    {
                        if (currentDocument.DocumentType == RequestDocumentType.ExternalTravel)
                        {
                            if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                            {
                                throw new BadRequestException("This task already Completed");
                            }
                            else
                            {
                                currentDocument.CurrentAction = RequestDocumentAction.Completed;
                                currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                                Context.RequestDocument.Update(currentDocument);
                                var oldDocumentAssignedGrouId = currentDocument.AssignedRouteConfigId;
                                RequestExternalTravelReschedule currentData = await Context.RequestExternalTravelReschedule.Where(rs => rs.DocumentId == request.documentId).FirstOrDefaultAsync();
                                var assignedOldGroupId = await Context.RequestGroupConfig.Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();
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


                                await ChangeTransportData(currentData.oldTransportId.Value, currentData.ScheduleId.Value, request.documentId, currentDocument.EmployeeId.Value, cancellationToken);

                                await transaction.CommitAsync(cancellationToken);
                            }
                        }
                        else
                        {
                            throw new BadRequestException("Document is not of type 'External'.");
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Document not found.");

                    }
                    await Task.CompletedTask;
                }
                catch (Exception ex)
                {

                    await transaction.RollbackAsync(cancellationToken);
                    throw new BadRequestException(ex.Message);
                }
            }
        }



        private async Task ChangeTransportData(int transportId, int scheduleId, int documentId, int employeeId, CancellationToken cancellationToken)
        {
            var currentTransport = await Context.Transport.Where(x => x.Id == transportId && x.EmployeeId == employeeId).FirstOrDefaultAsync();
            if (currentTransport != null)
            {

                await _checkDataRepository.CheckProfile(currentTransport.EmployeeId.Value, cancellationToken);
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentTransport.EmployeeId).FirstOrDefaultAsync();
                var newSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId).FirstOrDefaultAsync();
                if (currentTransport.Direction == "EXTERNAL")
                {

                    if (newSchedule?.EventDate.Date > currentTransport?.EventDate.Value.Date)
                    {


                        var currentDate = currentTransport.EventDate.Value.Date;

                        var nexttransport = await Context.Transport
                            .Where(x => x.EventDate.Value.Date < newSchedule.EventDate.Date && x.EventDate.Value.Date > currentTransport.EventDate.Value.Date
                        && x.EmployeeId == currentTransport.EmployeeId).FirstOrDefaultAsync();



                        currentTransport.ScheduleId = newSchedule.Id;
                        currentTransport.EventDate = newSchedule.EventDate;
                        currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                        currentTransport.EventDateTime = newSchedule.EventDate;
                        currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        currentTransport.ChangeRoute = $"Request external reschedule #{documentId}";
                        Context.Transport.Update(currentTransport);

                    }
                }

                await Task.CompletedTask;
            }


            #endregion


        }
    }

}