
using FluentValidation.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeFeature.CheckADAccountEmployee;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.EmployeeFeature.RosterExecuteEmployee;
using tas.Application.Features.EmployeeFeature.RosterExecutePreviewEmployee;
using tas.Application.Features.RosterExecuteFeature.BulkRosterExecute;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;
namespace tas.Persistence.Repositories
{


    public partial class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {

        #region RosterPreview


        public async Task<RosterExecutePreviewEmployeeResponse> RosterExecuteRequest(RosterExecutePreviewEmployeeRequest request, CancellationToken cancellationToken)
        {

            DateTime startDate = request.StartDate;
            DateTime endDate = startDate.AddMonths(request.MonthDuration);

            //var fromStartLocation = await Context.Location.FirstOrDefaultAsync(x => x.Id == request.LocationId);
            //if (fromStartLocation?.onSite == 1)
            //{
            //    new BadRequestException("The location information you submitted is incorrect.");
            //}
            var data = await Context.RosterDetail.Where(x => x.RosterId == request.RosterId && x.Active == 1).OrderBy(x => x.SeqNumber).ToListAsync();

            var eventDates = await CheckSchedule(startDate, endDate, request.RosterId,/* request.LocationId,*/ request.FlightGroupMasterId);
            var errorDates = eventDates.Where(x => x.ShiftId == null || x.ClusterId == null).ToList();

            var oldStatusDates = await GetEmployeeStatusDates(request.EmployeeId, startDate, endDate);
            var newStatusDates = EmployeeRoomRosterSetPreview(request, eventDates);
            var EmployeeOffsiteStatusData = await EmployeeOffsiteStatus(request);


            var rosterStartDateOnsite = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == request.StartDate.Date && x.RoomId != null).FirstOrDefaultAsync();

            if (rosterStartDateOnsite != null)
            {
                var firstINTransport = await Context.Transport
                        .Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date <= startDate && x.Direction == "IN").OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();


                var firstOUTTransport = await Context.Transport
                     .Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date >= startDate && x.Direction == "OUT").OrderBy(x => x.EventDate).FirstOrDefaultAsync();

                var employeeOnsiteStatusDates = await Context.EmployeeStatus
                     .Where(x => x.EmployeeId == request.EmployeeId
                     && x.EventDate >= firstINTransport.EventDate.Value.Date
                     && x.EventDate < firstOUTTransport.EventDate.Value.Date).ToListAsync();


                var onsiteData = new OnsiteData
                {
                    EventDate = startDate,
                    InTransportDate = firstINTransport.EventDate,
                    OutTransportDate = firstOUTTransport.EventDate,
                    Status = $"{firstINTransport.Status}"

                };

