using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.Drawing.Chart.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelRemove;
using tas.Application.Features.TransportFeature.RemoveTransport;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
 
    public sealed class RequestSiteTravelRemoveRepository : BaseRepository<RequestSiteTravelRemove>, IRequestSiteTravelRemoveRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly CacheService _memoryCache;
        public RequestSiteTravelRemoveRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, CacheService memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
        }



        #region UpdateAddTravel

        public async Task<int> UpdateRequestDocumentSiteTravelRemove(UpdateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestSiteTravelRemove.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                var lastScheduleIdDescr = await GetScheduleIdDescriptionData(request.LastScheduleId);

                var firstScheduleIdDescr = await GetScheduleIdDescriptionData(request.FirstScheduleId);

                var currentRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentData.RoomId = request.RoomId.HasValue ? request.RoomId.Value : currentData.RoomId;
                currentData.FirstScheduleId = request.FirstScheduleId;
                currentData.LastScheduleId = request.LastScheduleId;
                currentData.CostCodeId = request.CostCodeId;
                currentData.FirstScheduleIdDescr = firstScheduleIdDescr;
                currentData.LastScheduleIdDescr = lastScheduleIdDescr;
                currentData.ShiftId = request.shiftId;
                currentData.LastScheduleNoShow = request.LastScheduleNoShow;
                currentData.FirstScheduleNoShow = request.FirstScheduleNoShow;

                Context.RequestSiteTravelRemove.Update(currentData);

               await SaveDocumentHistoryTravelUpdate(currentData.DocumentId, cancellationToken);

                return currentData.DocumentId;

            }
            return 0;




        }



        private async Task SaveDocumentHistoryTravelUpdate(int DocumentId, CancellationToken cancellationToken)
        {

            var reqHist = new RequestDocumentHistory();
            reqHist.Comment = "Travel updated";
            reqHist.Active = 1;
            reqHist.DateCreated = DateTime.Now;
            reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.DocumentId = DocumentId;
            reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.CurrentAction = RequestDocumentAction.Saved;
            Context.RequestDocumentHistory.Add(reqHist);
            await Task.CompletedTask;

        }



        #endregion

        #region CreateRemoveTravel

        public async Task<int> CreateRequestDocumentSiteTravelRemove(CreateRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {

            var flightDataFirstSchedule = request.flightData.FirstScheduleId;
            var flightDataLastSchedule = request.flightData.LastScheduleId;

            var flightDataFirstScheduleDirection ="";
            var flightDataLastScheduleDirection = "";

            var lastScheduleIdDescr = await GetScheduleIdDescriptionData(request.flightData.LastScheduleId);

            var firstScheduleIdDescr = await GetScheduleIdDescriptionData(request.flightData.FirstScheduleId);


            var firstscheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == flightDataFirstSchedule).FirstOrDefaultAsync();
            if (firstscheduleData != null)
            {

                var firtscheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == firstscheduleData.ActiveTransportId).FirstOrDefaultAsync();
                if (firtscheduleTransport != null)
                {
                    flightDataFirstScheduleDirection = firtscheduleTransport.Direction;
                }


            }
            





            var lastscheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == flightDataLastSchedule).FirstOrDefaultAsync();
            if (lastscheduleData != null)
            {
              

                var lastcheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == lastscheduleData.ActiveTransportId).FirstOrDefaultAsync();
                if (lastcheduleTransport != null)
                {
                    flightDataLastScheduleDirection = lastcheduleTransport.Direction;
                }


            }


            if (flightDataFirstScheduleDirection == flightDataLastScheduleDirection)
            {
                throw new BadRequestException("Choose a different direction of route on the flight");
            }

            var reqDocument = new RequestDocument();
            reqDocument.CurrentAction = request.documentData.Action;
            reqDocument.Active = 1;
            reqDocument.DateCreated = DateTime.Now;
            reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqDocument.Description = reqDocument.Description;
            reqDocument.EmployeeId = request.documentData.EmployeeId;
            reqDocument.DocumentType = RequestDocumentType.SiteTravel;
            reqDocument.DocumentTag = "REMOVE";
            reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
            reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;

            Context.RequestDocument.Add(reqDocument);
            await Context.SaveChangesAsync();
            await CreateSaveAttachment(request.Files, reqDocument.Id);

            if (flightDataFirstScheduleDirection == "OUT")
            {
              var beforeRoomData =await  Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate.Value.Date == firstscheduleData.EventDate.Date.AddDays(-1) && x.EmployeeId == request.documentData.EmployeeId).FirstOrDefaultAsync();
                await CreateSaveFlightData(request.flightData, beforeRoomData?.RoomId, request.documentData.EmployeeId, reqDocument.Id, firstScheduleIdDescr, lastScheduleIdDescr);
            }
            else {
                await CreateSaveFlightData(request.flightData, null, request.documentData.EmployeeId, reqDocument.Id, firstScheduleIdDescr, lastScheduleIdDescr);
            }




            await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
            await Context.SaveChangesAsync();
            return reqDocument.Id;
        }








        private async Task<string?> GetScheduleIdDescriptionData(int scheduleId)
        {
            var scheduleData = await (from inschedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId)
                                        join transport in Context.ActiveTransport.AsNoTracking() on inschedule.ActiveTransportId equals transport.Id into transportData
                                        from transport in transportData.DefaultIfEmpty()
                                        select new
                                        {
                                            Id = inschedule.Id,
                                            Direction = transport.Direction,
                                            EventDate = inschedule.EventDate.Date,
                                            ScheduleCode = inschedule.Code,
                                            ScheduleDescription = inschedule.Description
                                        }).FirstOrDefaultAsync();

            if (scheduleData != null)
            {
                return $"{scheduleData?.EventDate.ToString("yyyy-MM-dd")} {scheduleData?.Direction} {scheduleData?.ScheduleCode} {scheduleData?.ScheduleDescription}";

            }
            else {
                return string.Empty;
            }

        }


        private async Task CreateSaveFlightData(CreateRequestDocumentSiteTravelRemoveData travelData, int? RoomId, int employeeId, int documentId, string? firstScheduleIdDescr, string? lastScheduleIdDescr)
        {
            
            var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();

            var newRecord = new RequestSiteTravelRemove
            {
                DocumentId = documentId,
                EmployeeId = employeeId,
                Active = 1,
                FirstScheduleId = travelData.FirstScheduleId,
                LastScheduleId = travelData.LastScheduleId,
                ShiftId = travelData.shiftId,
                RoomId = RoomId.HasValue ? RoomId : virtualRoom?.Id,
                DateCreated = DateTime.Now,
                Reason = travelData.Reason,
                CostCodeId = travelData.CostCodeId,
                FirstScheduleIdDescr = firstScheduleIdDescr,
                LastScheduleIdDescr = lastScheduleIdDescr,
                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                FirstScheduleNoShow = travelData.FirstScheduleNoShow,
                LastScheduleNoShow = travelData.LastScheduleNoShow,
            };
            Context.RequestSiteTravelRemove.Add(newRecord);
            await Task.CompletedTask;




        }
        //CreateRequestDocumentSiteTravelAttachment
        private async Task CreateSaveAttachment(List<CreateRequestDocumentSiteTravelRemoveAttachment> attachments, int documentId)
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

        private async Task SaveDocumentHistory(CreateRequestDocumentSiteTravelRemoveRequest request, int DocumentId, CancellationToken cancellationToken)
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

        #region GetTravel
        public async Task<GetRequestDocumentSiteTravelRemoveResponse> GetRequestDocumentSiteTravelRemove(GetRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetRequestDocumentSiteTravelRemoveResponse();
            var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
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
                returnData.RequesterMail = createdEmployee?.Email;
                returnData.RequesterMobile = createdEmployee?.PersonalMobile;
                returnData.AssignedEmployeeId = currentDocument.AssignedEmployeeId;
                returnData.AssignedRouteConfigId = currentDocument.AssignedRouteConfigId;
                returnData.EmployeeId = currentDocument?.EmployeeId;
                returnData.DelegateEmployeeId = deleagationEmployee?.ToEmployeeId;



                try
                {
                    returnData.DaysAway = currentDocument.DaysAwayDate.HasValue ? (DateTime.Today.Subtract(currentDocument.DaysAwayDate.Value).Days) * (-1) : (DateTime.Today.Subtract(currentDocument.DateCreated.Value).Days) * (-1);
                }
                catch (Exception)
                {

                    returnData.DaysAway = 0;
                }


                var currentTravelData = await Context.RequestSiteTravelRemove.Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                if (currentTravelData != null)
                {
                    var travelData = new GetRequestDocumentSiteTravelRemoveTravel();

                    if (currentTravelData.RoomId.HasValue)
                    {
                        var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == currentTravelData.RoomId.Value).FirstOrDefaultAsync();
                        travelData.RoomTypeId = currentRoom?.RoomTypeId;
                        travelData.RoomId = currentTravelData?.RoomId;
                        travelData.CampId = currentRoom?.CampId;
                        travelData.RoomNumber = currentRoom?.Number;

                    }

                    var firstscheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentTravelData.FirstScheduleId).FirstOrDefaultAsync();
                    if (firstscheduleData != null)
                    {
                        travelData.FirstScheduleDate = firstscheduleData.EventDate;

                      //  var firtscheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == firstscheduleData.ActiveTransportId).FirstOrDefaultAsync();

                        var firtscheduleTransport = await (from transport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == firstscheduleData.ActiveTransportId)
                                                          join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                          from tmode in modeData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              Code = transport.Code,
                                                              Direction = transport.Direction,
                                                              TransportMode = tmode.Code


                                                          }).FirstOrDefaultAsync();


                        if (firtscheduleTransport != null)
                        {
                            travelData.FirstScheduleDirection = firtscheduleTransport.Direction;
                            travelData.FirstScheduleDescription = $"{firtscheduleTransport.Code} {firstscheduleData.Description} {firtscheduleTransport.TransportMode}";
                        }


                    }

                    var lastscheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentTravelData.LastScheduleId).FirstOrDefaultAsync();




                    if (lastscheduleData != null)
                    {
                        travelData.LastScheduleDate = lastscheduleData.EventDate;

                      //  var lastcheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == lastscheduleData.ActiveTransportId).FirstOrDefaultAsync();

                        var lastcheduleTransport = await (from transport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == lastscheduleData.ActiveTransportId)
                                                         join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                         from tmode in modeData.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             Code = transport.Code,
                                                             Direction = transport.Direction,
                                                             TransportMode = tmode.Code


                                                         }).FirstOrDefaultAsync();


                        if (lastcheduleTransport != null)
                        {
                            travelData.LastScheduleDirection = lastcheduleTransport.Direction;
                            travelData.LastScheduleDescription =  $"{lastcheduleTransport.Code} {lastscheduleData.Description} {lastcheduleTransport.TransportMode}";
                        }


                    }
                    travelData.Id = currentTravelData.Id;
                    travelData.shiftId = currentTravelData?.ShiftId;
                    travelData.RoomId = currentTravelData?.RoomId;
                    travelData.FirstScheduleId = currentTravelData.FirstScheduleId;
                    travelData.LastScheduleId = currentTravelData.LastScheduleId;
                    //travelData.LastScheduleDescription = lastscheduleData?.Description;
                    //travelData.FirstScheduleDescription = firstscheduleData?.Description;
                    travelData.Reason = currentTravelData?.Reason;
                    travelData.FirstScheduleIdDescr = currentTravelData?.FirstScheduleIdDescr;
                    travelData.LastScheduleIdDescr = currentTravelData?.LastScheduleIdDescr;
                    travelData.CostCodeId = currentTravelData?.CostCodeId;
                    travelData.FirstScheduleNoShow = currentTravelData?.FirstScheduleNoShow;
                    travelData.LastScheduleNoShow = currentTravelData?.LastScheduleNoShow;

                    returnData.TravelData = travelData;
                }



            }
            return returnData;

        }


        #endregion


        #region Complete
        public async Task<CompleteRequestDocumentSiteTravelRemoveResponse?> CompleteRequestDocumentSiteTravelRemove(CompleteRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (currentDocument != null)
                    {
                        var oldDocumentAssignedGrouId = currentDocument.AssignedRouteConfigId;
                        if (currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                        {
                            if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                            {
                                throw new BadRequestException("This task already Completed");
                            }
                            else
                            {
                                var returnData =  await RemoveSchedule(request.Id, currentDocument.EmployeeId.Value, cancellationToken);
                                var assignedOldGroupId = await Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();
                                currentDocument.CurrentAction = RequestDocumentAction.Completed;
                                currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                                currentDocument.CompletedUserId = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentDocument.CompletedDate = DateTime.Now;
                                Context.RequestDocument.Update(currentDocument);
                                var newHistoryRecord = new RequestDocumentHistory
                                {
                                    Comment = RequestDocumentAction.Completed + " " + request.comment,
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    CurrentAction = RequestDocumentAction.Completed,
                                    ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    DocumentId = request.Id,
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


                                return returnData;
                            }
                        }
                        else
                        {
                            throw new BadRequestException("Document is not of type 'SiteTravel'.");
                        }
                    }
                    else {
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


        public async Task<CompleteRequestDocumentSiteTravelRemoveResponse?> RemoveSchedule(int documentId, int employeeId, CancellationToken cancellationToken)
        {
            //  await Context.RequestDocument.Where(x => x.Id == documentId).FirstOrDefaultAsync();

            var returnData = new CompleteRequestDocumentSiteTravelRemoveResponse();
            var currentRemoveScheduleData = await Context.RequestSiteTravelRemove.Where(x => x.DocumentId == documentId).Select(x=> new {x.ShiftId, x.Reason, x.DocumentId, x.FirstScheduleId, x.LastScheduleId, x.RoomId, x.FirstScheduleNoShow, x.LastScheduleNoShow  }).FirstOrDefaultAsync(cancellationToken);

            if (currentRemoveScheduleData != null)
            {
                int startScheduleId = currentRemoveScheduleData.FirstScheduleId;
                int endScheduleId = currentRemoveScheduleData.LastScheduleId;

                var startSchedule = await Context.Transport.Where(x => x.ScheduleId == startScheduleId && x.EmployeeId == employeeId).FirstOrDefaultAsync(cancellationToken);
                var endSchedule = await Context.Transport.Where(x => x.ScheduleId == endScheduleId && x.EmployeeId == employeeId).FirstOrDefaultAsync(cancellationToken);

                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == employeeId).AsNoTracking().FirstOrDefaultAsync();

                if (startSchedule != null && endSchedule != null)
                {

                    if (startSchedule.EventDate.Value.Date == endSchedule.EventDate.Value.Date)
                    {

                        startSchedule.ChangeRoute = $"Request remove schedule #{currentRemoveScheduleData.DocumentId}";
                        endSchedule.ChangeRoute = $"Request remove schedule #{currentRemoveScheduleData.DocumentId}";



                        if (currentRemoveScheduleData.FirstScheduleNoShow.HasValue)
                        {
                            if (currentRemoveScheduleData.FirstScheduleNoShow.Value == 1)
                            {

                                var FirstScheduleTransportData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == startScheduleId)
                                                                        join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                                        select new
                                                                        {
                                                                            TransportCode = activetransport.Code,
                                                                            ScheduleCode = schedule.Code

                                                                        }).FirstOrDefaultAsync();



                                var inschedulegoshow = new TransportNoShow
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = currentEmployee.Id,
                                    Direction = startSchedule.Direction,
                                    EventDate = startSchedule.EventDate,
                                    EventDateTime = startSchedule.EventDateTime,
                                    Reason = currentRemoveScheduleData?.Reason,
                                    Description = $"{FirstScheduleTransportData?.TransportCode} {FirstScheduleTransportData?.ScheduleCode}",
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    


                                };
                                Context.TransportNoShow.Add(inschedulegoshow);
                            }
                        }



                        if (currentRemoveScheduleData.LastScheduleNoShow.HasValue)
                        {
                            if (currentRemoveScheduleData.LastScheduleNoShow.Value == 1)
                            {

                                var LasttScheduleTransportData = await(from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == endScheduleId)
                                                                        join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                                        select new
                                                                        {
                                                                            TransportCode = activetransport.Code,
                                                                            ScheduleCode = schedule.Code

                                                                        }).FirstOrDefaultAsync();
                                var inschedulegoshow = new TransportNoShow
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = currentEmployee?.Id,
                                    Direction = startSchedule.Direction,
                                    EventDate = startSchedule.EventDate,
                                    EventDateTime = startSchedule.EventDateTime,
                                    Reason = currentRemoveScheduleData?.Reason,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    Description = $"{LasttScheduleTransportData?.TransportCode} {LasttScheduleTransportData?.ScheduleCode}",


                                };
                                Context.TransportNoShow.Add(inschedulegoshow);
                            }
                        }






                        Context.Transport.Remove(startSchedule);
                        Context.Transport.Remove(endSchedule);
                        
                        returnData.StartScheduleId = startSchedule.ScheduleId;
                        returnData.StartScheduleId = endSchedule.ScheduleId;

                        return returnData;


                    }
                    else {
                        if (startSchedule.Direction == "IN")
                        {
                            var startDate = startSchedule.EventDate.Value.Date;
                            var endDate = endSchedule.EventDate.Value.Date;
                            var employeeStatues = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date >= startDate.Date
                            && x.EventDate.Value.Date <= endDate.Date
                            && x.EmployeeId == employeeId).ToListAsync();


                            DateTime? currentDate = startDate.Date;

                            while (currentDate.Value.Date < endDate.Date)
                            {
                                var item = employeeStatues.Where(c => c.EventDate.Value.Date == currentDate.Value.Date).FirstOrDefault();
                                if (item != null)
                                {
                                    item.DateUpdated = DateTime.Now;
                                    item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    item.ShiftId = currentRemoveScheduleData.ShiftId;
                                    item.RoomId = null;
                                    item.BedId = null;
                                    item.ChangeRoute = $"Request remove schedule #{documentId}";
                                    Context.EmployeeStatus.Update(item);
                                }
                                else
                                {
                                    var newRecord = new EmployeeStatus
                                    {
                                        Active = 1,
                                        DateCreated = DateTime.Now,
                                        ShiftId = currentRemoveScheduleData.ShiftId,
                                        ChangeRoute = $"Request remove schedule #{documentId}",
                                        EmployeeId = employeeId,
                                        RoomId = null,
                                        BedId = null,
                                        CostCodeId = currentEmployee?.CostCodeId,
                                        DepId = currentEmployee.DepartmentId,
                                        EmployerId = currentEmployee.EmployerId,
                                        PositionId = currentEmployee.PositionId,
                                        EventDate = currentDate.Value.Date,
                                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                                    };

                                    Context.EmployeeStatus.Add(newRecord);
                                }

                                currentDate = currentDate.Value.AddDays(1);
                            }


                            var employeeTransports = await Context.Transport.AsNoTracking()
                                   .Where(x => x.EventDate > startDate.Date
                                   && x.EventDate.Value.Date < endDate.Date
                                   && x.EmployeeId == employeeId).ToListAsync(cancellationToken);
                            foreach (var item in employeeTransports)
                            {

                                item.ChangeRoute = $"Request remove schedule #{documentId}";
                                Context.Transport.Remove(item);
                            }

                            startSchedule.ChangeRoute = $"Request remove schedule #{documentId}";
                            endSchedule.ChangeRoute = $"Request remove schedule #{documentId}";

                            returnData.StartScheduleId = startSchedule.ScheduleId;
                            returnData.EndScheduleId = endSchedule.ScheduleId;


                            if (currentRemoveScheduleData.FirstScheduleNoShow.HasValue)
                            {
                                if (currentRemoveScheduleData.FirstScheduleNoShow.Value == 1)
                                {


                                    var FirstScheduleTransportData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == startScheduleId)
                                                                            join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                                            select new
                                                                            {
                                                                                TransportCode = activetransport.Code,
                                                                                ScheduleCode = schedule.Code

                                                                            }).FirstOrDefaultAsync();

                                    var inschedulenoshow = new TransportNoShow
                                    {
                                        Active = 1,
                                        DateCreated = DateTime.Now,
                                        EmployeeId = currentEmployee.Id,
                                        Direction = startSchedule.Direction,
                                        EventDate = startSchedule.EventDate,
                                        EventDateTime = startSchedule.EventDateTime,
                                        Description = $"{FirstScheduleTransportData?.TransportCode} {FirstScheduleTransportData?.ScheduleCode}",
                                        Reason = currentRemoveScheduleData.Reason,
                                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,


                                    };
                                    Context.TransportNoShow.Add(inschedulenoshow);
                                }
                            }



                            if (currentRemoveScheduleData.LastScheduleNoShow.HasValue)
                            {
                                if (currentRemoveScheduleData.LastScheduleNoShow.Value == 1)
                                {

                                  //  var LasttScheduleTransportData = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == endSchedule.ActiveTransportId).Select(x => new { x.Code }).FirstOrDefaultAsync();

                                    var LasttScheduleTransportData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id ==  endScheduleId)
                                                                            join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                                            select new
                                                                            {
                                                                                TransportCode = activetransport.Code,
                                                                                ScheduleCode = schedule.Code

                                                                            }).FirstOrDefaultAsync();

                                    var inschedulegoshow = new TransportNoShow
                                    {
                                        Active = 1,
                                        DateCreated = DateTime.Now,
                                        EmployeeId = currentEmployee.Id,
                                        Direction = endSchedule.Direction,
                                        EventDate = endSchedule.EventDate,
                                        EventDateTime = endSchedule.EventDateTime,
                                        Reason = currentRemoveScheduleData.Reason,
                                        Description = $"{LasttScheduleTransportData.TransportCode} {LasttScheduleTransportData.ScheduleCode}",
                                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,


                                    };
                                    Context.TransportNoShow.Add(inschedulegoshow);
                                }
                            }



                            Context.Transport.Remove(startSchedule);
                            Context.Transport.Remove(endSchedule);

                            return returnData;

                        }
                        else
                        {
                            var startDate = startSchedule.EventDate.Value.Date;
                            var endDate = endSchedule.EventDate.Value.Date;
                            var employeeStatues = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date >= startDate.Date
                            && x.EventDate.Value.Date <= endDate.Date
                            && x.EmployeeId == employeeId).ToListAsync();

                            int RoomId = 0;
                            bool virtulRoom = false;

                            if (currentRemoveScheduleData.RoomId != null || currentRemoveScheduleData.RoomId > 0)
                            {
                                RoomId = currentRemoveScheduleData.RoomId.Value;
                            }
                            else
                            {
                                RoomId = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).Select(x => x.Id).FirstOrDefaultAsync();
                                virtulRoom = true;
                            }


                            var RoomStatus = await CheckRoomDate(RoomId, startDate.Date, endDate.Date);



                            DateTime currentDate = startDate.Date;

                            while (currentDate.Date < endDate.Date)
                            {
                                int? bedId = null;

                                if (virtulRoom)
                                {
                                    bedId = await GetBedId(currentDate.Date, RoomId, employeeId);
                                }


                                var item = employeeStatues.Where(c => c.EventDate.Value.Date == currentDate.Date).FirstOrDefault();
                                if (item != null)
                                {
                                    item.DateUpdated = DateTime.Now;
                                    item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    item.ShiftId = currentRemoveScheduleData.ShiftId;
                                    item.RoomId = currentRemoveScheduleData.RoomId;
                                    item.BedId = virtulRoom == true ? null : bedId;
                                    item.ChangeRoute = $"Request remove schedule #{documentId}";
                                    Context.EmployeeStatus.Update(item);
                                }
                                else
                                {
                                    var newRecord = new EmployeeStatus
                                    {
                                        Active = 1,
                                        DateCreated = DateTime.Now,
                                        ShiftId = currentRemoveScheduleData.ShiftId,
                                        ChangeRoute = $"Request remove schedule #{documentId}",
                                        EmployeeId = employeeId,
                                        RoomId = currentRemoveScheduleData.RoomId,
                                        BedId = RoomStatus ? bedId : null,
                                        CostCodeId = currentEmployee?.CostCodeId,
                                        DepId = currentEmployee?.DepartmentId,
                                        EmployerId = currentEmployee?.EmployerId,
                                        PositionId = currentEmployee?.PositionId,
                                        EventDate = currentDate.Date,
                                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                                    };

                                    Context.EmployeeStatus.Add(newRecord);
                                }

                                currentDate = currentDate.AddDays(1);
                            }



                            var employeeTransports = await Context.Transport
                                   .Where(x => x.EventDate > startDate.Date
                                   && x.EventDate.Value.Date < endDate.Date
                                   && x.EmployeeId == employeeId).ToListAsync(cancellationToken);
                            foreach (var item in employeeTransports)
                            {

                                item.ChangeRoute = $"Remove #{currentRemoveScheduleData.DocumentId}";
                                Context.Transport.Remove(item);
                            }

                            startSchedule.ChangeRoute = $"Remove #{currentRemoveScheduleData.DocumentId}";
                            endSchedule.ChangeRoute = $"Remove #{currentRemoveScheduleData.DocumentId}";

                            returnData.StartScheduleId = startSchedule.ScheduleId;
                            returnData.EndScheduleId = endSchedule.ScheduleId;

                            Context.Transport.Remove(startSchedule);
                            Context.Transport.Remove(endSchedule);



                            return returnData;
                        }

                        return null;
                    }


                   
                }

                return null ;
            }

            return null;
        }

        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, DateTime endDate)
        {
            if (roomId == 0)
            {
                return false;
            }
            var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roomId);

            if (currentRoom == null)
            {
                return false;
            }

            if (currentRoom.VirtualRoom == 1)
            {
                return true;
            }

            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                var dateCountRoom = await Context.EmployeeStatus.AsNoTracking()
                    .Where(x => x.EventDate.Value.Date == currentDate.Date && x.RoomId == roomId)
                    .CountAsync();

                if (currentRoom.BedCount <= dateCountRoom)
                {
                    return false;
                }

                currentDate = currentDate.AddDays(1);
            }

            return true;
        }




        private async Task<int> GetBedId(DateTime eventDate, int RoomId, int employeeId)
        {
            var currentRoom = await Context.Room.AsNoTracking()
                .Where(x => x.Id == RoomId).Select(x => new { x.BedCount }).FirstOrDefaultAsync();


            var dateRoomEmployees =await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EventDate.Value.Date == eventDate.Date
                && x.RoomId == RoomId).Select(x => x.BedId).ToListAsync();
            int localDateEmployees = 0;
            var localEmployees = Context.EmployeeStatus.Local
                .Where(x => x.EmployeeId != employeeId && x.RoomId == RoomId && x.EventDate.Value.Date == eventDate.Date).ToList();
            foreach (var item in localEmployees)
            {
                if (Context.Entry(item).State == EntityState.Added)
                {
                    dateRoomEmployees.Add(item.BedId);
                    localDateEmployees++;
                }
            }
            if (dateRoomEmployees.Count > 0)
            {
                var activeBedId = await Context.Bed.AsNoTracking()
                .Where(x => x.RoomId == RoomId && !dateRoomEmployees.Contains(x.Id))
                .OrderBy(x => x.Id).Select(x => new { x.Id }).FirstOrDefaultAsync();
                if (activeBedId != null)
                {
                    return activeBedId.Id;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                var activeBedId = await Context.Bed.AsNoTracking().Where(x => x.RoomId == RoomId)
                .Select(x => new { x.Id }).FirstOrDefaultAsync();
                if (activeBedId != null)
                {
                    return activeBedId.Id;
                }
                else
                {
                    return 0;
                }
            }
        }



        #endregion


        #region Check

        public async Task<List<CheckRequestDocumentSiteTravelRemoveResponse>> CheckRequestDocumentSiteTravelRemove(CheckRequestDocumentSiteTravelRemoveRequest request, CancellationToken cancellationToken)
        {
            var notActiveDocumentAction = new List<string> { RequestDocumentAction.Cancelled, RequestDocumentAction.Completed };
         //   var travelData = await Context.RequestSiteTravelRemove.Where(x => x.EmployeeId == request.EmployeeId && x.FirstScheduleId == request.FirstScheduleId && x.LastScheduleId == request.LastScheduleId).FirstOrDefaultAsync();


            var currentFirstScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.FirstScheduleId).FirstOrDefaultAsync();
            var currentLastScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.LastScheduleId).FirstOrDefaultAsync();


            if (currentFirstScheduleData != null && currentLastScheduleData != null)
            {
                var firstScheduleIds = await Context.TransportSchedule.AsNoTracking().Where(x => x.EventDate.Date == currentFirstScheduleData.EventDate.Date).Select(x => x.Id).ToListAsync();

                var lastScheduleIds = await Context.TransportSchedule.AsNoTracking().Where(x => x.EventDate.Date == currentLastScheduleData.EventDate.Date).Select(x => x.Id).ToListAsync();

                var travelData = await Context.RequestSiteTravelRemove.Where(x => x.EmployeeId == request.EmployeeId && firstScheduleIds.Contains(x.FirstScheduleId) && lastScheduleIds.Contains(x.LastScheduleId)).FirstOrDefaultAsync();
                if (travelData != null)
                {
                    var returnData = await (from doc in Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                                            && x.DocumentTag == "REMOVE"
                                           && !notActiveDocumentAction.Contains(x.CurrentAction)
                                          && x.DateCreated.Value.Date == DateTime.Today.Date && x.EmployeeId == request.EmployeeId)
                                            join assignedEmployee in Context.Employee.AsNoTracking() on doc.AssignedEmployeeId equals assignedEmployee.Id into assignedEmployeeData
                                            from assignedEmployee in assignedEmployeeData.DefaultIfEmpty()

                                            join requestEmployee in Context.Employee.AsNoTracking() on doc.UserIdCreated equals requestEmployee.Id into requestEmployeeData
                                            from requestEmployee in requestEmployeeData.DefaultIfEmpty()
                                            select new CheckRequestDocumentSiteTravelRemoveResponse
                                            {
                                                Id = doc.Id,
                                                AssignedEmployee = assignedEmployee != null ? $"{assignedEmployee.Firstname} {assignedEmployee.Lastname}" : string.Empty,
                                                CurrentAction = doc.CurrentAction,
                                                Description = requestEmployee != null ? $"{requestEmployee.Firstname}  {requestEmployee.Lastname}" : string.Empty,
                                                CreatedAt = doc.DateCreated,
                                                DocumentTag = doc.DocumentTag,
                                                DocumentType = doc.DocumentType
                                            }
                                        ).ToListAsync();
                    return returnData;


                }
                else
                {
                    return new List<CheckRequestDocumentSiteTravelRemoveResponse>();
                }


            }
            else
            {
                return new List<CheckRequestDocumentSiteTravelRemoveResponse>();
            }
            

        }


        #endregion
    }

}
