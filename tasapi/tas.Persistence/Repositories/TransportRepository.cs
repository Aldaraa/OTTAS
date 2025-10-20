using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.GetDateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.RoomFeature.FindAvailableByRoomId;
using tas.Application.Features.TransportFeature.AddExternalTravel;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Features.TransportFeature.CheckDataRequest;
using tas.Application.Features.TransportFeature.DeleteScheduleTransport;
using tas.Application.Features.TransportFeature.EmployeeDateTransport;
using tas.Application.Features.TransportFeature.EmployeeExistingTransport;
using tas.Application.Features.TransportFeature.EmployeeTransportGoShow;
using tas.Application.Features.TransportFeature.EmployeeTransportNoShow;
using tas.Application.Features.TransportFeature.GetDataRequest;
using tas.Application.Features.TransportFeature.GetEmployeeTransport;
using tas.Application.Features.TransportFeature.GetEmployeeTransportAll;
using tas.Application.Features.TransportFeature.GetScheduleDetailTransport;
using tas.Application.Features.TransportFeature.RemoveExternalTransport;
using tas.Application.Features.TransportFeature.RemoveTransport;
using tas.Application.Features.TransportFeature.ReScheduleExternalTransport;
using tas.Application.Features.TransportFeature.ReScheduleMultiple;
using tas.Application.Features.TransportFeature.ReScheduleUpdate;
using tas.Application.Features.TransportFeature.SearchReSchedulePeople;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class TransportRepository : BaseRepository<Transport>, ITransportRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly ITransportCheckerRepository _transportCheckerRepository;
        private readonly IMemoryCache _memoryCache;
        public TransportRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, ICheckDataRepository checkDataRepository, IMemoryCache memoryCache, ITransportCheckerRepository transportCheckerRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _checkDataRepository = checkDataRepository;
            _memoryCache = memoryCache;
            _transportCheckerRepository = transportCheckerRepository;
        }

        public async Task<List<GetEmployeeTransportAllResponse>> EmployeeTransportAllSchedule(GetEmployeeTransportAllRequest request, CancellationToken cancellationToken)
        {
            var employeeTransports = await Context.Transport.Where(x =>
                        x.EventDate >= request.startDate
                        && x.EmployeeId == request.employeeId).ToListAsync(cancellationToken);
            var returnData = new List<GetEmployeeTransportAllResponse>();
            if (employeeTransports.Count > 0)
            {
                foreach (var item in employeeTransports)
                {
                    var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == item.ScheduleId)
                                                 join transport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals transport.Id into transportData
                                                 from transport in transportData.DefaultIfEmpty()
                                                 join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                 from tmode in modeData.DefaultIfEmpty()
                                                 select new
                                                 {
                                                     Code = schedule.Code,
                                                     Description = schedule.Description,
                                                     fromLocationId = transport.fromLocationId,
                                                     toLocationId = transport.toLocationId,
                                                     transportMode = tmode.Code

                                                 }).FirstOrDefaultAsync();
                    var newData = new GetEmployeeTransportAllResponse
                    {
                        Id = item.Id,
                        EventDate = item.EventDate,
                        Code = currentSchedule?.Code,
                        Description = currentSchedule?.Description,
                        Direction = item.Direction,
                        ScheduleId = item.ScheduleId,
                        Status = item.Status,
                        fromLocationId = currentSchedule?.fromLocationId,
                        toLocationId = currentSchedule?.toLocationId,
                        EventDateTime = item.EventDateTime,
                        TransportMode = currentSchedule?.transportMode

                    };

                    returnData.Add(newData);
                }

                return returnData.OrderBy(x => x.EventDateTime).ToList();

            }
            else {
                return new List<GetEmployeeTransportAllResponse>();
            }

        }



        public async Task<List<GetEmployeeTransportResponse>> EmployeeTransportSchedule(GetEmployeeTransportRequest request, CancellationToken cancellationToken)
        {
            if (request.startDate.HasValue)
            {
                var employeeTransports = await Context.Transport.AsNoTracking().Where(x =>
                            x.EventDate >= request.startDate
                            && x.EventDate <= request.endDate
                            && x.EmployeeId == request.employeeId).OrderBy(x=> x.EventDateTime).ToListAsync(cancellationToken);
                var returnData = new List<GetEmployeeTransportResponse>();
                if (employeeTransports.Count > 0)
                {
                    foreach (var item in employeeTransports)
                    {
                        var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == item.ScheduleId)
                                                     join transport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals transport.Id into transportData
                                                     from transport in transportData.DefaultIfEmpty()
                                                     join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                     from tmode in modeData.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         Code = schedule.Code,
                                                         Description = schedule.Description,
                                                         fromLocationId = transport.fromLocationId,
                                                         toLocationId = transport.toLocationId,
                                                         transportMode = tmode.Code

                                                     }).FirstOrDefaultAsync();
                        var newData = new GetEmployeeTransportResponse
                        {
                            Id = item.Id,
                            EventDate = item.EventDate,
                            Code = currentSchedule?.Code,
                            Description = currentSchedule?.Description,
                            Direction = item.Direction,
                            ScheduleId = item.ScheduleId,
                            Status = item.Status,
                            fromLocationId = currentSchedule?.fromLocationId,
                            toLocationId = currentSchedule?.toLocationId,
                            TransportMode = currentSchedule?.transportMode

                        };

                        returnData.Add(newData);
                    }

                    return returnData.OrderBy(x => x.EventDate).ToList();

                }
                else
                {
                    return null;
                }
            }
            else
            {

                var employeeLastTransports = await Context.Transport.AsNoTracking().Where(x =>
                        x.EventDate < DateTime.Today && x.EmployeeId == request.employeeId
                        ).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync(cancellationToken);


                var employeeFuturesTransports = await Context.Transport.AsNoTracking().Where(x =>
                        x.EventDate >= DateTime.Today && x.EmployeeId == request.employeeId

                        ).OrderBy(x => x.EventDate).Take(5).OrderBy(x=> x.EventDateTime).ToListAsync(cancellationToken);
                var returnData = new List<GetEmployeeTransportResponse>();
                foreach (var item in employeeFuturesTransports)
                {
                    //var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == item.ScheduleId).Select(x => new { x.Code, x.Description }).FirstOrDefaultAsync(cancellationToken);
                    var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == item.ScheduleId)
                                                 join transport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals transport.Id into transportData
                                                 from transport in transportData.DefaultIfEmpty()
                                                 join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData
                                                 from tmode in modeData.DefaultIfEmpty()
                                                 select new
                                                 {
                                                     Code = schedule.Code,
                                                     Description = schedule.Description,
                                                     fromLocationId = transport.fromLocationId,
                                                     toLocationId = transport.toLocationId,
                                                     transportMode = tmode.Code

                                                 }).FirstOrDefaultAsync();



                    var newData = new GetEmployeeTransportResponse
                    {
                        Id = item.Id,
                        EventDate = item.EventDate,
                        Code = currentSchedule?.Code,
                        Description = currentSchedule?.Description,
                        Direction = item.Direction,
                        ScheduleId = item.ScheduleId,
                        fromLocationId = currentSchedule?.fromLocationId,
                        toLocationId = currentSchedule?.toLocationId,
                        Status = item.Status,
                        TransportMode = currentSchedule?.transportMode

                    };

                    returnData.Add(newData);
                }

                if (employeeLastTransports != null)
                {
                    var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == employeeLastTransports.ScheduleId)
                                                 join transport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals transport.Id into transportData
                                                 from transport in transportData.DefaultIfEmpty()
                                                 join tmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals tmode.Id into modeData 
                                                 from tmode in modeData.DefaultIfEmpty()
                                                 select new
                                                 {
                                                     Code = schedule.Code,
                                                     Description = schedule.Description,
                                                     fromLocationId = transport.fromLocationId,
                                                     toLocationId = transport.toLocationId,
                                                     transportMode = tmode.Code

                                                 }).FirstOrDefaultAsync();
                    var newData = new GetEmployeeTransportResponse
                    {
                        Id = employeeLastTransports.Id,
                        EventDate = employeeLastTransports.EventDate,
                        Code = currentSchedule?.Code,
                        Description = currentSchedule?.Description,
                        Direction = employeeLastTransports.Direction,
                        ScheduleId = employeeLastTransports.ScheduleId,
                        Status = employeeLastTransports.Status,
                        fromLocationId = currentSchedule?.fromLocationId,
                        toLocationId = currentSchedule?.toLocationId,
                        TransportMode = currentSchedule?.transportMode

                    };

                    returnData.Add(newData);
                }

                return returnData.OrderBy(x => x.EventDate).ToList();
            }
        }





        public async Task AddTravel(AddTravelTransportRequest request, CancellationToken cancellationToken)
        {
            await _checkDataRepository.CheckProfile(request.EmployeeId, cancellationToken);

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {


                    var inSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.inScheduleId);
                    var outSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.outScheduleId);
                    if (inSchedule != null && outSchedule != null)
                    {

                        var inActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == inSchedule.ActiveTransportId);
                        var outActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == outSchedule.ActiveTransportId);


                        if (inActiveTransport?.Direction == outActiveTransport?.Direction)
                        {
                            throw new BadRequestException("It cannot be the same direction, please choose a different schedule");
                        }

                        var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.ShiftId).Select(x => new { x.OnSite }).FirstOrDefaultAsync(cancellationToken);

    



                        if (inSchedule.EventDate.Date < outSchedule.EventDate.Date.Date)
                        {
                            var dateRangeRoomFullStatus = await CheckRoomDate(request.RoomId, inSchedule.EventDate.Date, outSchedule.EventDate.Date.AddDays(-1), request.EmployeeId);
                            if (!dateRangeRoomFullStatus)
                            {
                                throw new BadRequestException($"The room is fully booked from {inSchedule.EventDate.ToString("yyyy-MM-dd")}, to " +
                                                        $"{outSchedule.EventDate.ToString("yyyy-MM-dd")}");
                            }


                            if (currentShift.OnSite == 1)
                            {
                                DateTime time = DateTime.ParseExact(inSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int hours = time.Hour;
                                int minutes = time.Minute;

                                if (!await CheckStartDateOnSite(request.EmployeeId, inSchedule.EventDate))
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
                                        ChangeRoute = "Add travel profile",
                                        Status = inSchedule.Seats > intransportcount ? "Confirmed" : "Over booked",
                                        GoShow = request.inScheduleGoShow


                                    };



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
                                        ChangeRoute = "Add travel profile",
                                        EmployerId = request.EmployerId,
                                        Status = outSchedule.Seats > outtransportcount ? "Confirmed" : "Over booked",
                                        GoShow = request.outScheduleGoShow

                                    };

                                    Context.Transport.Add(transportin);
                                    Context.Transport.Add(transportout);


                                    Stopwatch stopWatch = new Stopwatch();
                                    stopWatch.Start();

                                    await NoGoShowSave(request.EmployeeId, request.inScheduleId, request.inScheduleGoShow, string.Empty, false);
                                    await NoGoShowSave(request.EmployeeId, request.outScheduleId, request.outScheduleGoShow, string.Empty, false);

                                    await AddTravelSetRoom(transportin.EventDate.Value.Date, transportout.EventDate.Value.Date, request);

                                    stopWatch.Stop();
                                    Console.WriteLine($"AddTravelSetRoom ==================> {stopWatch.ElapsedMilliseconds}");
                                    await Context.SaveChangesAsync();
                                    await transaction.CommitAsync(cancellationToken);
                                }
                                else
                                {
                                    string errorMessage = $"This employee is onsite on {inSchedule.EventDate.ToString("yyyy-MM-dd")} Unable to add travel";
                               //     await transaction.RollbackAsync(cancellationToken);
                                    throw new BadRequestException(errorMessage);
                                }





                            }
                            else
                            {
                                var errorMessage = new List<string>();
                                errorMessage.Add("RoomStatus has an wrong value");
                           //     await transaction.RollbackAsync(cancellationToken);
                                throw new BadRequestException(errorMessage.ToArray());
                            }

                        }
                        else if (inSchedule.EventDate.Date > outSchedule.EventDate.Date.Date)
                        {


                            if (currentShift.OnSite != 1)
                            {
                                DateTime time = DateTime.ParseExact(inSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int hours = time.Hour;
                                int minutes = time.Minute;
                                var intransportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == inSchedule.Id);


                                DateTime outtime = DateTime.ParseExact(outSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = time.Hour;
                                int outminutes = time.Minute;
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
                                    GoShow = request.inScheduleGoShow

                                };


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
                                    GoShow = request.outScheduleGoShow

                                };

                                Context.Transport.Add(transportin);
                                Context.Transport.Add(transportout);

                                await NoGoShowSave(request.EmployeeId, request.inScheduleId, request.inScheduleGoShow,string.Empty, false);
                                await NoGoShowSave(request.EmployeeId, request.outScheduleId, request.outScheduleGoShow, string.Empty, false);
                                await AddTravelSetRoomOut(transportout.EventDate.Value.Date, transportin.EventDate.Value.Date, request);
                                await Context.SaveChangesAsync();
                                await transaction.CommitAsync(cancellationToken);
                            }
                            else
                            {
                                var errorMessage = new List<string>();
                                errorMessage.Add("RoomStatus has an wrong value");
                          //      await transaction.RollbackAsync(cancellationToken);
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


                                if (!await CheckStartDateOnSite(request.EmployeeId, inSchedule.EventDate))
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
                                        GoShow = request.inScheduleGoShow


                                    };



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
                                        GoShow = request.outScheduleGoShow

                                    };

                                    Context.Transport.Add(transportin);
                                    Context.Transport.Add(transportout);

                                    await NoGoShowSave(request.EmployeeId, request.inScheduleId, request.inScheduleGoShow, string.Empty, false);
                                    await NoGoShowSave(request.EmployeeId, request.outScheduleId, request.outScheduleGoShow, string.Empty, false);

                                    Stopwatch stopWatch = new Stopwatch();
                                    stopWatch.Start();



                                    //    await AddTravelSetRoom(transportin.EventDate.Value.Date, transportout.EventDate.Value.Date, request);

                                    stopWatch.Stop();
                                    Console.WriteLine($"AddTravelSetRoom ==================> {stopWatch.ElapsedMilliseconds}");
                                    await Context.SaveChangesAsync();
                                    await transaction.CommitAsync(cancellationToken);
                                }
                                else
                                {
                                    string errorMessage = $"This employee is onsite on {inSchedule.EventDate.ToString("yyyy-MM-dd")} Unable to add travel";
                                  //  await transaction.RollbackAsync(cancellationToken);
                                    throw new BadRequestException(errorMessage);
                                }





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
                                            GoShow = request.inScheduleGoShow


                                        };



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
                                            GoShow = request.outScheduleGoShow

                                        };



                                        Context.Transport.Add(transportin);
                                        Context.Transport.Add(transportout);

                                        await NoGoShowSave(request.EmployeeId, request.inScheduleId, request.inScheduleGoShow, string.Empty, false);
                                        await NoGoShowSave(request.EmployeeId, request.outScheduleId, request.outScheduleGoShow, string.Empty, false);


                                        await Context.SaveChangesAsync();
                                        await transaction.CommitAsync(cancellationToken);
                                    }
                                    else
                                    {
                                        // Roll back the transaction if an exception occurs
                                        throw new BadRequestException("Unable to create flight data");
                                    }

                                }
                                else
                                {
                                    // Roll back the transaction if an exception occurs
                                    var errorMessage = new List<string>();
                                    errorMessage.Add("RoomStatus has an wrong value");
                                    throw new BadRequestException(errorMessage.ToArray());

                                }
                            }
                        }

                    }
                    else
                    {
                        // Roll back the transaction if an exception occurs
                        throw new BadRequestException("Schedule not found may be deleted");
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


    



        private async Task<bool> CheckStartDateOnSite(int employeeId, DateTime eventDate)
        {
            var onsiteStatus = await Context.EmployeeStatus.AsNoTracking()
                 .AnyAsync(x => x.EventDate.Value.Date == eventDate.Date && x.EmployeeId == employeeId && x.RoomId != null);
            return onsiteStatus;
        }


        public async Task AddTravelSetRoomOut(DateTime StartDate, DateTime EndDate, AddTravelTransportRequest request)
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
                    currentStatus.BedId = null;
                    currentStatus.ShiftId = request.ShiftId;
                    currentStatus.DateUpdated = DateTime.Now;
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
                        ShiftId = request.ShiftId,
                        DepId = request.DepartmentId,
                        Active = 1,
                        CostCodeId = request.CostCodeId,
                        EmployerId = request.EmployerId,
                        PositionId = request.PositionId,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                        DateCreated = DateTime.Now

                    };
                    Context.EmployeeStatus.Add(employeeStatus);

                }
            }


            
            await Task.CompletedTask;
        }




        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, DateTime endDate, int EmployeeId)
        {
            try
            {
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roomId);

                DateTime sdate = startDate;
                DateTime edate = endDate;
                if (startDate > endDate)
                {
                    startDate = endDate;
                    endDate = sdate;
                }

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
                    var dateCountRoom = await Context.EmployeeStatus.Where(x => x.EmployeeId != EmployeeId).AsNoTracking()
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




        public async Task AddTravelSetRoom(DateTime StartDate, DateTime EndDate, AddTravelTransportRequest request )
        {

            if (StartDate.Date == EndDate.Date)
            {


                return;
                

            }
            else {
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.RoomId);
                if (currentRoom != null)
                {
                    DateTime currentDate = StartDate;

                    var currentShift = await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ShiftId);

                    var currentDatesEmployeeStatus = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date >= StartDate.Date && x.EventDate <= EndDate).ToListAsync();
                    while (currentDate.AddDays(1) <= EndDate)
                    {
                        if (currentRoom.VirtualRoom == 1)
                        {

                            var currentStatus = currentDatesEmployeeStatus.FirstOrDefault(x => x.EventDate.Value.Date == currentDate.Date);

                            if (currentStatus == null)
                            {
                                var employeeStatus = new EmployeeStatus
                                {
                                    EventDate = currentDate,
                                    EmployeeId = request.EmployeeId,
                                    ShiftId = request.ShiftId,
                                    RoomId = currentShift?.OnSite == 1 ? request.RoomId : null,
                                    DepId = request.DepartmentId,
                                    Active = 1,
                                    CostCodeId = request.CostCodeId,
                                    EmployerId = request.EmployerId,
                                    PositionId = request.PositionId,
                                    UserIdCreated = 1,
                                    DateCreated = DateTime.Now,
                                    ChangeRoute = $"Add travel Profile",

                                };
                                Context.EmployeeStatus.Add(employeeStatus);
                            }
                            else
                            {
                                currentStatus.ShiftId = request.ShiftId;
                                currentStatus.RoomId = currentShift?.OnSite == 1 ? request.RoomId : null;
                                currentStatus.Active = 1;
                                currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentStatus.ChangeRoute = $"Add travel Profile";
                                Context.EmployeeStatus.Update(currentStatus);

                            }

                            currentDate = currentDate.AddDays(1);

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

                            if (dateGuestCount > currentRoom.BedCount)
                            {
                                if (currentRoom.VirtualRoom == 1)
                                {

                                    var currentStatus = currentDatesEmployeeStatus.FirstOrDefault(x =>
                                         x.EventDate.Value.Date == currentDate.Date);

                                    if (currentStatus == null)
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
                                            UserIdCreated = 1,
                                            DateCreated = DateTime.Now,
                                            ChangeRoute = $"Add travel Profile",

                                        };
                                        Context.EmployeeStatus.Add(employeeStatus);
                                    }
                                    else
                                    {
                                        currentStatus.ShiftId = request.ShiftId;
                                        currentStatus.RoomId = currentShift?.OnSite == 1 ? request.RoomId : null;
                                        currentStatus.Active = 1;
                                        currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentStatus.ChangeRoute  = $"Add travel Profile";
                                        Context.EmployeeStatus.Update(currentStatus);
                                    }



                                }
                            }
                            else
                            {


                                //var currentStatus = await Context.EmployeeStatus.FirstOrDefaultAsync(x =>
                                //      x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == currentDate.Date);

                                var currentStatus = currentDatesEmployeeStatus.FirstOrDefault(x => x.EventDate.Value.Date == currentDate.Date);


                                if (currentStatus == null)
                                {
                                    var employeeStatus = new EmployeeStatus
                                    {
                                        EventDate = currentDate,
                                        EmployeeId = request.EmployeeId,
                                        ShiftId = request.ShiftId,
                                        RoomId = currentShift?.OnSite == 1 ? request.RoomId : null,
                                        BedId = bedId,
                                        DepId = request.DepartmentId,
                                        EmployerId = request.EmployerId,
                                        Active = 1,
                                        CostCodeId = request.CostCodeId,
                                        PositionId = request.PositionId,
                                        UserIdCreated = 1,
                                        DateCreated = DateTime.Now,
                                        ChangeRoute = $"Add travel Profile",

                                    };
                                    Context.EmployeeStatus.Add(employeeStatus);
                                    //     currentDate = currentDate.AddDays(1);

                                }
                                else
                                {
                                    currentStatus.ShiftId = request.ShiftId;
                                    currentStatus.RoomId = currentShift?.OnSite == 1 ? request.RoomId : null;
                                    currentStatus.Active = 1;
                                    currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                    currentStatus.ChangeRoute = $"Add travel Profile";
                                    
                                    Context.EmployeeStatus.Update(currentStatus);
                                }
                            }

                            currentDate = currentDate.AddDays(1);

                        }
                    }
                }
            }
     
            await  Task.CompletedTask;
        }




        private async Task<int?> getRoomBedId(int roomId, DateTime eventDate)
        {
            
                var roomBeds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == roomId).OrderBy(x=> x.Id).ToListAsync();
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

        public async Task ValidateAddTravel(AddTravelTransportRequest request, CancellationToken cancellationToken) 
        {
            var inSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.inScheduleId);
            var outSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.outScheduleId);

            if (inSchedule?.EventDate < outSchedule?.EventDate)
            {

                var employeeTransport = await Context.Transport.AsNoTracking()
                    .Where(e => e.EmployeeId == request.EmployeeId &&
                                e.EventDate >= inSchedule.EventDate &&
                                e.EventDate <= outSchedule.EventDate)
                    .ToListAsync(cancellationToken);
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.RoomId);

                List<string> errors = new List<string>();
                if (inSchedule == null)
                {
                    errors.Add("IN Scedule not found");
                }
                if (outSchedule == null)
                {
                    errors.Add("OUT Scedule not found");
                }
                if (employeeTransport.Count > 0)
                {
                    errors.Add("Transport request have been made during this period. Unable to add a travel");
                }
                if (currentRoom == null)
                {
                    errors.Add("Unable to booking a room");
                }

                if (currentRoom?.VirtualRoom != 1)
                {
                    DateTime currentDate = inSchedule.EventDate.Date;
                    while (currentDate.AddDays(1) <= outSchedule.EventDate.Date)
                    {

                        int roomId = request.RoomId;
                        int? bedId = null;
                        bedId = await getRoomBedId((int)roomId, currentDate);

                        int dateGuestCount =await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.RoomId == request.RoomId &&
                             x.EventDate.Value.Date == currentDate.Date
                         );

                        if (dateGuestCount > currentRoom?.BedCount)
                        {
                            errors.Add($"{currentDate} Room full");
                            currentDate = currentDate.AddDays(1);
                        }
                        else
                        {
                            currentDate = currentDate.AddDays(1);
                        }
                    }
                }



                if (errors.Count > 0)
                {
                    throw new BadRequestException(errors.ToArray());
                }
                await Task.CompletedTask;
            }

        }

        public async Task ValidateAddTravelShort(int EmployeeId, int FirstScheduleId, int LastScheduleId, int RoomId,  CancellationToken cancellationToken)
        {
            var inSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == FirstScheduleId);
            var outSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id ==LastScheduleId);

            if (inSchedule?.EventDate < outSchedule?.EventDate)
            {

                var employeeTransport = await Context.Transport.AsNoTracking()
                    .Where(e => e.EmployeeId == EmployeeId &&
                                e.EventDate >= inSchedule.EventDate &&
                                e.EventDate <= outSchedule.EventDate)
                    .ToListAsync(cancellationToken);
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == RoomId);

                List<string> errors = new List<string>();
                if (inSchedule == null)
                {
                    errors.Add("IN Scedule not found");
                }
                if (outSchedule == null)
                {
                    errors.Add("OUT Scedule not found");
                }
                if (employeeTransport.Count > 0)
                {
                    errors.Add("Transport request have been made during this period. Unable to add a travel");
                }
                if (errors.Count > 0)
                {
                    throw new BadRequestException(errors.ToArray());
                }
                await Task.CompletedTask;
            }

        }


        public async Task<List<GetScheduleDetailTransportResponse>> ScheduleDetail(GetScheduleDetailTransportRequest request, CancellationToken cancellationToken)
        {
            var currentSchedule =await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ScheduleId);
            if (currentSchedule != null)
            {
                DateTime time = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                int hours = time.Hour;
                int minutes = time.Minute;
                var currentSchheduleDateTime = currentSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes);
                var returnData = new List<GetScheduleDetailTransportResponse>();
                var transportData = await Context.Transport.AsNoTracking().Where(x =>
                  x.ScheduleId == request.ScheduleId).ToListAsync(cancellationToken);
                var x = 1;
                foreach (var item in transportData)
                {
                    var currentEmployee =await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.EmployeeId);
                    if (currentEmployee != null) {
                //        var currentCostCode = await Context.CostCodes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.CostCodeId);
                        var currentDepartment = await Context.Department.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.DepId);


                        var currentEmployer = await Context.Employer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.EmployerId);

                        if (item.SeatBlock == 1)
                        {
                            string name = "";
                            if (currentEmployee?.Firstname.Length > 15)
                            {
                                name = $"Person {x}";

                            }
                            else
                            {
                                name = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                            }
                            var newData = new GetScheduleDetailTransportResponse
                            {
                                Id = item.Id,
                                FullName = $"{name}",
                                Status = item.Status

                            };

                            returnData.Add(newData);
                            x++;
                        }
                        else
                        {
                            var newData = new GetScheduleDetailTransportResponse
                            {
                                Id = item.Id,
                                FullName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}",
                                Department = currentDepartment?.Name,
                                DateCreated = item.DateCreated,
                                DateUpdated = item.DateUpdated,
                                Description = currentSchedule.Description,
                                EmployeeId = item.EmployeeId,
                                Status = item.Status,
                                Employer = currentEmployer?.Description,

                            };

                            returnData.Add(newData);
                        }
                    }
                   

                }

                return returnData.OrderBy(c=> c.DateCreated).ToList();

            }
            else {
                return null;
            }
        }


        public async Task DeleteSchedules(DeleteScheduleTransportRequest request, CancellationToken cancellationToken) 
        {
            await ValidateScheduleDelete(request, cancellationToken);
            foreach (var scheduleId in request.ScheduleIds)
            {
                var currentScheduleTransports =await Context.Transport.Where(x => x.ScheduleId == scheduleId).ToListAsync(cancellationToken);
                if (currentScheduleTransports.Count == 0) 
                {
                    var deleteSchedule =await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == scheduleId);
                    if (deleteSchedule != null) {

                        await RemoveScheduleBusstop(scheduleId);

                        Context.TransportSchedule.Remove(deleteSchedule);
                    }
                }
            }

        }


        private async Task RemoveScheduleBusstop(int scheduleId)
        {

            var oldata = await Context.TransportScheduleBusstop.Where(x => x.ScheduleId == scheduleId).ToListAsync();
            if (oldata.Count > 0)
            {
                Context.TransportScheduleBusstop.RemoveRange(oldata);
            }

        }




        private async Task ValidateScheduleDelete(DeleteScheduleTransportRequest request, CancellationToken cancellationToken) 
        {
            List<string> errors = new List<string>();
            foreach (var scheduleId in request.ScheduleIds)
            {
                
                
                var currentScheduleTransports = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == scheduleId).ToListAsync(cancellationToken);
                

                if (currentScheduleTransports.Count > 0)
                {
                    errors.Add($"{scheduleId} This schedule cannot be deleted. This schedule is already booked.");
                }

                //var docstatus = await ValidateDocumentCheckScheduleStatus(scheduleId);
                //if (!docstatus) {
                //    errors.Add($"{scheduleId} This schedule cannot be deleted. This schedule is pending document booked.");
                //}
            }

            if (errors.Count > 0) { 
                throw new BadRequestException(errors.ToArray());
            }

          await  Task.CompletedTask;
        }

        private async Task<bool> ValidateDocumentCheckScheduleStatus(int scheduleId) 
        {
            var rescheduleDocument  = await Context.RequestSiteTravelReschedule.AsNoTracking().Where(x => x.ExistingScheduleId == scheduleId || x.ReScheduleId == scheduleId).Select(x => x.DocumentId).Distinct().ToListAsync();
            if (rescheduleDocument.Count > 0) {
                var currentDoc = await Context.RequestDocument.AsNoTracking().Where(x => rescheduleDocument.Contains(x.Id) && (x.CurrentAction != "Completed" || x.CurrentAction != "Cancelled")).CountAsync();
                if (currentDoc > 0) {
                    return false;
                }
            }
            var addDocument = await Context.RequestSiteTravelAdd.AsNoTracking().Where(x => x.inScheduleId == scheduleId || x.outScheduleId == scheduleId).Select(x => x.DocumentId).Distinct().ToListAsync();
            if (addDocument.Count > 0)
            {
                var currentDoc = await Context.RequestDocument.AsNoTracking().Where(x => addDocument.Contains(x.Id) && (x.CurrentAction != "Completed" || x.CurrentAction != "Cancelled")).CountAsync();
                if (currentDoc > 0)
                {
                    return false;
                }
            }

            var removeDocument = await Context.RequestSiteTravelRemove.Where(x => x.FirstScheduleId == scheduleId || x.LastScheduleId == scheduleId).Select(x => x.DocumentId).Distinct().ToListAsync();
            if (removeDocument.Count > 0)
            {
                var currentDoc = await Context.RequestDocument.AsNoTracking().Where(x => removeDocument.Contains(x.Id) && (x.CurrentAction != "Completed" || x.CurrentAction != "Cancelled")).CountAsync();
                if (currentDoc > 0)
                {
                    return false;
                }
            }


            return true;

        } 
        


        public async Task<EmployeeDateTransportResponse> EmployeeDateTransportSchedule(EmployeeDateTransportRequest request, CancellationToken cancellationToken)
        {
            var currentTransport = await  Context.Transport.AsNoTracking()
                .Where(x => x.EmployeeId == request.employeeId 
                && x.EventDate.Value.Date == request.currentDate.Date 
                && x.Direction == request.direction).Select(x=> new { x.Id, x.ScheduleId, x.ActiveTransportId }).FirstOrDefaultAsync();

            if (currentTransport != null)
            {

                var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == currentTransport.ScheduleId)
                    .Select(x => new { x.Description, x.Code, x.Id }).FirstOrDefaultAsync();

                if (currentSchedule != null)
                {
                    var returnData = new EmployeeDateTransportResponse
                    {
                        Id = currentTransport.Id,
                        ScheduleId = currentSchedule.Id,
                        ScheduleCode = $"{currentSchedule.Code} {currentSchedule.Description}"
                    };

                    return returnData;

                }
                else
                {
                    return new EmployeeDateTransportResponse();
                }

            }
            else {
                return new EmployeeDateTransportResponse();
            }


        }



        public async Task<RemoveTransportResponse?> RemoveSchedule(RemoveTransportRequest request, CancellationToken cancellationToken)
        {

            var defaultOnsiteShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "DS").FirstOrDefaultAsync();
            var defaultOffsiteShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "RR").FirstOrDefaultAsync();
            if (defaultOffsiteShift == null)
            {
                throw new BadRequestException("RR Shift not found. Please register");
            }

            if (defaultOnsiteShift == null)
            {
                throw new BadRequestException("DS Shift not found. Please register");
            }

            var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.shiftId).FirstOrDefaultAsync();
            if (currentShift == null)
            {
                throw new BadRequestException("Shift not found.");
            }



            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {
                    var startSchedule = await Context.Transport.Where(x => x.Id == request.startScheduleId).FirstOrDefaultAsync();
                    var endSchedule = await Context.Transport.Where(x => x.Id == request.endScheduleId).FirstOrDefaultAsync();
                    var returnData = new RemoveTransportResponse();

                    if (startSchedule != null && endSchedule != null)
                    {
                        await _checkDataRepository.CheckProfile(startSchedule.EmployeeId.Value, cancellationToken);
                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == startSchedule.EmployeeId).AsNoTracking().FirstOrDefaultAsync();

                        var employeeId = startSchedule.EmployeeId;
                        var startDate = startSchedule.EventDateTime;
                        var endDate = endSchedule.EventDateTime;


                        if (startSchedule.EventDate.Value.Date == endSchedule.EventDate.Value.Date)
                        {
                            startSchedule.NoShow = request.FirstScheduleNoShow;
                            endSchedule.GoShow = request.LastScheduleNoShow;
                            startSchedule.ChangeRoute = $"Remove  schedule profile";
                            endSchedule.ChangeRoute = $"Remove  schedule profile";

                            await NoGoShowSave(currentEmployee.Id, startSchedule.Id, request.FirstScheduleNoShow, string.Empty, true);
                            await NoGoShowSave(currentEmployee.Id, endSchedule.Id, request.LastScheduleNoShow, string.Empty, true);

                            Context.Transport.Remove(startSchedule);
                            Context.Transport.Remove(endSchedule);





                            await Context.SaveChangesAsync();
                            await transaction.CommitAsync(cancellationToken);
                            returnData.EndScheduleId = endSchedule.ScheduleId;
                            returnData.StartScheduleId = startSchedule.ScheduleId;

                            return returnData;
                        }
                        else
                        {
                            if (startSchedule.Direction == "IN")
                            {

                                var employeeStatues = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date >= startDate.Value.Date
                                && x.EventDate.Value.Date <= endDate.Value.Date
                                && x.EmployeeId == employeeId).ToListAsync();

                                DateTime? currentDate = startDate.Value.Date;

                                while (currentDate.Value.Date < endDate.Value.Date)
                                {
                                    var item = employeeStatues.Where(c => c.EventDate.Value.Date == currentDate.Value.Date).FirstOrDefault();
                                    if (item != null)
                                    {
                                        item.DateUpdated = DateTime.Now;
                                        item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        item.ShiftId = currentShift.OnSite != 1 ? request.shiftId : defaultOffsiteShift.Id;
                                        item.RoomId = null;
                                        item.BedId = null;
                                        item.ChangeRoute = $"Remove  schedule profile";
                                        Context.EmployeeStatus.Update(item);
                                    }
                                    else {
                                        var newRecord = new EmployeeStatus
                                        {
                                            Active = 1,
                                            DateCreated = DateTime.Now,
                                            ShiftId = currentShift.OnSite != 1 ? request.shiftId : defaultOffsiteShift.Id,
                                            ChangeRoute = $"Remove  schedule profile",
                                            EmployeeId = employeeId,
                                            RoomId = null,
                                            BedId = null,
                                            CostCodeId = currentEmployee.CostCodeId,
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


                                //foreach (var item in employeeStatues)
                                //{
                                //    item.DateUpdated = DateTime.Now;
                                //    item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                //    item.ShiftId = currentShift.OnSite != 1 ? request.shiftId : defaultOffsiteShift.Id;
                                //    item.RoomId = null;
                                //    item.BedId = null;
                                //    item.ChangeRoute = $"Remove  schedule profile";
                                //    Context.EmployeeStatus.Update(item);
                                //}

                                var employeeTransports = await Context.Transport
                                       .Where(x => x.EventDateTime > startDate
                                       && x.EventDateTime.Value.Date < endDate
                                       && x.EmployeeId == employeeId).ToListAsync(cancellationToken);
                                foreach (var item in employeeTransports)
                                {
                                    item.ChangeRoute = $"Remove  schedule profile";
                                    Context.Transport.Remove(item);
                                }

                                startSchedule.ChangeRoute = $"Remove  schedule profile";
                                endSchedule.ChangeRoute = $"Remove  schedule profile";

                                startSchedule.NoShow = request.FirstScheduleNoShow;
                                endSchedule.NoShow = request.LastScheduleNoShow;

                                await NoGoShowSave(currentEmployee.Id, startSchedule.Id, request.FirstScheduleNoShow, string.Empty, true);
                                await NoGoShowSave(currentEmployee.Id, endSchedule.Id, request.LastScheduleNoShow, string.Empty, true);


                                Context.Transport.Remove(startSchedule);
                                Context.Transport.Remove(endSchedule);


                                await Context.SaveChangesAsync();
                                await transaction.CommitAsync(cancellationToken);
                                returnData.EndScheduleId = endSchedule.ScheduleId;
                                returnData.StartScheduleId = startSchedule.ScheduleId;

                                return returnData;

                            }
                            else
                            {

                                var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                                if (virtualRoom == null)
                                {
                                    await transaction.RollbackAsync(cancellationToken);
                                    throw new BadRequestException("Please register virtual room");

                                }


                                var beforeRoomData = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date  == startSchedule.EventDate.Value.Date.AddDays(-1) &&  x.EmployeeId == employeeId).FirstOrDefaultAsync();


                                var employeeStatues = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date >= startDate.Value.Date
                                && x.EventDate.Value.Date <= endDate.Value.Date
                                && x.EmployeeId == employeeId).ToListAsync();

                                int RoomId = virtualRoom.Id;
                                if (beforeRoomData != null) {
                                    if (beforeRoomData.RoomId.HasValue)
                                    {
                                        RoomId = beforeRoomData.RoomId.Value;
                                    }
                                    
                                }
                                

                                bool virtulRoom = false;

                                var RoomStatus = await CheckRoomDate(RoomId, startDate.Value.Date, endDate.Value.Date, employeeId.Value);




                                DateTime? currentDate = startDate.Value.Date;

                                while (currentDate.Value.Date < endDate.Value.Date)
                                {
                                    int? bedId = null;

                                    if (RoomId != virtualRoom.Id)
                                    {
                                        bedId = await getRoomBedId(RoomId, currentDate.Value.Date);
                                    }


                                    var item = employeeStatues.Where(c => c.EventDate.Value.Date == currentDate.Value.Date).FirstOrDefault();
                                    if (item != null)
                                    {
                                        item.DateUpdated = DateTime.Now;
                                        item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        item.ShiftId = currentShift.OnSite == 1 ? request.shiftId : defaultOnsiteShift.Id;
                                        item.RoomId = RoomStatus ? RoomId : virtualRoom.Id;
                                        item.BedId = virtulRoom == true ? null : bedId;
                                        item.ChangeRoute = $"Remove  schedule profile";
                                        Context.EmployeeStatus.Update(item);
                                    }
                                    else
                                    {
                                        var newRecord = new EmployeeStatus
                                        {
                                            Active = 1,
                                            DateCreated = DateTime.Now,
                                            ShiftId = currentShift.OnSite == 1 ? request.shiftId : defaultOnsiteShift.Id,
                                            ChangeRoute = $"Remove  schedule profile",
                                            EmployeeId = employeeId,
                                            RoomId = RoomStatus ? RoomId : virtualRoom.Id,
                                            BedId = RoomStatus ? bedId : null,
                                            CostCodeId = currentEmployee?.CostCodeId,
                                            DepId = currentEmployee?.DepartmentId,
                                            EmployerId = currentEmployee?.EmployerId,
                                            PositionId = currentEmployee?.PositionId,
                                            EventDate = currentDate.Value.Date,
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                                        };

                                        Context.EmployeeStatus.Add(newRecord);
                                    }

                                    currentDate = currentDate.Value.AddDays(1);
                                }



                                //foreach (var item in employeeStatues)
                                //{

                                //    if (RoomStatus)
                                //    {
                                //        int? bedId = null;

                                //        if (RoomId != virtualRoom.Id)
                                //        {
                                //            bedId = await getRoomBedId(RoomId, item.EventDate.Value.Date);
                                //        }


                                //        item.DateUpdated = DateTime.Now;
                                //        item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                //        item.ShiftId = currentShift.OnSite == 1 ? request.shiftId : defaultOnsiteShift.Id;
                                //        item.RoomId = RoomId;
                                //        item.BedId = virtulRoom == true ? null : bedId;
                                //        item.ChangeRoute = $"Remove  schedule profile";

                                //        Context.EmployeeStatus.Update(item);
                                //    }
                                //    else
                                //    {
                                //        item.DateUpdated = DateTime.Now;
                                //        item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                //        item.ShiftId = currentShift.OnSite == 1 ? request.shiftId : defaultOnsiteShift.Id;
                                //        item.RoomId = virtualRoom.Id;
                                //        item.BedId = null;
                                //        item.ChangeRoute = $"Remove  schedule profile";
                                //        Context.EmployeeStatus.Update(item);
                                //    }


                                //}

                                var employeeTransports = await Context.Transport
                                       .Where(x => x.EventDate > startDate
                                       && x.EventDate.Value.Date < endDate
                                       && x.EmployeeId == employeeId).ToListAsync(cancellationToken);
                                foreach (var item in employeeTransports)
                                {
                                    item.ChangeRoute = "Remove  schedule profile ";
                                    Context.Transport.Remove(item);
                                }
                                startSchedule.NoShow = request.FirstScheduleNoShow;
                                endSchedule.NoShow = request.LastScheduleNoShow;
                                startSchedule.ChangeRoute = $"Remove  schedule profile";
                                endSchedule.ChangeRoute = $"Remove  schedule profile";

                                await NoGoShowSave(currentEmployee.Id, startSchedule.Id, request.FirstScheduleNoShow, string.Empty, true);
                                await NoGoShowSave(currentEmployee.Id, endSchedule.Id, request.LastScheduleNoShow, string.Empty, true);


                                Context.Transport.Remove(startSchedule);
                                Context.Transport.Remove(endSchedule);

                                try
                                {
                                    //if (request.NoShow == 1)
                                    //{
                                    //    var newData = new TransportNoShow
                                    //    {
                                    //        Active = 1,
                                    //        DateCreated = DateTime.Now,
                                    //        Direction = startSchedule.Direction,
                                    //        EmployeeId = startSchedule.EmployeeId,
                                    //        EventDate = startSchedule.EventDate,
                                    //        EventDateTime = startSchedule.EventDateTime,
                                    //        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                    //    };


                                    //    var newData2 = new TransportNoShow
                                    //    {
                                    //        Active = 1,
                                    //        DateCreated = DateTime.Now,
                                    //        Direction = endSchedule.Direction,
                                    //        EmployeeId = endSchedule.EmployeeId,
                                    //        EventDate = endSchedule.EventDate,
                                    //        EventDateTime = endSchedule.EventDateTime,
                                    //        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id


                                    //    };
                                    //    Context.TransportNoShow.Add(newData);
                                    //    Context.TransportNoShow.Add(newData2);

                                    //}

                                }
                                catch (Exception ex)
                                {
                                    await transaction.RollbackAsync(cancellationToken);
                                    throw new BadRequestException("Transport data not found. Please try again");
                                }
                            }

                            await Context.SaveChangesAsync();
                            await transaction.CommitAsync(cancellationToken);
                            returnData.EndScheduleId = endSchedule.ScheduleId;
                            returnData.StartScheduleId = startSchedule.ScheduleId;

                            return returnData;
                        }
                    }
                    else
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw new BadRequestException("Transport data not found");
                        // Roll back the transaction if an exception occurs
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



        #region SearchReschuleData 



        public async Task<SearchReSchedulePeopleResponse> SearchReschuleData(SearchReSchedulePeopleRequest request, CancellationToken cancellationToken)
        {
            var currentDateSchedule = await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.Id == request.ScheduleId && x.Active == 1)
                .Select(x => new { x.Id, x.EventDate, x.Active, x.ActiveTransportId, x.Seats })
                .FirstOrDefaultAsync();

            if (currentDateSchedule == null)
            {
                throw new BadRequestException("Schedule not not found");
            }
            else {

                var currentActiveTransport = await Context.ActiveTransport.AsNoTracking()
                    .Where(x => x.Id == currentDateSchedule.ActiveTransportId)
                    .Select(x => new { x.Id, x.Direction, x.DayNum, x.Seats })
                    .FirstOrDefaultAsync();



                var matchingTransport = await Context.Transport
                            .Where(x => x.ScheduleId == currentDateSchedule.Id)
                            .ToListAsync(cancellationToken);

                var returnData = new SearchReSchedulePeopleResponse();
                var returnPeoples = new List<SearchReSchedulePeople>();
                var returnShifts = new List<SearchReScheduleTOShift>();

                foreach (var item in matchingTransport)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking()
                        .Where(x => x.Id == item.EmployeeId)
                        .Select(x => new { x.Id, x.Lastname, x.Firstname, x.CostCodeId, x.DepartmentId, x.EmployerId, x.PositionId })
                        .FirstOrDefaultAsync(cancellationToken);

                    var currentShift = await (from employeeStatus in Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == item.EmployeeId && x.EventDate.Value.Date == item.EventDate.Value.Date)
                                              join shift in Context.Shift.AsNoTracking() on employeeStatus.ShiftId equals shift.Id into shifData
                                              from shift in shifData.DefaultIfEmpty()
                                              join shiftColor in Context.Color.AsNoTracking() on shift.ColorId equals shiftColor.Id into shiftColorData
                                              from shiftColor in shiftColorData.DefaultIfEmpty()
                                              select new {
                                                  shiftDescr = shift.Code,
                                                  shiftColor = shiftColor.Code
                                              }).FirstOrDefaultAsync();



                    if (currentEmployee == null)
                        return null;

                    var currentDepartment = await Context.Department.AsNoTracking()
                        .Where(x => x.Id == currentEmployee.DepartmentId)
                        .Select(x => new { x.Name }).FirstOrDefaultAsync(cancellationToken);

                    var currentEmployer = await Context.Employer.AsNoTracking()
                        .Where(x => x.Id == currentEmployee.EmployerId)
                        .Select(x => new { x.Description, x.Code })
                        .FirstOrDefaultAsync(cancellationToken);

                    var currentCostCode = await Context.CostCodes.AsNoTracking()
                        .Where(x => x.Id == currentEmployee.CostCodeId)
                        .Select(x => new { x.Description, x.Code })
                        .FirstOrDefaultAsync(cancellationToken);

                    var currentPosition = await Context.Position.AsNoTracking()
                        .Where(x => x.Id == item.PositionId)
                        .Select(x => new { x.Description, x.Code })
                        .FirstOrDefaultAsync(cancellationToken);

                    var currentActiveTransort = await Context.ActiveTransport.AsNoTracking()
                        .Where(x => x.Id == currentEmployee.PositionId)
                        .Select(x => new { x.Description, x.Code })
                        .FirstOrDefaultAsync(cancellationToken);

                    var newData = new SearchReSchedulePeople
                    {
                        Id = item.Id,
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                        Department = currentDepartment?.Name,
                        EmployerName = $"{currentEmployer?.Description}",
                        Status = item.Status,
                        DateCreated = item.DateCreated,
                        CostCodeDescr = currentCostCode != null ? $"{currentCostCode?.Description}" : null,
                        EmployeeId = item.EmployeeId,
                        Position = currentPosition != null ? $"{currentPosition?.Description}" : null,
                        TransportCode = currentActiveTransort != null ? $"{currentActiveTransort?.Code} {currentActiveTransort?.Description}" : null,
                        CreatedDate = item.DateCreated,
                        ShiftCode = currentShift != null ? $"{currentShift.shiftDescr}" : null,
                        ShiftCodeColor = currentShift != null ? $"{currentShift.shiftColor}" : null


                    };

                    returnPeoples.Add(newData);
                }



                if (currentActiveTransport?.Direction == "IN")
                {
                    if (currentDateSchedule.EventDate.Date >= request.toDate.Date)
                    {
                        returnShifts = await Context.Shift.AsNoTracking().Where(x => x.OnSite == 1 && x.Active == 1)
                            .Select(x => new SearchReScheduleTOShift { Id = x.Id, Code = x.Code, Description = x.Description, OnSite = x.OnSite })
                            .ToListAsync();
                    }
                    else
                    {
                        returnShifts = await Context.Shift.AsNoTracking().Where(x => x.OnSite == 0 && x.Active == 1)
                            .Select(x => new SearchReScheduleTOShift { Id = x.Id, Code = x.Code, Description = x.Description, OnSite = x.OnSite })
                            .ToListAsync();
                    }

                }
                else
                {

                    if (currentDateSchedule.EventDate.Date >= request.toDate.Date)
                    {

                        returnShifts = await Context.Shift.AsNoTracking()
                        .Where(x => x.OnSite == 0 && x.Active == 1)
                        .Select(x => new SearchReScheduleTOShift { Id = x.Id, Code = x.Code, Description = x.Description, OnSite = x.OnSite })
                        .ToListAsync();
                    }
                    else {

                        returnShifts = await Context.Shift.AsNoTracking()
                        .Where(x => x.OnSite == 1 && x.Active == 1)
                        .Select(x => new SearchReScheduleTOShift { Id = x.Id, Code = x.Code, Description = x.Description, OnSite = x.OnSite })
                        .ToListAsync();
                    }
                }

                    returnData.Peoples = returnPeoples;
                    returnData.ShifData = returnShifts;
                    returnData.toSchdules = await GetDateSchedule(request.toDate, currentActiveTransport.Direction, cancellationToken);
                
                    return returnData;
            }

           
        }

        private async Task<List<SearchReScheduleToSchedule>> GetDateSchedule(DateTime EventDate, string Direction, CancellationToken cancellationToken)
        {
            var transportIds =await Context.ActiveTransport.AsNoTracking()
                 .Where(x => x.DayNum == EventDate.ToString("dddd") && x.Direction == Direction && x.Active == 1)
                 .Select(x => x.Id)
                 .ToArrayAsync();

            var results =await Context.TransportSchedule.AsNoTracking()
                .Where(x => x.EventDate.Date == EventDate.Date  && transportIds.Contains(x.ActiveTransportId))
                .ToListAsync();
            var returnData = new List<SearchReScheduleToSchedule>();
            foreach (var item in results)
            {

                var currentTransport = await Context.ActiveTransport.AsNoTracking().Where(a => a.Id == item.ActiveTransportId)
                    .Select(x => new { x.Code, x.Description }).FirstOrDefaultAsync(cancellationToken);
                var newData = new SearchReScheduleToSchedule
                {
                    ActiveTransportId = item.ActiveTransportId,
                    ScheduleId = item.Id,
                    ScheduleDescription = $"{currentTransport?.Code} {EventDate.ToString("dddd")} {item.Description}"

                };
                returnData.Add(newData);
            }


            return returnData;
        }





    

        private async Task ChangeShiftStatus(int shiftId, int oldScheduled, int scheduleId, int employeeId, int? RoomId, CancellationToken cancellationToken)
        {
            var currentTransport = await Context.Transport.Where(x => x.ScheduleId == oldScheduled && x.EmployeeId == employeeId).FirstOrDefaultAsync();
            if (currentTransport != null)
            {

                await _checkDataRepository.CheckProfile(currentTransport.EmployeeId.Value, cancellationToken);
                var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == shiftId).FirstOrDefaultAsync();
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentTransport.EmployeeId).FirstOrDefaultAsync();
                var newSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId).FirstOrDefaultAsync();
                var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();

                var currentRoomDataId = virtualRoom.Id;

                if (RoomId.HasValue)
                {
                    var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == RoomId.Value).FirstOrDefaultAsync();
                    if (currentRoom != null)
                    {
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

                            var nexttransport = await Context.Transport
                                .Where(x => x.EventDate.Value.Date < newSchedule.EventDate.Date && x.EventDate.Value.Date > currentTransport.EventDate.Value.Date
                            && x.EmployeeId == currentTransport.EmployeeId).FirstOrDefaultAsync();
                            if (nexttransport == null)
                            {

                                var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

                                DateTime outtime = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = outtime.Hour;
                                int outminutes = outtime.Minute;



                                var currentRoomCheck = await CheckRoomDate(currentRoomDataId, currentDate, newSchedule.EventDate.Date, currentTransport.EmployeeId.Value);


                                if (!currentRoomCheck)
                                {
                                  //  currentRoomDataId = virtualRoom.Id;
                                    //
                                    throw new BadRequestException("Old room full. Contact Accommodation team.");
                                }




                                while (currentDate < newSchedule.EventDate.Date)
                                {
                                    var bedId = await getRoomBedId(currentRoomDataId, currentDate);
                                    var currentDateStatus = await Context.EmployeeStatus
                                        .Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == currentTransport.EmployeeId)
                                        .FirstOrDefaultAsync();
                                    if (currentDateStatus != null)
                                    {
                                        currentDateStatus.BedId = virtualRoom.Id == currentRoomDataId ? null : bedId;
                                        currentDateStatus.DateUpdated = DateTime.Now;
                                        currentDateStatus.RoomId = currentRoomDataId;
                                        currentDateStatus.ShiftId = shiftId;
                                        currentDateStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentDateStatus.ChangeRoute = $"Reschedule from Profile";
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
                                            ChangeRoute = $"Reschedule from Profile",
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                        };
                                        Context.EmployeeStatus.Add(newEmplpyeeStatus);
                                    }

                                    currentDate = currentDate.AddDays(1);
                                }

                                currentTransport.DateUpdated = DateTime.Now;
                                currentTransport.DateCreated = DateTime.Now;

                                currentTransport.ScheduleId = newSchedule.Id;
                                currentTransport.EventDate = newSchedule.EventDate;
                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                currentTransport.EventDateTime = newSchedule.EventDate.AddHours(outhours).AddMinutes(outminutes);
                                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentTransport.ChangeRoute = $"Reschedule from Profile";

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

                                var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

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
                                        currentDateStatus.DateUpdated = DateTime.Now;
                                        currentDateStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentDateStatus.ChangeRoute = $"Reschedule from Profile";
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
                                            ChangeRoute = $"Reschedule from Profile",
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                        };
                                        Context.EmployeeStatus.Add(newEmplpyeeStatus);
                                    }

                                    currentDate = currentDate.AddDays(1);
                                }
                                currentTransport.DateUpdated = DateTime.Now;
                                currentTransport.DateCreated = DateTime.Now;
                                currentTransport.ScheduleId = newSchedule.Id;
                                currentTransport.EventDate = newSchedule.EventDate;
                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                currentTransport.EventDateTime = newSchedule.EventDate.AddHours(outhours).AddMinutes(outminutes);
                                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentTransport.ChangeRoute = $"Reschedule from Profile";
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
                                var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

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
                                        currentDateStatus.ChangeRoute = $"Reschedule from Profile";
                                        currentDateStatus.DateUpdated = DateTime.Now;
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
                                            ChangeRoute = $"Reschedule from Profile",
                                            EventDate = currentDate.Date,
                                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                        };
                                        Context.EmployeeStatus.Add(newEmplpyeeStatus);
                                    }

                                    currentDate = currentDate.AddDays(1);
                                }
                                currentTransport.DateUpdated = DateTime.Now;
                                currentTransport.DateCreated = DateTime.Now;

                                currentTransport.ScheduleId = newSchedule.Id;
                                currentTransport.EventDate = newSchedule.EventDate;
                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                currentTransport.EventDateTime = newSchedule.EventDate.AddHours(outhours).AddMinutes(outminutes);
                                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentTransport.ChangeRoute = $"Reschedule from Profile";
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

                                var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == newSchedule.Id).FirstOrDefaultAsync();

                                DateTime outtime = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int outhours = outtime.Hour;
                                int outminutes = outtime.Minute;


                                var currentRoomCheck = await CheckRoomDate(currentRoomDataId, currentDate, newSchedule.EventDate.Date, currentTransport.EmployeeId.Value);


                                if (!currentRoomCheck)
                                {
                                //    currentRoomDataId = virtualRoom.Id;

                                    throw new BadRequestException("Old room full. Contact Accommodation team.");

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
                                        currentDateStatus.ChangeRoute = $"Reschedule from Profile";
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
                                            ChangeRoute = $"Reschedule from Profile",
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
                                currentTransport.ChangeRoute = $"Reschedule from Profile";
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



   



        private async Task<bool> GetRescheduleStatusRoomId(DateTime startDate, DateTime endDate, int RoomId)
        {

            var room = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == RoomId);
            DateTime currentDate = startDate;

            FindAvailableByRoomIdResponse result = new FindAvailableByRoomIdResponse();

            bool status = true;
            while (currentDate <= endDate)
            {
                int? activeBed = await GetAvailableBeds(room.Id, currentDate);
                if (activeBed == null || activeBed == 0)
                {
                    status = false;
                }

                currentDate = currentDate.AddDays(1);
            }
            if (status)
            { 
                return true;
            }
            return false;
        }


        public async Task<int?> GetAvailableBeds(int roomId, DateTime eventDate)
        {
            var statusRoomDate =await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate == eventDate.Date && x.RoomId == roomId).ToListAsync();

            if (statusRoomDate != null)
            {
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roomId);
                if (currentRoom != null)
                {
                    if (currentRoom.VirtualRoom == 1)
                    {
                        return currentRoom.BedCount;
                    }
                    else
                    {
                        return currentRoom.BedCount - statusRoomDate.Count;
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

        public async Task<int?> GetDateBedId(int roomId, DateTime eventDate, List<int> RoomBedIds)
        {

            if (RoomBedIds.Count == 0)
            {
                return null;
            }
            else { 
            
                var NonActiveBedIds = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EventDate.Value.Date == eventDate.Date && x.RoomId == roomId)
                .Select(x => x.BedId.Value)
                .ToListAsync();

                int returnRoomId = 0;
                foreach (int id in NonActiveBedIds)
                {
                    if (!RoomBedIds.Contains(id))
                    {
                        returnRoomId = id;
                    }
                }

                if (returnRoomId == 0)
                {
                    returnRoomId = RoomBedIds[0];
                }


                return returnRoomId;
            }
           

        }

        public async Task<List<EmployeeTransportNoShowResponse>> EmployeeTransportNoShow(EmployeeTransportNoShowRequest request, CancellationToken cancellationToken)
        { 
            List<EmployeeTransportNoShowResponse> result = new List<EmployeeTransportNoShowResponse>();
            var data =await Context.TransportNoShow.AsNoTracking()
                .Where(x => x.EmployeeId == request.employeeId && x.EventDate.Value.Year == request.year).ToListAsync();

            foreach (var item in data)
            {
                var newData = new EmployeeTransportNoShowResponse
                {
                    Id = item.Id,
                    EventDate = item.EventDate,
                    EventDateTime = item.EventDateTime,
                    Direction = item.Direction,
                    Description = item.Description,
                    Reason = item.Reason
                };

                result.Add(newData);
            }

            return result;
        }


        public async Task<List<EmployeeTransportGoShowResponse>> EmployeeTransportGoShow(EmployeeTransportGoShowRequest request, CancellationToken cancellationToken)
        {
            List<EmployeeTransportGoShowResponse> result = new List<EmployeeTransportGoShowResponse>();
            var data = await Context.TransportGoShow.AsNoTracking()
                .Where(x => x.EmployeeId == request.employeeId && x.EventDate.Value.Year == request.year).ToListAsync();

            foreach (var item in data)
            {
                var newData = new EmployeeTransportGoShowResponse
                {
                    Id = item.Id,
                    EventDate = item.EventDate,
                    EventDateTime = item.EventDateTime,
                    Direction = item.Direction,
                    Description = item.Description,
                    Reason = item.Reason
                };

                result.Add(newData);
            }

            return result;
        }








        public async Task<int?> ReScheduleUpdate(ReScheduleUpdateRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {

                    int? returnScheduleId;
                    var currentTransport = await Context.Transport.Where(x => x.Id == request.oldTransportId).FirstOrDefaultAsync();
                    if (currentTransport != null)
                    {

                        await _checkDataRepository.CheckProfile(currentTransport.EmployeeId.Value, cancellationToken);
                        var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.ShiftId).FirstOrDefaultAsync();
                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentTransport.EmployeeId).FirstOrDefaultAsync();
                        var newSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.ScheduleId).FirstOrDefaultAsync();
                        var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();

                        var beforeRoomData = await Context.EmployeeStatus.AsNoTracking()
                    .Where(x => x.EmployeeId == currentTransport.EmployeeId && x.EventDate < currentTransport.EventDate && x.RoomId != null).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync(); ;


                        if (currentTransport.Direction == "OUT")
                        {
                            returnScheduleId = currentTransport.ScheduleId;
                            if (newSchedule.EventDate.Date > currentTransport.EventDate.Value.Date)
                            {

                                if (currentShift.OnSite == 1)
                                {
                                    var currentDate = currentTransport.EventDate.Value.Date;

                                    var nexttransport = await Context.Transport.AsNoTracking()
                                        .Where(x => x.EventDate.Value.Date < newSchedule.EventDate.Date && x.EventDate.Value.Date > currentTransport.EventDate.Value.Date
                                    && x.EmployeeId == currentTransport.EmployeeId).FirstOrDefaultAsync();
                                    if (nexttransport == null)
                                    {


                                        await ChangeShiftStatus(request.ShiftId, currentTransport.ScheduleId.Value, newSchedule.Id, currentTransport.EmployeeId.Value, beforeRoomData?.RoomId, cancellationToken);


                                        await NoGoShowSave(currentEmployee.Id, currentTransport.ScheduleId.Value, request.ExistingScheduleIdNoShow, string.Empty, true);
                                        await NoGoShowSave(currentEmployee.Id, newSchedule.Id, request.ReScheduleGoShow, string.Empty, false);


                                        string cacheEntityName = $"Employee_{currentTransport.EmployeeId}";
                                        _memoryCache.Remove($"API::{cacheEntityName}");

                                        await Context.SaveChangesAsync();
                                        await transaction.CommitAsync(cancellationToken);

                                        return returnScheduleId;

                                    }
                                    else
                                    {
                             ///           await transaction.RollbackAsync(cancellationToken);
                                        throw new BadRequestException($"" +
                                            $"There is a flight to {nexttransport.Direction} on {nexttransport.EventDate.Value.Date.ToShortDateString()} flights can be changed before this date");
                                    }
                                }
                                else
                                {
                            ///        await transaction.RollbackAsync(cancellationToken);
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
                                        var nextRoomData = await Context.EmployeeStatus.AsNoTracking()
                                     .Where(x => x.EmployeeId == currentTransport.EmployeeId && x.EventDate >= currentTransport.EventDate && x.RoomId != null).OrderBy(x => x.EventDate).FirstOrDefaultAsync();


                                        await ChangeShiftStatus(request.ShiftId, currentTransport.ScheduleId.Value, newSchedule.Id, currentTransport.EmployeeId.Value, nextRoomData?.RoomId, cancellationToken);

                                        await NoGoShowSave(currentEmployee.Id, currentTransport.ScheduleId.Value, request.ExistingScheduleIdNoShow, string.Empty, true);
                                        await NoGoShowSave(currentEmployee.Id, newSchedule.Id, request.ReScheduleGoShow, string.Empty, false);

                                        await Context.SaveChangesAsync();
                                        await transaction.CommitAsync(cancellationToken);

                                        return returnScheduleId;
                                    }
                                    else
                                    {
                                 //       await transaction.RollbackAsync(cancellationToken);
                                        throw new BadRequestException($"" +
                                            $"There is a flight to {beforetransport.Direction} on {beforetransport.EventDate.Value.Date.ToShortDateString()} flights can be changed after this date");
                                    }
                                }
                                else
                                {
                             //       await transaction.RollbackAsync(cancellationToken);
                                    throw new BadRequestException($"OffSite type ShiftStatus can be changed");
                                }
                            }

                        }
                        if (currentTransport.Direction == "IN")
                        {

                            returnScheduleId = currentTransport.ScheduleId;
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


                                        await ChangeShiftStatus(request.ShiftId, currentTransport.ScheduleId.Value, newSchedule.Id, currentTransport.EmployeeId.Value, beforeRoomData?.RoomId, cancellationToken);

                                        await NoGoShowSave(currentEmployee.Id, currentTransport.ScheduleId.Value, request.ExistingScheduleIdNoShow, string.Empty, true);
                                        await NoGoShowSave(currentEmployee.Id, newSchedule.Id, request.ReScheduleGoShow, string.Empty, false);
                                        await Context.SaveChangesAsync();
                                        await transaction.CommitAsync(cancellationToken);
                                        return returnScheduleId;
                                    }
                                    else
                                    {
                                        await transaction.RollbackAsync(cancellationToken);
                                        throw new BadRequestException($"" +
                                            $"There is a flight to {nexttransport.Direction} on {nexttransport.EventDate.Value.Date.ToShortDateString()} flights can be changed before this date");
                                    }
                                }
                                else
                                {
                          //          await transaction.RollbackAsync(cancellationToken);
                                    throw new BadRequestException($"OffSite type ShiftStatus can be changed");
                                }

                            }
                            else
                            {
                                if (currentShift.OnSite == 1)
                                {
                                    var currentDate = currentTransport.EventDate.Value.Date;

                                    var beforetransport = await Context.Transport.AsNoTracking()
                                     .Where(x => x.EventDate.Value.Date > newSchedule.EventDate.Date
                                 && x.EmployeeId == currentTransport.EmployeeId && x.EventDate.Value.Date < currentDate.Date).FirstOrDefaultAsync();
                                    if (beforetransport == null)
                                    {
                                        var nextRoomData = await Context.EmployeeStatus.AsNoTracking()
                                            .Where(x => x.EmployeeId == currentTransport.EmployeeId && x.EventDate >= currentTransport.EventDate && x.RoomId != null).OrderBy(x => x.EventDate).FirstOrDefaultAsync();




                                        await ChangeShiftStatus(request.ShiftId, currentTransport.ScheduleId.Value, newSchedule.Id, currentTransport.EmployeeId.Value, nextRoomData?.RoomId, cancellationToken);

                                        await NoGoShowSave(currentEmployee.Id, currentTransport.ScheduleId.Value, request.ExistingScheduleIdNoShow, string.Empty, true);
                                        await NoGoShowSave(currentEmployee.Id, newSchedule.Id, request.ReScheduleGoShow, string.Empty, false);

                                        await Context.SaveChangesAsync();
                                        await transaction.CommitAsync(cancellationToken);

                                        return returnScheduleId;

                                    }
                                    else
                                    {
                              //          await transaction.RollbackAsync(cancellationToken);
                                        throw new BadRequestException($"" +
                                            $"There is a flight to {beforetransport.Direction} on {beforetransport.EventDate.Value.Date.ToShortDateString()} flights can be changed after this date");
                                    }
                                }
                                else
                                {
                             //       await transaction.RollbackAsync(cancellationToken);
                                    throw new BadRequestException($"Onsite type ShiftStatus can be changed");
                                }

                            }
                        }
                        else {
                            return null;
                        }


                    }
                    else {
                 //       await transaction.RollbackAsync(cancellationToken);
                        throw new BadRequestException($"Onsite type ShiftStatus can be changed");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    throw new BadRequestException(ex.Message);
                }
            }

        }




        #endregion


        #region EmployeeExistingTransport
        public async Task<List<EmployeeExistingTransportResponse>> EmployeeExistingTransportSchedule(EmployeeExistingTransportRequest request, CancellationToken cancellationToken)
        {

            var returnData = new List<EmployeeExistingTransportResponse>();
            var employeeTransports = await Context.Transport.AsNoTracking()
                .Where(x => x.EmployeeId == request.employeeId && x.EventDate.Value.Date >= request.startDate && x.EventDate < request.endDate)
                .OrderBy(x=> x.EventDateTime)
                .ToListAsync();

            foreach (var item in employeeTransports)
            { 

                var transportScheduleData =await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == item.ScheduleId)
                                             join transport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals transport.Id
                                             join fromlocation in Context.Location .AsNoTracking()on transport.fromLocationId equals fromlocation.Id
                                             join tolocation in Context.Location.AsNoTracking() on transport.toLocationId equals tolocation.Id
                                             select new
                                             {
                                                 id = schedule.Id,
                                                 ETA = schedule.ETA,
                                                 ETD = schedule.ETD,
                                                 scheduleCode = schedule.Code,
                                                 tolocationCode = tolocation.Code,
                                                 toLocationId =transport.toLocationId,
                                                 fromLocationId = transport.fromLocationId,
                                                 fromlocationCode = fromlocation.Code,
                                                 ScheduleId = schedule.Id

                                             }).FirstOrDefaultAsync() ;

                var newRecord = new EmployeeExistingTransportResponse
                {
                    Id = item.Id,
                    Direction = item.Direction,
                    status = item.Status,
                    ScheduleCode = transportScheduleData?.scheduleCode,
                    ScheduleId = transportScheduleData?.ScheduleId,
                    TravelDate = item.EventDate,
                    ETA = transportScheduleData?.ETA,
                    ETD = transportScheduleData?.ETD,
                    fromLocationId = transportScheduleData?.fromLocationId,
                    toLocationId = transportScheduleData?.toLocationId,
                    fromLocationCode = transportScheduleData?.fromlocationCode,
                    toLocationCode = transportScheduleData?.tolocationCode


                };

                returnData.Add(newRecord);
            }


            return returnData;
            
        }

        #endregion


        #region GetDataRequest

        public async Task<GetDataRequestResponse> GetDataRequestRequestChange(GetDataRequestRequest request, CancellationToken cancellationToken)
        {


            if(request.key != "taskey") {
                throw new BadRequestException("Invalid key");
            }
            var returnData = new GetDataRequestResponse();
            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                try
                {

                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        await sqlConnection.OpenAsync();
                    }
                    if (sqlConnection.State == ConnectionState.Open)
                    {

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.CommandText = request.datarequest;
                            command.CommandTimeout = 300;


                            try
                            {
                                using (var result = await command.ExecuteReaderAsync())
                                {
                                    var dynamicList = new List<dynamic>();
                                    int rowNumber = 0;
                                    while (await result.ReadAsync())
                                    {
                                        dynamic d = new ExpandoObject();
                                        d.No = ++rowNumber;
                                        for (int i = 0; i < result.FieldCount; i++)
                                        {
                                            ((IDictionary<string, object>)d).Add(result.GetName(i), result[i]);
                                        }
                                        dynamicList.Add(d);
                                    }
       
                                    returnData.Data = dynamicList;
                                    return returnData;
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new BadRequestException(ex.Message);
                            }

                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Database connection is not open.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while executing the query.", ex);
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Database connection is not of type SqlConnection.");
            }
        }



        public async Task CheckDataRequestRequestChange(CheckDataRequestRequest request, CancellationToken cancellationToken) 
        {
            if (request.key != "taskey")
            {
                throw new BadRequestException("Invalid key");
            }
            if (Context.Database.GetDbConnection() is SqlConnection sqlConnection)
            {
                try
                {
                    if (sqlConnection.State == ConnectionState.Closed)
                    {
                        await sqlConnection.OpenAsync();
                    }
                    if (sqlConnection.State == ConnectionState.Open)
                    {

                        using (var command = sqlConnection.CreateCommand())
                        {
                            command.CommandText = request.datarequest;
                            command.CommandTimeout = 300;


                            try
                            {
                                int affectedRows = await command.ExecuteNonQueryAsync();
                                return;
                            }
                            catch (Exception ex)
                            {
                                throw new BadRequestException(ex.Message);
                            }

                        }
                    }
                    else
                    {
                        throw new BadRequestException("Database connection is not open.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while executing the query.", ex);
                }
                finally
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
            else
            {
                throw new BadRequestException("Database connection is not of type SqlConnection.");
            }
        }



        #endregion


     


        private class GetRescheduleRoom
        { 
            public int RoomId { get; set; }
            
            public int? BedId { get; set; }
        }

    }

    
  

}
