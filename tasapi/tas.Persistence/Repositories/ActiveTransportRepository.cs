using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.CreateSpecialActiveTransport;
using tas.Application.Features.ActiveTransportFeature.DeleteActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetCalendarActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetDateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateAircraftCodeActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateDescrActiveTransport;
using tas.Application.Features.DashboardAccomAdminFeature.GetNonSiteInfo;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Application.Features.EmployeeFeature.SearchShortEmployee;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public partial class ActiveTransportRepository : BaseRepository<ActiveTransport>, IActiveTransportRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public ActiveTransportRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task<GetAllActiveTransportResponse> GetAllData(GetAllActiveTransportRequest request, CancellationToken cancellationToken)
        {

            var aa = request.model.TransportCode;
            var currentDate = DateTime.Today.AddDays(-1);
            IQueryable<ActiveTransport> ActiveTransportQuery  = Context.ActiveTransport;

            if (request.model.active.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery.Where(x => x.Active == request.model.active);
            }
            
            if (request.model.FromLocationId.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery.Where(t => t.fromLocationId == request.model.FromLocationId.Value);
            }
            if (request.model.ToLocationId.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery.Where(t => t.toLocationId == request.model.ToLocationId.Value);
            }
            if (!string.IsNullOrWhiteSpace(request.model.TransportCode))
            {

                var lowerCaseCode = request.model.TransportCode.ToLower();
                ActiveTransportQuery = ActiveTransportQuery.Where(t => t.Code.ToLower().Contains(lowerCaseCode));
            }
            if (!string.IsNullOrWhiteSpace(request.model.DayNum))
            {
                ActiveTransportQuery = ActiveTransportQuery.Where(t => t.DayNum == request.model.DayNum);
            }
            
            if (request.model.TransportModeId.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery.Where(t => t.TransportModeId == request.model.TransportModeId);
            }


            if (request.model.ScheduleDate.HasValue)
            {
                ActiveTransportQuery = ActiveTransportQuery
                         .Where(at => Context.TransportSchedule
                             .Any(ts => ts.ActiveTransportId == at.Id && ts.EventDate.Date == request.model.ScheduleDate.Value.Date));



            }


                var transportList = await ActiveTransportQuery
                .Join(Context.Carrier, t => t.CarrierId, r => r.Id, (t, r) => new { Transport = t, Carrier = r })
                .Join(Context.TransportMode, tr => tr.Transport.TransportModeId, m => m.Id, (tr, m) => new { Transport = tr.Transport, Carrier = tr.Carrier, TransportMode = m })
                .Join(Context.Location, t => t.Transport.fromLocationId, l => l.Id, (t, l) => new { Transport = t.Transport, Carrier = t.Carrier, TransportMode = t.TransportMode, FromLocation = l })
                .Join(Context.Location, t => t.Transport.toLocationId, l => l.Id, (t, l) => new { Transport = t.Transport, Carrier = t.Carrier, TransportMode = t.TransportMode, FromLocation = t.FromLocation, ToLocation = l })
                  .Select(t => new GetAllActiveTransportResult
                  {
                      Id = t.Transport.Id,
                      Code = t.Transport.Code,
                      DayNum = t.Transport.DayNum,
                      Direction = t.Transport.Direction,
                      Active = t.Transport.Active,
                      CarrierId = t.Carrier.Id,
                      CarrierName = t.Carrier.Description ?? string.Empty,
                      TransportModeId = t.TransportMode.Id,
                      TransportModeName = t.TransportMode.Code ?? string.Empty,
                      TransportAudit = t.Transport.TransportAudit,
                      Seats = t.Transport.Seats,
                      fromLocationId = t.FromLocation.Id,
                      fromLocationName = t.FromLocation.Description ?? string.Empty,
                      fromLocationCode = t.FromLocation.Code ?? string.Empty,
                      FrequencyWeeks = t.Transport.FrequencyWeeks,
                      Special = t.Transport.Special,
                      toLocationId = t.ToLocation.Id,
                      toLocationName = t.ToLocation.Description ?? string.Empty,
                      toLocationCode = t.ToLocation.Code ?? string.Empty,
                      DateCreated = t.Transport.DateCreated,
                      DateUpdated = t.Transport.DateUpdated,
                      CostCodeId = t.Transport.CostCodeId,
                      Description = t.Transport.Description,
                      ETA = t.Transport.ETA,
                      ETD = t.Transport.ETD,
                      AircraftCode = t.Transport.AircraftCode,
                  }).ToListAsync(cancellationToken);

            var retData = transportList.OrderBy(x=> x?.ETD).Skip(request.pageIndex * request.pageSize).Take(request.pageSize);

            foreach (var item in retData)
            {
                item.ScheduleStartDate =await transportScheduleDate(item.Id, true);
                item.ScheduleEndDate =await transportScheduleDate(item.Id, false);
            }


            var returnData = new GetAllActiveTransportResponse
                {
                    data = retData
                       .ToList<GetAllActiveTransportResult>(),
                    pageSize = request.pageSize == 0 ? 100 : request.pageSize,
                    currentPage = request.pageIndex,
                    totalcount = transportList.Count()
                };

                return returnData;



        }



        public async Task<GetAllActiveTransportResponse> GetAllData2(GetAllActiveTransportRequest request, CancellationToken cancellationToken)
        {


            var Ids = new List<int>();
            if (request.model.active == 1)
            {
                Ids =await activeTransposrtIds();
            }
            else
            {
                Ids =await inactiveTransposrtIds();
            }

            var transportList = await Context.ActiveTransport.Where(x => Ids.Distinct().Contains(x.Id))
                .Join(Context.Carrier, t => t.CarrierId, r => r.Id, (t, r) => new { Transport = t, Carrier = r })
                .Join(Context.TransportMode, tr => tr.Transport.TransportModeId, m => m.Id, (tr, m) => new { Transport = tr.Transport, Carrier = tr.Carrier, TransportMode = m })
                .Join(Context.Location, t => t.Transport.fromLocationId, l => l.Id, (t, l) => new { Transport = t.Transport, Carrier = t.Carrier, TransportMode = t.TransportMode, FromLocation = l })
                .Join(Context.Location, t => t.Transport.toLocationId, l => l.Id, (t, l) => new { Transport = t.Transport, Carrier = t.Carrier, TransportMode = t.TransportMode, FromLocation = t.FromLocation, ToLocation = l })
                  .Select(t => new GetAllActiveTransportResult
                  {
                      Id = t.Transport.Id,
                      Code = t.Transport.Code,
                      DayNum = t.Transport.DayNum,
                      Direction = t.Transport.Direction,
                      Active = t.Transport.Active,
                      CarrierId = t.Carrier.Id,
                      CarrierName = t.Carrier.Description ?? string.Empty,
                      TransportModeId = t.TransportMode.Id,
                      TransportModeName = t.TransportMode.Code ?? string.Empty,
                      TransportAudit = t.Transport.TransportAudit,
                      Seats = t.Transport.Seats,
                      fromLocationId = t.FromLocation.Id,
                      fromLocationName = t.FromLocation.Description ?? string.Empty,
                      fromLocationCode = t.FromLocation.Code ?? string.Empty,
                      FrequencyWeeks = t.Transport.FrequencyWeeks,
                      Special = t.Transport.Special,
                      toLocationId = t.ToLocation.Id,
                      toLocationName = t.ToLocation.Description ?? string.Empty,
                      toLocationCode = t.ToLocation.Code ?? string.Empty,
                      DateCreated = t.Transport.DateCreated,
                      DateUpdated = t.Transport.DateUpdated,
                      CostCodeId = t.Transport.CostCodeId,
                      Description = t.Transport.Description,
                      ETA = t.Transport.ETA,
                      ETD = t.Transport.ETD

                  })
        .ToListAsync(cancellationToken);

            foreach (var item in transportList)
            {
                item.ScheduleStartDate =await transportScheduleDate(item.Id, true);
                item.ScheduleEndDate =await transportScheduleDate(item.Id, false);
            }

            if (request.model.FromLocationId.HasValue)
            {
                transportList = transportList.Where(t => t.fromLocationId == request.model.FromLocationId.Value).ToList();
            }
            if (request.model.ToLocationId.HasValue)
            {
                transportList = transportList.Where(t => t.toLocationId == request.model.ToLocationId.Value).ToList();
            }
            //if (request.model.active.HasValue)
            //{
            //    transportList = transportList.Where(t => t.Active == request.model.active).ToList();
            //}
            if (!string.IsNullOrWhiteSpace(request.model.DayNum))
            {
                transportList = transportList.Where(t => t.DayNum == request.model.DayNum).ToList();
            }
            if (!string.IsNullOrWhiteSpace(request.model.TransportCode))
            {
                transportList = transportList.Where(t => t.Code.Contains(request.model.TransportCode.TrimEnd())).ToList();
            }
            if (request.model.ScheduleDate.HasValue)
            {
                int[] scheduleIDs = Context.TransportSchedule
                    .Where(t => t.EventDate.Date == request.model.ScheduleDate.Value.Date)
                    .Select(x => x.ActiveTransportId)
                    .Distinct()
                    .ToArray();
                transportList = transportList.Where(x => scheduleIDs.Contains(x.Id)).ToList();
            }

            var returnData = new GetAllActiveTransportResponse
            {
                data = transportList.OrderBy(x => x.Id)
                               .Skip(request.pageIndex * request.pageSize)
                               .Take(request.pageSize)
                               .ToList<GetAllActiveTransportResult>(),
                pageSize = request.pageSize == 0 ? 100 : request.pageSize,
                currentPage = request.pageIndex,
                totalcount = transportList.Count()
            };

            return returnData;


        }

        private async Task<List<int>> inactiveTransposrtIds()
        {
            var oldTransportIdData =await Context.ActiveTransport.AsNoTracking().Where(x => x.Active == 1).Select(x => x.Id).ToListAsync();
            var inactiveTransportIdData =await Context.ActiveTransport.AsNoTracking().Where(x => x.Active == 0).Select(x => x.Id).ToListAsync();
            foreach (var item in oldTransportIdData)
            {
                var scheduleDate = await transportScheduleDate(item, false);
                if (scheduleDate < DateTime.Today)
                {
                    inactiveTransportIdData.Add(item);
                }
            }
            return inactiveTransportIdData;
        }

        private async Task<List<int>> activeTransposrtIds()
        {
            var oldTransportIdData =await Context.ActiveTransport.AsNoTracking().Where(x => x.Active == 1).Select(x => x.Id).ToListAsync();
            var returndata = new List<int>();

            foreach (var item in oldTransportIdData)
            {
                var scheduleDate =await transportScheduleDate(item, false);
                if (scheduleDate >= DateTime.Today)
                {
                    returndata.Add(item);
                }
            }


            return returndata;
        }

        private async Task<DateTime?> transportScheduleDate(int? activeTransportId, bool startDate = true)
        {
            if (activeTransportId == null)
            {
                return null;
            }
            if (!startDate)
            {
                var maxRecord =await Context.TransportSchedule.AsNoTracking()
                    .Where(x => x.ActiveTransportId == activeTransportId)
                    .OrderByDescending(x => x.EventDate)
                    .FirstOrDefaultAsync();

                if (maxRecord != null)
                {
                    return maxRecord.EventDate;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var minRecord =await Context.TransportSchedule.AsNoTracking()
                    .Where(x => x.ActiveTransportId == activeTransportId)
                    .OrderBy(x => x.EventDate)
                    .FirstOrDefaultAsync();

                if (minRecord != null)
                {
                    return minRecord.EventDate;
                }
                else
                {
                    return null;
                }
            }

        }
      

        public async Task<ScheduleListActiveTransportResponse> ScheduleList(ScheduleListActiveTransportRequest request, CancellationToken cancellationToken)
        {

            DateTime startDate;
            DateTime endDate;


            if (!string.IsNullOrWhiteSpace(request.year) && int.TryParse(request.year, out var parsedYear))
            {
                startDate = new DateTime(parsedYear, 1, 1);
                endDate = new DateTime(parsedYear, 12, 31);
            }
            else
            {
                var currentYear = DateTime.Today.Year;
                startDate = new DateTime(currentYear, 1, 1);
                endDate = new DateTime(currentYear, 12, 31);
            }


            // Fetch the active transport information
            var transportQuery = from transport in Context.ActiveTransport
                                 where transport.Id == request.ActiveTransportId
                                 join carrier in Context.Carrier on transport.CarrierId equals carrier.Id
                                 join transportMode in Context.TransportMode on transport.TransportModeId equals transportMode.Id
                                 join fromLocation in Context.Location on transport.fromLocationId equals fromLocation.Id
                                 join toLocation in Context.Location on transport.toLocationId equals toLocation.Id
                                 select new
                                 {
                                     Transport = transport,
                                     Carrier = carrier,
                                     TransportMode = transportMode,
                                     FromLocation = fromLocation,
                                     ToLocation = toLocation,
                                     ETA = transport.ETA,
                                     ETD = transport.ETD
                                 };

            var transportInfo = await transportQuery.FirstOrDefaultAsync(cancellationToken);

            if (transportInfo == null)
            {
                return null; // Handle the case when the transport is not found
            }

            // Fetch schedules for the active transport
            var scheduleQuery = from schedule in Context.TransportSchedule.AsNoTracking()
                                where schedule.ActiveTransportId == request.ActiveTransportId && schedule.Active == 1
                                select new Schedules
                                {
                                    Id = schedule.Id,
                                    Code = schedule.Code,
                                    Seats = schedule.Seats,
                                    ETA = schedule.ETA,
                                    ETD = schedule.ETD,
                                    DateCreated = schedule.DateCreated,
                                    DateUpdated = schedule.DateUpdated,
                                    EventDate = schedule.EventDate,
                                    ActiveTransportId = schedule.ActiveTransportId,
                                    Description = schedule.Description,
                                    Bookings = 0,
                                    RealETD  = schedule.RealETD,
                                    Remark = schedule.Remark
                                    
                                    
                                };

            var schedules = await scheduleQuery.Where(x=> x.EventDate.Value.Date >= startDate && x.EventDate <= endDate.Date).ToListAsync(cancellationToken);

            // Calculate and set the actual bookings count for each schedule
            foreach (var schedule in schedules)
            {
                schedule.Bookings = await ScheduleBookingCount(schedule.Id);
                schedule.BusstopStatus = await ScheduleBustopStatus(schedule.Id);
            }

            // Create the response object
            var response = new ScheduleListActiveTransportResponse
            {
                Id = transportInfo.Transport.Id,
                Code = transportInfo.Transport.Code,
                DayNum = transportInfo.Transport.DayNum,
                Direction = transportInfo.Transport.Direction,
                Active = transportInfo.Transport.Active,
                CarrierId = transportInfo.Carrier.Id,
                CarrierName = transportInfo.Carrier.Description ?? string.Empty,
                TransportModeId = transportInfo.TransportMode.Id,
                TransportModeName = transportInfo.TransportMode.Code ?? string.Empty,
                TransportAudit = transportInfo.Transport.TransportAudit,
                Seats = transportInfo.Transport.Seats,
                fromLocationId = transportInfo.FromLocation.Id,
                fromLocationName = transportInfo.FromLocation.Description ?? string.Empty,
                fromLocationCode = transportInfo.FromLocation.Code ?? string.Empty,
                FrequencyWeeks = transportInfo.Transport.FrequencyWeeks,
                Special = transportInfo.Transport.Special,
                toLocationId = transportInfo.ToLocation.Id,
                toLocationName = transportInfo.ToLocation.Description ?? string.Empty,
                toLocationCode = transportInfo.ToLocation.Code ?? string.Empty,
                ETA = transportInfo.ETA,
                ETD = transportInfo.ETD,
                schedules = schedules,
                
            };

            return response;
        }


        //private async Task<int?> ScheduleBookingCount(int scheduleId, int activeTransportId)
        //{
        //    var schedule =await Context.TransportSchedule.Where(x => x.Id == scheduleId).FirstOrDefaultAsync();
        //    if (schedule != null)
        //    {
        //        return Context.Transport.Count(x => x.ActiveTransportId == activeTransportId && x.EventDate == schedule.EventDate.Date);
        //    }

        //    else {
        //        return 0;
        //    }
        //}

        private async Task<int?> ScheduleBookingCount(int scheduleId)
        {
                int bookingCount = await Context.Transport.AsNoTracking()
                    .CountAsync(x => x.ScheduleId == scheduleId);

                return bookingCount;
        }

        private async Task<bool> ScheduleBustopStatus(int scheduleId)
        {
            return await Context.TransportScheduleBusstop.AsNoTracking()
                .Where(x => x.ScheduleId == scheduleId).AnyAsync();
        }




        public async Task<List<GetCalendarActiveTransportResponse>> GetCalendarData(GetCalendarActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var currentDate = request.CurrentDate.Date;

            // Calculate the start (previous Monday) and end (next Sunday) dates
            var startDate = GetPreviousMonday(currentDate);
            var endDate = GetNextSunday(currentDate);

            // Fetch necessary data with AsNoTracking for better performance
            var transportSchedules = Context.TransportSchedule.AsNoTracking()
                .Where(ts => ts.EventDate >= startDate && ts.EventDate <= endDate);

            var activeTransports = Context.ActiveTransport.AsNoTracking();
            var transportModes = Context.TransportMode.AsNoTracking();
            var carriers = Context.Carrier.AsNoTracking();
            var locations = Context.Location.AsNoTracking();

            // Group and count transports by ScheduleId within the date range
            var transportCounts = Context.Transport.AsNoTracking()
                .Where(tt => tt.EventDate >= startDate && tt.EventDate <= endDate)
                .GroupBy(tt => tt.ScheduleId)
                .Select(g => new { ScheduleId = g.Key, Count = g.Count() });

            // Build the query with clear and corrected joins
            var query = from ts in transportSchedules
                        join at in activeTransports on ts.ActiveTransportId equals at.Id into atGroup
                        from activeTransport in atGroup.DefaultIfEmpty()

                        join tm in transportModes on activeTransport.TransportModeId equals tm.Id into tmGroup
                        from transportMode in tmGroup.DefaultIfEmpty()

                        join ttCount in transportCounts on ts.Id equals ttCount.ScheduleId into ttCountGroup
                        from ttCount in ttCountGroup.DefaultIfEmpty()

                        join carrier in carriers on activeTransport.CarrierId equals carrier.Id into carrierGroup
                        from carrier in carrierGroup.DefaultIfEmpty()

                        join fromLocation in locations on activeTransport.fromLocationId equals fromLocation.Id into fromLocationGroup
                        from fromLocation in fromLocationGroup.DefaultIfEmpty()

                        join toLocation in locations on activeTransport.toLocationId equals toLocation.Id into toLocationGroup
                        from toLocation in toLocationGroup.DefaultIfEmpty()

                        select new GetCalendarActiveTransportResponse
                        {
                            Id = ts.Id,
                            Code = ts.Code,
                            Description = ts.Description,
                            EventDate = ts.EventDate,
                            Seats = ts.Seats,
                            ETD = ts.ETD,
                            ETA = ts.ETA,
                            Direction = activeTransport.Direction,
                            TransportMode = transportMode.Code,
                            Carrier = carrier.Description,
                            FromLocationCode = fromLocation.Code,
                            ToLocationCode = toLocation.Code,
                            Booking = ttCount.Count,
                            TimeGroup = GetTimeGroupStatic(ts.ETD)
                        };

            return await query.ToListAsync(cancellationToken);
        }


        private static string GetTimeGroupStatic(string? ETD)
        {
            if (ETD == null)
            {
                return "AM";
            }
            if (DateTime.TryParseExact(
                ETD,
                "HHmm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime time))
            {
                return time.Hour < 12 ? "AM" : "PM";
            }
            else
            {
                return "AM";
            }
        }

        // Helper method to get the previous Monday from a given date
        private DateTime GetPreviousMonday(DateTime date)
        {
            int daysToSubtract = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-daysToSubtract).Date;
        }

        // Helper method to get the next Sunday from a given date
        private DateTime GetNextSunday(DateTime date)
        {
            int daysToAdd = (7 - (int)date.DayOfWeek) % 7;
            daysToAdd = daysToAdd == 0 ? 7 : daysToAdd; // If today is Sunday, get the next Sunday
            return date.AddDays(daysToAdd).Date;
        }

      
        #region Create Special Event 


        public async Task CreateSpecial(CreateSpecialActiveTransportRequest request)
        {
            string p_direction_first = "";
            // string fromLocationCode =await Context.Location.FirstOrDefaultAsync(x => x.Id == request.fromLocationId).Code ?? string.Empty;

            string fromLocationCode = await Context.Location.AsNoTracking().Where(x => x.Id == request.fromLocationId)
                                 .Select(x => x.Code)
                                 .FirstOrDefaultAsync() ?? string.Empty;

            string toLocationCode = await Context.Location.Where(x => x.Id == request.toLocationId)
                                             .Select(x => x.Code)
                                             .FirstOrDefaultAsync() ?? string.Empty;
            bool fromSiteStatus = await Context.Location.AsNoTracking().AnyAsync(x => x.Id == request.fromLocationId && x.onSite == 1);
            bool TosSiteStatus = await Context.Location.AsNoTracking().AnyAsync(x => x.Id == request.toLocationId && x.onSite == 1);

            if (!fromSiteStatus && TosSiteStatus)
            {
                p_direction_first = "IN";

            }
            if (fromSiteStatus && !TosSiteStatus)
            {
                p_direction_first = "OUT";
            }
            if (!fromSiteStatus && !TosSiteStatus)
            {
                p_direction_first = "EXTERNAL";
            }
            if (fromSiteStatus && TosSiteStatus)
            {
                throw new BadRequestException("Schedules cannot be created with this location");
                //Task.FromException(exception);
            }

            ActiveTransport activeTransport = new ActiveTransport
            {
                Active = 1,
                Code = request.Code,
                CarrierId = request.CarrierId,
                fromLocationId = request.fromLocationId,
                toLocationId = request.toLocationId,
                Seats = request.Seats,
                Direction = p_direction_first,
                Description = string.Format("{0} {1} {2} {3}", request.ETD, fromLocationCode, request.ETA, toLocationCode),
                TransportModeId = request.TransportModeId,
                Special = 1,
                TransportAudit = 0,
                DayNum = request.EventDate.ToString("dddd"),
                CostCodeId = request.CostCodeId,
                ETA = request.ETA,
                ETD = request.ETD,
                AircraftCode = request.AircraftCode

            };

                Context.ActiveTransport.Add(activeTransport);
                await Context.SaveChangesAsync();
                activeTransport.Id = activeTransport.Id;


            if (p_direction_first != "EXTERNAL")
            {
                if (p_direction_first == "OUT")
                {
                    TransportSchedule schedule = new TransportSchedule
                    {
                        Code = request.Code,
                        Description = string.Format("{0} {1} {2} {3}", request.ETD, fromLocationCode, request.ETA, toLocationCode),
                        EventDate = request.EventDate.Date,
                        Active = 1,
                        Seats = request.Seats,
                        ETD = request.ETD,
                        ETA = request.ETA,
                        ActiveTransportId = activeTransport.Id
                    };
                    Context.TransportSchedule.Add(schedule);
                }
                else {
                    if (p_direction_first == "IN" && !request.OutSeats.HasValue)
                    {
                        TransportSchedule schedule = new TransportSchedule
                        {
                            Code = request.Code,
                            Description = string.Format("{0} {1} {2} {3}", request.ETD, fromLocationCode, request.ETA, toLocationCode),
                            EventDate = request.EventDate.Date,
                            Active = 1,
                            Seats = request.Seats,
                            ETD = request.ETD,
                            ETA = request.ETA,
                            ActiveTransportId = activeTransport.Id
                        };
                        Context.TransportSchedule.Add(schedule);
                    }
                    else {

                        TransportSchedule scheduleIN = new TransportSchedule
                        {
                            Code = request.Code,
                            Description = string.Format("{0} {1} {2} {3}", request.ETD, fromLocationCode, request.ETA, toLocationCode),
                            EventDate = request.EventDate.Date,
                            Active = 1,
                            Seats = request.Seats,
                            ETD = request.ETD,
                            ETA = request.ETA,
                            ActiveTransportId = activeTransport.Id
                        };
                        Context.TransportSchedule.Add(scheduleIN);



                        ActiveTransport activeTransportout = new ActiveTransport
                        {
                            Active = 1,
                            Code = request.Code,
                            CarrierId = request.CarrierId,
                            fromLocationId = request.toLocationId, 
                            toLocationId = request.fromLocationId,
                            Seats = request.OutSeats,
                            Direction = "OUT",
                            Description = string.Format("{0} {1} {2} {3}", request.OUTETD, toLocationCode, request.OUTETA, fromLocationCode),
                            TransportModeId = request.TransportModeId,
                            Special = 1,
                            TransportAudit = 0,
                            DayNum = request.EventDate.ToString("dddd"),
                            CostCodeId = request.CostCodeId,
                            ETA = request.ETA,
                            ETD = request.ETD

                        };

                        Context.ActiveTransport.Add(activeTransportout);
                        await Context.SaveChangesAsync();


                        TransportSchedule scheduleOUT = new TransportSchedule
                        {
                            Code = request.Code,
                            Description = string.Format("{0} {1} {2} {3}", request.OUTETD, toLocationCode, request.OUTETA, fromLocationCode),
                            EventDate = request.EventDate.Date,
                            Active = 1,
                            Seats = request.OutSeats,
                            ETD = request.OUTETD,
                            ETA = request.OUTETA,
                            ActiveTransportId = activeTransportout.Id
                        };
                        Context.TransportSchedule.Add(scheduleOUT);
                    }
                }

            }
            else {

                TransportSchedule schedule = new TransportSchedule
                {
                    Code = request.Code,
                    Description = string.Format("{0} {1} {2} {3}", request.ETD, fromLocationCode, request.ETA, toLocationCode),
                    EventDate = request.EventDate.Date,
                    Active = 1,
                    Seats = request.Seats,
                    ETD = request.ETD,
                    ETA = request.ETA,
                    ActiveTransportId = activeTransport.Id
                };
                Context.TransportSchedule.Add(schedule);
            }




        }
 
        #endregion




        public async Task DeActive(DeleteActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var currentActiveTransport = await Context.ActiveTransport.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (currentActiveTransport != null)
            {
                var schedules = await Context.TransportSchedule.Where(x => x.ActiveTransportId == request.Id).ToListAsync();

                foreach (var schedule in schedules)
                {
                    Context.TransportSchedule.Remove(schedule);
                }

                var clusters = await Context.ClusterDetail.Where(x => x.ActiveTransportId == request.Id).ToListAsync();

                foreach (var cluster in clusters)
                {
                    Context.ClusterDetail.Remove(cluster);
                }

                await Context.SaveChangesAsync();
                Context.ActiveTransport.Remove(currentActiveTransport);

            }



        }


        public async Task<List<GetDateActiveTransportResponse>> GetDateData(GetDateActiveTransportRequest request, CancellationToken cancellationToken)
        {
            if (!request.DepartureLocationId.HasValue)
            {
                var transportIds = await Context.ActiveTransport.AsNoTracking()
                     .Where(x => x.DayNum == request.eventDate.ToString("dddd") && x.Direction == request.Direction && x.Active == 1)
                     .Select(x => x.Id)
                     .ToArrayAsync();

                var results = await Context.TransportSchedule.AsNoTracking()
                    .Where(x => x.EventDate.Date == request.eventDate.Date && transportIds.Contains(x.ActiveTransportId))
                    .ToListAsync();
                var returnData = new List<GetDateActiveTransportResponse>();
                foreach (var item in results)
                {

                    var currentTransport = await Context.ActiveTransport.AsNoTracking().Where(a => a.Id == item.ActiveTransportId && a.Active == 1)
                        .Select(x => new { x.Code, x.Description, x.Seats }).FirstOrDefaultAsync(cancellationToken);

                    var bookedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id).CountAsync();
                    var newData = new GetDateActiveTransportResponse
                    {
                        ActiveTransportId = item.ActiveTransportId,
                        ScheduleId = item.Id,
                        Description = $"{currentTransport?.Code} {request.eventDate.ToString("dddd")} {item.Description}",
                        Seat = currentTransport?.Seats,
                        BookedCount = bookedCount,


                    };
                    returnData.Add(newData);
                }


                return returnData;
            }
            else {
                var transportIds = await Context.ActiveTransport.AsNoTracking()
                     .Where(x => x.DayNum == request.eventDate.ToString("dddd") && x.Direction == request.Direction && x.Active == 1 && x.fromLocationId == request.DepartureLocationId && x.toLocationId == request.arriveLocationId.Value)
                     .Select(x => x.Id)
                     .ToArrayAsync();

                var results = await Context.TransportSchedule.AsNoTracking()
                    .Where(x => x.EventDate.Date == request.eventDate.Date && transportIds.Contains(x.ActiveTransportId))
                    .ToListAsync();
                var returnData = new List<GetDateActiveTransportResponse>();
                foreach (var item in results)
                {

                    var currentTransport = await Context.ActiveTransport.AsNoTracking().Where(a => a.Id == item.ActiveTransportId)
                        .Select(x => new { x.Code, x.Description, x.Seats }).FirstOrDefaultAsync(cancellationToken);

                    var bookedCount = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == item.Id).CountAsync();
                    var newData = new GetDateActiveTransportResponse
                    {
                        ActiveTransportId = item.ActiveTransportId,
                        ScheduleId = item.Id,
                        Description = $"{currentTransport?.Code} {request.eventDate.ToString("dddd")} {item.Description}",
                        Seat = currentTransport?.Seats,
                        BookedCount = bookedCount,


                    };
                    returnData.Add(newData);
                }


                return returnData;
            }

        }

        #region DeletActiveTransport Validation check
        public async Task DeleteTransportValidationDB(int ActiveTransportId, CancellationToken cancellationToken)
        {
            List<string> errors = new List<string>();
            var transportBookedCount = await Context.Transport.CountAsync(x => x.ActiveTransportId == ActiveTransportId);

            if (transportBookedCount > 0)
            {
                errors.Add($"Cannot deactivate the transport. {transportBookedCount} booking(s) have been placed on this transport");
            }

            if (errors.Count > 0)
            {
                throw new BadRequestException(errors.ToArray());
            }

            await Task.CompletedTask;


        }

        #endregion


        #region UpdateData


        public async Task ChangeTransport(UpdateActiveTransportRequest request, CancellationToken cancellationToken)
        {

            if (request.StartDate < DateTime.Now)
            {
                throw new BadRequestException("Please ensure the Start Date is set to a future date whenever the Transport method is changed");
            }
            var currentActiveTranport = await Context.ActiveTransport
                .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentActiveTranport != null)
            {
                if (currentActiveTranport.Active == 1)
                {
                    if (!string.IsNullOrWhiteSpace(request.ETD) && !string.IsNullOrWhiteSpace(request.ETA) && request.ETA.Length == 4)
                    {
                        var fromLoction =await Context.Location
                            .Where(x => x.Id == currentActiveTranport.fromLocationId)
                            .Select(x => new { x.Code }).FirstOrDefaultAsync();
                        var tomLoction =await Context.Location
                            .Where(x => x.Id == currentActiveTranport.toLocationId)
                            .Select(x => new { x.Code }).FirstOrDefaultAsync();

                        var descr = $"{request.ETD} {fromLoction?.Code} {request.ETA} {tomLoction?.Code}";
                        currentActiveTranport.CarrierId = request.CarrierId;
                        currentActiveTranport.Code = request.Code;
                        currentActiveTranport.Seats = request.Seats;
                        currentActiveTranport.Description = descr;
                        currentActiveTranport.DateUpdated = DateTime.Now;
                        currentActiveTranport.ETD = request.ETD;
                        currentActiveTranport.ETA = request.ETA;
                        currentActiveTranport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        Context.ActiveTransport.Update(currentActiveTranport);
                        await ChangeTranspportSchedule(currentActiveTranport.Id, request.ETD, request.ETA, descr, request.Code, descr, request.Seats, request.StartDate, request.EndDate);
                    }
                    else
                    {
                        currentActiveTranport.CarrierId = request.CarrierId;
                        currentActiveTranport.Code = request.Code;
                        currentActiveTranport.Seats = request.Seats;
                        currentActiveTranport.DateUpdated = DateTime.Now;
                        currentActiveTranport.ETA = request.ETA;
                        currentActiveTranport.ETD = request.ETD;
                        currentActiveTranport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        Context.ActiveTransport.Update(currentActiveTranport);

                    }

                }
                else
                {
                    throw new BadRequestException("This transport is inactive and cannot be modified");
                }
            }
            else
            {
                throw new BadRequestException("No record was found for this transport");
            }
        }


        private async Task ChangeTranspportSchedule(int ActiveTransportId, string ETD, string ETA, string Desrc, string code, string descr, int seats, DateTime startDate, DateTime endDate)
        {
            var TransportSchedules = await Context.TransportSchedule
                .Where(x => x.ActiveTransportId == ActiveTransportId && x.EventDate.Date >= startDate.Date && x.EventDate.Date <= endDate.Date).ToListAsync();

            bool seatchange = false;
            foreach (var TransportSchedule in TransportSchedules)
            {

                if (TransportSchedule.Seats != seats)
                {
                    seatchange = true;     
                }
                TransportSchedule.Code = code;
                TransportSchedule.Description = Desrc;
                TransportSchedule.DateUpdated = DateTime.Now;
                TransportSchedule.Description = descr;
                TransportSchedule.ETA = ETA;
                TransportSchedule.Seats = seats;
                TransportSchedule.ETD = ETD;
                TransportSchedule.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                Context.TransportSchedule.Update(TransportSchedule);

                var EmployeeTransports = await Context.Transport
                    .Where(x => x.ScheduleId == TransportSchedule.Id).ToListAsync();

                string hourString = ETD.Substring(0, 2);
                string minuteString = ETD.Substring(2, 2);

                int hour = int.Parse(hourString);
                int minute = int.Parse(minuteString);
                var currentSeatindex = 1;

                foreach (var item in EmployeeTransports)
                {
                    item.EventDate.Value.Date.AddHours(hour).AddMinutes(minute);
                    item.DateUpdated = DateTime.Now;
                    if (seatchange)
                    {
                        if (seats >= currentSeatindex)
                        {
                            item.Status = "Confirmed";
                        }
                        else {
                            item.Status = "Over Booked";
                        }
                        

                    }

                    item.ChangeRoute = "Active Transport update";
                    item.DateUpdated = DateTime.Now;
                    item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    Context.Transport.Update(item);
                    currentSeatindex++;
                }

            }
        }

        #endregion


        #region ChangeDescription
        public async Task ChangeTransportDescription(UpdateDescrActiveTransportRequest request, CancellationToken cancellationToken) 
        {
            var currentData = await Context.ActiveTransport.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                currentData.Description = request.description;
                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                Context.ActiveTransport.Update(currentData);
            }
            else {
                throw new BadRequestException("Transport not found");
            }

        }

        #endregion



        #region ChangeDescription
        public async Task ChangeTransportAircraftCode(UpdateAircraftCodeActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.ActiveTransport.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                currentData.AircraftCode = request.AircraftCode;
                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                Context.ActiveTransport.Update(currentData);
            }
            else
            {
                throw new BadRequestException("Transport not found");
            }

        }

        #endregion

    }

}
