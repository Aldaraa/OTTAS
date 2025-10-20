using MediatR;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DashboardFeature.RoomDashboard;
using tas.Application.Features.RequestDocumentFeature.CheckDuplicateRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CheckRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentSiteTravelAdd;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public sealed class RequestSiteTravelAddRepository : BaseRepository<RequestSiteTravelAdd>, IRequestSiteTravelAddRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly CacheService _memoryCache;
        public RequestSiteTravelAddRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, CacheService memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
        }

        #region UpdateAddTravel

        public async Task<int> UpdateRequestDocumentSiteTravelAdd(UpdateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;
            var currentData = await Context.RequestSiteTravelAdd.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {

                //var inSchedule = await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == request.inScheduleId);
                //var outSchedule = await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == request.outScheduleId);

                var inScheduleData = await (from inschedule in Context.TransportSchedule.Where(x => x.Id == request.inScheduleId)
                                            join transport in Context.ActiveTransport on inschedule.ActiveTransportId equals transport.Id into transportData
                                            from transport in transportData.DefaultIfEmpty()
                                            select new
                                            {
                                                Id = inschedule.Id,
                                                Direction = transport.Direction,
                                                EventDate = inschedule.EventDate.Date,
                                                ScheduleCode = inschedule.Code,
                                                ScheduleDescription = inschedule.Description
                                            }).FirstOrDefaultAsync();

                var outScheduleData = await (from inschedule in Context.TransportSchedule.Where(x => x.Id == request.outScheduleId)
                                             join transport in Context.ActiveTransport on inschedule.ActiveTransportId equals transport.Id into transportData
                                             from transport in transportData.DefaultIfEmpty()
                                             select new
                                             {
                                                 Id = inschedule.Id,
                                                 Direction = transport.Direction,
                                                 EventDate = inschedule.EventDate.Date,
                                                 ScheduleCode = inschedule.Code,
                                                 ScheduleDescription = inschedule.Description


                                             }).FirstOrDefaultAsync();


                var virtulRoom =await Context.Room.AsNoTracking().Where(c => c.VirtualRoom == 1).FirstOrDefaultAsync();
                if (virtulRoom == null) {
                    throw new BadRequestException("Virtual room not found");
                }

                if (request.CheckRoom.HasValue)
                {
                    if (request.CheckRoom.Value)
                    {
                        if (request.roomId.HasValue) {

                            if (request.roomId != virtulRoom.Id)
                            {
                                var dateRangeRoomFullStatus = await CheckRoomDate(request.roomId.Value, inScheduleData.EventDate.Date, outScheduleData.EventDate.Date, currentData.EmployeeId);

                                if (!dateRangeRoomFullStatus)
                                {
                                    throw new BadRequestException($"The room is fully booked from {inScheduleData.EventDate.ToString("yyyy-MM-dd")}, to " +
                                                            $"{outScheduleData.EventDate.ToString("yyyy-MM-dd")}");
                                }
                            }
                            
                        }

                        
                    }
                }


                string? inScheduleIdDescr = $"{inScheduleData?.EventDate.ToString("yyyy-MM-dd")} {inScheduleData?.Direction} {inScheduleData?.ScheduleCode} {inScheduleData?.ScheduleDescription}";
                string? outScheduleIdDescr = $"{outScheduleData?.EventDate.ToString("yyyy-MM-dd")} {outScheduleData?.Direction} {outScheduleData?.ScheduleCode} {outScheduleData?.ScheduleDescription}";



                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentData.RoomId = request.roomId.HasValue ? request.roomId : currentData.RoomId;
                currentData.inScheduleId = request.inScheduleId;
                currentData.outScheduleId = request.outScheduleId;
                currentData.CostCodeId = request.costcodeId;
                currentData.DepartmentId = request.departmentId;
                currentData.EmployerId = request.employerId;
                currentData.ShiftId = request.shiftId;
                currentData.inScheduleIdDescr = inScheduleIdDescr;
                currentData.outScheduleIdDescr = outScheduleIdDescr;
                currentData.inScheduleGoShow = request.inScheduleGoShow;
                currentData.outScheduleGoShow = request.outScheduleGoShow;  



                Context.RequestSiteTravelAdd.Update(currentData);


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


        #region CreateAddTravel

        public async Task<int> CreateRequestDocumentSiteTravelAdd(CreateRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {
                    var currentEmployee = await Context.Employee.Where(x => x.Id == request.documentData.EmployeeId).FirstOrDefaultAsync();
                    var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();

                    var inScheduleData = await (from inschedule in Context.TransportSchedule.Where(x => x.Id == request.flightData.inScheduleId)
                                                join transport in Context.ActiveTransport on inschedule.ActiveTransportId equals transport.Id into transportData
                                                from transport in transportData.DefaultIfEmpty()
                                                select new
                                                {
                                                    Id = inschedule.Id,
                                                    Direction = transport.Direction,
                                                    EventDate = inschedule.EventDate.Date,
                                                    ScheduleCode = inschedule.Code,
                                                    ScheduleDescription = inschedule.Description
                                                }).FirstOrDefaultAsync();

                    var outScheduleData = await (from inschedule in Context.TransportSchedule.Where(x => x.Id == request.flightData.outScheduleId)
                                                 join transport in Context.ActiveTransport on inschedule.ActiveTransportId equals transport.Id into transportData
                                                 from transport in transportData.DefaultIfEmpty()
                                                 select new
                                                 {
                                                     Id = inschedule.Id,
                                                     Direction = transport.Direction,
                                                     EventDate = inschedule.EventDate.Date,
                                                     ScheduleCode = inschedule.Code,
                                                     ScheduleDescription = inschedule.Description


                                                 }).FirstOrDefaultAsync();



                    if (currentEmployee == null) {
                   //     await transaction.RollbackAsync(cancellationToken);
                        throw new BadRequestException("Profile not found");
                    }
                    if (virtualRoom == null)
                    {
                    //    await transaction.RollbackAsync(cancellationToken);
                        throw new BadRequestException("Virtual room not found");
                    }

                    if (inScheduleData != null && outScheduleData != null)
                    {
                        string? inScheduleIdDescr = $"{inScheduleData.EventDate.ToString("yyyy-MM-dd")} {inScheduleData.Direction} {inScheduleData.ScheduleCode} {inScheduleData.ScheduleDescription}";
                        string? outScheduleIdDescr = $"{outScheduleData.EventDate.ToString("yyyy-MM-dd")} {outScheduleData.Direction} {outScheduleData.ScheduleCode} {outScheduleData.ScheduleDescription}";

                        if (inScheduleData.Direction == outScheduleData.Direction) {
                    //        await transaction.RollbackAsync(cancellationToken);
                            throw new BadRequestException("It cannot be the same direction, please choose a different schedule");
                        }


                        if (inScheduleData.EventDate < outScheduleData.EventDate)
                        {
                            
                            var onSiteData = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.documentData.EmployeeId && x.EventDate.Value.Date == inScheduleData.EventDate && x.RoomId != null).FirstOrDefaultAsync();

                            if (onSiteData == null)
                            {
                                var reqDocument = new RequestDocument();
                                reqDocument.CurrentAction = request.documentData.Action;
                                reqDocument.Active = 1;
                                reqDocument.DateCreated = DateTime.Now;
                                reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                reqDocument.Description = reqDocument.Description;
                                reqDocument.EmployeeId = request.documentData.EmployeeId;
                                reqDocument.DocumentType = RequestDocumentType.SiteTravel;
                                reqDocument.DocumentTag = "ADD";
                                reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
                                reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;

                                Context.RequestDocument.Add(reqDocument);
                                await Context.SaveChangesAsync();
                                await CreateSaveAttachment(request.Files, reqDocument.Id);

                                if (request.flightData.roomId.HasValue)
                                {
                                    await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, request.flightData.roomId, inScheduleIdDescr, outScheduleIdDescr);
                                }
                                else {
                                    if (currentEmployee.RoomId.HasValue)
                                    {
                                        await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, currentEmployee.RoomId, inScheduleIdDescr, outScheduleIdDescr);
                                    }
                                    else {
                                        await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, virtualRoom.Id, inScheduleIdDescr, outScheduleIdDescr);
                                    }
                                    
                                }



                                await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                                await Context.SaveChangesAsync();
                                await transaction.CommitAsync(cancellationToken);
                                return reqDocument.Id;
                            }
                            else
                            {
                     //           await transaction.RollbackAsync(cancellationToken);
                                throw new BadRequestException($"{inScheduleData.EventDate} Add travel registration is not possible because this person is on the site");
                            }

                        }
                        else if (inScheduleData.EventDate > outScheduleData.EventDate)
                        {
                            //var onSiteData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == request.documentData.EmployeeId && x.EventDate.Value.Date == inScheduleData.EventDate && x.RoomId != null).FirstOrDefaultAsync();
                            //if (onSiteData != null)
                            //{
                                var reqDocument = new RequestDocument();
                                reqDocument.CurrentAction = request.documentData.Action;
                                reqDocument.Active = 1;
                                reqDocument.DateCreated = DateTime.Now;
                                reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                reqDocument.Description = reqDocument.Description;
                                reqDocument.EmployeeId = request.documentData.EmployeeId;
                                reqDocument.DocumentType = RequestDocumentType.SiteTravel;
                                reqDocument.DocumentTag = "ADD";
                                reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
                                reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;

                                Context.RequestDocument.Add(reqDocument);
                                await Context.SaveChangesAsync();
                                await CreateSaveAttachment(request.Files, reqDocument.Id);


                                if (request.flightData.roomId.HasValue)
                                {
                                    await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, request.flightData.roomId, inScheduleIdDescr, outScheduleIdDescr);
                                }
                                else
                                {
                                    if (currentEmployee.RoomId.HasValue)
                                    {
                                        await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, currentEmployee.RoomId, inScheduleIdDescr, outScheduleIdDescr);
                                    }
                                    else
                                    {
                                        await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, virtualRoom.Id, inScheduleIdDescr, outScheduleIdDescr);
                                    }

                                }


                                await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                                await Context.SaveChangesAsync();
                                await transaction.CommitAsync(cancellationToken);
                                return reqDocument.Id;
                        //    }
                        //    else
                        //    {
                        /////        await transaction.RollbackAsync(cancellationToken);
                        //        throw new BadRequestException($"{inScheduleData.EventDate} Add travel registration is not possible because this person is off the site begin with out direction");
                        //    }

                        }
                        else
                        {
                            var reqDocument = new RequestDocument();
                            reqDocument.CurrentAction = request.documentData.Action;
                            reqDocument.Active = 1;
                            reqDocument.DateCreated = DateTime.Now;
                            reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            reqDocument.Description = reqDocument.Description;
                            reqDocument.EmployeeId = request.documentData.EmployeeId;
                            reqDocument.DocumentType = RequestDocumentType.SiteTravel;
                            reqDocument.DocumentTag = "ADD";
                            reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
                            reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;

                            Context.RequestDocument.Add(reqDocument);
                            await Context.SaveChangesAsync();
                            await CreateSaveAttachment(request.Files, reqDocument.Id);


                            await CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id, virtualRoom.Id, inScheduleIdDescr, outScheduleIdDescr);
                            await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                            await Context.SaveChangesAsync();
                            await transaction.CommitAsync(cancellationToken);
                            return reqDocument.Id;
                        }


                    }
                    else
                    {
                        //await transaction.RollbackAsync(cancellationToken);
                        throw new BadRequestException("Schedule not found");
                    }

                }
                catch (Exception ex)
                {
                    // Roll back the transaction if an exception occurs
                    await transaction.RollbackAsync(cancellationToken);

                    throw new BadRequestException(ex.Message);
                }



            }
        }


        private async Task CreateSaveFlightData(CreateRequestDocumentSiteTravelData travelData, int employeeId, int documentId, int? roomId, string? inScheduleIdDescr, string? outScheduleIdDescr)
        {


            var newRecord = new RequestSiteTravelAdd
            {
                DocumentId = documentId,
                EmployeeId = employeeId,
                Active = 1,
                RoomId = roomId,
                inScheduleId = travelData.inScheduleId,
                outScheduleId = travelData.outScheduleId,
                DepartmentId = travelData.departmentId,
                CostCodeId = travelData.costcodeId,
                EmployerId = travelData.employerId,
                PositionId = travelData.positionId,
                ShiftId = travelData.shiftId,
                DateCreated = DateTime.Now,
                Reason = travelData.Reason,
                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                inScheduleIdDescr = inScheduleIdDescr,
                outScheduleIdDescr = outScheduleIdDescr,
                inScheduleGoShow = travelData.inScheduleGoShow,
                outScheduleGoShow = travelData.outScheduleGoShow,
            };
            Context.RequestSiteTravelAdd.Add(newRecord);
            await Task.CompletedTask;
        }
        //CreateRequestDocumentSiteTravelAttachment
        private async Task CreateSaveAttachment(List<CreateRequestDocumentSiteTravelAttachment> attachments, int documentId)
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

        private async Task SaveDocumentHistory(CreateRequestDocumentSiteTravelAddRequest request, int DocumentId, CancellationToken cancellationToken)
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




        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, DateTime endDate, int EmployeeId)
        {
            try
            {
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



        #endregion

        public async Task<GetRequestDocumentSiteTravelAddResponse> GetRequestDocumentSiteTravelAdd(GetRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetRequestDocumentSiteTravelAddResponse();

            var currentDocument = await Context.RequestDocument
                .Where(x => x.Id == request.documentId)
                .FirstOrDefaultAsync();

            if (currentDocument != null)
            {
                var employeeIds = new List<int?>
                    {
                        currentDocument.EmployeeId,
                        currentDocument.AssignedEmployeeId,
                        currentDocument.UserIdUpdated,
                        currentDocument.UserIdCreated
                    };

                var employees = await Context.Employee
                    .AsNoTracking()
                    .Where(e => employeeIds.Contains(e.Id))
                    .ToDictionaryAsync(e => e.Id, e => e, cancellationToken);

                Employee currentEmployee = null;
                Employee assignEmployee = null;
                Employee updatedEmployee = null;
                Employee createdEmployee = null;

                if (currentDocument.EmployeeId.HasValue)
                    employees.TryGetValue(currentDocument.EmployeeId.Value, out currentEmployee);
                if (currentDocument.AssignedEmployeeId.HasValue)
                    employees.TryGetValue(currentDocument.AssignedEmployeeId.Value, out assignEmployee);
                if (currentDocument.UserIdUpdated.HasValue)
                    employees.TryGetValue(currentDocument.UserIdUpdated.Value, out updatedEmployee);
                if (currentDocument.UserIdCreated.HasValue)
                    employees.TryGetValue(currentDocument.UserIdCreated.Value, out createdEmployee);

                var delegationEmployee = await Context.RequestDelegates
                    .AsNoTracking()
                    .Where(x => x.FromEmployeeId == currentDocument.AssignedEmployeeId)
                    .FirstOrDefaultAsync();

                var userId = _HTTPUserRepository.LogCurrentUser()?.Id;

                returnData.Id = currentDocument.Id;
                returnData.RequestedDate = currentDocument.DateCreated;
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.EmployeeFullName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                returnData.EmployeeActive = currentEmployee?.Active ?? 0;
                returnData.AssignedEmployeeFullName = $"{assignEmployee?.Firstname} {assignEmployee?.Lastname}";
                returnData.DocumentType = currentDocument.DocumentType;
                returnData.UpdatedInfo = $"{currentDocument.DateUpdated} {updatedEmployee?.Firstname} {updatedEmployee?.Lastname}";
                returnData.RequesterFullName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}";
                returnData.RequesterMail = createdEmployee?.Email;
                returnData.RequesterMobile = createdEmployee?.PersonalMobile;
                returnData.RequestUserId = createdEmployee?.Id;
                returnData.AssignedEmployeeId = currentDocument.AssignedEmployeeId;
                returnData.AssignedRouteConfigId = currentDocument.AssignedRouteConfigId;
                returnData.EmployeeId = currentDocument.EmployeeId;
                returnData.DelegateEmployeeId = delegationEmployee?.ToEmployeeId;


                try
                {
                    returnData.DaysAway = currentDocument.DaysAwayDate.HasValue ? (DateTime.Today.Subtract(currentDocument.DaysAwayDate.Value).Days) * (-1) : (DateTime.Today.Subtract(currentDocument.DateCreated.Value).Days) * (-1);
                }
                catch (Exception)
                {

                    returnData.DaysAway = 0;
                }

                var currentTravelData = await Context.RequestSiteTravelAdd
                    .AsNoTracking()
                    .Where(x => x.DocumentId == request.documentId)
                    .FirstOrDefaultAsync();

                if (currentTravelData != null)
                {
                    var travelData = new GetRequestDocumentSiteTravelAddTravel();

                    if (currentTravelData.RoomId.HasValue)
                    {
                        var currentRoom = await Context.Room
                            .AsNoTracking()
                            .Where(x => x.Id == currentTravelData.RoomId.Value)
                            .FirstOrDefaultAsync();

                        travelData.RoomTypeId = currentRoom?.RoomTypeId;
                        travelData.RoomId = currentTravelData.RoomId;
                        travelData.RoomNumber = currentRoom?.Number;
                        travelData.CampId = currentRoom?.CampId;
                    }

                    var inscheduleData = await Context.TransportSchedule
                        .AsNoTracking()
                        .Where(x => x.Id == currentTravelData.inScheduleId)
                        .FirstOrDefaultAsync();

                    if (inscheduleData != null)
                    {
                        travelData.inScheduleDate = inscheduleData.EventDate;

                        var inscheduleTransport = await (from transport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == inscheduleData.ActiveTransportId)
                                                         join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                         from tmode in modeData.DefaultIfEmpty()
                                                         select new
                                                         {
                                                             Code = transport.Code,
                                                             Direction = transport.Direction,
                                                             TransportMode = tmode.Code,


                                                         }).FirstOrDefaultAsync();

                        if (inscheduleTransport != null)
                        {
                            travelData.inScheduleDirection = inscheduleTransport.Direction;
                            travelData.INScheduleDescription = $"{inscheduleTransport.Code} {inscheduleData.Description} {inscheduleTransport.TransportMode}";
                            travelData.INScheduleTransportMode = inscheduleTransport.TransportMode;
                            
                        }
                    }

                    var outscheduleData = await Context.TransportSchedule
                        .AsNoTracking()
                        .Where(x => x.Id == currentTravelData.outScheduleId)
                        .FirstOrDefaultAsync();

                    if (outscheduleData != null)
                    {
                        travelData.outScheduleDate = outscheduleData.EventDate;

                        //var outscheduleTransport = await Context.ActiveTransport
                        //    .AsNoTracking()
                        //    .Where(x => x.Id == outscheduleData.ActiveTransportId)
                        //    .FirstOrDefaultAsync();

                        var outscheduleTransport = await (from transport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == outscheduleData.ActiveTransportId)
                                                          join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                          from tmode in modeData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              Code = transport.Code,
                                                              Direction = transport.Direction,
                                                              TransportMode = tmode.Code,
                                                              

                                                          }).FirstOrDefaultAsync();
                    

                        if (outscheduleTransport != null)
                        {



                            travelData.outScheduleDirection = outscheduleTransport.Direction;
                            travelData.OUTScheduleDescription = $"{outscheduleTransport.Code} {outscheduleData.Description} {outscheduleTransport.TransportMode}";
                            travelData.OUTScheduleTransportMode = outscheduleTransport.TransportMode;
                                
                        }
                    }


                    travelData.Id = currentTravelData.Id;
                    travelData.DepartmentId = currentTravelData.DepartmentId;
                    travelData.Reason = currentTravelData.Reason;
                    travelData.shiftId = currentTravelData?.ShiftId;
                    travelData.RoomId = currentTravelData?.RoomId;
                    travelData.CostCodeId = currentTravelData?.CostCodeId;
                    travelData.EmployerId = currentTravelData?.EmployerId;
                    travelData.inScheduleId = currentTravelData.inScheduleId;
                    travelData.outScheduleId = currentTravelData.outScheduleId;
                    travelData.PositionId = currentTravelData.PositionId;
                    travelData.inScheduleIdDescr = currentTravelData.inScheduleIdDescr;
                    travelData.outScheduleIdDescr = currentTravelData.outScheduleIdDescr;
                    travelData.inScheduleGoShow = currentTravelData.inScheduleGoShow;
                    travelData.outScheduleGoShow = currentTravelData.outScheduleGoShow;



                    returnData.TravelData = travelData;
                }
            }

            return returnData;
        }




        #region Complete 

        public async Task<CompleteRequestDocumentSiteTravelAddResponse?> CompleteRequestDocumentSiteTravelAdd(CompleteRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
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
                                var travelData = await Context.RequestSiteTravelAdd.AsNoTracking().FirstOrDefaultAsync(x => x.DocumentId == request.Id);

                                if (travelData != null)
                                {
                                    var assignedOldGroupId = await Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();
                                    var currentEmployee = await Context.Employee.Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync();
                                    if (currentEmployee != null)
                                    {
                                        if (currentEmployee.Active != 1)
                                        {
                                            currentEmployee.DateUpdated = DateTime.Now;
                                            currentEmployee.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                            currentEmployee.Active = 1;
                                            Context.Employee.Update(currentEmployee);
                                        }



                                        var retdata = await AddTravelRequestComplete(request.Id, cancellationToken);
                                        currentDocument.CurrentAction = RequestDocumentAction.Completed;
                                        currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                                        currentDocument.CompletedDate = DateTime.Now;
                                        currentDocument.CompletedUserId = _HTTPUserRepository.LogCurrentUser()?.Id;
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

                                        await Context.SaveChangesAsync(cancellationToken);
                                        await transaction.CommitAsync(cancellationToken);

                                        return retdata;
                                    }
                                    else {
                                        return null;
                                    }
                                  
                                }
                                else
                                {
                                    return null;
                                }

                            }
                        }
                        else
                        {
                            throw new BadRequestException("Invalid document type");
                        }
                    }
                    else {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new BadRequestException(ex.Message);
                }
            }
        }


        public async Task<CompleteRequestDocumentSiteTravelAddResponse?> AddTravelRequestComplete(int documentId, 
            CancellationToken cancellationToken)
        {

            var returnData = new CompleteRequestDocumentSiteTravelAddResponse();
            var request = await Context.RequestSiteTravelAdd.AsNoTracking().Where(x => x.DocumentId == documentId).FirstOrDefaultAsync(cancellationToken);

            if (request != null)
            {
                var inSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.inScheduleId);
                var outSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.outScheduleId);
                if (inSchedule == null || outSchedule == null)
                {
                    throw new BadRequestException("Schedule not found");
                }
                var inActiveTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == inSchedule.ActiveTransportId).FirstOrDefaultAsync();

                var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.ShiftId).Select(x => new { x.OnSite }).FirstOrDefaultAsync(cancellationToken);
                var outActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == outSchedule.ActiveTransportId);

                if (inActiveTransport == null || outActiveTransport == null)
                {
                    throw new BadRequestException("Transport data not found");
                }

                if (inSchedule.EventDate.Date < outSchedule.EventDate.Date.Date)
                {
                    if (currentShift.OnSite == 1)
                    {
                        DateTime time = DateTime.ParseExact(inSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int hours = time.Hour;
                        int minutes = time.Minute;


                        DateTime outtime = DateTime.ParseExact(outSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int outhours = outtime.Hour;
                        int outminutes = outtime.Minute;

                        var currentTransportDirectionIn = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == inSchedule.ActiveTransportId).Select(x => new { x.Direction, x.Code}).FirstOrDefaultAsync();
                        var transportin = new Transport
                        {
                            EmployeeId = request.EmployeeId,
                            PositionId = request.PositionId,
                            DepId = request.DepartmentId,
                            CostCodeId = request.CostCodeId,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            ActiveTransportId = inSchedule.ActiveTransportId,
                            EventDate = inSchedule.EventDate.Date,
                            EventDateTime = inSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                            ScheduleId = inSchedule.Id,
                            Active = 1,
                            Direction = currentTransportDirectionIn.Direction,
                            EmployerId = request.EmployerId,
                            Status = "Confirmed",
                            ChangeRoute = $"Request add travel #{request.DocumentId}",
                            GoShow = request.inScheduleGoShow,

                        };


                        if (request.inScheduleGoShow.HasValue)
                        {
                            if (request.inScheduleGoShow.Value == 1)
                            {
                                var inschedulegoshow = new TransportGoShow
                                {
                                    Active = 1,
                                    DateCreated  = DateTime.Now,
                                    EmployeeId = request.EmployeeId,
                                    Direction = currentTransportDirectionIn.Direction,
                                    EventDate = transportin.EventDate,
                                    EventDateTime  =transportin.EventDateTime,
                                    Reason = request.Reason,
                                    Description = $"{currentTransportDirectionIn.Code} {inSchedule.Description} {currentTransportDirectionIn.Direction}",
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    
                                   
                                };
                                Context.TransportGoShow.Add(inschedulegoshow);
                            }
                        }



                        var currentTransportDirectionOut = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == outSchedule.ActiveTransportId).Select(x => new { x.Direction, x.Code }).FirstOrDefaultAsync();
                        var transportout = new Transport
                        {
                            EmployeeId = request.EmployeeId,
                            PositionId = request.PositionId,
                            DepId = request.DepartmentId,
                            CostCodeId = request.CostCodeId,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            ActiveTransportId = outSchedule.ActiveTransportId,
                            EventDate = outSchedule.EventDate.Date,
                            EventDateTime = outSchedule.EventDate.Date.AddHours(outhours).AddMinutes(outminutes),
                            ScheduleId = outSchedule.Id,
                            Active = 1,
                            Direction = currentTransportDirectionOut.Direction,
                            EmployerId = request.EmployerId,
                            Status = "Confirmed",
                            ChangeRoute = $"Request add travel #{request.DocumentId}",
                            GoShow = request.outScheduleGoShow

                        };


                        if (request.outScheduleGoShow.HasValue)
                        {
                            if (request.outScheduleGoShow.Value == 1)
                            {
                                var outschedulegoshow = new TransportGoShow
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = request.EmployeeId,
                                    Direction = currentTransportDirectionOut.Direction,
                                    EventDate = transportout.EventDate,
                                    EventDateTime = transportout.EventDateTime,
                                    Reason = request.Reason,
                                    Description = $"{currentTransportDirectionOut.Code} {outSchedule.Description} {currentTransportDirectionOut.Direction}",
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,


                                };
                                Context.TransportGoShow.Add(outschedulegoshow);
                            }
                        }


                        Context.Transport.Add(transportin);
                        Context.Transport.Add(transportout);

                        await AddTravelSetRoom(transportin.EventDate.Value.Date, transportout.EventDate.Value.Date, documentId, request.EmployeeId);

                        returnData.InScheduleId = transportin.ScheduleId;
                        returnData.OutScheduleId = transportout.ScheduleId;

                        return returnData;

                    }
                    else
                    {
                        var errorMessage = new List<string>();
                        errorMessage.Add("RoomStatus has an wrong value");
                        throw new BadRequestException(errorMessage.ToArray());
                    }

                }
                else if (inSchedule.EventDate.Date > outSchedule.EventDate.Date.Date)
                {
                    var RRShift = await Context.Shift.Where(x => x.Code == "RR").FirstOrDefaultAsync();

                    if (currentShift.OnSite != 1 || RRShift != null)
                    {
                        DateTime time = DateTime.ParseExact(inSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int hours = time.Hour;
                        int minutes = time.Minute;



                        DateTime outtime = DateTime.ParseExact(outSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int outhours = outtime.Hour;
                        int outminutes = outtime.Minute;

                        var currentTransportDirectionIn = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == inSchedule.ActiveTransportId).Select(x => new { x.Direction, x.Code }).FirstOrDefaultAsync();
                        var transportin = new Transport
                        {
                            EmployeeId = request.EmployeeId,
                            PositionId = request.PositionId,
                            DepId = request.DepartmentId,
                            CostCodeId = request.CostCodeId,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            ActiveTransportId = inSchedule.ActiveTransportId,
                            EventDate = inSchedule.EventDate.Date,
                            EventDateTime = inSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                            ScheduleId = inSchedule.Id,
                            Active = 1,
                            Direction = currentTransportDirectionIn.Direction,
                            EmployerId = request.EmployerId,
                            Status = "Confirmed",
                            ChangeRoute = $"Request add travel #{request.DocumentId}",
                            GoShow = request.inScheduleGoShow

                        };

                        var currentTransportDirectionOut = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == outSchedule.ActiveTransportId).Select(x => new { x.Direction, x.Code }).FirstOrDefaultAsync();

                        var transportout = new Transport
                        {
                            EmployeeId = request.EmployeeId,
                            PositionId = request.PositionId,
                            DepId = request.DepartmentId,
                            CostCodeId = request.CostCodeId,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            ActiveTransportId = outSchedule.ActiveTransportId,
                            EventDate = outSchedule.EventDate.Date,
                            EventDateTime = outSchedule.EventDate.Date.AddHours(outhours).AddMinutes(outminutes),
                            ScheduleId = outSchedule.Id,
                            Active = 1,
                            Direction = currentTransportDirectionOut.Direction,
                            EmployerId = request.EmployerId,
                            Status = "Confirmed",
                            ChangeRoute = $"Request add travel #{request.DocumentId}",
                            GoShow = request.outScheduleGoShow
                        };

                        if (request.inScheduleGoShow.HasValue)
                        {
                            if (request.inScheduleGoShow.Value == 1)
                            {
                                var inschedulegoshow = new TransportGoShow
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = request.EmployeeId,
                                    Direction = currentTransportDirectionIn.Direction,
                                    EventDate = transportin.EventDate,
                                    EventDateTime = transportin.EventDateTime,
                                    Reason = request.Reason,
                                    Description = $"{currentTransportDirectionIn.Code} {inSchedule.Description} {currentTransportDirectionIn.Direction}",

                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,


                                };
                                Context.TransportGoShow.Add(inschedulegoshow);
                            }
                        }

                        if (request.outScheduleGoShow.HasValue)
                        {
                            if (request.outScheduleGoShow.Value == 1)
                            {
                                var inschedulegoshow = new TransportGoShow
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = request.EmployeeId,
                                    Direction = currentTransportDirectionOut.Direction,
                                    EventDate = transportout.EventDate,
                                    EventDateTime = transportout.EventDateTime,
                                    Reason = request.Reason,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    Description = $"{currentTransportDirectionOut.Code} {outSchedule.Description} {currentTransportDirectionOut.Direction}",

                                };
                                Context.TransportGoShow.Add(inschedulegoshow);
                            }
                        }



                        Context.Transport.Add(transportin);
                        Context.Transport.Add(transportout);

                        await AddTravelSetRoomOut(transportout.EventDate.Value.Date, transportin.EventDate.Value.Date, request, currentShift.OnSite == 1 ? RRShift.Id : request.ShiftId);

                        returnData.InScheduleId = transportin.ScheduleId;
                        returnData.OutScheduleId = transportout.ScheduleId;

                        return returnData;

                    }
                    else
                    {
                        var errorMessage = new List<string>();
                        errorMessage.Add("RoomStatus has an wrong value");
                        throw new BadRequestException(errorMessage.ToArray());
                    }

                }
                else
                {
                    if (currentShift.OnSite == 1)
                    {
                        DateTime time = DateTime.ParseExact(inSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int hours = time.Hour;
                        int minutes = time.Minute;


                        DateTime outtime = DateTime.ParseExact(outSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int outhours = outtime.Hour;
                        int outminutes = outtime.Minute;

                        var currentTransportDirectionIn = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == inSchedule.ActiveTransportId).Select(x => new { x.Direction,x.Code }).FirstOrDefaultAsync();
                        var transportin = new Transport
                        {
                            EmployeeId = request.EmployeeId,
                            PositionId = request.PositionId,
                            DepId = request.DepartmentId,
                            CostCodeId = request.CostCodeId,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            ActiveTransportId = inSchedule.ActiveTransportId,
                            EventDate = inSchedule.EventDate.Date,
                            EventDateTime = inSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                            ScheduleId = inSchedule.Id,
                            Active = 1,
                            Direction = currentTransportDirectionIn.Direction,
                            EmployerId = request.EmployerId,
                            Status = "Confirmed",
                            ChangeRoute = $"Request add travel #{request.DocumentId}",
                            GoShow = request.inScheduleGoShow,

                        };


                        if (request.inScheduleGoShow.HasValue)
                        {
                            if (request.inScheduleGoShow.Value == 1)
                            {
                                var inschedulegoshow = new TransportGoShow
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = request.EmployeeId,
                                    Direction = currentTransportDirectionIn.Direction,
                                    EventDate = transportin.EventDate,
                                    EventDateTime = transportin.EventDateTime,
                                    Reason = request.Reason,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    Description = $"{currentTransportDirectionIn.Code} {inSchedule.Description} {currentTransportDirectionIn.Direction}",

                                };
                                Context.TransportGoShow.Add(inschedulegoshow);
                            }
                        }



                        var currentTransportDirectionOut = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == outSchedule.ActiveTransportId).Select(x => new { x.Direction, x.Code }).FirstOrDefaultAsync();
                        var transportout = new Transport
                        {
                            EmployeeId = request.EmployeeId,
                            PositionId = request.PositionId,
                            DepId = request.DepartmentId,
                            CostCodeId = request.CostCodeId,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            ActiveTransportId = outSchedule.ActiveTransportId,
                            EventDate = outSchedule.EventDate.Date,
                            EventDateTime = outSchedule.EventDate.Date.AddHours(outhours).AddMinutes(outminutes),
                            ScheduleId = outSchedule.Id,
                            Active = 1,
                            Direction = currentTransportDirectionOut.Direction,
                            EmployerId = request.EmployerId,
                            Status = "Confirmed",
                            ChangeRoute = $"Request add travel #{request.DocumentId}",
                            GoShow= request.outScheduleGoShow,

                        };


                        if (request.outScheduleGoShow.HasValue)
                        {
                            if (request.outScheduleGoShow.Value == 1)
                            {
                                var inschedulegoshow = new TransportGoShow
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = request.EmployeeId,
                                    Direction = currentTransportDirectionOut.Direction,
                                    EventDate = transportout.EventDate,
                                    EventDateTime = transportout.EventDateTime,
                                    Reason = request.Reason,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    Description = $"{currentTransportDirectionOut.Code} {outSchedule.Description} {currentTransportDirectionOut.Direction}",

                                };
                                Context.TransportGoShow.Add(inschedulegoshow);
                            }
                        }

                        Context.Transport.Add(transportin);
                        Context.Transport.Add(transportout);

                        returnData.InScheduleId = transportin.ScheduleId;
                        returnData.OutScheduleId = transportout.ScheduleId;

                        return returnData;

                        //  await AddTravelSetRoom(transportin.EventDate.Value.Date, transportout.EventDate.Value.Date, documentId);
                        await Task.CompletedTask;
                    }
                    else
                    {
                        
                        DateTime time = DateTime.ParseExact(inSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int hours = time.Hour;
                        int minutes = time.Minute;


                        if (await CheckStartDateOnSite(request.EmployeeId, inSchedule.EventDate))
                        {

                            if (inSchedule.EventDate.Date == outSchedule.EventDate.Date)
                            {

                                DateTime outtime = DateTime.ParseExact(outSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = outtime.Hour;
                                int outminutes = outtime.Minute;

                                var intransportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == inSchedule.Id);

                                var transportin = new Transport
                                {
                                    EmployeeId = request.EmployeeId,
                                    PositionId = request.PositionId,
                                    DepId = request.DepartmentId,
                                    CostCodeId = request.CostCodeId,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    ActiveTransportId = inSchedule.ActiveTransportId,
                                    EventDate = inSchedule.EventDate.Date,
                                    EventDateTime = inSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                                    ScheduleId = inSchedule.Id,
                                    Active = 1,
                                    Direction = inActiveTransport.Direction,
                                    EmployerId = request.EmployerId,
                                    Status = inSchedule.Seats > intransportcount ? "Confirmed" : "Over booked",
                                    GoShow = request.inScheduleGoShow,


                                };



                                if (request.inScheduleGoShow.HasValue)
                                {
                                    if (request.inScheduleGoShow.Value == 1)
                                    {
                                        var currentTransportDirectionIN = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == inSchedule.ActiveTransportId).Select(x => new { x.Direction, x.Code }).FirstOrDefaultAsync();


                                        var inschedulegoshow = new TransportGoShow
                                        {
                                            Active = 1,
                                            DateCreated = DateTime.Now,
                                            EmployeeId = request.EmployeeId,
                                            EventDate = transportin.EventDate,
                                            EventDateTime = transportin.EventDateTime,
                                            Reason = request.Reason,
                                            Description = $"{currentTransportDirectionIN?.Code} {inSchedule.Description} {currentTransportDirectionIN?.Direction}",
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,


                                        };
                                        Context.TransportGoShow.Add(inschedulegoshow);
                                    }
                                }




                                var outtransportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == outSchedule.Id);


                                var transportout = new Transport
                                {
                                    EmployeeId = request.EmployeeId,
                                    PositionId = request.PositionId,
                                    DepId = request.DepartmentId,
                                    CostCodeId = request.CostCodeId,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    ActiveTransportId = outSchedule.ActiveTransportId,
                                    EventDate = outSchedule.EventDate.Date,
                                    EventDateTime = outSchedule.EventDate.Date.AddHours(outhours).AddMinutes(outminutes),
                                    ScheduleId = outSchedule.Id,
                                    Active = 1,
                                    Direction = outActiveTransport.Direction,
                                    EmployerId = request.EmployerId,
                                    Status = outSchedule.Seats > outtransportcount ? "Confirmed" : "Over booked",
                                    GoShow = request.outScheduleGoShow,

                                };

                                if (request.outScheduleGoShow.HasValue)
                                {
                                    if (request.outScheduleGoShow.Value == 1)
                                    {
                                        var currentTransportDirectionOut = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == outSchedule.ActiveTransportId).Select(x => new { x.Direction, x.Code }).FirstOrDefaultAsync();


                                        var inschedulegoshow = new TransportGoShow
                                        {
                                            Active = 1,
                                            DateCreated = DateTime.Now,
                                            EmployeeId = request.EmployeeId,
                                            EventDate = transportin.EventDate,
                                            EventDateTime = transportin.EventDateTime,
                                            Reason = request.Reason,
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                            Description = $"{currentTransportDirectionOut?.Code} {outSchedule.Description} {currentTransportDirectionOut?.Direction}",


                                        };
                                        Context.TransportGoShow.Add(inschedulegoshow);
                                    }
                                }



                                Context.Transport.Add(transportin);
                                Context.Transport.Add(transportout);

                                returnData.InScheduleId = transportin.ScheduleId;
                                returnData.OutScheduleId = transportout.ScheduleId;

                                return returnData;
                            }
                            else
                            {
                                throw new BadRequestException("Unable to create flight data");
                            }

                        }
                        else
                        {
                            throw new BadRequestException("RoomStatus has an wrong value");
                        }
                    }

                }

            }
            else {
                return null;
            }

        }

        private async Task<bool> CheckStartDateOnSite(int employeeId, DateTime eventDate)
        {
            var onsiteStatus = await Context.EmployeeStatus.AsNoTracking()
                 .AnyAsync(x => x.EventDate.Value.Date == eventDate.Date && x.EmployeeId == employeeId && x.RoomId != null);
            return onsiteStatus;
        }


        public async Task AddTravelSetRoomOut(DateTime StartDate, DateTime EndDate, RequestSiteTravelAdd request, int shiftId)
        {

                DateTime currentDate = StartDate;

                var currentShift = await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ShiftId);
                while (currentDate.AddDays(1) <= EndDate)
                {

                    var currentStatus = await Context.EmployeeStatus.FirstOrDefaultAsync(x =>
                    x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == currentDate.Date);
                    if (currentStatus != null)
                    {
                        currentStatus.RoomId = null;
                        currentStatus.ShiftId = shiftId;
                        currentStatus.DateUpdated = DateTime.Now;
                        currentStatus.ChangeRoute = $"Request add travel #{request.DocumentId}";
                        currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;


                        Context.EmployeeStatus.Update(currentStatus);
                        currentDate = currentDate.AddDays(1);
                    }
                    else
                    {

                        var employeeStatus = new EmployeeStatus
                        {
                            EventDate = currentDate,
                            EmployeeId = request.EmployeeId,
                            ShiftId = shiftId,
                            DepId = request.DepartmentId,
                            Active = 1,
                            CostCodeId = request.CostCodeId,
                            EmployerId = request.EmployerId,
                            PositionId = request.PositionId,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            ChangeRoute = $"Request add travel #{request.DocumentId}",
                            DateCreated = DateTime.Now

                        };
                        Context.EmployeeStatus.Add(employeeStatus);

                    }
                }

            
            await Task.CompletedTask;
        }

        public async Task AddTravelSetRoom(DateTime StartDate, DateTime EndDate, int documentId, int EmployeeId )
        {


            if (StartDate.Date == EndDate.Date)
            {
                var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                if (virtualRoom != null)
                {
                    var request = await Context.RequestSiteTravelAdd.AsNoTracking().FirstOrDefaultAsync(x => x.DocumentId == documentId);
                    var currentShift = await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ShiftId);
                    if (currentShift?.OnSite == 1)
                    {
                        var employeeStatus = new EmployeeStatus
                        {
                            EventDate = StartDate.Date,
                            EmployeeId = request.EmployeeId,
                            ShiftId = request.ShiftId,
                            RoomId = virtualRoom.Id,
                            DepId = request.DepartmentId,
                            Active = 1,
                            CostCodeId = request.CostCodeId,
                            EmployerId = request.EmployerId,
                            PositionId = request.PositionId,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            DateCreated = DateTime.Now,
                            ChangeRoute = $"Request add travel #{request.DocumentId}",

                        };
                        Context.EmployeeStatus.Add(employeeStatus);
                    }

                }
                else {
                    throw new BadRequestException("Please register virtual room");
                }


            }
            else {
                Room currentRoom;
                var request = await Context.RequestSiteTravelAdd.AsNoTracking().FirstOrDefaultAsync(x => x.DocumentId == documentId);

                if (request.RoomId.HasValue)
                {
                    currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.RoomId.Value);
                }
                else {
                    currentRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                        
                }
                
                

                if (currentRoom != null)
                {
                    var currentRoomCheck = await CheckRoomDate(currentRoom.Id, StartDate, EndDate, EmployeeId);


                    if (currentRoom == null || !currentRoomCheck)
                    {
                        currentRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                    }

                }
                else
                {
                    currentRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                }




                DateTime currentDate = StartDate;

                var currentShift = await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ShiftId);
                while (currentDate.AddDays(1) <= EndDate)
                {

                    var currentData =await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();


                    if (currentRoom?.VirtualRoom == 1)
                    {
                        if (currentData == null)
                        {
                            var employeeStatus = new EmployeeStatus
                            {
                                EventDate = currentDate,
                                EmployeeId = request.EmployeeId,
                                ShiftId = request.ShiftId,
                                RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null,
                                DepId = request.DepartmentId,
                                Active = 1,
                                CostCodeId = request.CostCodeId,
                                EmployerId = request.EmployerId,
                                PositionId = request.PositionId,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                DateCreated = DateTime.Now,
                                ChangeRoute = $"Request add travel #{request.DocumentId}",

                            };

                            Context.EmployeeStatus.Add(employeeStatus);
                            currentDate = currentDate.AddDays(1);
                        }
                        else {
                            currentData.ShiftId = request.ShiftId;
                            currentData.ChangeRoute = $"Request add travel #{request.DocumentId}";
                            currentData.RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null;
                            currentData.DepId = request.DepartmentId;
                            currentData.Active = 1;
                            currentData.CostCodeId = request.CostCodeId;
                            currentData.EmployerId = request.EmployerId;
                            currentData.PositionId = request.PositionId;
                            currentData.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            currentData.DateCreated = DateTime.Now;

                            Context.EmployeeStatus.Update(currentData);
                            currentDate = currentDate.AddDays(1);

                        }


                    }
                    else
                    {
                        int? roomId = currentShift?.OnSite == 1 ? request.RoomId : null;
                        int? bedId = null;
                        if (roomId != null)
                        {
                            bedId = await getRoomBedId((int)roomId, currentDate);
                        }
                        int dateGuestCount = await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.RoomId == request.RoomId &&
                             x.EventDate == currentDate
                         );

                        //if (dateGuestCount > currentRoom.BedCount)
                        //{
                            if (currentRoom.VirtualRoom == 1)
                            {
                                if (currentData == null)
                                { 
                                
                                    var employeeStatus = new EmployeeStatus
                                    {
                                        EventDate = currentDate,
                                        EmployeeId = request.EmployeeId,
                                        ShiftId = request.ShiftId,
                                        RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null,
                                        BedId = null,
                                        DepId = request.DepartmentId,
                                        EmployerId = request.EmployerId,
                                        Active = 1,
                                        CostCodeId = request.CostCodeId,
                                        PositionId = request.PositionId,
                                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                        DateCreated = DateTime.Now,
                                        ChangeRoute = $"Request add travel #{request.DocumentId}",

                                    };
                                    Context.EmployeeStatus.Add(employeeStatus);
                                    currentDate = currentDate.AddDays(1);
                                }
                                else
                                {
                                    currentData.ShiftId = request.ShiftId;
                                    currentData.ChangeRoute = $"Request add travel #{request.DocumentId}";
                                    currentData.RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null;
                                    currentData.BedId = null;
                                    currentData.DepId = request.DepartmentId;
                                    currentData.Active = 1;
                                    currentData.CostCodeId = request.CostCodeId;
                                    currentData.EmployerId = request.EmployerId;
                                    currentData.PositionId = request.PositionId;
                                    currentData.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    currentData.DateCreated = DateTime.Now;

                                    Context.EmployeeStatus.Update(currentData);
                                    currentDate = currentDate.AddDays(1);
                                }


                            }
                            else
                            {
                                if (currentData == null)
                                {
                                    var employeeStatus = new EmployeeStatus
                                    {
                                        EventDate = currentDate,
                                        EmployeeId = request.EmployeeId,
                                        ShiftId = request.ShiftId,
                                        RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null,
                                        BedId = currentRoom.VirtualRoom > 0 ? null : bedId,
                                        DepId = request.DepartmentId,
                                        EmployerId = request.EmployerId,
                                        Active = 1,
                                        CostCodeId = request.CostCodeId,
                                        PositionId = request.PositionId,
                                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                        DateCreated = DateTime.Now,
                                        ChangeRoute = $"Request add travel #{request.DocumentId}",

                                    };
                                    Context.EmployeeStatus.Add(employeeStatus);

                                    currentDate = currentDate.AddDays(1);
                                }
                                else
                                {
                                    currentData.ShiftId = request.ShiftId;
                                    currentData.RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null;
                                    currentData.BedId = currentRoom.VirtualRoom > 0 ? null : bedId;
                                    currentData.DepId = request.DepartmentId;
                                    currentData.EmployerId = request.EmployerId;
                                    currentData.Active = 1;
                                    currentData.CostCodeId = request.CostCodeId;
                                    currentData.PositionId = request.PositionId;
                                    currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    currentData.DateUpdated = DateTime.Now;
                                    currentData.ChangeRoute = $"Request add travel #{request.DocumentId}";

                                    Context.EmployeeStatus.Update(currentData);

                                    currentDate = currentDate.AddDays(1);
                                }


                            //}
                            //else
                            //{
                            //    if (currentData == null)
                            //    {
                            //        var employeeStatus = new EmployeeStatus
                            //        {
                            //            EventDate = currentDate,
                            //            EmployeeId = request.EmployeeId,
                            //            ShiftId = request.ShiftId,
                            //            RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null,
                            //            BedId = currentRoom.VirtualRoom > 0 ?  null : bedId,
                            //            DepId = request.DepartmentId,
                            //            EmployerId = request.EmployerId,
                            //            Active = 1,
                            //            CostCodeId = request.CostCodeId,
                            //            PositionId = request.PositionId,
                            //            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            //            DateCreated = DateTime.Now,
                            //            ChangeRoute = $"Request add travel #{request.DocumentId}",

                            //        };
                            //        Context.EmployeeStatus.Add(employeeStatus);

                            //        currentDate = currentDate.AddDays(1);
                            //    }
                            //    else {
                            //        currentData.ShiftId = request.ShiftId;
                            //        currentData.RoomId = currentShift?.OnSite == 1 ? currentRoom.Id : null;
                            //        currentData.BedId = currentRoom.VirtualRoom > 0 ? null : bedId;
                            //        currentData.DepId = request.DepartmentId;
                            //        currentData.EmployerId = request.EmployerId;
                            //        currentData.Active = 1;
                            //        currentData.CostCodeId = request.CostCodeId;
                            //        currentData.PositionId = request.PositionId;
                            //        currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            //        currentData.DateUpdated = DateTime.Now;
                            //        currentData.ChangeRoute = $"Request add travel #{request.DocumentId}";

                            //        Context.EmployeeStatus.Update(currentData);

                            //        currentDate = currentDate.AddDays(1);
                            //    }


                           }


                       }


                }
                await Task.CompletedTask;
            }

          
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


        #region CheckData

        public async Task<List<CheckRequestDocumentSiteTravelAddResponse>> CheckRequestDocumentSiteTravelAdd(CheckRequestDocumentSiteTravelAddRequest request, CancellationToken cancellationToken)
        {
            var notActiveDocumentAction = new List<string> { RequestDocumentAction.Cancelled, RequestDocumentAction.Completed };
            
            
            
            var currentInScheduleData =await  Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.inScheduleId).FirstOrDefaultAsync();
            var currentOutScheduleData =await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.outScheduleId).FirstOrDefaultAsync();
            if (currentInScheduleData != null && currentOutScheduleData != null)
            { 
                var InscheduleIds =await  Context.TransportSchedule.AsNoTracking().Where(x=> x.EventDate.Date == currentInScheduleData.EventDate.Date).Select(x => x.Id).ToListAsync();

                var OutscheduleIds = await Context.TransportSchedule.AsNoTracking().Where(x => x.EventDate.Date == currentOutScheduleData.EventDate.Date).Select(x => x.Id).ToListAsync();

                var travelData = await Context.RequestSiteTravelAdd.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId && InscheduleIds.Contains(x.inScheduleId) && OutscheduleIds.Contains(x.outScheduleId)).FirstOrDefaultAsync();
                if (travelData != null)
                {
                    var returnData = await (from doc in Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                                            && x.DocumentTag == "ADD"
                                           && !notActiveDocumentAction.Contains(x.CurrentAction)
                                            && x.DateCreated.Value.Date == DateTime.Today.Date && x.EmployeeId == request.EmployeeId)
                                            join assignedEmployee in Context.Employee.AsNoTracking() on doc.AssignedEmployeeId equals assignedEmployee.Id into assignedEmployeeData
                                            from assignedEmployee in assignedEmployeeData.DefaultIfEmpty()

                                            join requestEmployee in Context.Employee.AsNoTracking() on doc.UserIdCreated equals requestEmployee.Id into requestEmployeeData
                                            from requestEmployee in requestEmployeeData.DefaultIfEmpty()
                                            select new CheckRequestDocumentSiteTravelAddResponse
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
                    return new List<CheckRequestDocumentSiteTravelAddResponse>();
                }



            }
            else
            {
                return new List<CheckRequestDocumentSiteTravelAddResponse>();
            }

        }

        #endregion


    }
}
