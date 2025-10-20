using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Reflection.Metadata;
using System.Threading;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelRemove;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelReschedule;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelReschedule;
using tas.Application.Features.TransportFeature.ReScheduleUpdate;
using tas.Application.Features.TransportFeature.SearchReSchedulePeople;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public sealed class RequestSiteTravelRescheduleRepository : BaseRepository<RequestSiteTravelReschedule>, IRequestSiteTravelRescheduleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly CacheService _memoryCache;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        public RequestSiteTravelRescheduleRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, ICheckDataRepository checkDataRepository, CacheService memoryCache, ITransportCheckerRepository transportCheckerRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _checkDataRepository = checkDataRepository;
            _memoryCache = memoryCache;
            _transportCheckerRepository = transportCheckerRepository;
        }




        #region CreateAddTravel
        public async Task<int> CreateRequestDocumentSiteTravelReschedule(CreateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            if (await ValidateSiteTravelRescheduleCheck(request))
            {
                var virtualRoom = await Context.Room.AsNoTracking().Where(c => c.VirtualRoom == 1).FirstOrDefaultAsync();
                if (virtualRoom == null) {
                    throw new BadRequestException("Virtual room not found");
                }

                var reqDocument = new RequestDocument();
                reqDocument.CurrentAction = request.documentData.Action;
                reqDocument.Active = 1;
                reqDocument.DateCreated = DateTime.Now;
                reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqDocument.Description = reqDocument.Description;
                reqDocument.EmployeeId = request.documentData.EmployeeId;
                reqDocument.DocumentType = RequestDocumentType.SiteTravel;
                reqDocument.DocumentTag = "RESCHEDULE";
                reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
                reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;
               

                Context.RequestDocument.Add(reqDocument);
                await Context.SaveChangesAsync();
                await CreateSaveAttachment(request.Files, reqDocument.Id);

                var employeeRoomId = await GetActiveRoomId(request);

                
                if (employeeRoomId.HasValue)
                {
                    await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, employeeRoomId);
                }
                else
                {

                    await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, virtualRoom.Id);
                }
                

                await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                await Context.SaveChangesAsync();
                return reqDocument.Id;
            }
            else {
                return 0;
            }

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
            else
            {
                return string.Empty;
            }

        }


        private async Task<bool> ValidateSiteTravelRescheduleCheck(CreateRequestDocumentSiteTravelRescheduleRequest request)
        {
            var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.flightData.shiftId).FirstOrDefaultAsync();
            if (currentShift != null) {
                if (currentShift.OnSite == 1)
                {
                    var currentExisitingSchedule =await Context.TransportSchedule.AsNoTracking().Where(x=> x.Id == request.flightData.existingScheduleId).FirstOrDefaultAsync();

                    var currentReSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.flightData.reScheduleId).FirstOrDefaultAsync();

                    if (currentExisitingSchedule != null && currentReSchedule != null)
                    {
                        var currentExisitingScheduleActiveTransport = await Context.ActiveTransport.Where(x => x.Id == currentExisitingSchedule.ActiveTransportId).FirstOrDefaultAsync();
                        var currentReScheduleActiveTransport = await Context.ActiveTransport.Where(x => x.Id == currentReSchedule.ActiveTransportId).FirstOrDefaultAsync();

                        if (currentExisitingScheduleActiveTransport != null && currentReScheduleActiveTransport != null)
                        {
                            if (currentReScheduleActiveTransport.Direction == currentExisitingScheduleActiveTransport.Direction)
                            {

                                if (currentExisitingSchedule?.EventDate.Date > currentReSchedule?.EventDate.Date)
                                {
                                   var currenttransport =await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == request.flightData.existingScheduleId && x.EmployeeId == request.documentData.EmployeeId).FirstOrDefaultAsync();
                                    if (currenttransport != null)
                                    {
                                        await _transportCheckerRepository.TransportRescheduleValidDirectionSequenceCheck(currenttransport.Id, request.flightData.reScheduleId);
                                        return true;

                                    }
                                    else {
                                        return false;
                                    }

                                    
                                }
                                else
                                {
                                    var currenttransport = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == request.flightData.existingScheduleId && x.EmployeeId == request.documentData.EmployeeId).FirstOrDefaultAsync();
                                    if (currenttransport != null)
                                    {
                                        await _transportCheckerRepository.TransportRescheduleValidDirectionSequenceCheck(currenttransport.Id, request.flightData.reScheduleId);
                                        return true;

                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else {
                                return false;
                            }

                        }
                        else {
                            return false;
                        } 


                    }
                    else {
                        return false;
                    }


                }
                else {


                    var currentExisitingSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.flightData.existingScheduleId).FirstOrDefaultAsync();

                    var currentReSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.flightData.reScheduleId).FirstOrDefaultAsync();

                    if (currentExisitingSchedule != null && currentReSchedule != null)
                    {
                        var currentExisitingScheduleActiveTransport = await Context.ActiveTransport.Where(x => x.Id == currentExisitingSchedule.ActiveTransportId).FirstOrDefaultAsync();
                        var currentReScheduleActiveTransport = await Context.ActiveTransport.Where(x => x.Id == currentReSchedule.ActiveTransportId).FirstOrDefaultAsync();

                        if (currentExisitingScheduleActiveTransport != null && currentReScheduleActiveTransport != null)
                        {
                            if (currentReScheduleActiveTransport.Direction == currentExisitingScheduleActiveTransport.Direction)
                            {

                                if (currentExisitingSchedule?.EventDate.Date > currentReSchedule?.EventDate.Date)
                                {
                                    var currenttransport = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == request.flightData.existingScheduleId && x.EmployeeId == request.documentData.EmployeeId).FirstOrDefaultAsync();
                                    if (currenttransport != null)
                                    {
                                        await _transportCheckerRepository.TransportRescheduleValidDirectionSequenceCheck(currenttransport.Id, request.flightData.reScheduleId);
                                        return true;

                                    }
                                    else
                                    {
                                        return false;
                                    }


                                }
                                else
                                {
                                    var currenttransport = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == request.flightData.existingScheduleId && x.EmployeeId == request.documentData.EmployeeId).FirstOrDefaultAsync();
                                    if (currenttransport != null)
                                    {
                                        await _transportCheckerRepository.TransportRescheduleValidDirectionSequenceCheck(currenttransport.Id, request.flightData.reScheduleId);
                                        return true;

                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }


                    }
                    else
                    {
                        return false;
                    }




                    //var currentExisitingSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.flightData.existingScheduleId).FirstOrDefaultAsync();

                    //var currentReSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.flightData.reScheduleId).FirstOrDefaultAsync();


                    //if (currentExisitingSchedule?.EventDate.Date > currentReSchedule?.EventDate.Date)
                    //{
                    //    return true;
                    //}
                    //else
                    //{
                    //    return true;
                    //}
                }
            }

            return false;
        }



        private async Task CreateSaveFlightData(CreateRequestDocumentSiteTravelRescheduleData travelData, int employeeId, int documentId, int? roomId)
        {

            string? existingScheduleIdDescr =await GetScheduleIdDescriptionData(travelData.existingScheduleId);
            var virtualRoom =await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
            string? reScheduleIdDescr = await GetScheduleIdDescriptionData(travelData.reScheduleId);
            var travelRoomDataId = virtualRoom?.Id;
            if (roomId.HasValue) {
                travelRoomDataId = roomId;
            }


            var reScheduleData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == travelData.reScheduleId)
                                     join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                     select new
                                     {
                                         EventDate = schedule.EventDate,
                                         Direction = activetransport.Direction
                                     }).FirstOrDefaultAsync();



            var existingScheduleData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == travelData.existingScheduleId)
                                        join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                        select new
                                        {
                                            EventDate = schedule.EventDate,
                                            Direction = activetransport.Direction
                                        }).FirstOrDefaultAsync();

            if (reScheduleData?.Direction == "OUT")
            {
                if (reScheduleData.EventDate >= existingScheduleData?.EventDate.Date)
                {
                   var lastRoomData =await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.EventDate <= existingScheduleData.EventDate && x.RoomId != null).OrderByDescending(x=> x.EventDate).FirstOrDefaultAsync();
                    if (lastRoomData != null) {
                        travelRoomDataId = lastRoomData?.RoomId;
                    }
                }
                
            }

            if (reScheduleData?.Direction == "IN")
            {
                if (reScheduleData.EventDate <= existingScheduleData?.EventDate.Date)
                {
                    var lastRoomData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.EventDate >= existingScheduleData.EventDate && x.RoomId != null).OrderBy(x => x.EventDate).FirstOrDefaultAsync();
                    if (lastRoomData != null)
                    {
                        travelRoomDataId = lastRoomData?.RoomId;
                    }
                }

            }



            var newRecord = new RequestSiteTravelReschedule
            {
                DocumentId = documentId,
                EmployeeId = employeeId,
                Active = 1,
                RoomId = travelRoomDataId,
                ExistingScheduleId = travelData.existingScheduleId,
                ExistingScheduleNoShow = travelData.ExistingScheduleIdNoShow,
                ReScheduleGoShow = travelData.ReScheduleGoShow,
                ReScheduleId = travelData.reScheduleId,
                ShiftId = travelData.shiftId,
                Reason = travelData.Reason,
                DateCreated = DateTime.Now,
                ExistingScheduleIdDescr = existingScheduleIdDescr,
                ReScheduleIdDescr = reScheduleIdDescr,
                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
            };
            Context.RequestSiteTravelReschedule.Add(newRecord);
            await Task.CompletedTask;
        }
        //CreateRequestDocumentSiteTravelAttachment
        private async Task CreateSaveAttachment(List<CreateRequestDocumentSiteTravelRescheduleAttachment> attachments, int documentId)
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

        private async Task SaveDocumentHistory(CreateRequestDocumentSiteTravelRescheduleRequest request, int DocumentId, CancellationToken cancellationToken)
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


        private async Task<int?> GetActiveRoomId(CreateRequestDocumentSiteTravelRescheduleRequest request)
        {
            var currentEmployee = await Context.Employee
                .Where(x => x.Id == request.documentData.EmployeeId)
                .FirstOrDefaultAsync();

            if (currentEmployee?.RoomId.HasValue ?? false)
            {
                var inSchedule = await Context.TransportSchedule.Where(x => x.Id == request.flightData.existingScheduleId).FirstOrDefaultAsync();
                var outSchedule = await Context.TransportSchedule.Where(x => x.Id == request.flightData.reScheduleId).FirstOrDefaultAsync();


                if (inSchedule != null && outSchedule != null)
                {

                    var currentTransport = await Context.Transport.Where(x => x.Direction == "IN" && x.Id == inSchedule.ActiveTransportId).FirstOrDefaultAsync();

                    DateTime startDate = inSchedule.EventDate.Date;
                    DateTime endDate = outSchedule.EventDate.Date;

                    var roomstatus = true;

                    for (DateTime currentDate = startDate; currentDate < endDate; currentDate = currentDate.AddDays(1))
                    {
                        var roomDateStatus = await GetAvailableBeds(currentEmployee.RoomId.Value, currentDate);

                        if (!roomDateStatus)
                        {
                            roomstatus = false;
                            break;
                        }
                    }

                    if (roomstatus)
                    {
                        return currentEmployee.RoomId.Value;
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
            else
            {
                return null;
            }
        }

        public async Task<bool> GetAvailableBeds(int roomId, DateTime eventDate)
        {
            var statusRoomDate = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date == eventDate.Date && x.RoomId == roomId).ToListAsync();

            if (statusRoomDate != null)
            {
                var currentRoom = await Context.Room.FirstOrDefaultAsync(x => x.Id == roomId);

                if (currentRoom != null)
                {
                    if (currentRoom.VirtualRoom == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return currentRoom.BedCount - statusRoomDate.Count > 0;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        #endregion

        #region UpdateRescheduleTravel

        public async Task<int> UpdateRequestDocumentSiteTravelReschedule(UpdateRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestSiteTravelReschedule.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                string? existingScheduleIdDescr = await GetScheduleIdDescriptionData(request.ExistingScheduleId);

                string? reScheduleIdDescr = await GetScheduleIdDescriptionData(request.ReScheduleId);

                var virtulRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();

                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentData.ExistingScheduleId = request.ExistingScheduleId;
                currentData.ReScheduleId = request.ReScheduleId;
                if (request.RoomId.HasValue) {
                    currentData.RoomId = request.RoomId;
                }
                
                currentData.ShiftId = request.shiftId;
                currentData.ReScheduleIdDescr = reScheduleIdDescr;
                currentData.ExistingScheduleIdDescr = existingScheduleIdDescr;
                currentData.ExistingScheduleNoShow = request.ExistingScheduleIdNoShow;
                currentData.ReScheduleGoShow = request.ReScheduleGoShow;


                Context.RequestSiteTravelReschedule.Update(currentData);

                await SaveDocumentHistoryTravelUpdate(currentData.DocumentId);


                return currentData.DocumentId;
            }

            return 0;


        }



        private async Task SaveDocumentHistoryTravelUpdate(int DocumentId)
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

        #region GetRequest
        public async Task<GetRequestDocumentSiteTravelRescheduleResponse> GetRequestDocumentSiteTravelReschedule(GetRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetRequestDocumentSiteTravelRescheduleResponse();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync(cancellationToken);
                var assignEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync(cancellationToken);
                var updatedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdUpdated).FirstOrDefaultAsync(cancellationToken);
                var createdEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdCreated).FirstOrDefaultAsync(cancellationToken);


                var deleagationEmployee = await Context.RequestDelegates.Where(x => x.FromEmployeeId == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync();
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



                var currentTravelData = await Context.RequestSiteTravelReschedule.AsNoTracking().Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                if (currentTravelData != null)
                {

                    var travelData = new GetRequestDocumentSiteTravelRescheduleTravel();

                    if (currentTravelData.RoomId.HasValue)
                    {
                        var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == currentTravelData.RoomId.Value).FirstOrDefaultAsync();
                        travelData.RoomTypeId = currentRoom?.RoomTypeId;
                        travelData.Reason = currentTravelData.Reason;
                        travelData.RoomId = currentTravelData.RoomId;
                        travelData.RoomnNumber = currentRoom?.Number;
                        travelData.CampId = currentRoom?.CampId;

                    }




                    var existingscheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentTravelData.ExistingScheduleId).FirstOrDefaultAsync();
                    if (existingscheduleData != null)
                    {
                        travelData.ExistingScheduleId = existingscheduleData.Id;
                        travelData.ExistingScheduleDate = existingscheduleData.EventDate;
                       // var inscheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == existingscheduleData.ActiveTransportId).FirstOrDefaultAsync();

                        var inscheduleTransport = await (from transport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == existingscheduleData.ActiveTransportId)
                                                         join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                         from tmode in modeData.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             Code = transport.Code,
                                                             Direction = transport.Direction,
                                                             TransportMode = tmode.Code,
                                                             fromLocationId = transport.fromLocationId,
                                                             toLocationId = transport.toLocationId,

                                                         }).FirstOrDefaultAsync();

                        if (inscheduleTransport != null)
                        {
                            travelData.ExistingScheduleDirection = inscheduleTransport.Direction;
                            // travelData.ExistingScheduleDescription = existingscheduleData.Description;
                            travelData.ExistingFromLocationid = inscheduleTransport.fromLocationId;
                            travelData.ExistingToLocationid = inscheduleTransport.toLocationId;
                            travelData.ExistingScheduleDescription  = $"{inscheduleTransport.Code} {existingscheduleData.Description} {inscheduleTransport.TransportMode}";
                            travelData.ExistingTransportMode =  inscheduleTransport.TransportMode;
                        }


                    }

                    var ReScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentTravelData.ReScheduleId).FirstOrDefaultAsync();
                    if (ReScheduleData != null)
                    {
                        travelData.ReScheduleDate = ReScheduleData.EventDate;
                        travelData.ReScheduleId = ReScheduleData.Id;
                       // var ReScheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == ReScheduleData.ActiveTransportId).FirstOrDefaultAsync();

                        var ReScheduleTransport = await (from transport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == ReScheduleData.ActiveTransportId)
                                                         join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                         from tmode in modeData.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             Code = transport.Code,
                                                             Direction = transport.Direction,
                                                             TransportMode = tmode.Code,
                                                             fromLocationId = transport.fromLocationId,
                                                             toLocationId = transport.toLocationId,


                                                         }).FirstOrDefaultAsync();



                        if (ReScheduleTransport != null)
                        {
                            travelData.ReScheduleDirection = ReScheduleTransport.Direction;
                            travelData.ReScheduleDescription = $"{ReScheduleTransport.Code} {ReScheduleData.Description} {ReScheduleTransport.TransportMode}";
                            travelData.ReScheduleFromLocationid = ReScheduleTransport.fromLocationId;
                            travelData.ReScheduleToLocationid = ReScheduleTransport.toLocationId;
                            travelData.ReScheduleTransportMode = ReScheduleTransport.TransportMode;

                        }
      
                    }
                    travelData.Id = currentTravelData.Id;
                    travelData.shiftId = currentTravelData?.ShiftId;
                    travelData.RoomId = currentTravelData?.RoomId;
                    travelData.Reason = currentTravelData?.Reason;
                    travelData.ExistingScheduleIdDescr = currentTravelData?.ExistingScheduleIdDescr;
                    travelData.ReScheduleIdDescr = currentTravelData?.ReScheduleIdDescr;

                    travelData.ExistingScheduleIdNoShow = currentTravelData?.ExistingScheduleNoShow;
                    travelData.ReScheduleGoShow = currentTravelData?.ReScheduleGoShow;






                    returnData.TravelData = travelData;
                   
                }



            }
            return returnData;

        }

        #endregion

        #region Complete
        public async Task<CompleteRequestDocumentSiteTravelRescheduleResponse> CompleteRequestDocumentSiteTravelReschedule(CompleteRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var returnData = new CompleteRequestDocumentSiteTravelRescheduleResponse();
                    await CompleteRequestDocumentSiteTravelRescheduleValidateSequeence(request, cancellationToken);
                    var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                    if (currentDocument != null)
                    {
                        if (currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                        {
                            if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                            {
                                throw new BadRequestException("This task already Completed");
                            }
                            else
                            {
                                currentDocument.CurrentAction = RequestDocumentAction.Completed;
                                currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                                currentDocument.CompletedUserId = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentDocument.CompletedDate = DateTime.Now;
                                Context.RequestDocument.Update(currentDocument);
                                var oldDocumentAssignedGrouId = currentDocument.AssignedRouteConfigId;
                                RequestSiteTravelReschedule currentData = await Context.RequestSiteTravelReschedule.AsNoTracking().Where(rs => rs.DocumentId == request.Id).FirstOrDefaultAsync();
                                var assignedOldGroupId = await Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();
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


                 


                                await ChangeShiftStatus(
                                    shiftId: currentData.ShiftId,
                                    oldScheduled: currentData.ExistingScheduleId,
                                    scheduleId: currentData.ReScheduleId, request.Id, currentDocument.EmployeeId.Value, currentData.RoomId, cancellationToken);

                                returnData.NewScheduleId = currentData.ReScheduleId;
                                returnData.OldScheduleId = currentData.ExistingScheduleId;

                                await  SaveNoGoShow(request.Id, currentDocument.EmployeeId.Value);

                                await transaction.CommitAsync(cancellationToken);

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


        private async Task SaveNoGoShow(int documentId, int employeeId)
        {
            var currentData = await Context.RequestSiteTravelReschedule.AsNoTracking().Where(x => x.DocumentId == documentId).FirstOrDefaultAsync();
            if (currentData != null)
            {
                if (currentData.ExistingScheduleNoShow.HasValue)
                {
                    if (currentData.ExistingScheduleNoShow.Value == 1)
                    {
                        var currentSchedule = await  Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentData.ExistingScheduleId).FirstOrDefaultAsync();

                        if (currentSchedule != null)
                        {
                            var ScheduleTransportData = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == currentSchedule.ActiveTransportId).Select(x => new { x.Code,x.Direction }).FirstOrDefaultAsync();
                            var inschedulegoshow = new TransportNoShow
                            {
                                Active = 1,
                                DateCreated = DateTime.Now,
                                EmployeeId = employeeId,
                                Direction = ScheduleTransportData.Direction,
                                EventDate = currentSchedule.EventDate,
                                EventDateTime = currentSchedule.EventDate,
                                Reason = currentData.Reason,
                                Description = $"{ScheduleTransportData.Code} {currentSchedule.Code}",

                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,


                            };
                            Context.TransportNoShow.Add(inschedulegoshow);
                        }
                            


                    }
                }
                if (currentData.ReScheduleGoShow.HasValue)
                {
                    if (currentData.ReScheduleGoShow.Value == 1)
                    {
                        var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentData.ReScheduleId).FirstOrDefaultAsync();

                        if (currentSchedule != null)
                        {
                            var ScheduleTransportData = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == currentSchedule.ActiveTransportId).Select(x => new { x.Code, x.Direction }).FirstOrDefaultAsync();
                            var inschedulegoshow = new TransportGoShow
                            {
                                Active = 1,
                                DateCreated = DateTime.Now,
                                EmployeeId = employeeId,
                                Direction = ScheduleTransportData.Direction,
                                EventDate = currentSchedule.EventDate,
                                EventDateTime = currentSchedule.EventDate,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                Reason = currentData.Reason,
                                Description = $"{ScheduleTransportData.Code} {currentSchedule.Code}",

                            };
                            Context.TransportGoShow.Add(inschedulegoshow);
                        }



                    }
                }

            }

        }



        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, DateTime endDate, int EmployeeId)
        {
            try
            {
                DateTime sdate = startDate;
                DateTime edate = endDate;
                if (startDate > endDate) { 
                    startDate = endDate;
                    endDate = sdate;
                }

                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roomId);

                if (currentRoom == null)
                {
                    return false;
                }

                if (currentRoom.VirtualRoom > 0)
                {
                    return true;
                }

                var currentDate = startDate;

                var sessionRoomCount = await Context.EmployeeStatus.AsNoTracking()
                        .Where(x => x.EventDate.Value.Date >= startDate.Date && x.EventDate <= endDate.Date && x.RoomId == roomId && x.EmployeeId != EmployeeId)
                        .CountAsync();

                if (sessionRoomCount == 0)
                {
                    return true;
                }

                while (currentDate <= endDate)
                {
                    var dateCountRoom = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId != EmployeeId).AsNoTracking()
                        .Where(x => x.EventDate.Value.Date == currentDate.Date && x.RoomId == roomId)
                        .CountAsync();



                    if (currentDate == startDate)
                    {
                        if (currentRoom.BedCount <= dateCountRoom)
                        {
                            var onsiteEmployees = await Context.EmployeeStatus.AsNoTracking().Where(c => c.RoomId == roomId && c.EventDate.Value.Date == currentDate.Date && c.EmployeeId != EmployeeId).Select(x => x.EmployeeId).ToListAsync();
                            if (onsiteEmployees.Count > 0)
                            {
                                var tomorrowOut = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date == currentDate.AddDays(1).Date && onsiteEmployees.Contains(x.EmployeeId) && x.Direction == "OUT").ToListAsync();
                                if (tomorrowOut.Count == 0)
                                {
                                    return false;
                                }
                            }
                        }

                        currentDate = currentDate.AddDays(1);
                    }
                    else
                    {
                        if (currentRoom.BedCount <= dateCountRoom)
                        {
                            return false;
                        }

                        currentDate = currentDate.AddDays(1);
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        public async Task CompleteRequestDocumentSiteTravelRescheduleValidateSequeence(CompleteRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                {
                    if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {
                        throw new BadRequestException("This task already Completed");
                    }
                    else
                    {
                        var currentData = await Context.RequestSiteTravelReschedule.AsNoTracking().Where(rs => rs.DocumentId == request.Id).FirstOrDefaultAsync();
                        var currentTransport = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == currentData.ExistingScheduleId && x.EmployeeId == currentData.EmployeeId).FirstOrDefaultAsync();
                        if (currentData != null && currentTransport != null)
                        {
                            await _transportCheckerRepository.TransportRescheduleValidDirectionSequenceCheck(currentTransport.Id, currentData.ReScheduleId);
                        }
                        else {
                            throw new BadRequestException("Transport schedule deleted or oldtransport deleted. Please try again another request");
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }

        private async Task ChangeShiftStatus(int shiftId, int oldScheduled, int scheduleId, int documentId, int employeeId, int? RoomId, CancellationToken cancellationToken)
        {
            var currentTransport =await Context.Transport.Where(x => x.ScheduleId == oldScheduled && x.EmployeeId == employeeId).FirstOrDefaultAsync();
            if (currentTransport != null)
            {

                await _checkDataRepository.CheckProfile(currentTransport.EmployeeId.Value, cancellationToken);
                var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == shiftId).FirstOrDefaultAsync();
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentTransport.EmployeeId).FirstOrDefaultAsync();
                var newSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId).FirstOrDefaultAsync();
                var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();

                var currentRoomDataId = virtualRoom.Id;

                if (RoomId.HasValue) {
                    var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == RoomId.Value).FirstOrDefaultAsync();
                    if (currentRoom != null) {
                        currentRoomDataId = currentRoom.Id;
                    }
                }




                if (currentTransport.Direction == "OUT")
                {

                    if (newSchedule?.EventDate.Date > currentTransport?.EventDate.Value.Date)
                    {

                        if (currentShift.OnSite == 1)
                        {
                            var currentDate = currentTransport.EventDate.Value.Date;

                            var nexttransport = await Context.Transport.AsNoTracking()
                                .Where(x => x.EventDate.Value.Date < newSchedule.EventDate.Date && x.EventDate.Value.Date > currentTransport.EventDate.Value.Date
                            && x.EmployeeId == currentTransport.EmployeeId).FirstOrDefaultAsync();
                            if (nexttransport == null)
                            {

                                var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

                                DateTime outtime = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = outtime.Hour;
                                int outminutes = outtime.Minute;




                                var currentRoomCheck = await CheckRoomDate(currentRoomDataId, currentDate, newSchedule.EventDate.Date, currentTransport.EmployeeId.Value);


                                if (!currentRoomCheck)
                                {
                                    currentRoomDataId = virtualRoom.Id;
                                        

                                }



                                while (currentDate < newSchedule.EventDate.Date)
                                {
                                    var bedId = await getRoomBedId(currentRoomDataId, currentDate);
                                    var currentDateStatus = await Context.EmployeeStatus
                                        .Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == currentTransport.EmployeeId)
                                        .FirstOrDefaultAsync();
                                    if (currentDateStatus != null)
                                    {
                                        currentDateStatus.RoomId = currentRoomDataId;
                                        currentDateStatus.ShiftId = shiftId;
                                        currentDateStatus.BedId = virtualRoom.Id == currentRoomDataId ? null : bedId;
                                        currentDateStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentDateStatus.ChangeRoute = $"Request reschedule #{documentId}";
                                        Context.EmployeeStatus.Update(currentDateStatus);

                                    }
                                    else
                                    {
                                        var newEmplpyeeStatus = new EmployeeStatus
                                        {
                                            Active = 1,
                                            BedId = virtualRoom.Id == currentRoomDataId ? null : bedId,
                                            RoomId = currentRoomDataId,
                                            DateCreated = DateTime.Now,
                                            EmployeeId = currentTransport.EmployeeId,
                                            PositionId = currentEmployee?.PositionId,
                                            DepId = currentEmployee?.DepartmentId,
                                            EmployerId = currentEmployee?.EmployerId,
                                            ShiftId = currentShift.Id,
                                            EventDate = currentDate.Date,
                                            ChangeRoute = $"Request reschedule #{documentId}",
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                        };
                                        Context.EmployeeStatus.Add(newEmplpyeeStatus);
                                    }

                                    currentDate = currentDate.AddDays(1);
                                }

                                currentTransport.DateCreated = DateTime.Now;
                                currentTransport.DateUpdated = DateTime.Now;

                                currentTransport.ScheduleId = newSchedule.Id;
                                currentTransport.EventDate = newSchedule.EventDate;
                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                currentTransport.EventDateTime = newSchedule.EventDate.AddHours(outhours).AddMinutes(outminutes);




                                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentTransport.ChangeRoute = $"Request reschedule #{documentId}";
                                Context.Transport.Update(currentTransport);


                            }
                            else
                            {
                                throw new BadRequestException($"" +
                                    $"There is a flight to {nexttransport.Direction} on {nexttransport.EventDate.Value.Date.ToShortDateString()} flights can be changed before this date");
                            }
                        }
                        else
                        {
                            throw new BadRequestException($"Onsite type ShiftStatus can be changed");
                        }

                    }
                    else
                    {



                        if (currentShift.OnSite != 1)
                        {
                            var currentDate = newSchedule.EventDate.Date;

                            var beforetransport = await Context.Transport
                             .Where(x => x.EventDate.Value.Date > newSchedule.EventDate.Date && x.EventDate.Value.Date < currentTransport.EventDate.Value.Date
                         && x.EmployeeId == currentTransport.EmployeeId).FirstOrDefaultAsync();
                            if (beforetransport == null)
                            {

                                var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

                                DateTime outtime = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = outtime.Hour;
                                int outminutes = outtime.Minute;



                                while (currentDate < currentTransport.EventDate.Value)
                                {
                                    var currentDateStatus = await Context.EmployeeStatus
                                       .Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == currentTransport.EmployeeId)
                                       .FirstOrDefaultAsync();
                                    if (currentDateStatus != null)
                                    {
                                        currentDateStatus.RoomId = null;
                                        currentDateStatus.BedId = null;
                                        currentDateStatus.ShiftId = shiftId;
                                        currentDateStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentDateStatus.ChangeRoute = $"Request reschedule #{documentId}";
                                        Context.EmployeeStatus.Update(currentDateStatus);

                                    }
                                    else
                                    {
                                        var newEmplpyeeStatus = new EmployeeStatus
                                        {
                                            Active = 1,
                                            BedId = null,
                                            RoomId = null,
                                            DateCreated = DateTime.Now,
                                            EmployeeId = currentTransport.EmployeeId,
                                            PositionId = currentEmployee?.PositionId,
                                            DepId = currentEmployee?.DepartmentId,
                                            EmployerId = currentEmployee?.EmployerId,
                                            ShiftId = currentShift.Id,
                                            EventDate = currentDate.Date,
                                            ChangeRoute = $"Request reschedule #{documentId}",
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                        };
                                        Context.EmployeeStatus.Add(newEmplpyeeStatus);
                                    }

                                    currentDate = currentDate.AddDays(1);
                                }
                                currentTransport.DateCreated = DateTime.Now;
                                currentTransport.ScheduleId = newSchedule.Id;
                                currentTransport.EventDate = newSchedule.EventDate;
                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                currentTransport.EventDateTime = newSchedule.EventDate.AddHours(outhours).AddMinutes(outminutes);
                                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentTransport.ChangeRoute = $"Request reschedule #{documentId}";
                                Context.Transport.Update(currentTransport);
                            }
                            else
                            {
                                throw new BadRequestException($"" +
                                    $"There is a flight to {beforetransport.Direction} on {beforetransport.EventDate.Value.Date.ToShortDateString()} flights can be changed after this date");
                            }
                        }
                        else
                        {
                            throw new BadRequestException($"OffSite type ShiftStatus can be changed");
                        }
                    }

                }
                if (currentTransport.Direction == "IN")
                {
                    if (newSchedule.EventDate.Date > currentTransport.EventDate.Value.Date)
                    {
                        if (currentShift.OnSite != 1)
                        {
                            var currentDate = currentTransport.EventDate.Value.Date;

                            var nexttransport = await Context.Transport
                                .Where(x => x.EventDate.Value.Date < newSchedule.EventDate.Date
                                && x.EventDate.Value.Date > currentDate.Date
                            && x.EmployeeId == currentTransport.EmployeeId).FirstOrDefaultAsync();
                            if (nexttransport == null)
                            {

                                var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

                                DateTime outtime = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = outtime.Hour;
                                int outminutes = outtime.Minute;


                                while (currentDate < newSchedule.EventDate.Date)
                                {
                                    var currentDateStatus = await Context.EmployeeStatus
                                      .Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == currentTransport.EmployeeId)
                                      .FirstOrDefaultAsync();
                                    if (currentDateStatus != null)
                                    {
                                        currentDateStatus.RoomId = null;
                                        currentDateStatus.BedId = null;
                                        currentDateStatus.ShiftId = shiftId;
                                        currentDateStatus.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentDateStatus.ChangeRoute = $"Request reschedule #{documentId}";
                                        Context.EmployeeStatus.Update(currentDateStatus);

                                    }
                                    else
                                    {
                                        var newEmplpyeeStatus = new EmployeeStatus
                                        {
                                            Active = 1,
                                            BedId = null,
                                            RoomId = null,
                                            DateCreated = DateTime.Now,
                                            EmployeeId = currentTransport.EmployeeId,
                                            PositionId = currentEmployee?.PositionId,
                                            DepId = currentEmployee?.DepartmentId,
                                            EmployerId = currentEmployee?.EmployerId,
                                            ShiftId = currentShift.Id,
                                            ChangeRoute =  $"Request reschedule #{documentId}",
                                            EventDate = currentDate.Date,
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                        };
                                        Context.EmployeeStatus.Add(newEmplpyeeStatus);
                                    }

                                    currentDate = currentDate.AddDays(1);
                                }
                                currentTransport.DateCreated = DateTime.Now;
                                currentTransport.ScheduleId = newSchedule.Id;
                                currentTransport.EventDate = newSchedule.EventDate;
                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                currentTransport.EventDateTime = newSchedule.EventDate.AddHours(outhours).AddMinutes(outminutes);
                                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentTransport.ChangeRoute = $"Request reschedule #{documentId}";
                                Context.Transport.Update(currentTransport);
                            }
                            else
                            {
                                throw new BadRequestException($"" +
                                    $"There is a flight to {nexttransport.Direction} on {nexttransport.EventDate.Value.Date.ToShortDateString()} flights can be changed before this date");
                            }
                        }
                        else
                        {
                            throw new BadRequestException($"OffSite type ShiftStatus can be changed");
                        }

                    }
                    else
                    {
                        if (currentShift.OnSite == 1)
                        {
                            var currentDate = currentTransport.EventDate.Value.Date;

                            var beforetransport = await Context.Transport
                             .Where(x => x.EventDate.Value.Date > newSchedule.EventDate.Date
                         && x.EmployeeId == currentTransport.EmployeeId && x.EventDate.Value.Date < currentDate.Date).FirstOrDefaultAsync();
                            if (beforetransport == null)
                            {
                                var currentSchedule =await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

                                DateTime outtime = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = outtime.Hour;
                                int outminutes = outtime.Minute;

                                var currentRoomCheck = await CheckRoomDate(currentRoomDataId, currentDate, newSchedule.EventDate.Date, currentTransport.EmployeeId.Value);


                                if (!currentRoomCheck)
                                {
                                    currentRoomDataId = virtualRoom.Id;


                                }



                                while (currentDate >= newSchedule.EventDate.Date)
                                {
                                    var bedId = await getRoomBedId(currentRoomDataId, currentDate);
                                    var currentDateStatus = await Context.EmployeeStatus
                                        .Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == currentTransport.EmployeeId)
                                        .FirstOrDefaultAsync();
                                    if (currentDateStatus != null)
                                    {
                                        currentDateStatus.RoomId = currentRoomDataId;
                                        currentDateStatus.BedId = currentRoomDataId == virtualRoom.Id ? null : bedId;
                                        currentDateStatus.ShiftId = shiftId;
                                        currentDateStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentDateStatus.ChangeRoute = $"Request reschedule #{documentId}";
                                        Context.EmployeeStatus.Update(currentDateStatus);

                                    }
                                    else
                                    {

                                        var newEmplpyeeStatus = new EmployeeStatus
                                        {
                                            Active = 1,
                                            BedId = currentRoomDataId == virtualRoom.Id ? null : bedId,
                                            RoomId = currentRoomDataId,
                                            DateCreated = DateTime.Now,
                                            EmployeeId = currentTransport.EmployeeId,
                                            PositionId = currentEmployee?.PositionId,
                                            DepId = currentEmployee?.DepartmentId,
                                            EmployerId = currentEmployee?.EmployerId,
                                            ShiftId = currentShift.Id,
                                            EventDate = currentDate.Date,
                                            ChangeRoute = $"Request reschedule #{documentId}",
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                        };
                                        Context.EmployeeStatus.Add(newEmplpyeeStatus);
                                    }

                                    currentDate = currentDate.AddDays(-1);
                                }

                                currentTransport.DateCreated = DateTime.Now;
                                currentTransport.ScheduleId = newSchedule.Id;
                                currentTransport.EventDate = newSchedule.EventDate;
                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                currentTransport.EventDateTime = newSchedule.EventDate.AddHours(outhours).AddMinutes(outminutes);
                                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentTransport.ChangeRoute = $"Request reschedule #{documentId}";
                                Context.Transport.Update(currentTransport);





                            }
                            else
                            {
                                throw new BadRequestException($"" +
                                    $"There is a flight to {beforetransport.Direction} on {beforetransport.EventDate.Value.Date.ToShortDateString()} flights can be changed after this date");
                            }
                        }
                        else
                        {
                            throw new BadRequestException($"Onsite type ShiftStatus can be changed");
                        }

                    }
                }

            }

            await Task.CompletedTask;
        }






        private async Task<int?> getRoomBedId(int roomId, DateTime eventDate)
        {

            var roomBeds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == roomId).OrderBy(x => x.Id).ToListAsync();
            foreach (var item in roomBeds)
            {
                var currentData = await Context.EmployeeStatus.AsNoTracking().FirstOrDefaultAsync(x => x.BedId == item.Id && x.EventDate == eventDate);
                if (currentData == null)
                {
                    return item.Id;
                }
            }
            return null;


        }

        #endregion


        #region CheckDuplicate

        public async Task<List<CheckRequestDocumentSiteTravelRescheduleResponse>> CheckRequestDocumentSiteTravelReschedule(CheckRequestDocumentSiteTravelRescheduleRequest request, CancellationToken cancellationToken)
        {
            var notActiveDocumentAction = new List<string> { RequestDocumentAction.Cancelled, RequestDocumentAction.Completed };

            var currentReScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.reScheduleId).FirstOrDefaultAsync();
            var currentExistingScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.existingScheduleId).FirstOrDefaultAsync();

            if (currentReScheduleData != null && currentExistingScheduleData != null)
            {

                var RescheduleIds = await Context.TransportSchedule.AsNoTracking().Where(x => x.EventDate.Date == currentReScheduleData.EventDate.Date).Select(x => x.Id).ToListAsync();

                var ExistscheduleIds = await Context.TransportSchedule.AsNoTracking().Where(x => x.EventDate.Date == currentExistingScheduleData.EventDate.Date).Select(x => x.Id).ToListAsync();

                var travelData = await Context.RequestSiteTravelReschedule.Where(x => x.EmployeeId == request.EmployeeId && RescheduleIds.Contains(x.ReScheduleId) && ExistscheduleIds.Contains(x.ExistingScheduleId)).FirstOrDefaultAsync();



             //  var travelData = await Context.RequestSiteTravelReschedule.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId && x.ExistingScheduleId == request.existingScheduleId && x.ReScheduleId == request.reScheduleId).FirstOrDefaultAsync();

                if (travelData != null)
                {
                    var returnData = await (from doc in Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                                            && x.DocumentTag == "RESCHEDULE"
                                           && !notActiveDocumentAction.Contains(x.CurrentAction)
                                          && x.DateCreated.Value.Date == DateTime.Today.Date && x.EmployeeId == request.EmployeeId && x.Id == travelData.DocumentId)
                                            join assignedEmployee in Context.Employee.AsNoTracking() on doc.AssignedEmployeeId equals assignedEmployee.Id into assignedEmployeeData
                                            from assignedEmployee in assignedEmployeeData.DefaultIfEmpty()

                                            join requestEmployee in Context.Employee.AsNoTracking() on doc.UserIdCreated equals requestEmployee.Id into requestEmployeeData
                                            from requestEmployee in requestEmployeeData.DefaultIfEmpty()
                                            select new CheckRequestDocumentSiteTravelRescheduleResponse
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
                    return new List<CheckRequestDocumentSiteTravelRescheduleResponse>();
                }

            }
            else
            {
                return new List<CheckRequestDocumentSiteTravelRescheduleResponse>();
            }



        }
        #endregion

    }
}
