using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Application.Features.RoomFeature.DateProfileRoomExport;
using tas.Application.Features.TransportScheduleFeature.BusstopTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.CreateScheduleActiveTransport;
using tas.Application.Features.TransportScheduleFeature.CreateScheduleDriveTransport;
using tas.Application.Features.TransportScheduleFeature.GetDateDriveTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.GetMonthTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.ManageTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.SearchTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.SeatInfoTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleExport;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo;
using tas.Application.Features.TransportScheduleFeature.UpdateDescription;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETD;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETDByDate;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public partial class TransportScheduleRepository : BaseRepository<TransportSchedule>, ITransportScheduleRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public TransportScheduleRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
        }


        public async Task<List<SearchTransportScheduleResponse>> Search(SearchTransportScheduleRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<SearchTransportScheduleResponse>();
            return returnData;
        }



        #region GetDateDriveTransportSchedule
        public async Task<GetDateDriveTransportScheduleResponse> GetDateDriveTransportSchedule(GetDateDriveTransportScheduleRequest request, CancellationToken cancellationToken) 
        {
            var driveMode = await  Context.TransportMode.AsNoTracking().Where(x => x.Code == "Drive").FirstOrDefaultAsync();


            if (driveMode != null)
            {

                var result = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x=> x.EventDate.Date == request.EventDate.Date)
                                    join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id 
                                    where (activetransport.TransportModeId == driveMode.Id && activetransport.Direction == request.Direction)
                                    
                                select new GetDateDriveTransportScheduleResponse
                                {
                                    Id = schedule.Id,
                                    Description = schedule.Description,
                                    EventDate = schedule.EventDate.Date,
                                    EventDateTime = schedule.EventDate,
                                    Special = activetransport.Special,
                                    Seats = schedule.Seats,
                                    Direction = activetransport.Direction,
                                    Code = activetransport.Code,
                                    ETD = schedule.ETD,
                                    ETA = schedule.ETA,
                                    FromLocationId = activetransport.fromLocationId,
                                    ToLocationId = activetransport.toLocationId

                                }).ToListAsync();
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        DateTime time = DateTime.ParseExact(item.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int hours = time.Hour;
                        int minutes = time.Minute;


                        item.EventDateTime = request.EventDate.Date.AddHours(hours).AddMinutes(minutes);

                    }

                    if (request.Morning)
                    {
                        var returnData =   result.Where(x => x.EventDateTime.Value.Hour <= 13).FirstOrDefault();
                        if (returnData != null)
                        {
                            return returnData;
                        }
                        else {
                            throw new BadRequestException("No Drive type schedule has been created for the morning.");
                        }
                    }
                    else {
                        var returnData = result.Where(x => x.EventDateTime.Value.Hour > 13).FirstOrDefault();
                        if (returnData != null)
                        {
                            return returnData;
                        }
                        else
                        {
                            throw new BadRequestException("No Drive type schedule has been created for the evening.");
                        }
                    }


                }
                else {
                    throw new BadRequestException("No schedule has been created for the Drive type.");
                }
            }
            else {
                throw new BadRequestException("Please register drive mode");
            }


        }

        #endregion


        #region ManageSchedule



        public async Task<ManageTransportScheduleResponse> ManageTransportSchedule(ManageTransportScheduleRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 100 : request.pageSize;
            int pageIndex = request.pageIndex;



            IQueryable<TransportSchedule> scheduleFilter = Context.TransportSchedule;
            if (request.model.startDate.HasValue && request.model.endDate.HasValue)
            {
                if (request.model.startDate == request.model.endDate)
                {
                    scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date == request.model.startDate.Value.Date);
                }
                else {
                    scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date >= request.model.startDate.Value.Date && x.EventDate.Date <= request.model.endDate.Value.Date);
                }


            }
            if (request.model.startDate.HasValue && !request.model.endDate.HasValue)
            {
                scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date == request.model.startDate.Value.Date);
            }

            if (!request.model.startDate.HasValue && request.model.endDate.HasValue)
            {
                scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date == request.model.endDate.Value.Date);
            }



            if (request.model.arriveLocationId.HasValue && !request.model.departLocationId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.toLocationId == request.model.arriveLocationId && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }
            if (!request.model.arriveLocationId.HasValue && request.model.departLocationId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.fromLocationId == request.model.departLocationId && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }
            if (request.model.arriveLocationId.HasValue && request.model.departLocationId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.fromLocationId == request.model.departLocationId && x.toLocationId == request.model.arriveLocationId && x.Active == 1).Select(x => x.Id).ToListAsync();

                var activess = await Context.ActiveTransport.AsNoTracking().Where(x => activeTransportIds.Contains(x.Id)).ToListAsync();
                //    if (activeTransportIds.Count > 0)
                ///  {
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //     }

            }
            if (request.model.transportModeId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.TransportModeId == request.model.transportModeId && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }
            if (!string.IsNullOrEmpty(request.model.Code))
            {

                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.Code.Contains(request.model.Code.Trim()) && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }







            int totalCount = await scheduleFilter.CountAsync();


            var schedulefilterQuery = scheduleFilter.OrderBy(x => x.EventDate)
                    .Skip((pageIndex) * pageSize)
                    .Take(pageSize);

            var result = await (from schedule in Context.TransportSchedule.AsNoTracking()
                                where schedulefilterQuery.AsNoTracking().Contains(schedule)
                                join transport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals transport.Id into transportData
                                from transport in transportData.DefaultIfEmpty()
                                join mode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals mode.Id into transportModeData
                                from mode in transportModeData.DefaultIfEmpty()
                                select new {
                                    Id = schedule.Id,
                                    Description = schedule.Description,
                                    EventDate = schedule.EventDate,
                                    ETD = schedule.ETD,
                                    ETA = schedule.ETA,
                                    Special = transport.Special,
                                    Seats = schedule.Seats,
                                    Direction = transport.Direction,
                                    Code = transport.Code,
                                    Mode = mode.Code,
                                    FromLocationid = transport.fromLocationId,
                                    ToLocationId = transport.toLocationId

                                }).ToListAsync();

            var retData = new List<ManageTransportScheduleResult>();
            foreach (var item in result)
            {
                var itemTransportConfirmedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id).CountAsync();
               // var itemTransportOverbookedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id && x.Status == "Over Booked").CountAsync();


                string ETD = item.ETD.Replace(":", "");
                string ETA = item.ETA.Replace(":", "");

                string timePattern = @"^(?:[01]\d|2[0-3])[0-5]\d$";


                bool isValidTimeETD = Regex.IsMatch(ETD, timePattern);



                bool isValidTimeETA = Regex.IsMatch(ETA, timePattern);





                DateTime schedulETD = DateTime.ParseExact(isValidTimeETD == true ? ETD : "0000", "HHmm", CultureInfo.InvariantCulture);
                int ETDhours = schedulETD.Hour;
                int ETDminutes = schedulETD.Minute;


                DateTime schedulETA = DateTime.ParseExact(isValidTimeETA == true ? ETA : "0000", "HHmm", CultureInfo.InvariantCulture);
                int ETAhours = schedulETA.Hour;
                int ETAminutes = schedulETA.Minute;
                var newRecord = new ManageTransportScheduleResult
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    EventDate = DateOnly.FromDateTime(item.EventDate.Date),
                 //   OvertBooked = itemTransportOverbookedCount,
                    Confirmed = itemTransportConfirmedCount,
                    Direction = item.Direction,
                    Special = item.Special,
                    Seats = item?.Seats,
                    EventDateETA = item.EventDate.Date.AddHours(ETAhours).AddMinutes(ETAminutes),
                    EventDateETD = item.EventDate.Date.AddHours(ETDhours).AddMinutes(ETDminutes),
                    TransportMode = item.Mode,
                    FromLocationId = item.FromLocationid,
                    ToLocationId = item.ToLocationId
                };

                retData.Add(newRecord);


            }

            var returnData = new ManageTransportScheduleResponse
            {
                data = retData.OrderBy(x => x.EventDate).OrderBy(x => x.EventDateETD)
         .ToList<ManageTransportScheduleResult>(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = totalCount
            };

            return returnData;

        }
        #endregion



        #region UpdateSchedule


        public async Task ChangeSchedule(UpdateTransportScheduleRequest request, CancellationToken cancellationToken)
        {

            var currentSchedule = await Context.TransportSchedule.FirstOrDefaultAsync(x => x.Id == request.Id);
            bool changeSeat = false;
            if (currentSchedule != null)
            {
                if (currentSchedule.Active == 1)
                {

                    if (currentSchedule.Seats != request.Seats)
                    {
                        changeSeat = true;
                    }


                    var currentActiveTransport = await Context.ActiveTransport.Where(x => x.Id == currentSchedule.ActiveTransportId).FirstOrDefaultAsync();



                    if (currentActiveTransport.CarrierId == request.CarrierId && currentActiveTransport.Code == request.TransportCode && request.TransportModeId == request.TransportModeId)
                    {



                        var currentTransportInfo = await (from transport in Context.ActiveTransport.Where(x => x.Id == currentSchedule.ActiveTransportId)
                                                          join fromlocation in Context.Location on transport.fromLocationId equals fromlocation.Id into fromlocationData
                                                          from fromlocation in fromlocationData.DefaultIfEmpty()
                                                          join tolocation in Context.Location on transport.toLocationId equals tolocation.Id into tolocationData
                                                          from tolocation in tolocationData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              Description = $"{request.ETD} {fromlocation.Code} {request.ETA} {tolocation.Code}"
                                                          }).FirstOrDefaultAsync();


                        currentSchedule.Seats = request.Seats;
                        currentSchedule.ETA = request.ETA;
                        currentSchedule.ETD = request.ETD;
                        currentSchedule.Description = currentTransportInfo?.Description;
                        currentSchedule.DateUpdated = DateTime.Now;
                        currentSchedule.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        Context.TransportSchedule.Update(currentSchedule);

                        var EmployeeTransports = await Context.Transport
                            .Where(x => x.ScheduleId == currentSchedule.Id).OrderBy(x => x.DateCreated).ToListAsync();

                        string hourString = request.ETD.Substring(0, 2);
                        string minuteString = request.ETD.Substring(2, 2);

                        int hour = int.Parse(hourString);
                        int minute = int.Parse(minuteString);
                        int currentSeatIndex = 1;
                        foreach (var item in EmployeeTransports)
                        {
                            item.EventDateTime =    item.EventDate.Value.Date.AddHours(hour).AddMinutes(minute);
                            item.DateUpdated = DateTime.Now;
                            item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                            if (changeSeat)
                            {
                                if (request.Seats >= currentSeatIndex)
                                {
                                    item.Status = "Confirmed";
                                }
                                else
                                {
                                    item.Status = "Over Booked";
                                }
                                currentSeatIndex++;
                            }
                            Context.Transport.Update(item);
                        }

                    }
                    else {



                        ActiveTransport newTransport = new ActiveTransport
                        {
                            Active = 1,
                            CarrierId = request.CarrierId,
                            Code = request.TransportCode,
                            DateCreated = DateTime.Now,
                            DayNum = currentSchedule.EventDate.ToString("dddd"),
                            Direction = currentActiveTransport.Direction,
                            FrequencyWeeks = currentActiveTransport.FrequencyWeeks,
                            fromLocationId = currentActiveTransport.fromLocationId,
                            toLocationId = currentActiveTransport.toLocationId,
                            Seats = request.Seats,
                            Special = currentActiveTransport.Special,
                            TransportAudit = currentActiveTransport.TransportAudit,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            TransportModeId = request.TransportModeId,
                            CostCodeId = currentActiveTransport.CostCodeId,
                            AircraftCode = currentActiveTransport.AircraftCode,
                            ETA = currentActiveTransport.ETA,
                            ETD = currentActiveTransport.ETA,

                        };

                        Context.ActiveTransport.Add(newTransport);
                        var status = await Context.SaveChangesAsync();

                        newTransport.Id = newTransport.Id;



                        int newTransportId = newTransport.Id;

                        var currentTransportInfo = await (from transport in Context.ActiveTransport.Where(x => x.Id == newTransportId)
                                                          join fromlocation in Context.Location on transport.fromLocationId equals fromlocation.Id into fromlocationData
                                                          from fromlocation in fromlocationData.DefaultIfEmpty()
                                                          join tolocation in Context.Location on transport.toLocationId equals tolocation.Id into tolocationData
                                                          from tolocation in tolocationData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              Description = $"{request.ETD} {fromlocation.Code} {request.ETA} {tolocation.Code}"
                                                          }).FirstOrDefaultAsync();


                        newTransport.Description = $"{currentTransportInfo.Description}";

                        currentSchedule.Code = request.TransportCode;
                        currentSchedule.Seats = request.Seats;
                        currentSchedule.ETA = request.ETA;
                        currentSchedule.ActiveTransportId = newTransportId;
                        currentSchedule.ETD = request.ETD;
                        currentSchedule.Description = currentTransportInfo.Description;
                        currentSchedule.DateUpdated = DateTime.Now;
                        currentSchedule.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        Context.TransportSchedule.Update(currentSchedule);
                        Context.ActiveTransport.Update(newTransport);

                        var EmployeeTransports = await Context.Transport
                            .Where(x => x.ScheduleId == currentSchedule.Id).OrderBy(x => x.DateCreated).ToListAsync();

                        string hourString = request.ETD.Substring(0, 2);
                        string minuteString = request.ETD.Substring(2, 2);

                        int hour = int.Parse(hourString);
                        int minute = int.Parse(minuteString);
                        int currentSeatIndex = 1;
                        foreach (var item in EmployeeTransports)
                        {
                            item.EventDateTime = item.EventDate.Value.Date.AddHours(hour).AddMinutes(minute);
                            item.DateUpdated = DateTime.Now;
                            item.ActiveTransportId = newTransportId;

                            item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        
                            if (changeSeat)
                            {
                                if (request.Seats >= currentSeatIndex)
                                {
                                    item.Status = "Confirmed";
                                }
                                else
                                {
                                    item.Status = "Over Booked";
                                }
                                currentSeatIndex++;
                            }

                            Context.Transport.Update(item);
                        }
                    }




                }
                else {
                    throw new BadRequestException("This Schedule is inactive and cannot be modified");
                }
            }
            else {
                throw new BadRequestException("No record was found for this schedule");
            }
        }


        private async Task<int> CreateNewActiveTransport(ActiveTransport activeTransport)
        {
            Context.ActiveTransport.Add(activeTransport);
            await Context.SaveChangesAsync();

            return activeTransport.Id;
        }

        #endregion

        #region ScheduleInfo

        public async Task<TransportScheduleInfoResponse> GetTransportScheduleInfo(TransportScheduleInfoRequest request, CancellationToken cancellationToken)
        {
            var returnData = await (from schedule in Context.TransportSchedule.Where(x => x.Id == request.Id)
                                    join activetransport in Context.ActiveTransport on schedule.ActiveTransportId equals activetransport.Id into acivetransportData
                                    from activetransport in acivetransportData.DefaultIfEmpty()
                                    select new TransportScheduleInfoResponse
                                    {
                                        Id = schedule.Id,
                                        CarrierId = activetransport.CarrierId,
                                        ETA = schedule.ETA,
                                        ETD = schedule.ETD,
                                        EventDate = schedule.EventDate,
                                        Seats = schedule.Seats,
                                        TransportCode = activetransport.Code,
                                        TransportModeId = activetransport.TransportModeId
                                    }).FirstOrDefaultAsync();

            return returnData;
        }


        #endregion


        //Request module use

        #region Month transport Schedule

        public async Task<List<GetMonthTransportScheduleResponse>> GetMonthTransportSchedule(GetMonthTransportScheduleRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<GetMonthTransportScheduleResponse>();

            var currentDate = DateTime.Today.Date;

            var driveTransporData = await Context.TransportMode.AsNoTracking().Where(x => x.Code.ToLower() == "drive").Select(x => new { x.Id }).FirstOrDefaultAsync();
            IQueryable<ActiveTransport> ActiveTransportQuery = Context.ActiveTransport;
            if (driveTransporData != null)
            {
                ActiveTransportQuery = ActiveTransportQuery.AsNoTracking().Where(x => x.TransportModeId != driveTransporData.Id);
            }
            if (request.FromLocationId.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery.AsNoTracking().Where(t => t.fromLocationId == request.FromLocationId.Value);
            }
            if (request.ToLocationId.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery.AsNoTracking().Where(t => t.toLocationId == request.ToLocationId.Value);
            }
            if (!string.IsNullOrWhiteSpace(request.TransportCode))
            {
                ActiveTransportQuery = ActiveTransportQuery.AsNoTracking().Where(t => t.Code.Contains(request.TransportCode.TrimEnd()));
            }
            if (request.TransportModeId.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery.AsNoTracking().Where(t => t.TransportModeId == request.TransportModeId);
            }


            var activeTransportIds = await ActiveTransportQuery.Select(x => x.Id).ToListAsync();
            IQueryable<TransportSchedule> transportScheduleQuery = Context.TransportSchedule;
            if (request.ScheduleDate.HasValue)
            {
                transportScheduleQuery = transportScheduleQuery.AsNoTracking().Where(x => x.EventDate.Date == request.ScheduleDate.Value.Date);
            }
            if (!request.ScheduleDate.HasValue)
            {
                DateTime startDate = DateTime.Today.Date;
                DateTime endDate = DateTime.Today.AddDays(-1).AddMonths(1);
                transportScheduleQuery = transportScheduleQuery.Where(ts => ts.EventDate.Date >= startDate && ts.EventDate <= endDate);
            }


            //    if (activeTransportIds.Count > 0)
            //   {
            transportScheduleQuery = transportScheduleQuery.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
            //    }
            var transportSchedules = await transportScheduleQuery.ToListAsync();

            foreach (var item in transportSchedules)
            {

                var itemTransportConfirmedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id).CountAsync();
               //var itemTransportOverbookedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id && x.Status == "Over Booked").CountAsync();

                var itemTransport = await Context.ActiveTransport.AsNoTracking()
                    .Where(x => x.Id == item.ActiveTransportId)
                    .Select(x => new { x.Seats, x.Direction }).FirstOrDefaultAsync();

                DateTime schedulETD = DateTime.ParseExact(item?.ETD, "HHmm", CultureInfo.InvariantCulture);
                int ETDhours = schedulETD.Hour;
                int ETDminutes = schedulETD.Minute;


                DateTime schedulETA = DateTime.ParseExact(item?.ETA, "HHmm", CultureInfo.InvariantCulture);
                int ETAhours = schedulETA.Hour;
                int ETAminutes = schedulETA.Minute;
                var newRecord = new GetMonthTransportScheduleResponse
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    EventDate = item.EventDate.Date,
                    Confirmed = itemTransportConfirmedCount,
                    Direction = itemTransport?.Direction,
                    Seats = item?.Seats,

                    EventDateETA = item.EventDate.Date.AddHours(ETAhours).AddMinutes(ETAminutes),
                    EventDateETD = item.EventDate.Date.AddHours(ETDhours).AddMinutes(ETDminutes),
                };

                returnData.Add(newRecord);


            }


            return returnData;

        }

        #endregion

        #region Create Drive Schedule
        public async Task CreateScheduleDrive(CreateScheduleDriveTransportRequest request)
        {
            string pattern = @"^([01]\d|2[0-3])[0-5]\d$";

            string p_direction_first = "";
            string p_direction_second = "";
            bool doubleScheduleGenerate = false;

            var currentfromLocation = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.fromLocationId);
            var currenttoLocationCode = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.toLocationId);
            var currentfromSiteStatus = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.fromLocationId);
            var currentTosSiteStatus = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.toLocationId);

            string fromLocationCode = currentfromLocation?.Code ?? string.Empty;
            string toLocationCode = currenttoLocationCode?.Code ?? string.Empty;
            bool fromSiteStatus = currentfromSiteStatus?.onSite == 1;
            bool TosSiteStatus = currentTosSiteStatus?.onSite == 1;
            var driveTransportDriveMode = await Context.TransportMode.Where(x => x.Code == "Drive").FirstOrDefaultAsync();
            var currentCarrier = await Context.Carrier.FirstOrDefaultAsync();

            string scheduleGeneration = ""; /*= CheckDriveScheduleTime(request);*/

            if (driveTransportDriveMode != null)
            {



                //if (!fromSiteStatus && TosSiteStatus /* && scheduleGeneration == "INANDOUT"*/)
                //{
                //    doubleScheduleGenerate = true;
                //    p_direction_first = "IN";
                //    p_direction_second = "OUT";
                //}
                if (!fromSiteStatus && TosSiteStatus /* && scheduleGeneration == "ONLYIN"*/)
                {
                    p_direction_first = "IN";
                    scheduleGeneration = "ONLYIN";
                }
                if (fromSiteStatus && !TosSiteStatus /*&& scheduleGeneration == "ONLYOUT"*/)
                {
                    p_direction_first = "OUT";
                    doubleScheduleGenerate = false;
                    scheduleGeneration = "ONLYOUT";
                }
                if (fromSiteStatus && TosSiteStatus)
                {
                    throw new BadRequestException("Schedules cannot be created with this location");
                }
                if (!fromSiteStatus && !TosSiteStatus)
                {
                    throw new BadRequestException("Schedules cannot be created with this location");
                }
                //if (!fromSiteStatus && TosSiteStatus /* && scheduleGeneration == "ONLYOUT"*/)
                //{
                //    throw new BadRequestException("Schedules cannot be created with this location");
                //}
                //if (fromSiteStatus && !TosSiteStatus/* && scheduleGeneration == "ONLYIN"*/)
                //{
                //    throw new BadRequestException("Schedules cannot be created with this location");
                //}
                //if (fromSiteStatus && !TosSiteStatus /* && scheduleGeneration == "INANDOUT"*/)
                //{
                //    throw new BadRequestException("Schedules cannot be created with this location");
                //}


                var aajjj = p_direction_first;
                var p_doublegenerate = doubleScheduleGenerate;



                foreach (string day in request.dayNums)
                {
                    if (Enum.TryParse(day, out DayOfWeekNumber dayOfWeek))
                    {

                        if (scheduleGeneration == "ONLYIN")
                        {
                            ActiveTransport activeTransport = new ActiveTransport
                            {
                                Active = 1,
                                Code = request.Code,
                                CarrierId = currentCarrier.Id,
                                FrequencyWeeks = request.FrequencyWeeks,
                                fromLocationId = request.fromLocationId,
                                toLocationId = request.toLocationId,
                                Seats = 0,
                                Direction = p_direction_first,
                                TransportModeId = driveTransportDriveMode.Id,
                                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                DateCreated = DateTime.Now,
                                Special = 0,
                                TransportAudit = 0,
                                AircraftCode = request.AircraftCode,
                                ETA = request.ETD,
                                ETD = request.ETD,
                                DayNum = day,
                                Description = string.Format("{0} {1} {2} {3}", request.ETD, fromLocationCode, request.ETD, toLocationCode),
                            };

                            Context.ActiveTransport.Add(activeTransport);
                            await Context.SaveChangesAsync();
                            activeTransport.Id = activeTransport.Id;
                            string[] localdays = new string[1];
                            localdays[0] = day;
                            int aa = activeTransport.Id;

                            await CreateScheduleDrive(
                                 request.ETD, request.ETD,
                                 request.Code, aa, request.StartDate,
                                 request.EndDate, request.FrequencyWeeks,
                                 localdays, fromLocationCode, toLocationCode);
                        }
                        else if (scheduleGeneration == "ONLYOUT")
                        {
                            ActiveTransport activeTransport = new ActiveTransport
                            {
                                Active = 1,
                                Code = request.Code,
                                CarrierId = currentCarrier.Id,
                                FrequencyWeeks = request.FrequencyWeeks,
                                fromLocationId = request.fromLocationId,
                                toLocationId = request.toLocationId,
                                Seats = 0,
                                Direction = p_direction_first,
                                TransportModeId = driveTransportDriveMode.Id,
                                Special = 0,
                                TransportAudit = 0,
                                DayNum = day,
                                AircraftCode = request.AircraftCode,
                                Description = string.Format("{0} {1} {2} {3}", request.outETD, fromLocationCode, request.outETD, toLocationCode),
                                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                DateCreated = DateTime.Now,
                                ETA = request.ETD,
                                ETD = request.ETD
                               
                            };

                            Context.ActiveTransport.Add(activeTransport);
                            await Context.SaveChangesAsync();
                            activeTransport.Id = activeTransport.Id;
                            string[] localdays = new string[1];
                            localdays[0] = day;
                            int aa = activeTransport.Id;

                            await CreateScheduleDrive(
                                 request.outETD, request.outETD,
                                 request.Code, aa, request.StartDate,
                                 request.EndDate, request.FrequencyWeeks,
                                 localdays, fromLocationCode, toLocationCode);
                        }




                    }
                }
                return;
                //  }

            }
            else
            {
                throw new BadRequestException("Virtual Drive Mode Not Registered");
            }

        }




        private string CheckDriveScheduleTime(CreateScheduleDriveTransportRequest request)
        {
            string pattern = @"^([01]\d|2[0-3])[0-5]\d$";
            bool etdStatus = Regex.IsMatch(request.ETD == null ? "" : request.ETD, pattern);
            bool outEtdStatus = Regex.IsMatch(request.outETD == null ? "" : request.outETD, pattern);

            if (etdStatus && !outEtdStatus)
            {
                return "ONLYIN";
            }
            else if (outEtdStatus && !etdStatus)
            {
                return "ONLYOUT";
            }
            else if (outEtdStatus && etdStatus)
            {
                return "INANDOUT";
            }
            else
            {
                throw new BadRequestException("Schedule time is invalid. Please send the correct time");
            }

        }


        private async Task CreateScheduleDrive(
         string etd, string eta, string code,
        int activeTransportId, DateTime startDate, DateTime endDate, int frequencyWeeks,
        string[] dayNums, string fromLocationCode, string toLocationCode)
        {
            List<DayOfWeekNumber> daysOfWeek = new List<DayOfWeekNumber>();
            foreach (string day in dayNums)
            {
                if (Enum.TryParse(day, out DayOfWeekNumber dayOfWeek))
                {
                    daysOfWeek.Add(dayOfWeek);
                }
            }

            DateTime currentDate = startDate;
            int aa = 0;

            while (currentDate <= endDate)
            {
                if (daysOfWeek.Contains((DayOfWeekNumber)currentDate.DayOfWeek))
                {

                    TransportSchedule schedule = new TransportSchedule
                    {
                        Code = code,
                        Description = string.Format("{0} {1} {2} {3}", etd, fromLocationCode, eta, toLocationCode),
                        EventDate = currentDate,
                        Seats = 0,
                        ETD = etd,
                        ETA = eta,
                        ActiveTransportId = activeTransportId,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id


                    };
                    Context.TransportSchedule.Add(schedule);
                    aa++;
                    //  context.SaveChangesAsync();
                }

                currentDate = currentDate.AddDays(1);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    currentDate = currentDate.AddDays(7 * (frequencyWeeks - 1));
                }
            }
            await Context.SaveChangesAsync();

        }
        #endregion

        #region Create Main Schedule

        public async Task CreateSchedule(CreateScheduleActiveTransportRequest request)
        {
            string p_direction_first = "";
            string p_direction_second = "";
            bool doubleScheduleGenerate = false;

            var currfromLocationCode = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.fromLocationId);
            var currtoLocationCode = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.toLocationId);
            var currfromSiteStatus = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.fromLocationId);
            var currTosSiteStatus = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.toLocationId);


            string fromLocationCode = currfromLocationCode?.Code ?? string.Empty;
            string toLocationCode = currtoLocationCode?.Code ?? string.Empty;
            bool fromSiteStatus = currfromSiteStatus?.onSite == 1;
            bool TosSiteStatus = currTosSiteStatus?.onSite == 1;

            string scheduleGeneration = CheckScheduleTime(request);

            if (!fromSiteStatus && TosSiteStatus && scheduleGeneration == "INANDOUT")
            {
                doubleScheduleGenerate = true;
                p_direction_first = "IN";
                p_direction_second = "OUT";
            }
            if (!fromSiteStatus && TosSiteStatus && scheduleGeneration == "ONLYIN")
            {
                p_direction_first = "IN";
            }
            if (fromSiteStatus && !TosSiteStatus && scheduleGeneration == "ONLYOUT")
            {
                p_direction_first = "OUT";
                doubleScheduleGenerate = false;
            }
            if (!fromSiteStatus && !TosSiteStatus && (scheduleGeneration == "ONLYIN" || scheduleGeneration == "INANDOUT"))
            {
                p_direction_first = "EXTERNAL";
            }
            if (fromSiteStatus && TosSiteStatus)
            {
                throw new BadRequestException("Schedules cannot be created with this location");
            }
            if (!fromSiteStatus && !TosSiteStatus && (scheduleGeneration == "ONLYOUT"))
            {
                throw new BadRequestException("Schedules cannot be created with this location");
            }
            if (!fromSiteStatus && TosSiteStatus && scheduleGeneration == "ONLYOUT")
            {
                throw new BadRequestException("Schedules cannot be created with this location");
            }
            if (fromSiteStatus && !TosSiteStatus && scheduleGeneration == "ONLYIN")
            {
                throw new BadRequestException("Schedules cannot be created with this location");
            }
            if (fromSiteStatus && !TosSiteStatus && scheduleGeneration == "INANDOUT")
            {
                throw new BadRequestException("Schedules cannot be created with this location");
            }





            //if (!fromSiteStatus && TosSiteStatus)
            //{
            //    p_direction_first = "IN";
            //    p_direction_second = "OUT";
            //    doubleScheduleGenerate = true;

            //}
            //if (fromSiteStatus && !TosSiteStatus)
            //{
            //    p_direction_first = "OUT";
            //    doubleScheduleGenerate = false;
            //}
            //if (!fromSiteStatus && !TosSiteStatus)
            //{
            //    p_direction_first = "EXTERNAL";
            //    doubleScheduleGenerate = false;
            //}
            //if (fromSiteStatus && TosSiteStatus)
            //{
            //    throw new BadRequestException("Schedules cannot be created with this location");
            //    //Task.FromException(exception);
            //}


            if (doubleScheduleGenerate)
            {

                var p_directions = new string[2];
                p_directions[0] = p_direction_first;
                p_directions[1] = p_direction_second;

                foreach (string v_direction in p_directions)
                {
                    foreach (string day in request.dayNums)
                    {
                        if (Enum.TryParse(day, out DayOfWeekNumber dayOfWeek))
                        {
                            var vv_fromlocationId = request.fromLocationId;
                            var vv_tolocationId = request.toLocationId;

                            if (scheduleGeneration == "INANDOUT")
                            {
                                if (v_direction == "IN")
                                {
                                    vv_fromlocationId = request.fromLocationId;
                                    vv_tolocationId = request.toLocationId;
                                }
                                if (v_direction == "OUT")
                                {
                                    vv_fromlocationId = request.toLocationId;
                                    vv_tolocationId = request.fromLocationId;
                                }
                            }

                            ActiveTransport activeTransport = new ActiveTransport
                            {
                                Active = 1,
                                Code = request.Code,
                                CarrierId = request.CarrierId,
                                FrequencyWeeks = request.FrequencyWeeks,
                                fromLocationId = vv_fromlocationId,
                                toLocationId = vv_tolocationId,
                                Seats = v_direction == "IN" ? request.inSeats : request.OutSeats,
                                Direction = v_direction,
                                TransportModeId = request.TransportModeId,
                                Special = 0,
                                ETA = v_direction == "IN" ? request.ETA : request.outETA,
                                ETD = v_direction == "IN" ? request.ETD : request.outETD,
                                DateCreated = DateTime.Now,
                                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                TransportAudit = 0,
                                DayNum = day,
                                CostCodeId = null,
                                AircraftCode = request.AircraftCode,
                                Description = string.Format("{0} {1} {2} {3}", v_direction == "IN" ? request.ETD : request.outETD, v_direction == "IN" ? fromLocationCode : toLocationCode, v_direction == "IN" ? request.ETA : request.outETA, v_direction == "IN" ? toLocationCode : fromLocationCode),
                            };

                            Context.ActiveTransport.Add(activeTransport);
                            await Context.SaveChangesAsync();
                            activeTransport.Id = activeTransport.Id;
                            string[] localdays = new string[1];
                            localdays[0] = day;
                            int aa = activeTransport.Id;

                            await CreateScheduleMain(
                                   v_direction == "IN" ? request.ETD : request.outETD,
                                   v_direction == "IN" ? request.ETA : request.outETA,
                                   request.Code, aa, request.StartDate,
                                   request.EndDate, v_direction == "IN" ? request.inSeats : request.OutSeats, request.FrequencyWeeks,
                                   localdays,
                                   v_direction == "IN" ? fromLocationCode : toLocationCode,
                                   v_direction == "IN" ? toLocationCode : fromLocationCode);


                        }
                    }
                }

                return;

            }
            else
            {
                foreach (string day in request.dayNums)
                {
                    if (Enum.TryParse(day, out DayOfWeekNumber dayOfWeek))
                    {
                        if (scheduleGeneration == "ONLYIN" || scheduleGeneration == "INANDOUT")
                        {

                            ActiveTransport activeTransport = new ActiveTransport
                            {
                                Active = 1,
                                Code = request.Code,
                                CarrierId = request.CarrierId,
                                FrequencyWeeks = request.FrequencyWeeks,
                                fromLocationId = request.fromLocationId,
                                toLocationId = request.toLocationId,
                                Seats = request.inSeats,
                                Direction = p_direction_first,
                                TransportModeId = request.TransportModeId,
                                Special = 0,
                                TransportAudit = 0,
                                ETA = request.ETA,
                                ETD = request.ETD,
                                DayNum = day,
                                Description = string.Format("{0} {1} {2} {3}", request.ETD, fromLocationCode, request.ETA, toLocationCode),
                                AircraftCode = request.AircraftCode,
                            };

                            Context.ActiveTransport.Add(activeTransport);
                            await Context.SaveChangesAsync();
                            activeTransport.Id = activeTransport.Id;
                            string[] localdays = new string[1];
                            localdays[0] = day;
                            int aa = activeTransport.Id;

                            await CreateScheduleMain(
                                 request.ETD, request.ETA,
                                 request.Code, aa, request.StartDate,
                                 request.EndDate, request.inSeats, request.FrequencyWeeks,
                                 localdays, fromLocationCode, toLocationCode);

                        }
                        if (scheduleGeneration == "ONLYOUT")
                        {

                            ActiveTransport activeTransport = new ActiveTransport
                            {
                                Active = 1,
                                Code = request.Code,
                                CarrierId = request.CarrierId,
                                FrequencyWeeks = request.FrequencyWeeks,
                                fromLocationId = request.fromLocationId,
                                toLocationId = request.toLocationId,
                                Seats = request.inSeats,
                                Direction = p_direction_first,
                                TransportModeId = request.TransportModeId,
                                ETA = request.outETA,
                                ETD = request.outETD,
                                Special = 0,
                                TransportAudit = 0,
                                DayNum = day,
                                Description = string.Format("{0} {1} {2} {3}", request.outETD, fromLocationCode, request.outETA, toLocationCode),
                                AircraftCode = request.AircraftCode
                            };

                            Context.ActiveTransport.Add(activeTransport);
                            await Context.SaveChangesAsync();
                            activeTransport.Id = activeTransport.Id;
                            string[] localdays = new string[1];
                            localdays[0] = day;
                            int aa = activeTransport.Id;

                            await CreateScheduleMain(
                                 request.outETD, request.outETA,
                                 request.Code, aa, request.StartDate,
                                 request.EndDate, request.OutSeats, request.FrequencyWeeks,
                                 localdays, fromLocationCode, toLocationCode);

                        }

                    }
                }
                return;
            }

        }

        private async Task CreateScheduleMain(
             string etd, string eta, string code,
            int activeTransportId, DateTime startDate, DateTime endDate, int seats, int frequencyWeeks,
            string[] dayNums, string fromLocationCode, string toLocationCode)
        {
            List<DayOfWeekNumber> daysOfWeek = new List<DayOfWeekNumber>();
            foreach (string day in dayNums)
            {
                if (Enum.TryParse(day, out DayOfWeekNumber dayOfWeek))
                {
                    daysOfWeek.Add(dayOfWeek);
                }
            }

            DateTime currentDate = startDate;
            int aa = 0;

            while (currentDate <= endDate)
            {
                if (daysOfWeek.Contains((DayOfWeekNumber)currentDate.DayOfWeek))
                {

                    TransportSchedule schedule = new TransportSchedule
                    {
                        Code = code,
                        Description = string.Format("{0} {1} {2} {3}", etd, fromLocationCode, eta, toLocationCode),
                        EventDate = currentDate,
                        Seats = seats,
                        ETD = etd,
                        ETA = eta,
                        ActiveTransportId = activeTransportId,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id


                    };
                    Context.TransportSchedule.Add(schedule);
                    aa++;
                    //  context.SaveChangesAsync();
                }

                currentDate = currentDate.AddDays(1);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    currentDate = currentDate.AddDays(7 * (frequencyWeeks - 1));
                }
            }
            await Context.SaveChangesAsync();

        }


        private string CheckScheduleTime(CreateScheduleActiveTransportRequest request)
        {
            string pattern = @"^([01]\d|2[0-3])[0-5]\d$";
            bool etdStatus = Regex.IsMatch(request.ETD == null ? "" : request.ETD, pattern);
            bool outEtdStatus = Regex.IsMatch(request.outETD == null ? "" : request.outETD, pattern);
            bool etaStatus = Regex.IsMatch(request.ETA == null ? "" : request.ETA, pattern);
            bool outEtaStatus = Regex.IsMatch(request.outETA == null ? "" : request.outETA, pattern);

            if ((etdStatus && etaStatus) && (!outEtdStatus || !outEtaStatus))
            {
                return "ONLYIN";
            }
            else if ((outEtdStatus && outEtaStatus) && (!etdStatus || !etaStatus))
            {
                return "ONLYOUT";
            }
            else if (outEtdStatus && outEtaStatus && etdStatus && etaStatus)
            {
                return "INANDOUT";
            }
            else

            {
                throw new BadRequestException("Schedule time is invalid. Please send the correct time");
            }

        }

        #endregion



        #region UpdateScheduleDescription

        public async Task UpdateScheduleDescription(UpdateDescriptionRequest request, CancellationToken cancellationToken)
        {
            var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentSchedule != null) {
                currentSchedule.Description = request.description;
                currentSchedule.DateUpdated = DateTime.Now;
                currentSchedule.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                Context.TransportSchedule.Update(currentSchedule);
            }
        }

        #endregion



        #region SeatInfo


        public async Task<SeatInfoTransportScheduleResponse> SeatInfoTransportSchedule(SeatInfoTransportScheduleRequest request, CancellationToken cancellationToken)
        {
            var currentSchedule = await Context.TransportSchedule
                      .AsNoTracking()
                      .Where(x => x.Id == request.ScheduleId)
                      .Select(x => new { x.Seats })
                      .FirstOrDefaultAsync(cancellationToken);
            var returnData = new SeatInfoTransportScheduleResponse();
            if (currentSchedule != null)
            {
                var bookedCount  = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == request.ScheduleId).CountAsync();
                returnData.Seats = currentSchedule.Seats;
                returnData.BookedCount = bookedCount;
                returnData.AvailableSeatCount = currentSchedule.Seats - bookedCount;
                return returnData;

            }
            else {
                return returnData;
            }

            
        }


        #endregion



        #region TransportScheduleExport

        public async Task<TransportScheduleExportResponse> TransportScheduleExport(TransportScheduleExportRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            IQueryable<TransportScheduleExportResponseData> query = from schedule in Context.TransportSchedule.AsNoTracking()
                                                                    where schedule.EventDate.Date >= request.StartDate.Date && schedule.EventDate.Date <= request.EndDate.Date
                                                                    join activetransport in Context.ActiveTransport.AsNoTracking().Where(x => x.Active == 1)
                                                                    on schedule.ActiveTransportId equals activetransport.Id into activetransportData
                                                                    from activetransport in activetransportData.DefaultIfEmpty()
                                                                    join fromLocation in Context.Location.AsNoTracking()
                                                                    on activetransport.fromLocationId equals fromLocation.Id into fromLocationData
                                                                    from fromlocation in fromLocationData.DefaultIfEmpty()
                                                                    join toLocation in Context.Location.AsNoTracking()
                                                                    on activetransport.toLocationId equals toLocation.Id into toLocationData
                                                                    from toLocation in toLocationData.DefaultIfEmpty()
                                                                    join carrier in Context.Carrier.DefaultIfEmpty()
                                                                    on activetransport.CarrierId equals carrier.Id into carrierData
                                                                    from carrier in carrierData.DefaultIfEmpty()


                                                                    select new TransportScheduleExportResponseData
                                                                    {
                                                                        //Date = $"{schedule.EventDate.ToString("yyyy-MM-dd")}",
                                                                        //Direction = $"{fromlocation.Code}-{toLocation.Code}",
                                                                        //TransportCode = activetransport.Code,
                                                                        //Description = schedule.Description,
                                                                        //ETD = schedule.ETD,
                                                                        //ETA = schedule.ETA,
                                                                        //Seat = schedule.Seats,
                                                                        //Carrier = carrier.Description,
                                                                        //ActiveTransportModeId = activetransport.TransportModeId,
                                                                        //DirectionData = activetransport.Direction,
                                                                        //EventDate = schedule.EventDate,
                                                                        //AvailableSeat = schedule.Seats - (transportData != null ? transportData.Count() : 0)
                                                                        Date = schedule.EventDate.ToString("yyyy-MM-dd"),
                                                                        Direction = (fromlocation != null && toLocation != null) ? $"{fromlocation.Code}-{toLocation.Code}" : "Unknown",
                                                                        TransportCode = activetransport != null ? activetransport.Code : "N/A",
                                                                        Description = schedule.Description,
                                                                        ETD = schedule.ETD,
                                                                        ETA = schedule.ETA,
                                                                        Seat = schedule.Seats,
                                                                        Carrier = carrier != null ? carrier.Description : "Unknown",
                                                                        ActiveTransportModeId = activetransport != null ? activetransport.TransportModeId : (int?)null,
                                                                        DirectionData = activetransport != null ? activetransport.Direction : "Unknown",
                                                                        EventDate = schedule.EventDate,
                                                                        ScheduleId = schedule.Id,

                                                                    };

            // Apply optional filtering
            if (request.TransportModeId.HasValue)
            {
                query = query.Where(x => x.ActiveTransportModeId == request.TransportModeId);
            }

            if (!string.IsNullOrWhiteSpace(request.Direction))
            {
                query = query.Where(x => x.DirectionData == request.Direction);
            }

            var data = await query.OrderBy(c => c.EventDate).ToListAsync(cancellationToken);


            if (data.Count > 0)
            {

                foreach (var dataItem in data)
                {
                    var scheduleTransportCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == dataItem.ScheduleId).CountAsync();
                    dataItem.AvailableSeat = dataItem.Seat - scheduleTransportCount;
                }


                return new TransportScheduleExportResponse
                {
                    ExcelFile = ExcelExport($"TransportSchedule-{request.StartDate.Date.ToString("yyyy-MM-dd")}--{request.EndDate.ToString("yyyy-MM-dd")}", data.ToList<dynamic>(), request)
                };
            }
            else
            {
                throw new BadRequestException($"TransportSchedule-{request.StartDate.Date.ToString("yyyy-MM-dd")}--{request.EndDate.ToString("yyyy-MM-dd")} data not found");
            }



        }


        private byte[] ExcelExport(string sheetName, List<dynamic> objectData, TransportScheduleExportRequest request)
        {
            using (var package = new ExcelPackage())
            {

                var data = ConvertToDictionaryList(objectData);
                var headerProps = ((IDictionary<string, object>)data[0]).Keys;
                var auditParams = new List<ExcelAuditParam>();



           //     auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Direction : ", FieldValueCaption = request.Direction ==  null ? "ALL" : request.Direction });
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Start date : ", FieldValueCaption = request.StartDate.ToString("yyyy-MM-dd") });
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "End date : ", FieldValueCaption = request.EndDate.ToString("yyyy-MM-dd") });

                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Executed date : ", FieldValueCaption = request.StartDate.ToString() });
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = data.Count.ToString("N") });


                var worksheet = package.Workbook.Worksheets.Add(sheetName);



                var titlecells = worksheet.Cells[1, 1];
                titlecells.Value = $"Transport Availbility";
                titlecells.Style.Font.Size = 12;

                int row = 2;
                foreach (var item in auditParams)
                {
                    row++;
                    var paramCaptionCells = worksheet.Cells[row, 1];
                    paramCaptionCells.Value = $"{item.FieldCaption}";
                    paramCaptionCells.Style.Font.Size = 12;

                    var paramValueCells = worksheet.Cells[row, 2];
                    paramValueCells.Value = item.FieldValueCaption;
                    paramValueCells.Style.Font.Size = 12;
                    paramValueCells.Style.Font.Bold = true;
                    paramValueCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    if (item.FieldName == "#MetaData")
                    {
                        paramValueCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2233e3"));
                    }
                    else
                    {
                        paramValueCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));

                    }
                    paramValueCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));


                }


                row = row + 2;

                int column = 1;
                worksheet.View.FreezePanes(row + 1, column);



                foreach (var header in headerProps)
                {
                        worksheet.Cells[row, column].Value = AddSpacesToSentence(header, true);
                        var headerCells = worksheet.Cells[row, column];
                        headerCells.Style.Font.Bold = true;
                        headerCells.Style.Font.Size = 13;
                        headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));

                        headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));

                        headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        headerCells.AutoFilter = true;
                        column++;
                }
                worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
                row++;
                foreach (var d in data)
                {
                    column = 1;
                    foreach (var prop in (IDictionary<string, object>)d)
                    {
                        worksheet.Cells[row, column].Value = prop.Value;
                        column++;


                    }
                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                package.Save();
                return package.GetAsByteArray();
            }
        }



        private static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    {
                        newText.Append(' ');
                    }
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }


        private List<IDictionary<string, object>> ConvertToDictionaryList(List<dynamic> dynamicList)
        {
            List<string> mainColumns = new List<string>
            {
                "Date",
                "Direction",
                "Carrier",
                "TransportCode",
                "Description",
                "ETD",
                "ETA",
                "Seat",
                "AvailableSeat"
            };

            var dictionaryList = new List<IDictionary<string, object>>();

            foreach (var item in dynamicList)
            {
                var dictionary = new Dictionary<string, object>();

                foreach (var column in mainColumns)
                {
                    // Ensure only the columns defined in 'mainColumns' are added to the dictionary
                    var property = item.GetType().GetProperty(column);
                    if (property != null)
                    {
                        dictionary[column] = property.GetValue(item, null);
                    }
                    else
                    {
                        // In case a property is missing, you can choose to handle it (optional)
                        dictionary[column] = null;
                    }
                }

                dictionaryList.Add(dictionary);
            }

            return dictionaryList;
        }





        #endregion


        #region UpdateRealETD

        public async Task UpdateTransportScheduleRealETD(UpdateTransportScheduleRealETDRequest request, CancellationToken cancellationToken)
        {
          var currentData =await Context.TransportSchedule.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {


                currentData.RealETD = request.RealETD;
                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                currentData.DateUpdated = DateTime.Now;
                currentData.Remark = request.Remark;
                Context.TransportSchedule.Update(currentData);



            }
            else {
                throw new BadRequestException("Schedule not found");
            }
        }



        #endregion


        #region UpdateRealETD

        public async Task UpdateTransportScheduleRealETDByDate(UpdateTransportScheduleRealETDByDateRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.TransportSchedule.Where(x => x.ActiveTransportId == request.ActiveTransportId && x.EventDate.Date == request.ScheduleDate.Date).FirstOrDefaultAsync();
            if (currentData != null)
            {


                currentData.RealETD = request.RealETD;
                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                currentData.DateUpdated = DateTime.Now;
                currentData.Remark = request.Remark;
                Context.TransportSchedule.Update(currentData);



            }
            else
            {
                throw new BadRequestException("Schedule not found");
            }
        }



        #endregion



        #region Busstop


        public async Task<BusstopTransportScheduleResponse> BusstopTransportSchedule(BusstopTransportScheduleRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 100 : request.pageSize;
            int pageIndex = request.pageIndex;



            IQueryable<TransportSchedule> scheduleFilter = Context.TransportSchedule;
            if (request.model.startDate.HasValue && request.model.endDate.HasValue)
            {
                if (request.model.startDate == request.model.endDate)
                {
                    scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date == request.model.startDate.Value.Date);
                }
                else
                {
                    scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date >= request.model.startDate.Value.Date && x.EventDate.Date <= request.model.endDate.Value.Date);
                }


            }
            if (request.model.startDate.HasValue && !request.model.endDate.HasValue)
            {
                scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date == request.model.startDate.Value.Date);
            }

            if (!request.model.startDate.HasValue && request.model.endDate.HasValue)
            {
                scheduleFilter = scheduleFilter.Where(x => x.EventDate.Date == request.model.endDate.Value.Date);
            }



            if (request.model.arriveLocationId.HasValue && !request.model.departLocationId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.toLocationId == request.model.arriveLocationId && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }
            if (!request.model.arriveLocationId.HasValue && request.model.departLocationId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.fromLocationId == request.model.departLocationId && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }
            if (request.model.arriveLocationId.HasValue && request.model.departLocationId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.fromLocationId == request.model.departLocationId && x.toLocationId == request.model.arriveLocationId && x.Active == 1).Select(x => x.Id).ToListAsync();

                var activess = await Context.ActiveTransport.AsNoTracking().Where(x => activeTransportIds.Contains(x.Id)).ToListAsync();
                //    if (activeTransportIds.Count > 0)
                ///  {
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //     }

            }
            if (request.model.transportModeId.HasValue)
            {
                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.TransportModeId == request.model.transportModeId && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }
            if (!string.IsNullOrEmpty(request.model.Code))
            {

                var activeTransportIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.Code.Contains(request.model.Code.Trim()) && x.Active == 1).Select(x => x.Id).ToListAsync();
                scheduleFilter = scheduleFilter.Where(x => activeTransportIds.Contains(x.ActiveTransportId));
                //if (activeTransportIds.Count > 0)
                //{

                //}

            }







            int totalCount = await scheduleFilter.CountAsync();


            var schedulefilterQuery = scheduleFilter.OrderBy(x => x.EventDate)
                    .Skip((pageIndex) * pageSize)
                    .Take(pageSize);

            var result = await (from schedule in Context.TransportSchedule.AsNoTracking()
                                where schedulefilterQuery.AsNoTracking().Contains(schedule)
                                join transport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals transport.Id into transportData
                                from transport in transportData.DefaultIfEmpty()
                                join mode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals mode.Id into transportModeData
                                from mode in transportModeData.DefaultIfEmpty()
                                select new
                                {
                                    Id = schedule.Id,
                                    Description = schedule.Description,
                                    EventDate = schedule.EventDate,
                                    ETD = schedule.ETD,
                                    ETA = schedule.ETA,
                                    Special = transport.Special,
                                    Seats = schedule.Seats,
                                    Direction = transport.Direction,
                                    Code = transport.Code,
                                    Mode = mode.Code,
                                    FromLocationid = transport.fromLocationId,
                                    ToLocationId = transport.toLocationId

                                }).ToListAsync();

            var retData = new List<BusstopTransportScheduleResult>();
            foreach (var item in result)
            {
                var itemTransportConfirmedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id).CountAsync();
                // var itemTransportOverbookedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id && x.Status == "Over Booked").CountAsync();


                string ETD = item.ETD.Replace(":", "");
                string ETA = item.ETA.Replace(":", "");

                string timePattern = @"^(?:[01]\d|2[0-3])[0-5]\d$";


                bool isValidTimeETD = Regex.IsMatch(ETD, timePattern);



                bool isValidTimeETA = Regex.IsMatch(ETA, timePattern);





                DateTime schedulETD = DateTime.ParseExact(isValidTimeETD == true ? ETD : "0000", "HHmm", CultureInfo.InvariantCulture);
                int ETDhours = schedulETD.Hour;
                int ETDminutes = schedulETD.Minute;


                DateTime schedulETA = DateTime.ParseExact(isValidTimeETA == true ? ETA : "0000", "HHmm", CultureInfo.InvariantCulture);
                int ETAhours = schedulETA.Hour;
                int ETAminutes = schedulETA.Minute;
                var newRecord = new BusstopTransportScheduleResult
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    EventDate = DateOnly.FromDateTime(item.EventDate.Date),
                    //   OvertBooked = itemTransportOverbookedCount,
                    Confirmed = itemTransportConfirmedCount,
                    Direction = item.Direction,
                    Special = item.Special,
                    Seats = item?.Seats,
                    EventDateETA = item.EventDate.Date.AddHours(ETAhours).AddMinutes(ETAminutes),
                    EventDateETD = item.EventDate.Date.AddHours(ETDhours).AddMinutes(ETDminutes),
                    TransportMode = item.Mode,
                    FromLocationId = item.FromLocationid,
                    ToLocationId = item.ToLocationId,
                    BusstopStatus = await ScheduleBustopStatus(item.Id)
                };

                retData.Add(newRecord);


            }

            var returnData = new BusstopTransportScheduleResponse
            {
                data = retData.OrderBy(x => x.EventDate).OrderBy(x => x.EventDateETD)
         .ToList<BusstopTransportScheduleResult>(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = totalCount
            };

            return returnData;
        }



        private async Task<bool> ScheduleBustopStatus(int scheduleId)
        {
            return await Context.TransportScheduleBusstop.AsNoTracking()
                .Where(x => x.ScheduleId == scheduleId).AnyAsync();
        }


        #endregion
    }
}
