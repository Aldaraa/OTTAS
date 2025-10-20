using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CompleteRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.CreateRequestExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.GetRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentExternalTravelFeature.UpdateRequestDocumentExternalTravelAdd;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public sealed class RequestExternalTravelAddRepository : BaseRepository<RequestExternalTravelAdd>, IRequestExternalTravelAddRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ITransportCheckerRepository _transportCheckerRepository;   
        public RequestExternalTravelAddRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, IMemoryCache memoryCache, ITransportCheckerRepository transportCheckerRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
            _transportCheckerRepository = transportCheckerRepository;
        }


        #region GetData

        public async Task<GetRequestDocumentExternalTravelAddResponse> GetRequestDocumentExternalTravelAdd(GetRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetRequestDocumentExternalTravelAddResponse();
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
                returnData.EmployeeActive = currentEmployee?.Active;
                returnData.AssignedEmployeeFullName = $"{assignEmployee?.Firstname} {assignEmployee?.Lastname}";
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.DocumentType = currentDocument.DocumentType;
                returnData.UpdatedInfo = $"{currentDocument.DateUpdated} {updatedEmployee?.Firstname} {updatedEmployee?.Lastname}";
                returnData.RequesterFullName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}";
                returnData.RequestUserId = createdEmployee?.Id;
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


                var currentTravelData = await Context.RequestExternalTravelAdd.AsNoTracking().Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                if (currentTravelData != null)
                {

                    var travelData = new GetRequestDocumentExternalTravelAddTravel();


                    var firstscheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentTravelData.FirstScheduleId).FirstOrDefaultAsync();
                    if (firstscheduleData != null)
                    {
                        travelData.FirstScheduleDate = firstscheduleData.EventDate;

                        var inscheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == firstscheduleData.ActiveTransportId).FirstOrDefaultAsync();
                        if (inscheduleTransport != null)
                        {
                            travelData.FirstScheduleDirection = inscheduleTransport.Direction;
                        }


                    }

                    travelData.Id = currentTravelData.Id;
                    travelData.DepartmentId = currentTravelData.DepartmentId;
                    travelData.CostCodeId = currentTravelData?.CostCodeId;
                    travelData.EmployerId = currentTravelData?.EmployerId;
                    travelData.PositionId = currentTravelData?.PositionId;


                    int? firstScheduleSeatsCount = 0;
                    int? lastScheduleSeatsCount = 0;


                    int firstCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == currentTravelData.FirstScheduleId).CountAsync();
                    if (firstscheduleData != null)
                    {
                        travelData.FirstScheduleId = currentTravelData.FirstScheduleId;

                        if (firstscheduleData.Seats.HasValue)
                        {
                            firstScheduleSeatsCount = firstscheduleData.Seats.Value - firstCount;
                        }
                        else
                        {
                            firstScheduleSeatsCount = 0;
                        }


                    }
                    else
                    {
                        firstScheduleSeatsCount = 0;
                    }


                    travelData.FirstScheduleDescription = firstscheduleData?.Description;
                    travelData.FirstScheduleSeatsCount = firstScheduleSeatsCount;

                    if (currentTravelData.LastScheduleId.HasValue)
                    {
                        var lastScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == currentTravelData.LastScheduleId).FirstOrDefaultAsync();
                        if (lastScheduleData != null)
                        {
                            travelData.LastScheduleDate = lastScheduleData.EventDate;

                            var outscheduleTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == lastScheduleData.ActiveTransportId).FirstOrDefaultAsync();
                            if (outscheduleTransport != null)
                            {
                                travelData.LastScheduleDirection = outscheduleTransport.Direction;
                            }

                            travelData.LastScheduleId = currentTravelData.LastScheduleId;


                            int lastCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == currentTravelData.LastScheduleId).CountAsync();
                            if (lastScheduleData.Seats.HasValue)
                            {
                                lastScheduleSeatsCount = lastScheduleData.Seats.Value - lastCount;
                            }
                            else
                            {
                                lastScheduleSeatsCount = 0;
                            }
                           


                            travelData.LastScheduleDescription = lastScheduleData?.Description;
                            travelData.LastScheduleSeatsCount = lastScheduleSeatsCount;

                        }

                      

                    }


                    returnData.TravelData = travelData;
                }



            }
            return returnData;
        }

        #endregion

        #region UpdateAddTravel

        public async Task UpdateRequestDocumentExternalTravelAdd(UpdateRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken)
        {


            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;
            var currentData = await Context.RequestExternalTravelAdd.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                await _transportCheckerRepository.TransportExternalAddValidCheck(currentData.EmployeeId, request.FirstScheduleId, request.LastScheduleId);

                var inSchedule = await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == request.FirstScheduleId);
                var outSchedule = await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == request.LastScheduleId);

                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentData.FirstScheduleId = request.FirstScheduleId;
                currentData.LastScheduleId = request.LastScheduleId;
                currentData.CostCodeId = request.costcodeId;
                currentData.DepartmentId = request.departmentId;
                currentData.EmployerId = request.employerId;

                Context.RequestExternalTravelAdd.Update(currentData);
            }
            else {
                throw new  BadRequestException("Data not found");
            }
            await Task.CompletedTask;


        }

        #endregion


        #region CreateAddTravel

        public async Task<int> CreateRequestExternalTravelAdd(CreateRequestExternalTravelAddRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {
                    var firstScheduleData = await (from inschedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.flightData.FirstScheduleId)
                                                join transport in Context.ActiveTransport.AsNoTracking() on inschedule.ActiveTransportId equals transport.Id into transportData
                                                from transport in transportData.DefaultIfEmpty()
                                                select new
                                                {
                                                    Id = inschedule.Id,
                                                    Direction = transport.Direction,
                                                    EventDate = inschedule.EventDate.Date,
                                                }).FirstOrDefaultAsync();

                    var lastScheduleData = await (from inschedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.flightData.LastScheduleId)
                                                 join transport in Context.ActiveTransport.AsNoTracking() on inschedule.ActiveTransportId equals transport.Id into transportData
                                                 from transport in transportData.DefaultIfEmpty()
                                                 select new
                                                 {
                                                     Id = inschedule.Id,
                                                     Direction = transport.Direction,
                                                     EventDate = inschedule.EventDate.Date,
                                                 }).FirstOrDefaultAsync();

                    if (firstScheduleData == null) {
                        throw new BadRequestException("Schedule not found");
                    }



                    if (firstScheduleData != null && lastScheduleData != null)
                    {

                        if (firstScheduleData.EventDate < lastScheduleData.EventDate)
                        {

                            var onExternalData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == request.documentData.EmployeeId && x.EventDate.Value.Date == firstScheduleData.EventDate && x.RoomId != null).FirstOrDefaultAsync();

                            if (onExternalData == null)
                            {
                                var reqDocument = new RequestDocument();
                                reqDocument.CurrentAction = request.documentData.Action;
                                reqDocument.Active = 1;
                                reqDocument.DateCreated = DateTime.Now;
                                reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                reqDocument.Description = reqDocument.Description;
                                reqDocument.EmployeeId = request.documentData.EmployeeId;
                                reqDocument.DocumentType = RequestDocumentType.ExternalTravel;
                                reqDocument.DocumentTag = "ADD";
                                reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
                                reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;

                                Context.RequestDocument.Add(reqDocument);
                                await Context.SaveChangesAsync();
                                await CreateSaveAttachment(request.Files, reqDocument.Id);


                                CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id);
                                await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                                await Context.SaveChangesAsync();
                                await transaction.CommitAsync(cancellationToken);
                                return reqDocument.Id;
                            }
                            else
                            {
                                throw new BadRequestException($"{firstScheduleData.EventDate} External add travel registration is not possible because this person is on the External");
                            }


                        }
                        else {
                            throw new BadRequestException("The first schedule date cannot be after the last schedule date.");
                        }


                    }
                    else if (firstScheduleData != null && lastScheduleData == null) {

                        var onExternalData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == request.documentData.EmployeeId && x.EventDate.Value.Date == firstScheduleData.EventDate && x.RoomId != null).FirstOrDefaultAsync();

                        if (onExternalData == null)
                        {
                            var reqDocument = new RequestDocument();
                            reqDocument.CurrentAction = request.documentData.Action;
                            reqDocument.Active = 1;
                            reqDocument.DateCreated = DateTime.Now;
                            reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            reqDocument.Description = reqDocument.Description;
                            reqDocument.EmployeeId = request.documentData.EmployeeId;
                            reqDocument.DocumentType = RequestDocumentType.ExternalTravel;
                            reqDocument.DocumentTag = "ADD";
                            reqDocument.AssignedEmployeeId = request.documentData?.AssignedEmployeeId;
                            reqDocument.AssignedRouteConfigId = request.documentData.NextGroupId;

                            Context.RequestDocument.Add(reqDocument);
                            await Context.SaveChangesAsync();
                            await CreateSaveAttachment(request.Files, reqDocument.Id);


                            CreateSaveFlightData(request.flightData, request.documentData.EmployeeId, reqDocument.Id);
                            await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                            await Context.SaveChangesAsync();
                            await transaction.CommitAsync(cancellationToken);
                            return reqDocument.Id;
                        }
                        else
                        {
                            throw new BadRequestException($"{firstScheduleData.EventDate} External add travel registration is not possible because this person is on the External");
                        }

                    }
                    else
                    {
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


        private void  CreateSaveFlightData(CreateRequestDocumentExternalTravelData travelData, int employeeId, int documentId)
        {

            var newRecord = new RequestExternalTravelAdd
            {
                DocumentId = documentId,
                EmployeeId = employeeId,
                Active = 1,
                FirstScheduleId = travelData.FirstScheduleId,
                LastScheduleId = travelData.LastScheduleId,
                DepartmentId = travelData.departmentId,
                CostCodeId = travelData.costcodeId,
                EmployerId = travelData.employerId,
                PositionId = travelData.positionId,
                DateCreated = DateTime.Now,
                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
            };
            Context.RequestExternalTravelAdd.Add(newRecord);
        }
        //CreateRequestDocumentExternalTravelAttachment
        private async Task CreateSaveAttachment(List<CreateRequestDocumentExternalTravelAttachment> attachments, int documentId)
        {
            foreach (var item in attachments)
            {
                var currentFile = await Context.SysFile.AsNoTracking().Where(x => x.Id == item.FileAddressId).FirstOrDefaultAsync();
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

        private async Task SaveDocumentHistory(CreateRequestExternalTravelAddRequest request, int DocumentId, CancellationToken cancellationToken)
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


        #region CompleteAddTrave;

        public async Task CompleteRequestDocumentExternalTravelAdd(CompleteRequestDocumentExternalTravelAddRequest request, CancellationToken cancellationToken)
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
                                var travelData = await Context.RequestExternalTravelAdd.AsNoTracking().FirstOrDefaultAsync(x => x.DocumentId == request.documentId);

                                if (travelData != null)
                                {
                                    var assignedOldGroupId = await Context.RequestGroupConfig.Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();
                                    await AddTravelRequestComplete(request.documentId, cancellationToken);
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

                                    await Context.SaveChangesAsync(cancellationToken);
                                    await transaction.CommitAsync(cancellationToken);
                                }

                            }
                        }
                        else
                        {
                            throw new BadRequestException("Invalid document type");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new BadRequestException(ex.Message);
                }
            }
        }


        public async Task AddTravelRequestComplete(int documentId,
            CancellationToken cancellationToken)
        {

            var request = await Context.RequestExternalTravelAdd.Where(x => x.DocumentId == documentId).FirstOrDefaultAsync(cancellationToken);

            if (request != null)
            {


                if (request.LastScheduleId.HasValue)
                {
                    var lastSchedule = await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == request.LastScheduleId);

                    var lastActiveTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == lastSchedule.ActiveTransportId).FirstOrDefaultAsync();
                    if (lastActiveTransport == null)
                        throw new BadRequestException("Active transport not found");


                    DateTime outtime = DateTime.ParseExact(lastSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                    int outhours = outtime.Hour;
                    int outminutes = outtime.Minute;

                    var lastTransport = new Transport
                    {
                        EmployeeId = request.EmployeeId,
                        PositionId = request.PositionId,
                        DepId = request.DepartmentId,
                        CostCodeId = request.CostCodeId,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                        ActiveTransportId = lastSchedule.ActiveTransportId,
                        EventDate = lastSchedule.EventDate.Date,
                        EventDateTime = lastSchedule.EventDate.Date.AddHours(outhours).AddMinutes(outminutes),
                        ScheduleId = lastSchedule.Id,
                        Active = 1,
                        Direction = lastActiveTransport.Direction,
                        EmployerId = request.EmployerId,
                        Status = "Confirmed",
                        ChangeRoute = $"Request add travel #{request.DocumentId}"

                    };

                    Context.Transport.Add(lastTransport);

                }

                var firstSchedule = await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == request.FirstScheduleId);

                if (firstSchedule == null)
                    throw new BadRequestException("Schedule not found");
                var firstActiveTransport = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == firstSchedule.ActiveTransportId).FirstOrDefaultAsync();

                if (firstActiveTransport == null)
                    throw new BadRequestException("Active transport not found");
                DateTime time = DateTime.ParseExact(firstSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                int hours = time.Hour;
                int minutes = time.Minute;

                var firsttransport = new Transport
                {
                    EmployeeId = request.EmployeeId,
                    PositionId = request.PositionId,
                    DepId = request.DepartmentId,
                    CostCodeId = request.CostCodeId,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    ActiveTransportId = firstSchedule.ActiveTransportId,
                    EventDate = firstSchedule.EventDate.Date,
                    EventDateTime = firstSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                    ScheduleId = firstSchedule.Id,
                    Active = 1,
                    Direction = firstActiveTransport.Direction,
                    EmployerId = request.EmployerId,
                    Status = "Confirmed",
                    ChangeRoute = $"Request external add travel #{request.DocumentId}"



                };

                Context.Transport.Add(firsttransport);



                await Task.CompletedTask;



            }
            else {
                throw new BadRequestException("Request data not found");
            }
               

             

        }


        #endregion


    }

}
