using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.EmployeeFeature.RosterExecuteEmployee;
using tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectute;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExectutePreview;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecute;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecutePreview;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static tas.Persistence.Repositories.EmployeeRepository;

namespace tas.Persistence.Repositories
{
    public class RosterExecuteRepository : BaseRepository<Roster>, IRosterExecuteRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        private readonly ILogger _logger;
        private readonly ITransportScheduleCalculateRepository _transportScheduleCalculateRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public RosterExecuteRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, ILogger<RosterExecuteRepository> logger, ITransportScheduleCalculateRepository transportScheduleCalculateRepository, IEmployeeRepository employeeRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
            _logger = logger;
            _transportScheduleCalculateRepository = transportScheduleCalculateRepository;
            _employeeRepository = employeeRepository;
        }






        #region PreviewRoster






        public async Task<List<BulkRosterExecutePreviewResponse>> ExecuteBulkRosterPreview(BulkRosterExecutePreviewRequest request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate.Date;
            var endDate = startDate.AddMonths(request.DurationMonth);
            var virtualRoom = await Context.Room.Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
            if (virtualRoom == null)
            {
                throw new BadRequestException("Virtual room not registered");
            }
            var returnData = new List<BulkRosterExecutePreviewResponse>();
            foreach (var item in request.Employees)
            {
                var EmployeeRosterDates = await GetEmployeeRosterDates(item, startDate, endDate);
                var EmployeeFutureStatus = await Context.EmployeeStatus
                    .Where(x => x.EmployeeId == item.EmployeeId && x.EventDate >= startDate && x.EventDate <= endDate)
                    .ToListAsync();
                // var EmployeeRoomData = await GetEmployeeRoomData(item, EmployeeRosterDates, startDate, endDate);
                var EmployeeOnsiteData = await GetEmployeeOnSiteData(item, startDate);
                var EmployeeOffsiteData = await GetEmployeeOffSiteData(item, EmployeeRosterDates, startDate, endDate);

                if (/*EmployeeRoomData?.Count > 0 ||*/ EmployeeOnsiteData != null || EmployeeOffsiteData.Count > 0)
                {
                    var CurrentEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId)
                        .Select(x => new { x.Id, x.Firstname, x.Lastname }).FirstOrDefaultAsync();

                    bool v_EmployeeRoomDataStatus = false;




                    var newData = new BulkRosterExecutePreviewResponse
                    {
                        EmpId = item.EmployeeId,
                        Fullname = $"{CurrentEmployee.Firstname} {CurrentEmployee.Lastname}",
                        OffSiteStatus = EmployeeOffsiteData,
                        OnsiteData = EmployeeOnsiteData != null ? EmployeeOnsiteData : null,
                        EmpOffSiteStatus = EmployeeOffsiteData.Count() > 0 ? true : false,
                        EmpOnSiteStatus = EmployeeOnsiteData != null ? true : false,
                        EmployeeRoomDataStatus = v_EmployeeRoomDataStatus,


                    };

                    returnData.Add(newData);

                }


            }

            //   var rosterDetail =await Context.RosterDetail.AsNoTracking().Where(x => x.RosterId == 1).ToListAsync();

            //var data =await EmployeeActiveDates(startDate, endDate, rosterDetail, null, virtualRoom.Id, );
            //var itms = JsonSerializer.Serialize(data);
            return returnData;
        }


        private async Task<List<BulkRosterExecutePreviewResponseEmployeeRoomData>> GetEmployeeRoomData(BulkRosterExecutePreviewEmployee employee, List<DateTime> rosterDates, DateTime startDate, DateTime endDate)
        {
            var currentEmployee = await Context.Employee.Where(x => x.Id == employee.EmployeeId && x.RoomId != null).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                var employeeRoomData = new List<BulkRosterExecutePreviewResponseEmployeeRoomData>();
                var data = await Context.EmployeeStatus.Where(x => x.RoomId == currentEmployee.RoomId && rosterDates.Contains(x.EventDate.Value.Date) && x.EmployeeId != employee.EmployeeId).ToListAsync();
                foreach (var item in data)
                {
                    var RoomEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId).Select(x => new { x.Id, x.Firstname, x.Lastname, x.RoomId }).FirstOrDefaultAsync();
                    if (RoomEmployee != null)
                    {
                        if (RoomEmployee?.RoomId != item.RoomId || RoomEmployee?.RoomId == null)
                        {
                            var newData = new BulkRosterExecutePreviewResponseEmployeeRoomData
                            {
                                EmpId = RoomEmployee.Id,
                                Fullname = $"{RoomEmployee.Firstname} {RoomEmployee.Lastname}",
                                EventDate = item.EventDate.Value.Date
                            };
                            employeeRoomData.Add(newData);
                        }
                    }



                }

                return employeeRoomData;
            }
            return new List<BulkRosterExecutePreviewResponseEmployeeRoomData>();
        }

        private async Task<List<BulkRosterExecutePreviewResponseOffSiteStatus>> GetEmployeeOffSiteData(BulkRosterExecutePreviewEmployee employee, List<DateTime> rosterDates, DateTime startDate, DateTime endDate)
        {
            var offSiteData = new List<BulkRosterExecutePreviewResponseOffSiteStatus>();

            var data = await Context.EmployeeStatus
                .Where(x => x.RoomId == null && x.EventDate >= startDate && x.EventDate <= endDate && x.EmployeeId == employee.EmployeeId)
                .ToListAsync();

            foreach (var item in data)
            {
                var currentShift = await Context.Shift
                    .Where(x => x.Id == item.ShiftId)
                    .Select(x => new { x.Id, x.Code, x.OnSite, x.Description })
                    .FirstOrDefaultAsync();

                if (currentShift?.Code == "AN")
                {
                    var newData = new BulkRosterExecutePreviewResponseOffSiteStatus
                    {
                        EventDate = item.EventDate.Value.Date,
                        ShiftCode = currentShift.Code,
                        ShiftId = currentShift.Id
                    };
                    offSiteData.Add(newData);
                }

            }

            return offSiteData;
        }


        private async Task<BulkRosterExecutePreviewResponseOnsiteData?> GetEmployeeOnSiteData(BulkRosterExecutePreviewEmployee employee, DateTime startDate)
        {

            var dateRange = new List<DateTime>
            {
                startDate.Date,
              /*  startDate.AddDays(-1).Date,
                startDate.AddDays(-2).Date,*/
                startDate.AddDays(1).Date,
            //    startDate.AddDays(2).Date
            };



            var rosterStartDateOnsite = await Context.EmployeeStatus.AsNoTracking()
            .Where(x => x.EmployeeId == employee.EmployeeId && dateRange.Contains(x.EventDate.Value.Date) && x.RoomId != null).FirstOrDefaultAsync();
            if (rosterStartDateOnsite != null)
            {


                var firstINTransport = await (from emptransport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == employee.EmployeeId && x.EventDate.Value.Date <= startDate && x.Direction == "IN").OrderByDescending(x => x.EventDate)
                                              join transport in Context.ActiveTransport.AsNoTracking() on emptransport.ActiveTransportId equals transport.Id into transportData
                                              from transport in transportData.DefaultIfEmpty()
                                              join schedule in Context.TransportSchedule.AsNoTracking() on emptransport.ScheduleId equals schedule.Id into scheduleData
                                              from schedule in scheduleData.DefaultIfEmpty()
                                              select new
                                              {
                                                  EventDate = emptransport.EventDate,
                                                  Status = emptransport.Status,
                                                  Code = schedule.Code,
                                                  ScheduleDescription = schedule.Description,
                                                  ActiveTransportCode = transport.Code,

                                              }).FirstOrDefaultAsync();

                var firstOUTTransport = await (from emptransport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == employee.EmployeeId && x.EventDate.Value.Date >= startDate && x.Direction == "OUT").OrderBy(x => x.EventDate)
                                               join transport in Context.ActiveTransport.AsNoTracking() on emptransport.ActiveTransportId equals transport.Id into transportData
                                               from transport in transportData.DefaultIfEmpty()
                                               join schedule in Context.TransportSchedule.AsNoTracking() on emptransport.ScheduleId equals schedule.Id into scheduleData
                                               from schedule in scheduleData.DefaultIfEmpty()
                                               select new
                                               {
                                                   EventDate = emptransport.EventDate,
                                                   Status = emptransport.Status,
                                                   Code = schedule.Code,
                                                   ScheduleDescription = schedule.Description,
                                                   ActiveTransportCode = transport.Code,

                                               }).FirstOrDefaultAsync();


                //var firstINTransport = await (from emptransport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == employee.EmployeeId && dateRange.Contains(x.EventDate.Value.Date) && x.Direction == "IN").OrderByDescending(x => x.EventDate)
                //                              join transport in Context.ActiveTransport.AsNoTracking() on emptransport.ActiveTransportId equals transport.Id into transportData
                //                              from transport in transportData.DefaultIfEmpty()
                //                              join schedule in Context.TransportSchedule.AsNoTracking() on emptransport.ScheduleId equals schedule.Id into scheduleData
                //                              from schedule in scheduleData.DefaultIfEmpty()
                //                              select new
                //                              {
                //                                  EventDate = emptransport.EventDate,
                //                                  Status = emptransport.Status,
                //                                  Code = schedule.Code,
                //                                  ScheduleDescription = schedule.Description,
                //                                  ActiveTransportCode = transport.Code,

                //                              }).FirstOrDefaultAsync();

                //var firstOUTTransport = await (from emptransport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == employee.EmployeeId && dateRange.Contains(x.EventDate.Value.Date) && x.Direction == "OUT").OrderBy(x => x.EventDate)
                //                               join transport in Context.ActiveTransport.AsNoTracking() on emptransport.ActiveTransportId equals transport.Id into transportData
                //                               from transport in transportData.DefaultIfEmpty()
                //                               join schedule in Context.TransportSchedule.AsNoTracking() on emptransport.ScheduleId equals schedule.Id into scheduleData
                //                               from schedule in scheduleData.DefaultIfEmpty()
                //                               select new
                //                               {
                //                                   EventDate = emptransport.EventDate,
                //                                   Status = emptransport.Status,
                //                                   Code = schedule.Code,
                //                                   ScheduleDescription = schedule.Description,
                //                                   ActiveTransportCode = transport.Code,

                //                               }).FirstOrDefaultAsync();


                var employeeOnsiteStatusDates = await Context.EmployeeStatus.AsNoTracking()
                     .Where(x => x.EmployeeId == employee.EmployeeId
                     && x.EventDate >= firstINTransport.EventDate.Value.Date
                     && x.EventDate < firstOUTTransport.EventDate.Value.Date).ToListAsync();

                //var employeeOnsiteStatusDates = await Context.EmployeeStatus.AsNoTracking()
                //.Where(x => x.EmployeeId == employee.EmployeeId
                //            && (firstINTransport == null || x.EventDate >= firstINTransport.EventDate.Value.Date)
                //            && (firstOUTTransport == null || x.EventDate < firstOUTTransport.EventDate.Value.Date)).OrderByDescending(x=> x.EventDate)
                //.FirstOrDefaultAsync();


                bool AllowDelete = true;

                if (firstINTransport?.EventDate.Value.Date <= startDate && firstOUTTransport?.EventDate.Value.Date >= startDate.Date)
                {
                    AllowDelete = false;
                }
                else if (firstINTransport?.EventDate.Value.Date >= startDate || firstINTransport?.EventDate.Value.Date <= startDate.AddDays(2).Date)
                {
                    AllowDelete = false;
                }
                else if (firstOUTTransport?.EventDate.Value.Date >= startDate || firstOUTTransport?.EventDate.Value.Date <= startDate.AddDays(2).Date)
                {
                    AllowDelete = false;
                }
                else {
                    AllowDelete = true;
                }

                var onsiteData = new BulkRosterExecutePreviewResponseOnsiteData
                {
                    EventDate = startDate,
                    InTransportDate = firstINTransport.EventDate,
                    OutTransportDate = firstOUTTransport.EventDate,
                    Status = $"{firstINTransport.Status}",
                    INActiveTransportCode = firstINTransport?.ActiveTransportCode,
                    INScheduleDescription = firstINTransport?.ScheduleDescription,
                    OUTActiveTransportCode = firstOUTTransport?.ActiveTransportCode,
                    OUTScheduleDescription = firstOUTTransport?.ScheduleDescription,
                    AllowDelete = AllowDelete

                };



                return onsiteData;


            }




            return null;

        }



        private async Task<List<BulkRosterActiveDate>> EmployeeActiveDates(DateTime startDate, DateTime endDate, List<RosterDetail> rosterDetails, int? RoomId, int VirtualRoomId, int EmployeeId)
        {
            var cycleDates = new List<BulkRosterActiveDate>();
            DateTime currentDate = startDate;


            var DateCycles = rosterDetails.Select(x => new { x.DaysOn.Value, x.ShiftId }).ToList();

            var cycleAllDays = DateCycles.Sum(c => c.Value);


            if (DateCycles.Count > 0)
            {
                int cycleIndex = 0;
                while (currentDate <= endDate)
                {
                    string Direction = "IN";
                    int onsite = 1;
                    var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == DateCycles[cycleIndex].ShiftId).Select(x => new { x.OnSite }).FirstOrDefaultAsync();
                    if (currentShift?.OnSite != 1)
                    {
                        Direction = "OUT";
                        onsite = 0;
                    }
        
                    cycleDates.Add(new BulkRosterActiveDate { EventDate = currentDate, Direction = Direction, ShiftId = DateCycles[cycleIndex].ShiftId.Value, OnSite = onsite });
                    int cycle = DateCycles[cycleIndex].Value;
                    currentDate = currentDate.AddDays(cycle);
                    cycleIndex = (cycleIndex + 1) % DateCycles.Count;
                }
            }

            for (int i = 0; i < cycleDates.Count; i++)
            {

                if (i + 1 < cycleDates.Count)
                {
                    var cycleStartDate = cycleDates[i].EventDate;
                    var cycleEndDate = cycleDates[i + 1].EventDate;
                    int RosterRoomId = VirtualRoomId;

                    if (cycleDates[i].OnSite == 1)
                    {
                        if (RoomId.HasValue)
                        {
                            var roomStatus = await CheckRoomDate(RoomId.Value, cycleStartDate, cycleEndDate.AddDays(-1), EmployeeId);
                            if (roomStatus)
                            {
                                RosterRoomId = RoomId.Value;
                            }
                            else
                            {
                                RosterRoomId = VirtualRoomId;
                            }
                        }
                        else
                        {
                            RosterRoomId = VirtualRoomId;
                        }


                    }
                    var itemDates = new List<BulkRosterActiveDateDetail>();
                    while (cycleStartDate < cycleEndDate)
                    {
                        var newData = new BulkRosterActiveDateDetail
                        {
                            EventDate = cycleStartDate,
                            OnSite = cycleDates[i].OnSite,
                            ShifId = cycleDates[i].ShiftId,
                            RoomId = RosterRoomId

                        };
                        itemDates.Add(newData);
                        cycleStartDate = cycleStartDate.AddDays(1);
                    }

                    cycleDates[i].details = itemDates;
                }

            }

            if (cycleDates.Last().Direction == "IN")
            {
                cycleDates.Remove(cycleDates.Last());
            }


            return cycleDates;
        }


        private async Task<List<DateTime>> GetEmployeeRosterDates(BulkRosterExecutePreviewEmployee employee, DateTime startDate, DateTime endDate)
        {
            DateTime currentDate = startDate;
            DateTime v_date = startDate;

            var returnData = new List<DateTime>();
            while (v_date <= endDate)
            {

                var data = await Context.RosterDetail.Where(x => x.RosterId == employee.RosterId && x.Active == 1).OrderBy(x => x.SeqNumber).ToListAsync();
                if (data.Count == 0)
                {
                    return returnData;
                }
                foreach (RosterDetail detail in data)
                {
                    returnData.Add(v_date);
                    v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));

                }
            }

            return returnData;
        }


        #endregion


        #region ValidateRoster


        private async Task<bool> ValidDateRoster(int rosterId)
        {
            var currentRoster = await Context.Roster.AsNoTracking().Where(x => x.Id == rosterId && x.Active == 1).FirstOrDefaultAsync();
            if (currentRoster != null)
            {
                var rosterDetails = await Context.RosterDetail.AsNoTracking().Where(x => x.RosterId == rosterId && x.Active == 1).OrderBy(x => x.SeqNumber).ToListAsync();
                if (rosterDetails.Count > 1)
                {
                    int? firstShiftId = rosterDetails.First().ShiftId;
                    if (firstShiftId.HasValue)
                    {
                        var firstShiftData = await Context.Shift.AsNoTracking().Where(x => x.Id == firstShiftId).FirstOrDefaultAsync();
                        if (firstShiftData != null)
                        {
                            if (firstShiftData.OnSite == 1)
                            {
                                return true;
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


        #region ExecuteRoster
        public async Task<BulkRosterExecuteResponse> ExecuteBulkRoster(BulkRosterExecuteRequest request, CancellationToken cancellationToken)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var employeeStatusLocal = new List<EmployeeStatus>();
            var employeeTransportLocal = new List<Transport>();

            var d2Shift = await Context.Shift.AsNoTracking().Where(x => x.Code == "D2").FirstOrDefaultAsync();
            var n2Shift = await Context.Shift.AsNoTracking().Where(x => x.Code == "N2").FirstOrDefaultAsync();

            var dsShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "DS").FirstOrDefaultAsync();
            var nsShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "NS").FirstOrDefaultAsync();

            if (d2Shift == null || n2Shift == null|| dsShift == null || nsShift == null )
            {
                throw new BadRequestException("Remember to register your D2 (Day Shift 2) and N2 (Night Shift 2) shift");
            }


            var shiftDataDS = new List<Shift>();
            shiftDataDS.Add(dsShift);
            shiftDataDS.Add(nsShift);
            shiftDataDS.Add(n2Shift);
            shiftDataDS.Add(d2Shift);




            var startDate = request.StartDate.Date;

            if (request.DurationMonth > 18)
            {
                throw new BadRequestException("Month duration is too long.");
            }

            var endDate = startDate.AddMonths(request.DurationMonth);
            var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
            if (virtualRoom == null)
            {
                throw new BadRequestException("Virtual room is not registered. Please contact the admin team for no action");
            }


            int CurrentRoomId = virtualRoom.Id;

            var employeeIds = request.Employees.Select(e => e.EmployeeId).ToList();


            var allemployees = await (from emp in Context.Employee.AsNoTracking().Where(x => employeeIds.Contains(x.Id))
                                      join room in Context.Room.AsNoTracking() on emp.RoomId equals room.Id into roomData
                                      from room in roomData.DefaultIfEmpty()
                                      select new
                                      {
                                          emp.Id,
                                          RoomId = emp.RoomId, 
                                          BedCount = room != null ? room.BedCount : (int?)null
                                      }).ToListAsync();


            var savedEmployeeIds = new List<int>();
            var skippedEmployeeIds = new List<int>();


            var FlightGroupDetails =await Context.FlightGroupDetail.AsNoTracking().Where(x => x.FlightGroupMasterId == request.FlightGroupMasterId).OrderBy(x => x.SeqNumber).ToListAsync();

            var drivetransportMode = await Context.TransportMode.AsNoTracking().Where(x => x.Code.ToLower() == "drive").FirstOrDefaultAsync();

            stopWatch.Stop();
         //   _logger.LogInformation($"beginRoster ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            foreach (var item in request.Employees)
            {

                var itemOnsiteStatus = await CheckStartDateOnSite(item.EmployeeId, startDate);
                if (!itemOnsiteStatus)
                {

                    if (await CheckStartDateDirection(item.EmployeeId, startDate))
                    {
                        //  var rosterDetails =await Context.RosterDetail.AsNoTracking().Where(x => x.RosterId == item.RosterId && x.Active == 1).OrderBy(x => x.SeqNumber).ToListAsync();

                        var rosterStatus = await ValidDateRoster(item.RosterId);

                        if (rosterStatus)
                        {
                            var currentEmployee = allemployees.Where(x => x.Id == item.EmployeeId).FirstOrDefault();
                            if (currentEmployee != null)
                            {
                                var rosterDetail = await Context.RosterDetail.AsNoTracking().Where(x => x.RosterId == item.RosterId).ToListAsync();
                                // var itms = await EmployeeActiveDates(startDate, endDate, rosterDetail);
                                var itms = await EmployeeActiveDates(startDate, endDate, rosterDetail, currentEmployee.RoomId, virtualRoom.Id, currentEmployee.Id);
                                var employeeOnSiteDates = EmployeeOnsiteDates(itms);
                                List<DateTime> sortedDates = employeeOnSiteDates.OrderBy(d => d.Date).ToList();
                                //   var v_startDate = sortedDates.First().Date;
                                //    var v_endDate = sortedDates.Last().Date;


                                var v_startDate = employeeOnSiteDates.Min(d => d.Date);
                                var v_endDate = employeeOnSiteDates.Max(d => d.Date);

                                await DeleteTransport(item, v_startDate, v_endDate);
                                await SetTransport(item, itms, request.FlightGroupMasterId, FlightGroupDetails, drivetransportMode, shiftDataDS);
                                await DeleteEmployeeStatusOldData(item, v_startDate, v_endDate, itms);

                                await SetRoom(item, itms, CurrentRoomId, currentEmployee.BedCount == null ? 1 : currentEmployee.BedCount.Value, item.RosterId,
                     item.DepartmentId, item.EmployerId, item.PositionId, item.CostCodeId, virtualRoom.Id);

                                savedEmployeeIds.Add(item.EmployeeId);


                                var currentProfile = await Context.Employee.Where(e => e.Id == currentEmployee.Id).FirstOrDefaultAsync();

                                if (currentProfile != null)
                                {
                                    currentProfile.RosterExecutedDate = DateTime.Now;
                                    currentProfile.RosterExecuteMonthDuration = request.DurationMonth;
                                    currentProfile.RosterExecuteLastDate = v_endDate;
                                    currentProfile.RosterId = item.RosterId;
                                    currentProfile.FlightGroupMasterId = request.FlightGroupMasterId;
                                    Context.Employee.Update(currentProfile);
                                }
                            }
                            else
                            {

                                skippedEmployeeIds.Add(item.EmployeeId);
                            }
                        }
                        else
                        {
                            skippedEmployeeIds.Add(item.EmployeeId);

                        }

                    }
                    else {
                        skippedEmployeeIds.Add(item.EmployeeId);
                    }
                }
                else {

                    skippedEmployeeIds.Add(item.EmployeeId);

                }

            }





            await Context.SaveChangesAsync();

            stopWatch.Stop();
      //      _logger.LogInformation($"endRoster ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            var returnDataRosterExecuted = new List<BulkRosterExecutedEmployees>();
            if (savedEmployeeIds.Count > 0)
            {


                var query = await (from transport in Context.Transport.AsNoTracking().Where(x => savedEmployeeIds.Contains(x.EmployeeId.Value) && x.EventDate >= request.StartDate && x.EventDate <= endDate.Date)

                                   join employee in Context.Employee.AsNoTracking() on transport.EmployeeId equals employee.Id into employeeData
                                   from employee in employeeData.DefaultIfEmpty()
                                   join schedule in Context.TransportSchedule on transport.ScheduleId equals schedule.Id into ScheduleData
                                   from schedule in ScheduleData.DefaultIfEmpty()
                                   join activeTransport in Context.ActiveTransport.AsNoTracking() on transport.ActiveTransportId equals activeTransport.Id into activeTransportData
                                   from activeTransport in activeTransportData.DefaultIfEmpty()
                                   join tmode in Context.TransportMode.AsNoTracking() on activeTransport.TransportModeId equals tmode.Id into modeData
                                   from tmoode in modeData.DefaultIfEmpty()
                                   select new BulkRosterExecutedEmployees
                                   {
                                       EmployeeId = employee.Id,
                                       FullName = $"{employee.Firstname} {employee.Lastname}",
                                       EventDate = transport.EventDate,
                                       EventDateTime = transport.EventDateTime,
                                       Direction = transport.Direction,
                                       TransportCode = activeTransport.Code,
                                       TransportMode = tmoode.Code,
                                       Description = schedule.Description,
                                       ScheduleId = schedule.Id,
                                       Seats = activeTransport.Seats,
                                       


                                   }).OrderBy(x => x.EventDate).ToListAsync();


                var rrshift = await (from localShift in Context.Shift.AsNoTracking().Where(x => x.Code == "RR")
                                     join color in Context.Color.AsNoTracking() on localShift.ColorId equals color.Id into colorData
                                     from color in colorData.DefaultIfEmpty()
                                     select new
                                     {
                                         descr = localShift.Code,
                                         colorCode = color.Code
                                     }).FirstOrDefaultAsync();

                var calculedScheduleIds = new List<int>();
                foreach (var item in query)
                {
                    if (item.ScheduleId.HasValue)
                    {
                        if (calculedScheduleIds.IndexOf(item.ScheduleId.Value) == -1)
                        {
                            await _transportScheduleCalculateRepository.CalculateByScheduleId(item.ScheduleId.Value, cancellationToken);
                            calculedScheduleIds.Add(item.ScheduleId.Value);
                        }
                        
                    }
                    

                    var scheduleConfirmedCount = await Context.Transport.AsNoTracking().Where(c => c.ScheduleId == item.ScheduleId && c.Status == "Confirmed").CountAsync();
                    var scheduleOverBookedCount = await Context.Transport.AsNoTracking().Where(c => c.ScheduleId == item.ScheduleId && c.Status == "Over Booked").CountAsync();


                    var shiftInf = await (from empStatus in Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == item.EmployeeId && x.RoomId != null && x.EventDate.Value == item.EventDate)
                                          join shift in Context.Shift.AsNoTracking() on empStatus.ShiftId equals shift.Id into shiftData
                                          from shift in shiftData.DefaultIfEmpty()
                                          join color in Context.Color.AsNoTracking() on shift.ColorId equals color.Id into colorData
                                          from color in colorData.DefaultIfEmpty()
                                          select new
                                          {
                                              descr = shift.Code,
                                              colorCode = color.Code
                                          }).FirstOrDefaultAsync();

                    //item.ShiftCode = shiftInf?.descr;
                    //item.ShiftColorCode = shiftInf?.colorCode;

                    item.ShiftCode = shiftInf != null ? shiftInf?.descr : rrshift?.descr;
                    item.ShiftColorCode = shiftInf != null ? shiftInf?.colorCode : rrshift.colorCode;

                    item.OverBooked = scheduleOverBookedCount;
                    item.Confirmed = scheduleConfirmedCount;
                }


                returnDataRosterExecuted = query;
            }


            stopWatch.Stop();
            _logger.LogInformation($"savedLogRosrter ==================> {stopWatch.ElapsedMilliseconds}");
            Console.WriteLine($"savedLogRosrter ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            var returnDataRosterSkipped = new List<BulkRosterSkippedEmployees>();

            if (skippedEmployeeIds.Count > 0)
            {

                var employees = await Context.Employee.AsNoTracking().Where(x => skippedEmployeeIds.Contains(x.Id)).Select(x => new { x.Id, x.Lastname, x.Firstname, x.SAPID }).ToListAsync();

                foreach (var item in employees)
                {
                    var employeeOnsiteStatusDates = await Context.EmployeeStatus.AsNoTracking()
                   .Where(x => x.EmployeeId == item.Id
                   && x.EventDate.Value.Date >= startDate.Date
                  && x.RoomId != null).FirstOrDefaultAsync();

                    if (employeeOnsiteStatusDates != null)
                    {
                        var firstINTransport = await Context.Transport.AsNoTracking()
                              .Where(x => x.EmployeeId == item.Id && x.EventDate.Value.Date <= startDate && x.Direction == "IN").OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();



                        var firstOUTTransport = await Context.Transport.AsNoTracking()
                             .Where(x => x.EmployeeId == item.Id && x.EventDate.Value.Date >= startDate && x.Direction == "OUT").OrderBy(x => x.EventDate).FirstOrDefaultAsync();

                        if (firstINTransport != null && firstOUTTransport != null)
                        {

                            var onsiteData = new BulkRosterSkippedEmployees
                            {
                                EmployeeId = item.Id,
                                FullName = $"{item.SAPID} {item.Firstname} {item.Lastname}",
                                EventDate = startDate,
                                InTransportDate = firstINTransport.EventDate,
                                OutTransportDate = firstOUTTransport.EventDate,
                                Status = $"{firstINTransport.Status}"

                            };

                            returnDataRosterSkipped.Add(onsiteData);


                        }

                    }

                }
            }
            stopWatch.Stop();
            _logger.LogInformation($"skippedLogRosrter ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();

            GC.Collect();  // Forces a collection of all generations
            GC.WaitForPendingFinalizers();


            return new BulkRosterExecuteResponse
            {
                RosterExecutedEmployees = returnDataRosterExecuted,
                RosterSkippedEmployees = returnDataRosterSkipped
            };
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //  await Task.CompletedTask;

        }



        private async Task<bool> CheckStartDateOnSite(int employeeId, DateTime eventDate)
        {
           var onsiteStatus = await Context.EmployeeStatus.AsNoTracking()
                .AnyAsync(x => x.EventDate.Value.Date == eventDate.Date && x.EmployeeId == employeeId && x.RoomId != null);

            return onsiteStatus;



        }


        private async Task<bool> CheckStartDateDirection(int employeeId, DateTime eventDate)
        {
            var lastTransportDirection = await Context.Transport.AsNoTracking()
                .Where(x => x.EventDate.Value.Date < eventDate && x.EmployeeId == employeeId).OrderByDescending(x => x.EventDateTime).FirstOrDefaultAsync();
            var employeeRosterStatus =await _employeeRepository.OnsiteCheckEmployeeByRoster(employeeId, eventDate);
            if (employeeRosterStatus)
            {
                if (lastTransportDirection != null)
                {
                    if (lastTransportDirection.Direction == "IN")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else {
                return false;
            }

        }


        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, DateTime endDate, int employeeId)
        {
            try
            {
                // Fetch the room data once
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roomId);

                if (currentRoom == null)
                {
                    return false;
                }

                if (currentRoom.VirtualRoom == 1)
                {
                    return true;
                }

                // Fetch all relevant employee statuses in the given date range for the specified room from the database
                var employeeStatuses = await Context.EmployeeStatus.AsNoTracking()
                    .Where(x => x.EventDate.Value.Date >= startDate.Date && x.EventDate <= endDate.Date && x.RoomId == roomId && x.EmployeeId != employeeId)
                    .ToListAsync();

                // Fetch local (unsaved) changes
                var localChanges = Context.EmployeeStatus.Local
                    .Where(x => x.EventDate.HasValue && x.EventDate.Value.Date >= startDate.Date && x.EventDate.Value.Date <= endDate.Date && x.RoomId == roomId && x.EmployeeId != employeeId)
                    .ToList();

                // Combine local changes with data from the database
                employeeStatuses.AddRange(localChanges);

                // Fetch transport data for the relevant employees
                var employeeIds = employeeStatuses.Select(x => x.EmployeeId).Distinct().ToList();
                var transportData = await Context.Transport.AsNoTracking()
                    .Where(x => employeeIds.Contains(x.EmployeeId))
                    .ToListAsync();

                var currentDate = startDate;
                while (currentDate <= endDate)
                {
                    var dateCountRoom = employeeStatuses.Count(x => x.EventDate.Value.Date == currentDate.Date);

                    if (currentRoom.BedCount <= dateCountRoom)
                    {
                        if (currentDate == startDate)
                        {
                            var onsiteEmployees = employeeStatuses
                                .Where(x => x.EventDate.Value.Date == currentDate.Date)
                                .Select(x => x.EmployeeId)
                                .Distinct()
                                .ToList();

                            var tomorrowOut = transportData
                                .Where(x => x.EventDate.Value.Date == currentDate.AddDays(1).Date && onsiteEmployees.Contains(x.EmployeeId) && x.Direction == "OUT")
                                .ToList();

                            if (!tomorrowOut.Any())
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    currentDate = currentDate.AddDays(1);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return false;
            }
        }

   


        private async Task<bool> CheckAnnualYear(BulkRosterExecuteEmployee employee, List<DateTime> rosterDates) 
        {
            var data = await Context.EmployeeStatus
              .Where(x => x.RoomId == null && rosterDates.Contains(x.EventDate.Value.Date) && x.EmployeeId == employee.EmployeeId)
              .ToListAsync();

            foreach (var item in data)
            {
                var currentShift = await Context.Shift
                    .Where(x => x.Id == item.ShiftId)
                    .Select(x => new { x.Id, x.Code, x.OnSite, x.Description })
                    .FirstOrDefaultAsync();

                if (currentShift?.Code == "AN")
                {
                    return false;
                }

            }

            return true;
        }


        private async Task DeleteTransport(BulkRosterExecuteEmployee employee, DateTime startDate, DateTime endDate)
        {
            var DeleteData = await Context.Transport
                .Where(x => x.EmployeeId == employee.EmployeeId
                    && x.EventDate.Value.Date >= startDate/* 
                    && x.EventDate.Value.Date <= endDate*/).ToListAsync();

            DeleteData.ForEach(x => {
                x.ChangeRoute = "RosterExecute deleted";
                x.UserIdDeleted = _hTTPUserRepository.LogCurrentUser()?.Id;
                x.DateDeleted = DateTime.Now;
            });


            Context.Transport.RemoveRange(DeleteData);

            await Task.CompletedTask;

        }

        private async Task SetTransport(BulkRosterExecuteEmployee employee, List<BulkRosterActiveDate> activeDates, int flightGroupMasterId, List<FlightGroupDetail> FlightGroupDetails, TransportMode mode, List<Shift> shiftData)
        {

            var activeDateChanged = new List<BulkRosterActiveDate>();
            string beforeDirection = "";


            foreach (var item in activeDates)
            {
                if (beforeDirection != item.Direction)
                {
                    activeDateChanged.Add(item);
                }

                beforeDirection = item.Direction;

            }

            var newRecords = new List<Transport>();
            foreach (var item in activeDateChanged)
            {


                var currentCluster = (FlightGroupDetail)null;
                if (item.Direction == "IN")
                {

                    if (Transliterator.GetWeekNumber(item.EventDate) % 2 == 0)
                    {
                        if (shiftData.FirstOrDefault(x => x.Id == item.ShiftId)?.Code == "DS")
                        {
                            var evenShiftDataDS = shiftData.FirstOrDefault(x => x.Code == "DS");

                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                x.Direction == item.Direction && x.ShiftId == evenShiftDataDS?.Id &&
                                x.Active == 1 &&
                                x.DayNum == item.EventDate.ToString("dddd"));

                        }
                        else
                        {
                            var evenShiftDataNS = shiftData.FirstOrDefault(x => x.Code == "NS");

                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                x.Direction == item.Direction && x.ShiftId == evenShiftDataNS?.Id &&
                                x.Active == 1 &&
                                x.DayNum == item.EventDate.ToString("dddd"));
                        }

                    }
                    else {


                        if (shiftData.FirstOrDefault(x => x.Id == item.ShiftId)?.Code == "DS")
                        {
                            var oddShiftData = shiftData.FirstOrDefault(x => x.Code == "D2");

                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                             x.Direction == item.Direction && x.ShiftId == oddShiftData?.Id &&
                             x.Active == 1 &&
                             x.DayNum == item.EventDate.ToString("dddd"));
                        }
                        if (shiftData.FirstOrDefault(x => x.Id == item.ShiftId)?.Code == "NS")
                        {
                            var oddShiftData = shiftData.FirstOrDefault(x => x.Code == "N2");


                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                               x.Direction == item.Direction && x.ShiftId == oddShiftData?.Id &&
                               x.Active == 1 &&
                               x.DayNum == item.EventDate.ToString("dddd"));
                        }



                    }



                }
                else
                {
                    var beforeData = activeDateChanged.Where(x => x.EventDate < item.EventDate && x.OnSite == 1).OrderByDescending(x => x.EventDate).FirstOrDefault();


                    if (Transliterator.GetWeekNumber(item.EventDate) % 2 == 0)
                    {

                        if (beforeData != null)
                        {
                            //currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                            //x.Direction == item.Direction && x.ShiftId == beforeData.ShiftId
                            //&& x.Active == 1 &&
                            //x.DayNum == item.EventDate.ToString("dddd"));

                            if (shiftData.FirstOrDefault(x => x.Id == beforeData.ShiftId)?.Code == "DS")
                            {
                                var evenShiftDataDS = shiftData.FirstOrDefault(x => x.Code == "DS");

                                currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                    x.Direction == item.Direction && x.ShiftId == evenShiftDataDS?.Id &&
                                    x.Active == 1 &&
                                    x.DayNum == item.EventDate.ToString("dddd"));

                            }
                            else
                            {
                                var evenShiftDataNS = shiftData.FirstOrDefault(x => x.Code == "NS");

                                currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                    x.Direction == item.Direction && x.ShiftId == evenShiftDataNS?.Id &&
                                    x.Active == 1 &&
                                    x.DayNum == item.EventDate.ToString("dddd"));
                            }
                        }
                        else
                        {
                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                x.Direction == item.Direction
                                && x.Active == 1 &&
                                x.DayNum == item.EventDate.ToString("dddd"));
                        }
                    }
                    else {
                        if (beforeData != null)
                        {
                            if (shiftData.FirstOrDefault(x => x.Id == beforeData?.ShiftId)?.Code == "DS")
                            {
                                var oddShiftData = shiftData.FirstOrDefault(x => x.Code == "D2");

                                currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                 x.Direction == item.Direction && x.ShiftId == oddShiftData?.Id &&
                                 x.Active == 1 &&
                                 x.DayNum == item.EventDate.ToString("dddd"));
                            }
                            if (shiftData.FirstOrDefault(x => x.Id == beforeData?.ShiftId)?.Code == "NS")
                            {
                                var oddShiftData = shiftData.FirstOrDefault(x => x.Code == "N2");
                                currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                   x.Direction == item.Direction && x.ShiftId == oddShiftData?.Id &&
                                   x.Active == 1 &&
                                   x.DayNum == item.EventDate.ToString("dddd"));
                            }
                        }
                        else {
                            var oddShiftData = shiftData.FirstOrDefault(x => x.Code == "N2");
                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                               x.Direction == item.Direction && x.ShiftId == oddShiftData?.Id &&
                               x.Active == 1 &&
                               x.DayNum == item.EventDate.ToString("dddd"));
                        }
                    }



                }




                if (currentCluster != null && currentCluster.ClusterId.HasValue)
                {
                    var transportData = await GetActiveTransportData(currentCluster.ClusterId.Value, item.EventDate.Date);

                    if (transportData.EventDateTime == DateTime.MinValue)
                    {

                        transportData = await GetDriveTransport(item.EventDate.Date, item.Direction, mode);

                        var transport = new Transport
                        {
                            Active = 1,
                            DepId = employee.DepartmentId,
                            CostCodeId = employee.CostCodeId,
                            ActiveTransportId = transportData.ActiveTransportId,
                            Direction = item.Direction,
                            EmployerId = employee.EmployerId,
                            EmployeeId = employee.EmployeeId,
                            DateCreated = DateTime.Now,
                            ChangeRoute = "Bulk roster executed",
                            ScheduleId = transportData.ScheduleId,
                            Status = transportData.Status,
                            EventDate = transportData.EventDateTime.Date,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            EventDateTime = transportData.EventDateTime
                            


                        };

                        newRecords.Add(transport);

                    }
                    else
                    {

                        var transport = new Transport
                        {
                            Active = 1,
                            DepId = employee.DepartmentId,
                            CostCodeId = employee.CostCodeId,
                            ActiveTransportId = transportData.ActiveTransportId,
                            Direction = item.Direction,
                            EmployeeId = employee.EmployeeId,
                            EmployerId = employee.EmployerId,
                            DateCreated = DateTime.Now,
                            ChangeRoute = "Bulk roster executed",
                            //   PositionId = employee.PositionId,
                            ScheduleId = transportData.ScheduleId,
                            Status = transportData.Status,
                            EventDate = transportData.EventDateTime.Date,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            EventDateTime = transportData.EventDateTime,


                        };

                        newRecords.Add(transport);

                    }
                }
                else
                {

                    /*Drive Transport Set*/

                    var driveTransportMode = await Context.TransportMode.AsNoTracking().Where(x => x.Code.ToLower() == "drive").FirstOrDefaultAsync();
                    if (driveTransportMode != null)
                    {
                        var activeTransportDriveIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.TransportModeId == driveTransportMode.Id && x.DayNum == item.EventDate.ToString("dddd")
                        && x.Direction == item.Direction && x.Active == 1
                        ).Select(x => x.Id).ToListAsync();
                        if (activeTransportDriveIds.Count > 0)
                        {
                            var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => activeTransportDriveIds.Contains(x.ActiveTransportId) && x.EventDate.Date == item.EventDate.Date && x.Active == 1).FirstOrDefaultAsync();
                            if (currentSchedule != null)
                            {



                                string ETD = currentSchedule.ETD.Replace(":", "");


                                string timePattern = @"^(?:[01]\d|2[0-3])[0-5]\d$";

                                bool isValidTimeETD = Regex.IsMatch(ETD, timePattern);
                                DateTime schedulETD = DateTime.ParseExact(isValidTimeETD == true ? ETD : "0000", "HHmm", CultureInfo.InvariantCulture);
                                int ETDhours = schedulETD.Hour;
                                int ETDminutes = schedulETD.Minute;

                                var transport = new Transport
                                {
                                    Active = 1,
                                    DepId = employee.DepartmentId,
                                    CostCodeId = employee.CostCodeId,
                                    ActiveTransportId = currentSchedule.ActiveTransportId,
                                    Direction = item.Direction,
                                    EmployerId = employee.EmployerId,
                                    EmployeeId = employee.EmployeeId,
                                    DateCreated = DateTime.Now,
                                    ChangeRoute = "Bulk roster executed",
                                    //   PositionId = employee.PositionId,
                                    ScheduleId = currentSchedule.Id,
                                    Status = "Confirmed",
                                    EventDate = item.EventDate.Date,
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                    EventDateTime = currentSchedule.EventDate.AddHours(schedulETD.Hour).AddMinutes(schedulETD.Minute),


                                };

                                newRecords.Add(transport);
                            }
                            else
                            {
                                throw new BadRequestException($"Schedule not found at {item.EventDate} and {item.Direction} direction");
                            }

                        }
                        else
                        {
                            throw new BadRequestException($"Schedule not found at {item.EventDate} and {item.Direction} direction");
                        }
                    }

                }
            }

            Context.Transport.AddRange(newRecords);
            await Task.CompletedTask;


        }







        private async Task<ActiveTransportRetData> GetDriveTransport(DateTime eventDate, string direction, TransportMode transportMode)
        {
            var returnData = new ActiveTransportRetData();
            if (transportMode != null)
            {
                var currentTransportIds = await Context.ActiveTransport.AsNoTracking()
                     .Where(x => x.DayNum == eventDate.ToString("dddd")
                     && x.TransportModeId == transportMode.Id && x.Direction == direction && x.Active == 1).Select(x => x.Id)
                      .ToListAsync();
                if (currentTransportIds.Count > 0)
                {
                    var currentSchedule = await Context.TransportSchedule.AsNoTracking()
                        .Where(x => currentTransportIds.Contains(x.ActiveTransportId) && x.EventDate.Date == eventDate.Date && x.Active == 1)

                        .FirstOrDefaultAsync();


                    if (currentSchedule != null)
                    {
                        TimeSpan time = TimeSpan.ParseExact(currentSchedule.ETD, "hhmm", CultureInfo.InvariantCulture);
                        DateTime combinedDateTime = eventDate.AddHours(time.Hours).AddMinutes(time.Minutes);

                        returnData.ActiveTransportId = currentSchedule.ActiveTransportId;
                        returnData.EventDateTime = combinedDateTime;
                        returnData.ScheduleId = currentSchedule.Id;
                        returnData.Status = "Confirmed";

                        return returnData;
                    }
                    else
                    {
                        throw new BadRequestException($"{eventDate.ToString("yyyy-MM-dd")} Tranport schedule is not available.");
                    }
                }
                else
                {
                    throw new BadRequestException($"{eventDate.ToString("yyyy-MM-dd")} Transport  is not available.");
                }

            }
            else
            {
                throw new BadRequestException($"{eventDate.ToString("yyyy-MM-dd")} Drive Transport  is not available.");
            }

            return returnData;
        }


        private async Task<ActiveTransportRetData> GetActiveTransportData(int ClusterId, DateTime eventDate)
        {

            ActiveTransportRetData returnActiveTransportData = new ActiveTransportRetData
            {
                ActiveTransportId = 0,
                EventDateTime = DateTime.MinValue,
                ScheduleId = null,
                Status = "Confirmed"
            };

            var clusterDetails = await Context.ClusterDetail.AsNoTracking().Where(x => x.ClusterId == ClusterId && x.ActiveTransportId != null).OrderBy(x => x.SeqNumber).ToListAsync();

            if (clusterDetails.Count == 0)
            {
                var currentCluster = await Context.Cluster.AsNoTracking().Where(x => x.Id == ClusterId && x.Active == 1).FirstOrDefaultAsync();
                if (currentCluster != null)
                {


                    throw new BadRequestException(@$"{currentCluster.Code} {currentCluster.DayNum} {currentCluster.Direction} direction cluster detail does not register the required transport information. Unable to create roster ");
                }
                else
                {
                    throw new BadRequestException("Please register cluster");
                }
            }


            var ActiveTransportIds = clusterDetails.Select(x => x.ActiveTransportId).ToList();


            var transportSchedules = await Context.TransportSchedule.AsNoTracking().Where(x =>
                   ActiveTransportIds.Contains(x.ActiveTransportId) &&
                    x.EventDate.Date == eventDate && x.Active == 1
                ).ToListAsync();


            foreach (var detail in clusterDetails)
            {
                var transportSchedule = transportSchedules.FirstOrDefault(x => x.ActiveTransportId == detail.ActiveTransportId);
                if (transportSchedule != null)
                {
                    TimeSpan time = TimeSpan.ParseExact(transportSchedule.ETD, "hhmm", CultureInfo.InvariantCulture);
                    DateTime combinedDateTime = eventDate.AddHours(time.Hours).AddMinutes(time.Minutes);
                    var transportCount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == transportSchedule.Id);
                    var transporLocalCount = Context.Transport.Local.Count(x => x.ScheduleId == transportSchedule.Id);


                    if (transportCount + transporLocalCount < transportSchedule.Seats)
                    {

                        returnActiveTransportData.ActiveTransportId = transportSchedule.ActiveTransportId;
                        returnActiveTransportData.EventDateTime = combinedDateTime;
                        returnActiveTransportData.ScheduleId = transportSchedule.Id;
                        returnActiveTransportData.Status = "Confirmed";

                        return returnActiveTransportData;
                    }
                }
            }


            //  return returnActiveTransportData;

            //TODO 2025-02-05 Cluster дүүрсэн тохиолдолд эхний онгоцруу хийдэг байсан хэсггийг drive руу оруулдаг болгов

            var transportScheduleFirst = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x =>
                    x.ActiveTransportId == clusterDetails[0].ActiveTransportId &&
                    x.EventDate.Date == eventDate.Date);


            var transportScheduleS = transportSchedules.FirstOrDefault(x => x.ActiveTransportId == clusterDetails[0].ActiveTransportId);
            if (transportScheduleS != null)
            {
                TimeSpan time = TimeSpan.ParseExact(transportScheduleS.ETD, "hhmm", CultureInfo.InvariantCulture);
                DateTime combinedDateTime = eventDate.AddHours(time.Hours).AddMinutes(time.Minutes);

                if (transportScheduleS != null)
                {

                    var activeTransportOV = await (from activetransportData in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == transportScheduleS.ActiveTransportId)
                                                   join transportMode in Context.TransportMode
                                                           on activetransportData.TransportModeId equals transportMode.Id into transportModeData
                                                   from transportMode in transportModeData.DefaultIfEmpty()
                                                   select new
                                                   {
                                                       id = transportMode.Id,
                                                       Mode = transportMode.Code
                                                   }).FirstOrDefaultAsync();
                    if (activeTransportOV != null)
                    {
                        returnActiveTransportData.ActiveTransportId = transportScheduleS.ActiveTransportId;
                        returnActiveTransportData.EventDateTime = combinedDateTime;
                        returnActiveTransportData.ScheduleId = transportScheduleS.Id;
                        returnActiveTransportData.Status = activeTransportOV.Mode.ToLower() == "drive" ? "Confirmed" : "Over Booked";
                    }
                    else
                    {
                        returnActiveTransportData.ActiveTransportId = transportScheduleS.ActiveTransportId;
                        returnActiveTransportData.EventDateTime = combinedDateTime;
                        returnActiveTransportData.ScheduleId = transportScheduleS.Id;
                        returnActiveTransportData.Status = "Over Booked";
                    }



                }
                return returnActiveTransportData;
            }
            else
            {
                return returnActiveTransportData;
            }



        }




        private async Task SetRoom(BulkRosterExecuteEmployee employee, List<BulkRosterActiveDate> activeDates, int RoomId, int bedCount, int? RosterId, int? departmentId, int? employerId, int? positionId, int? costCodeId, int virtualRoomId)
        {

           // var currentRoom =await Context.Room.Where(x => x.Id == RoomId).Select(x => new { x.Id, x.BedCount }).FirstOrDefaultAsync();
            var newRecords = new List<EmployeeStatus>();
            foreach (var item in activeDates)
            {
                if (item.OnSite == 1)
                {
                    if (item.details != null) {
                        foreach (var detail in item.details) 
                        {
                            int bedId = 0;
                            if (virtualRoomId != detail.RoomId)
                            {
                                bedId = await GetBedId(detail.EventDate.Date, detail.RoomId.Value, employee.EmployeeId);

                            }

                            var currentData = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date == detail.EventDate.Date && x.EmployeeId == employee.EmployeeId).FirstOrDefaultAsync();
                            if (currentData == null)
                            {
                                var newData = new EmployeeStatus
                                {
                                    Active = 1,
                                    CostCodeId = costCodeId,
                                    DepId = departmentId,
                                    EventDate = detail.EventDate,
                                    RoomId = detail.RoomId,
                                    EmployerId = employerId,
                                    BedId = bedId == 0 ? null : bedId,
                                    ShiftId = detail.ShifId,
                                    RosterMasterId = RosterId,
                                    PositionId = positionId,
                                    DateCreated = DateTime.Now,
                                    ChangeRoute = "Bulk roster executed",
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                    EmployeeId = employee.EmployeeId
                                };

                                newRecords.Add(newData);

                            }
                            else {
                                currentData.Active = 1;
                                currentData.CostCodeId = costCodeId;
                                currentData.DepId = departmentId;
                                currentData.EventDate = detail.EventDate;
                                currentData.ShiftId = detail.ShifId;
                                currentData.RoomId = detail.RoomId;
                                currentData.BedId  = bedId == 0 ? null : bedId;
                                currentData.EmployerId = employerId;
                                currentData.RosterMasterId = RosterId;
                                currentData.ChangeRoute = $"Roster executed from profile";
                                currentData.DateCreated = DateTime.Now;
                                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                currentData.EmployeeId = employee.EmployeeId;
                                Context.EmployeeStatus.Update(currentData);
                            }
                            
                        }
                    }

                }
                else {
                    if (item.details != null)
                    {
                        foreach (var detail in item.details)
                        {

                            var currentData = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date == detail.EventDate.Date && x.EmployeeId == employee.EmployeeId).FirstOrDefaultAsync();
                            if (currentData == null)
                            {

                                var newData = new EmployeeStatus
                                {
                                    Active = 1,
                                    CostCodeId = costCodeId,
                                    DepId = departmentId,
                                    EventDate = detail.EventDate,
                                    RoomId = null,
                                    BedId = null,
                                    PositionId = positionId,
                                    RosterMasterId = RosterId,
                                    EmployerId = employerId,
                                    ShiftId = detail.ShifId,
                                    ChangeRoute = $"Roster executed from profile",
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                    EmployeeId = employee.EmployeeId,

                                };
                                newRecords.Add(newData);
                            }
                            else
                            {
                                currentData.Active = 1;
                                currentData.CostCodeId = costCodeId;
                                currentData.DepId = departmentId;
                                currentData.EventDate = detail.EventDate;
                                currentData.ShiftId = detail.ShifId;
                                currentData.RoomId = null;
                                currentData.BedId = null;
                                currentData.EmployerId = employerId;
                                currentData.RosterMasterId = RosterId;
                                currentData.ChangeRoute = $"Roster executed from profile";
                                currentData.DateCreated = DateTime.Now;
                                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                currentData.EmployeeId = employee.EmployeeId;
                                Context.EmployeeStatus.Update(currentData);
                            }

                        }
                    }

                }
            }

            Context.EmployeeStatus.AddRange(newRecords);


            await Task.CompletedTask;
        }




        private async Task<int> GetBedId(DateTime eventDate, int roomId, int employeeId)
        {
            var dateRoomEmployees = await Context.EmployeeStatus.AsNoTracking()
              .Where(x => x.EventDate.Value.Date == eventDate.Date && x.RoomId == roomId)
              .Select(x => x.BedId)
              .ToListAsync();

            var activeBedId = await Context.Bed.AsNoTracking()
                .Where(x => x.RoomId == roomId && !dateRoomEmployees.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (activeBedId != 0)
                return activeBedId;

            return await Context.Bed.AsNoTracking()
                .Where(x => x.RoomId == roomId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();


          
        }

        private async Task DeleteEmployeeStatusOldData(BulkRosterExecuteEmployee employee, DateTime startDate, DateTime endDate, List<BulkRosterActiveDate> itms)
        {
            var oldData =await Context.EmployeeStatus
                .Where(x => x.EventDate.Value.Date >= startDate 
                    && x.EmployeeId == employee.EmployeeId
                    )
                .ToListAsync();


            oldData.ForEach(x => {
                x.ChangeRoute = "RosterExecute deleted";
                x.UserIdDeleted = _hTTPUserRepository.LogCurrentUser()?.Id;
                x.DateDeleted = DateTime.Now;
            });


            Context.EmployeeStatus.RemoveRange(oldData);
        }

        private List<DateTime> EmployeeOnsiteDates(List<BulkRosterActiveDate> activeDates)
        {
            var returnData = new List<DateTime>();
            for (int i = 0; i < activeDates.Count; i++)
            {
                 var items =   activeDates[i].details;
                if (items != null) {
                    foreach (var item in items)
                    {
                        if (item.OnSite == 1)
                        {
                            returnData.Add(item.EventDate);
                        }
                    }
                }

            }

            return returnData;

        }


       


        private class ObjectDates
        {
            public int DayCycle { get; set; }

            public DateTime EventDate { get; set; }
        }



        private class BulkRosterActiveDate
        {

            public DateTime EventDate { get; set; }

            public string Direction { get; set; }

            public int ShiftId { get; set; }

            public  int OnSite  { get; set; }

            public List<BulkRosterActiveDateDetail> details { get; set; } 



        }


        public class BulkRosterActiveDateDetail
        {
            public DateTime EventDate { get; set; }

            public int OnSite { get; set; }

            public int ShifId { get; set; }

            public int? RoomId { get; set; }
        }

        private class ActiveTransportRetData
        {
            public int ActiveTransportId { get; set; }

            public DateTime EventDateTime { get; set; }

            public int? ScheduleId { get; set; }

            public string Status { get; set; }

        }

        #endregion


    }

}