                return new RosterExecutePreviewEmployeeResponse
                {
                    newStatusDates = newStatusDates,
                    oldStatusDates = oldStatusDates,
                    EmployeeOffSiteStatus = EmployeeOffsiteStatusData,
                    OnsiteData = onsiteData
                };


            }
            else
            {
                return new RosterExecutePreviewEmployeeResponse
                {
                    newStatusDates = newStatusDates,
                    oldStatusDates = oldStatusDates,
                    EmployeeOffSiteStatus = EmployeeOffsiteStatusData
                };
            }







        }

        #endregion


        #region RosterExecute

        public async Task<List<RosterExecuteEmployeeResponse>> RosterExecute(RosterExecuteEmployeeRequest request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate.Date;
            if (request.MonthDuration > 18)
            {
                throw new BadRequestException("Month duration is too long.");
            }

            var endDate = startDate.AddMonths(request.MonthDuration);
            var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
            if (virtualRoom == null)
            {
                throw new BadRequestException("Virtual room is not registered. Please contact the admin team for no action");
            }

            await ValidDateRoster(request.RosterId);


            var d2Shift = await Context.Shift.AsNoTracking().Where(x => x.Code == "D2").FirstOrDefaultAsync();
            var n2Shift = await Context.Shift.AsNoTracking().Where(x => x.Code == "N2").FirstOrDefaultAsync();

            var dsShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "DS").FirstOrDefaultAsync();
            var nsShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "NS").FirstOrDefaultAsync();

            if (d2Shift == null || n2Shift == null || dsShift == null || nsShift == null)
            {
                throw new BadRequestException("Remember to register your D2 (Day Shift 2) and N2 (Night Shift 2) shift");
            }


            var shiftDataDS = new List<Shift>();
            shiftDataDS.Add(dsShift);
            shiftDataDS.Add(nsShift);
            shiftDataDS.Add(n2Shift);
            shiftDataDS.Add(d2Shift);

            var rosterExecuteStatus = await OnsiteCheckEmployeeByRoster(request.EmployeeId, request.StartDate);
            if (!rosterExecuteStatus)
            {
                throw new BadRequestException("This roster cannot be executed because it is restricted by Roster rules. Please review the Roster requirements or contact the administrator for assistance.");
            }
            //else {
            //    throw new BadRequestException("Success");
            //}

            //var rosterDetails =await Context.RosterDetail.Where(x => x.RosterId == request.RosterId && x.Active == 1).OrderBy(x => x.SeqNumber).ToListAsync();
            //if (rosterDetails.Count == 0)
            //{
            //    throw new BadRequestException("Roster data is missing");
            //}
            int CurrentRoomId = virtualRoom.Id;

            var onsiteDateStatus = await CheckStartDateOnSite(request.EmployeeId, startDate);
            if (!onsiteDateStatus)
            {

                await CheckStartDateDirection(request.EmployeeId, startDate);
                var currentEmployee = await Context.Employee.Where(x => x.Id == request.EmployeeId && x.Active == 1).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {

                    var rosterDetail = await Context.RosterDetail.AsNoTracking().Where(x => x.RosterId == request.RosterId).ToListAsync();
                    var itms = await EmployeeActiveDates(startDate, endDate, rosterDetail, currentEmployee.RoomId, virtualRoom.Id, request.EmployeeId);
                    var employeeOnSiteDates = EmployeeOnsiteDates(itms);

                    List<DateTime> sortedDates = employeeOnSiteDates.OrderBy(d => d.Date).ToList();

                    var v_startDate = employeeOnSiteDates.Min(d => d.Date);
                    var v_endDate = employeeOnSiteDates.Max(d => d.Date);

                    var vv_startDate = itms.Min(x => x.EventDate);
                    var vv_endDate = itms.Max(x => x.EventDate);
                    //var v_startDate = sortedDates.First().Date;
                    //var v_endDate = sortedDates.Last().Date;
                    //var vv_startDate = itms.OrderBy(x => x.EventDate).First().EventDate;
                    //var vv_endDate = itms.OrderBy(x => x.EventDate).Last().EventDate;



                    await DeleteTransport(request.EmployeeId, vv_startDate);
                    await SetTransport(request, itms, request.FlightGroupMasterId, shiftDataDS);
                    await DeleteEmployeeStatusOldData(request, v_startDate, v_endDate, itms);


                    await SetRoom(request, itms, request.RosterId, request.DepartmentId, request.EmployerId, request.PositionId, request.CostCodeId, virtualRoom.Id);


                    currentEmployee.RosterId = request.RosterId;

                    currentEmployee.DateUpdated = DateTime.Now;

                    currentEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    currentEmployee.RosterExecutedDate = DateTime.Now;
                    currentEmployee.RosterExecuteMonthDuration  = request.MonthDuration;
                    currentEmployee.RosterExecuteLastDate = vv_endDate;
                    currentEmployee.FlightGroupMasterId = request.FlightGroupMasterId;


                    Context.Employee.Update(currentEmployee);


                    await Context.SaveChangesAsync();





                    var returnData = new List<RosterExecuteEmployeeResponse>();
                    var query = await (from transport in Context.Transport.Where(x => x.EmployeeId == request.EmployeeId
                                       && x.EventDate.Value.Date >= startDate.Date && x.EventDate.Value.Date <= endDate.Date)

                                       join employee in Context.Employee.AsNoTracking() on transport.EmployeeId equals employee.Id into employeeData
                                       from employee in employeeData.DefaultIfEmpty()
                                       join schedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals schedule.Id into ScheduleData
                                       from schedule in ScheduleData.DefaultIfEmpty()
                                       join activeTransport in Context.ActiveTransport.AsNoTracking() on transport.ActiveTransportId equals activeTransport.Id into activeTransportData
                                       from activeTransport in activeTransportData.DefaultIfEmpty()
                                       join tmode in Context.TransportMode.AsNoTracking() on activeTransport.TransportModeId equals tmode.Id into modeData

                                       from tmoode in modeData.DefaultIfEmpty()

                                       select new RosterExecuteEmployeeResponse
                                       {
                                           EmployeeId = employee.Id,
                                           FirstName = employee.Firstname,
                                           LastName = employee.Lastname,
                                           EventDate = transport.EventDate,
                                           EventDateTime = transport.EventDateTime,
                                           Direction = transport.Direction,
                                           TransportCode = activeTransport.Code,
                                           TransportMode = tmoode.Code,
                                           Description = schedule.Description,
                                           Seats = activeTransport.Seats,
                                           ScheduleId = schedule.Id,



                                       }).OrderBy(x => x.EventDate).ToListAsync();

                    var rrshift = await (from localShift in Context.Shift.AsNoTracking().Where(x => x.Code == "RR")
                                         join color in Context.Color.AsNoTracking() on localShift.ColorId equals color.Id into colorData
                                         from color in colorData.DefaultIfEmpty()
                                         select new
                                         {
                                             descr = localShift.Code,
                                             colorCode = color.Code
                                         }).FirstOrDefaultAsync();

                    foreach (var item in query)
                    {


                        var scheduleConfirmedCount = await Context.Transport.AsNoTracking().Where(c => c.ScheduleId == item.ScheduleId && c.Status == "Confirmed").CountAsync();
                        var scheduleOverBookedCount = await Context.Transport.AsNoTracking().Where(c => c.ScheduleId == item.ScheduleId && c.Status == "Over Booked").CountAsync();


                        var shiftInf = await (from empStatus in Context.EmployeeStatus.Where(x => x.EmployeeId == item.EmployeeId && x.RoomId != null && x.EventDate.Value == item.EventDate)
                                              join shift in Context.Shift.AsNoTracking() on empStatus.ShiftId equals shift.Id into shiftData
                                              from shift in shiftData.DefaultIfEmpty()
                                              join color in Context.Color on shift.ColorId equals color.Id into colorData
                                              from color in colorData.DefaultIfEmpty()
                                              select new
                                              {
                                                  descr = shift.Code,
                                                  colorCode = color.Code
                                              }).FirstOrDefaultAsync();

                        item.ShiftCode = shiftInf != null ? shiftInf?.descr : rrshift?.descr;
                        item.ShiftColorCode = shiftInf != null ? shiftInf?.colorCode : rrshift.colorCode;
                        item.OverBooked = scheduleOverBookedCount;
                        item.Confirmed = scheduleConfirmedCount;
                    }


                    return query;
                }
                else
                {
                    throw new BadRequestException("Employee not active");
                }


            }
            else
            {
                string errorMessage = $"This employee is onsite on {startDate.ToString("yyyy-MM-dd")} Unable to execute Roster";
                throw new BadRequestException(errorMessage);
            }






        }




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
                                throw new BadRequestException("Roster detail shift is not valid");
                            }
                        }
                        else
                        {
                            throw new BadRequestException("Roster detail shift is not valid");
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Roster detail is not valid");
                    }


                }
                else
                {
                    throw new BadRequestException("Roster detail is not valid");
                }
            }
            else
            {
                throw new BadRequestException("Roster not found");
            }
        }

        private async Task<int?> GetRoomGuestId(int roomId, List<DateTime> OnsiteDates)
        {
            var roomEmpoyees = await Context.EmployeeStatus
                .Where(x => x.RoomId == roomId && OnsiteDates.Contains(x.EventDate.Value.Date)).Select(x => x.EmployeeId)
                .Distinct().ToListAsync();

            foreach (var item in roomEmpoyees)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == item).Select(x => new { x.RoomId }).FirstOrDefaultAsync();
                if (currentEmployee?.RoomId != roomId)
                {
                    return item;
                }

            }
            return null;

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
                    var dateCountRoom = await Context.EmployeeStatus.Where(x=> x.EmployeeId != EmployeeId).AsNoTracking()
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


        private async Task SetRoomVirtual(RosterExecuteEmployeeRequest employee, List<BulkRosterActiveDate> activeDates, int VirtulaRoomId, int? RosterId, int? departmentId,
            int employerId, int? positionId, int? costCodeId)
        {
            var newRecords = new List<EmployeeStatus>();
            foreach (var item in activeDates)
            {
                if (item.OnSite == 1)
                {
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
                                    RoomId = VirtulaRoomId,
                                    ChangeRoute = $"Roster executed from profile",
                                    BedId = null,
                                    ShiftId = detail.ShifId,
                                    RosterMasterId = RosterId,
                                    EmployerId = employerId,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                    EmployeeId = employee.EmployeeId
                                };
                                newRecords.Add(newData);
                            }
                            else
                            {
                                currentData.Active = 1;
                                currentData.CostCodeId = costCodeId;
                                currentData.DepId = departmentId;
                                currentData.EventDate = detail.EventDate;
                                currentData.RoomId = VirtulaRoomId;
                                currentData.EmployerId = employerId;
                                currentData.PositionId = positionId;
                                currentData.BedId = null;
                                currentData.ShiftId = detail.ShifId;
                                currentData.RosterMasterId = RosterId;
                                currentData.DateCreated = DateTime.Now;
                                currentData.ChangeRoute = $"Roster executed from profile";
                                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                                currentData.EmployeeId = employee.EmployeeId;
                                Context.EmployeeStatus.Update(currentData);
                            }
                        }
                    }

                }
                else
                {
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
                                    ShiftId = detail.ShifId,
                                    EmployerId = employerId,
                                    RosterMasterId = RosterId,
                                    PositionId = positionId,
                                    RoomId = null,
                                    BedId = null,
                                    DateCreated = DateTime.Now,
                                    ChangeRoute = $"Roster executed from profile",
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
                                currentData.RoomId = null;
                                currentData.BedId = null;
                                currentData.EmployerId = employerId;
                                currentData.PositionId = positionId;
                                currentData.ShiftId = detail.ShifId;
                                currentData.RosterMasterId = RosterId;
                                currentData.DateUpdated = DateTime.Now;
                                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                currentData.EmployeeId = employee.EmployeeId;
                                currentData.ChangeRoute = $"Roster executed from profile";
                                Context.EmployeeStatus.Update(currentData);
                            }

                        }
                    }

                }
            }

            Context.EmployeeStatus.AddRange(newRecords);


            await Task.CompletedTask;
        }



        private async Task<bool> CheckStartDateOnSite(int employeeId, DateTime eventDate)
        {
         
                var onsiteStatus = await Context.EmployeeStatus.AsNoTracking()
         .AnyAsync(x => x.EventDate.Value.Date == eventDate.Date && x.EmployeeId == employeeId && x.RoomId != null);
                return onsiteStatus;

        }



        private async Task CheckStartDateDirection(int employeeId, DateTime eventDate)
        {
            var lastTransportDirection = await Context.Transport.AsNoTracking()
                .Where(x => x.EventDate.Value.Date < eventDate && x.EmployeeId == employeeId).OrderByDescending(x => x.EventDateTime).FirstOrDefaultAsync();
            if (lastTransportDirection != null)
            {
                if (lastTransportDirection.Direction == "IN")
                {
                    throw new BadRequestException("Overlapping flights Please select another date");
                }
            }
        }


        private async Task SetRoom(RosterExecuteEmployeeRequest employee, List<BulkRosterActiveDate> activeDates/*, int RoomId,*/, int? RosterId,
            int? departmentId, int employerId, int? positionId, int? costCodeId, int virtualRoomId)
        {

            var newRecords = new List<EmployeeStatus>();
            foreach (var item in activeDates)
            {
                if (item.OnSite == 1)
                {
                    if (item.details != null)
                    {
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
                                    EmployerId = employerId,
                                    EventDate = detail.EventDate,
                                    RoomId = detail.RoomId,
                                    BedId = bedId == 0 ? null : bedId,
                                    ShiftId = detail.ShifId,
                                    ChangeRoute = $"Roster executed from profile",
                                    RosterMasterId = RosterId,
                                    PositionId = positionId,
                                    DateCreated = DateTime.Now,
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
                                currentData.BedId = bedId == 0 ? null : bedId;
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
                else
                {
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



        private async Task<bool> EmployeeRoomOccupancyCheck(int empId, int roomId, int bedCount, DateTime currentDate, int virtualRoomId)
        {
            var localRoomIds = Context.EmployeeStatus.Local
                .Where(x => x.RoomId == roomId
                    && x.EventDate.Value.Date == currentDate.Date).ToList();
            int localOwnerDateCount = 0;
            int dbOwnerDateCount = 0;

            foreach (var item in localRoomIds)
            {
                if (Context.Entry(item).State == EntityState.Added || Context.Entry(item).State == EntityState.Modified)
                {
                    var currentEmployee = await Context.Employee.Where(x => x.RoomId != roomId && x.Id == item.EmployeeId).FirstOrDefaultAsync();
                    if (currentEmployee != null)
                    {
                        item.RoomId = virtualRoomId;
                        item.BedId = null;
                        Context.Entry(item).State = Context.Entry(item).State;
                    }
                    else
                    {
                        localOwnerDateCount++;
                    }

                }
            }

            var dbroomData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.RoomId == roomId && x.EventDate.Value.Date == currentDate.Date).ToListAsync();
            foreach (var item in dbroomData)
            {
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.RoomId != roomId && x.Id == item.EmployeeId).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    item.RoomId = virtualRoomId;
                    item.BedId = null;

                }
                else
                {
                    dbOwnerDateCount++;
                }
            }


            if ((localOwnerDateCount + dbOwnerDateCount) < bedCount)
            {
                return true;
            }
            else
            {
                return false;
            }


        }


        private async Task DeleteEmployeeStatusOldData(RosterExecuteEmployeeRequest employee, DateTime startDate, DateTime endDate, List<BulkRosterActiveDate> itms)
        {
            var oldData = await Context.EmployeeStatus
                .Where(x => x.EmployeeId == employee.EmployeeId && x.EventDate.Value.Date >= startDate
                  /*  && x.EventDate.Value.Date <= endDate*/
                    
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
                var items = activeDates[i].details;
                if (items != null)
                {
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


        private async Task DeleteTransport(int  employeeId, DateTime startDate)
        {
            // Fetch the data to be deleted
            var deleteData = await Context.Transport
                .Where(x => x.EmployeeId == employeeId
                            && x.EventDate.HasValue
                            && x.EventDate.Value.Date >= startDate)
                .ToListAsync();

            // Update properties for logical delete
            foreach (var item in deleteData)
            {
                item.ChangeRoute = "RosterExecute deleted";
                item.UserIdDeleted = _hTTPUserRepository.LogCurrentUser()?.Id;
                item.DateDeleted = DateTime.Now;
            }

            // Remove the entities from the context
            Context.Transport.RemoveRange(deleteData);

            // Persist changes to the database
            await Context.SaveChangesAsync();

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

            var currentCluster = await Context.Cluster.AsNoTracking().Where(x => x.Id == ClusterId && x.Active == 1).FirstOrDefaultAsync();
            if (currentCluster == null)
            {
                return returnActiveTransportData;
            }
            var clusterDetails = await Context.ClusterDetail.AsNoTracking().Where(x => x.ClusterId == ClusterId && x.ActiveTransportId != null).OrderBy(x => x.SeqNumber).ToListAsync();
            if (clusterDetails.Count == 0)
            {
                return returnActiveTransportData;
            }

            var ActiveTransportIds = clusterDetails.Select(x => x.ActiveTransportId).ToList();


            var transportSchedules = await Context.TransportSchedule.AsNoTracking().Where(x =>
                   ActiveTransportIds.Contains(x.ActiveTransportId) &&
                    x.EventDate.Date == eventDate
                ).ToListAsync();


            foreach (var detail in clusterDetails)
            {
                var transportSchedule = transportSchedules.FirstOrDefault(x => x.ActiveTransportId == detail.ActiveTransportId);
                if (transportSchedule != null)
                {
                    TimeSpan time = TimeSpan.ParseExact(transportSchedule.ETD, "hhmm", CultureInfo.InvariantCulture);
                    DateTime combinedDateTime = eventDate.AddHours(time.Hours).AddMinutes(time.Minutes);
                    var transportCount = await Context.Transport.CountAsync(x => x.ScheduleId == transportSchedule.Id);
                    if (transportCount < transportSchedule.Seats)
                    {
                        returnActiveTransportData.ActiveTransportId = transportSchedule.ActiveTransportId;
                        returnActiveTransportData.EventDateTime = combinedDateTime;
                        returnActiveTransportData.ScheduleId = transportSchedule.Id;
                        returnActiveTransportData.Status = "Confirmed";

                        return returnActiveTransportData;
                    }
                }
            }



            //   return returnActiveTransportData;

            //TODO 2025-02-05 Cluster дүүрсэн тохиолдолд эхний онгоцруу хийдэг байсан хэсггийг drive руу оруулдаг болгов
            var transportScheduleFirst = await Context.TransportSchedule.FirstOrDefaultAsync(x =>
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



        private async Task SetTransport(RosterExecuteEmployeeRequest employee, List<BulkRosterActiveDate> activeDates, int flightGroupMasterId, List<Shift> shiftData)
        {

            var FlightGroupDetails = await Context.FlightGroupDetail.AsNoTracking().Where(x => x.FlightGroupMasterId == flightGroupMasterId).OrderBy(x => x.SeqNumber).ToListAsync();

            var newRecords = new List<Transport>();


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
                        else {
                            var evenShiftDataNS = shiftData.FirstOrDefault(x => x.Code == "NS");

                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                x.Direction == item.Direction && x.ShiftId == evenShiftDataNS?.Id &&
                                x.Active == 1 &&
                                x.DayNum == item.EventDate.ToString("dddd"));
                        }
                        


                    }
                    else
                    {
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



                    //if (currentCluster != null)
                    //{
                    //    Context.Cluster.AsNoTracking().Where(x => x.Active == 1 && x.Id == currentCluster.ClusterId);
                    //}

                }
                else
                {

                    var beforeData = activeDateChanged.Where(x => x.EventDate < item.EventDate && x.OnSite == 1).OrderByDescending(x => x.EventDate).FirstOrDefault();


                    if (Transliterator.GetWeekNumber(item.EventDate) % 2 == 0)
                    {



                        if (beforeData != null)
                        {

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

                            //currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                            //x.Direction == item.Direction && x.ShiftId == beforeData.ShiftId
                            //&& x.Active == 1 &&
                            //x.DayNum == item.EventDate.ToString("dddd"));
                        }
                        else
                        {
                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                                x.Direction == item.Direction
                                && x.Active == 1 &&
                                x.DayNum == item.EventDate.ToString("dddd"));

                            //var evenShiftDataDS = shiftData.FirstOrDefault(x => x.Code == "DS");

                            //currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                            //    x.Direction == item.Direction && x.ShiftId == evenShiftDataDS?.Id &&
                            //    x.Active == 1 &&
                            //    x.DayNum == item.EventDate.ToString("dddd"));
                        }
                    }
                    else
                    {
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
                        else
                        {
                            var oddShiftData = shiftData.FirstOrDefault(x => x.Code == "N2");
                            currentCluster = FlightGroupDetails.FirstOrDefault(x =>
                               x.Direction == item.Direction && x.ShiftId == oddShiftData?.Id &&
                               x.Active == 1 &&
                               x.DayNum == item.EventDate.ToString("dddd"));
                        }
                    }

                }



                //var currentCluster = Context.FlightGroupDetail.FirstOrDefault(x => x.Direction == item.Direction && x.DayNum ==
                //    item.EventDate.ToString("dddd") && x.FlightGroupMasterId == flightGroupMasterId);

                if (currentCluster != null && currentCluster.ClusterId.HasValue)
                {
                    var transportData = await GetActiveTransportData(currentCluster.ClusterId.Value, item.EventDate.Date);

                    if (transportData.EventDateTime == DateTime.MinValue)
                    {

                        transportData = await GetDriveTransport(item.EventDate.Date, item.Direction);

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
                            ChangeRoute = $"Roster executed from profile",
                            ScheduleId = transportData.ScheduleId,
                            Status = transportData.Status,
                            EventDate = transportData.EventDateTime.Date,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            EventDateTime = transportData.EventDateTime,


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
                            EmployerId= employee.EmployerId,
                            EmployeeId = employee.EmployeeId,
                            DateCreated = DateTime.Now,
                            //   PositionId = employee.PositionId,
                            ChangeRoute = $"Roster executed from profile",
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
                        var activeTransportDriveIds = await Context.ActiveTransport.AsNoTracking().Where(x => x.TransportModeId == driveTransportMode.Id && x.DayNum == item.EventDate.ToString("dddd") && x.Direction == item.Direction && x.Active == 1).Select(x => x.Id).ToListAsync();
                        if (activeTransportDriveIds.Count > 0)
                        {
                            var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => activeTransportDriveIds.Contains(x.ActiveTransportId) && x.EventDate.Date == item.EventDate && x.Active == 1).FirstOrDefaultAsync();
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
                                    EmployerId = employee.EmployerId,
                                    CostCodeId = employee.CostCodeId,
                                    ActiveTransportId = currentSchedule.ActiveTransportId,
                                    Direction = item.Direction,
                                    EmployeeId = employee.EmployeeId,
                                    DateCreated = DateTime.Now,
                                    //   PositionId = employee.PositionId,
                                    ScheduleId = currentSchedule.Id,
                                    Status = "Confirmed",
                                    EventDate = item.EventDate.Date,
                                    ChangeRoute = $"Roster executed from profile",
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




        private async Task<ActiveTransportRetData> GetDriveTransport(DateTime eventDate, string direction)
        {
            var transportMode = await Context.TransportMode.Where(x => x.Code.ToLower() == "drive").FirstOrDefaultAsync();
            var returnData = new ActiveTransportRetData();
            if (transportMode != null)
            {
                var currentTransportIds = await Context.ActiveTransport
                     .Where(x => x.DayNum == eventDate.ToString("dddd")
                     && x.TransportModeId == transportMode.Id && x.Direction == direction && x.Active == 1).Select(x => x.Id)
                      .ToListAsync();
                if (currentTransportIds.Count > 0)
                {
                    var currentSchedule = await Context.TransportSchedule
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
                        else {
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









        private async Task<int> GetBedId(DateTime eventDate, int RoomId, int employeeId)
        {
            var currentRoom = await Context.Room.AsNoTracking()
                .Where(x => x.Id == RoomId).Select(x => new { x.BedCount }).FirstOrDefaultAsync();


            var dateRoomEmployees = await Context.EmployeeStatus
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



        #region RoomSave



        private List<EmployeeStatusDate> EmployeeRoomRosterSetPreview(RosterExecutePreviewEmployeeRequest request, List<transportActiveDates> dates)
        {
            List<EmployeeStatusDate> returnData = new List<EmployeeStatusDate>();

            for (int i = 0; i < dates.Count; i++)
            {

                if (i + 1 < dates.Count)
                {
                    DateTime startDate = dates[i].EventDate;
                    DateTime endDate = dates[i + 1].EventDate;
                    DateTime currentDate = startDate;
                    int? shiftId = dates[i].ShiftId;

                    var currentShift = Context.Shift.FirstOrDefault(x => x.Id == shiftId);
                    while (currentDate.AddDays(1) <= endDate)
                    {

                        var employeeStatus = new EmployeeStatusDate
                        {
                            EventDate = currentDate,
                            ShiftCode = currentShift?.Code,
                            Direction = dates.FirstOrDefault(x => x.EventDate == currentDate)?.Direction

                        };
                        returnData.Add(employeeStatus);
                        currentDate = currentDate.AddDays(1);
                    }
                }
            }

            return returnData;


        }

        private async Task<List<RosterExecutePreviewEmployeeStatus>> EmployeeOffsiteStatus(RosterExecutePreviewEmployeeRequest request)
        {
            List<RosterExecutePreviewEmployeeStatus> returnData = new List<RosterExecutePreviewEmployeeStatus>();
            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.StartDate.Date.AddMonths(request.MonthDuration);

            var currentDate = startDate;

            while (currentDate <= endDate)
            {

                var currentOffSiteEmployeeStatus = await Context.EmployeeStatus
                .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId
                    && x.EventDate.Value.Date == currentDate.Date
                    && x.RoomId == null
                    && x.ShiftId != null);
                if (currentOffSiteEmployeeStatus != null)
                {
                    var currentShift = await Context.Shift.Where(x => x.Id == currentOffSiteEmployeeStatus.ShiftId).FirstOrDefaultAsync();
                    if (currentShift.OnSite != 1)
                    {
                        if (currentShift.Code != "RR")
                        {
                            var employeeStatus = new RosterExecutePreviewEmployeeStatus
                            {
                                EventDate = currentDate,
                                ShiftCode = currentShift?.Code,
                                ShiftName = currentShift?.Description,
                                ShiftId = currentShift.Id

                            };
                            returnData.Add(employeeStatus);
                        }

                    }

                }


                currentDate = currentDate.AddDays(1);
            }



            return returnData;


        }


        #endregion



        #region transport save






        private async Task<List<transportActiveDates>> CheckSchedule(DateTime startDate, DateTime endDate, int RosterId,/* int LocationId,*/ int FlightGroupMasterId)
        {
            List<transportActiveDates> dates = new List<transportActiveDates>();
            var data = await Context.RosterDetail.Where(x => x.RosterId == RosterId && x.Active == 1).OrderBy(x => x.SeqNumber).ToListAsync();
            var FlightGroupDetails = await Context.FlightGroupDetail.Where(x => x.FlightGroupMasterId == FlightGroupMasterId).OrderBy(x => x.SeqNumber).ToListAsync();
            if (data.Count == 0)
            {
                return new List<transportActiveDates>();
            }

            //  var fromStartLocation = await context.Location.FirstOrDefaultAsync(x => x.Id == LocationId);
            string currentDirection = "IN";

            DateTime currentDate = startDate;
            DateTime v_date = startDate;
            while (v_date <= endDate)
            {

                foreach (RosterDetail detail in data)
                {
                    if (v_date <= endDate)
                    {

                        if (v_date == startDate)
                        {
                            var currentFlightGroupDetail =
                                FlightGroupDetails.FirstOrDefault(x =>
                                    x.Direction == currentDirection
                                    && x.DayNum == v_date.ToString("dddd")
                                    && x.FlightGroupMasterId == FlightGroupMasterId
                                  );

                            if (currentFlightGroupDetail != null)
                            {
                                if (currentFlightGroupDetail.ClusterId != null)
                                {
                                    dates.Add(new transportActiveDates
                                    {
                                        EventDate = v_date.Date,
                                        Direction = currentDirection,
                                        DayNum = v_date.ToString("dddd"),
                                        ClusterId = currentFlightGroupDetail.ClusterId,
                                        ShiftId = detail.ShiftId,
                                        DetailId = detail.Id
                                    });

                                    v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));
                                }
                                else
                                {
                                    dates.Add(new transportActiveDates
                                    {
                                        EventDate = v_date.Date,
                                        Direction = currentDirection,
                                        DayNum = v_date.ToString("dddd"),
                                        ShiftId = detail.ShiftId,
                                        DetailId = detail.Id
                                    });

                                    v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));
                                }
                            }
                            else
                            {
                                dates.Add(new transportActiveDates
                                {
                                    EventDate = v_date.Date,
                                    Direction = currentDirection,
                                    DayNum = v_date.ToString("dddd"),
                                    ShiftId = detail.ShiftId,
                                    DetailId = detail.Id
                                });

                                v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));
                            }
                        }
                        else
                        {
                            currentDirection = currentDirection == "IN" ? "OUT" : "IN";
                            var currentFlightGroupDetail = FlightGroupDetails.FirstOrDefault(x => x.Direction == currentDirection && x.DayNum == v_date.ToString("dddd") && x.FlightGroupMasterId == FlightGroupMasterId);
                            if (currentFlightGroupDetail != null)
                            {
                                if (currentFlightGroupDetail.ClusterId != null)
                                {
                                    dates.Add(new transportActiveDates
                                    {
                                        EventDate = v_date.Date,
                                        Direction = currentDirection,
                                        DayNum = v_date.ToString("dddd"),
                                        ClusterId = currentFlightGroupDetail.ClusterId,
                                        ShiftId = detail.ShiftId,
                                        DetailId = detail.Id
                                    });

                                    v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));
                                }
                                else
                                {
                                    dates.Add(new transportActiveDates
                                    {
                                        EventDate = v_date.Date,
                                        Direction = currentDirection,
                                        DayNum = v_date.ToString("dddd"),
                                        ShiftId = detail.ShiftId,
                                        DetailId = detail.Id
                                    });

                                    v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));
                                }
                            }
                            else
                            {
                                dates.Add(new transportActiveDates
                                {
                                    EventDate = v_date.Date,
                                    Direction = currentDirection,
                                    DayNum = v_date.ToString("dddd"),
                                    ShiftId = detail.ShiftId,
                                    DetailId = detail.Id
                                });

                                v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));
                            }
                        }
                    }
                    else
                    {
                        v_date = v_date.AddDays(Convert.ToInt32(detail.DaysOn));
                    }


                }



            }
            string missingDatesStr = JsonSerializer.Serialize(dates);
            if (dates[dates.Count - 1].Direction == "IN")
            {
                var ee = dates[dates.Count - 1];
                var lasDetailId = dates[dates.Count - 1].DetailId;
                var CurrentLastDetail = data.FirstOrDefault(x => x.Id == lasDetailId);
                var nextLastDetail = data.FirstOrDefault(x => x.SeqNumber >= CurrentLastDetail.SeqNumber);
                var currentFlightGroupDetail =
                    FlightGroupDetails.FirstOrDefault(x =>
                        x.Direction == currentDirection
                        && x.DayNum == ee.EventDate.AddDays(Convert.ToInt32(nextLastDetail.DaysOn)).ToString("dddd")
                        && x.FlightGroupMasterId == FlightGroupMasterId
                      );
                dates.Add(new transportActiveDates
                {
                    EventDate = ee.EventDate.AddDays(Convert.ToInt32(nextLastDetail?.DaysOn)),
                    Direction = "OUT",
                    ClusterId = currentFlightGroupDetail?.ClusterId,
                    DayNum = ee.EventDate.AddDays(Convert.ToInt32(nextLastDetail?.DaysOn)).ToString("dddd"),
                    ShiftId = nextLastDetail?.ShiftId,
                    DetailId = nextLastDetail?.Id
                });

            }
            else
            {

            }



            return dates;
        }

        #endregion


        #region Check AdAccount
        public async Task<CheckADAccountEmployeeResponse> CheckADAccount(CheckADAccountEmployeeRequest request, CancellationToken cancellationToken)
        {
            var currentEmployee = await Context.Employee.Where(x =>
             x.ADAccount == request.AdAccount
             && x.Id != request.EmployeeId).FirstOrDefaultAsync(cancellationToken);

            if (currentEmployee != null)
            {
                var returnData = new CheckADAccountEmployeeResponse
                {
                    AdAccountValidationStatus = false,
                    AdAccountValidationFailedReason = $"Another person is registered on the AdAccount.",
                    EmployeeId = currentEmployee.Id,
                    Lastname = currentEmployee.Lastname,
                    Firstname = currentEmployee.Firstname

                };

                return returnData;
            }

            ActiveDirectoryService ac = new ActiveDirectoryService();
            var user = ac.GetUserFromAd(_configuration.GetSection("AppSettings:Domain").Value, request.AdAccount);

            if (user == null)
            {
                var returnData = new CheckADAccountEmployeeResponse
                {
                    AdAccountValidationStatus = false,
                    AdAccountValidationFailedReason = $"No one with this address was found in the Active Directory service!"
                };

                return returnData;
            }
            else
            {
                var returnData = new CheckADAccountEmployeeResponse
                {
                    AdAccountValidationStatus = true,
                    AdAccountValidationFailedReason = $"AdAccount is correct"
                };

                return returnData;
            }




        }


        #endregion


        #region variables
        public class transportActiveDates
        {
            public DateTime EventDate { get; set; }
            public string Direction { get; set; }

            public string DayNum { get; set; }

            public int? ClusterId { get; set; }

            public int? ShiftId { get; set; }

            public int? DetailId { get; set; }


        }


        public class ActiveTransportRetData
        {
            public int ActiveTransportId { get; set; }

            public DateTime EventDateTime { get; set; }

            public int? ScheduleId { get; set; }

            public string Status { get; set; }

        }

        private class BulkRosterActiveDate
        {

            public DateTime EventDate { get; set; }

            public string Direction { get; set; }

            public int ShiftId { get; set; }

            public int OnSite { get; set; }


            public List<BulkRosterActiveDateDetail> details { get; set; }



        }


        public class BulkRosterActiveDateDetail
        {
            public DateTime EventDate { get; set; }

            public int OnSite { get; set; }

            public int ShifId { get; set; }

            public int? RoomId { get; set; }
        }


        #endregion
    }





}
