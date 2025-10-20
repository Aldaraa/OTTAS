using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.TransportFeature.ReScheduleMultiple;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class TransportRepository
    {

        #region RescheduleMultiple



        public async Task<List<ReScheduleMultipleResponse>> ReScheduleMultiple(ReScheduleMultipleRequest request, CancellationToken cancellationToken)
        {


            try
            {
                if (request.TransportIds.Count > 0)
                {


                    await ValidateDirectionCehck(request);
                    var currentShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.ShiftId).FirstOrDefaultAsync();

                    if (currentShift != null)
                    {
                        int firstTransportId = request.TransportIds[0];
                        var currentTransportDirection = await Context.Transport
                            .Where(x => x.Id == firstTransportId)
                            .Select(x => new { x.Direction }).FirstOrDefaultAsync();



                        if (currentTransportDirection != null)
                        {
                            //if ((currentTransportDirection?.Direction == "IN" && currentShift.OnSite == 0) || currentTransportDirection?.Direction == "OUT" && currentShift.OnSite == 1)
                            //{
                            var currentScheduleSeat = await Context.TransportSchedule.Where(x => x.Id == request.ScheduleId).Select(y => y.Seats).FirstOrDefaultAsync(cancellationToken);


                            var skippedEmployees = new List<int>();

                            var newScheduleCount = await Context.Transport.CountAsync(x => x.ScheduleId == request.ScheduleId);
                            int successCount = 0;

                            foreach (int Id in request.TransportIds)
                            {
                                var currentTransport = await Context.Transport
                                 .FirstOrDefaultAsync(x => x.Id == Id);




                                if (currentTransport != null)
                                {
                                    var newSchedule = Context.TransportSchedule
                                .Where(x => x.Id == request.ScheduleId)
                                .Select(x => new { x.Id, x.EventDate, x.ETD, x.ActiveTransportId }).FirstOrDefaultAsync().Result;


                                    if (newSchedule != null)
                                    {

                                        var timeSeqStatus = await _transportCheckerRepository.TransportRescheduleValidDirectionSequenceCheckStatus(Id, newSchedule.Id);

                                        if (timeSeqStatus)
                                        {
                                            if (currentTransport.Direction == "IN")
                                            {
                                                if (currentTransport.EventDate.Value.Date > newSchedule?.EventDate.Date)
                                                {
                                                    if (currentShift.OnSite == 0)
                                                    {
                                                        currentShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "DS").FirstOrDefaultAsync();
                                                    }
                                                }
                                                else if (currentTransport.EventDate.Value.Date < newSchedule?.EventDate.Date)
                                                {
                                                    if (currentShift.OnSite == 1)
                                                    {
                                                        currentShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "RR").FirstOrDefaultAsync();
                                                    }
                                                }
                                            }
                                            if (currentTransport.Direction == "OUT")
                                            {
                                                if (currentTransport.EventDate.Value.Date > newSchedule?.EventDate.Date)
                                                {
                                                    if (currentShift.OnSite == 1)
                                                    {
                                                        currentShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "RR").FirstOrDefaultAsync();
                                                    }
                                                }
                                                else if (currentTransport.EventDate.Value.Date < newSchedule?.EventDate.Date)
                                                {
                                                    if (currentShift.OnSite == 0)
                                                    {
                                                        currentShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "DS").FirstOrDefaultAsync();
                                                    }
                                                }
                                            }


                                            if (currentScheduleSeat > newScheduleCount + successCount)
                                            {
                                                currentTransport.Status = "Confirmed";

                                            }
                                            else
                                            {
                                                currentTransport.Status = "Over Booked";
                                            }

                                            var oldTransportDate = currentTransport.EventDate.Value.Date;

                                            if (oldTransportDate.Date != newSchedule?.EventDate.Date)
                                            {




                                                var ChangeEmployeeId = await ChangeEmployeeStatusMultipleSkipped((int)currentTransport.EmployeeId, oldTransportDate, newSchedule.EventDate, currentShift.Id, currentTransportDirection.Direction, null, null);
                                                if (ChangeEmployeeId.HasValue)
                                                {
                                                    skippedEmployees.Add(ChangeEmployeeId.Value);
                                                }
                                                else
                                                {
                                                    int hour = int.Parse(newSchedule.ETD.Substring(0, 2));
                                                    int minute = int.Parse(newSchedule.ETD.Substring(2, 2));
                                                    currentTransport.EventDate = newSchedule.EventDate.Date.AddHours(hour).AddMinutes(minute);
                                                    currentTransport.EventDateTime = newSchedule.EventDate.Date.AddHours(hour).AddMinutes(minute);
                                                    currentTransport.ScheduleId = request.ScheduleId;
                                                    currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                                    currentTransport.ChangeRoute = "Reschedule travel profile";
                                                    currentTransport.DateCreated = DateTime.Now;
                                                    Context.Transport.Update(currentTransport);

                                                    successCount++; 
                                                }
                                            }
                                            else
                                            {
                                                int hour = int.Parse(newSchedule.ETD.Substring(0, 2));
                                                int minute = int.Parse(newSchedule.ETD.Substring(2, 2));
                                                currentTransport.EventDate = newSchedule.EventDate.Date.AddHours(hour).AddMinutes(minute);
                                                currentTransport.EventDateTime = newSchedule.EventDate.Date.AddHours(hour).AddMinutes(minute);
                                                currentTransport.ScheduleId = request.ScheduleId;
                                                currentTransport.ActiveTransportId = newSchedule.ActiveTransportId;
                                                currentTransport.ChangeRoute = "Reschedule travel profile";
                                                currentTransport.DateCreated = DateTime.Now;

                                                Context.Transport.Update(currentTransport);
                                                successCount++;

                                            }




                                            string cacheEntityName = $"Employee_{currentTransport.EmployeeId.Value}";
                                            _memoryCache.Remove($"API::{cacheEntityName}");
                                        }


                                    }

                                }


                            }

                            if (skippedEmployees.Count > 0)
                            {
                                var returnData = await Context.Employee.AsNoTracking()
                                       .Where(x => skippedEmployees.Contains(x.Id))
                                       .Select(x => new ReScheduleMultipleResponse { Id = x.Id, Fullname = $"{x.Firstname} {x.Lastname}" }).ToListAsync();

                                await Context.SaveChangesAsync();
                                return returnData;
                            }
                            else
                            {

                                await Context.SaveChangesAsync();
                                return new List<ReScheduleMultipleResponse>();

                            }



                        }
                        else
                        {
                            //        await transaction.RollbackAsync(cancellationToken);
                            throw new BadRequestException("Current transport direction invalid");
                        }

                    }
                    else
                    {
                        var errorMessage = new List<string>();
                        errorMessage.Add("Shift not found");
                        throw new BadRequestException(errorMessage.ToArray());
                    }


                }
                else
                {
                    return new List<ReScheduleMultipleResponse>();
                }

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

        }


        private async Task ValidateDirectionCehck(ReScheduleMultipleRequest  request) 
        {
            var firstTransport = await Context.Transport.AsNoTracking().Where(x => x.Id == request.TransportIds.First()).FirstOrDefaultAsync();
            if (firstTransport != null)
            {
                var currentSchedule = await Context.TransportSchedule.Where(x => x.Id == request.ScheduleId).FirstOrDefaultAsync();
                if (currentSchedule != null)
                {
                   var currentActiveTransport = await Context.ActiveTransport.AsNoTracking()
                        .Where(c => c.Id == currentSchedule.ActiveTransportId).FirstOrDefaultAsync();
                    if (currentActiveTransport != null)
                    {
                        if (currentActiveTransport.Direction != firstTransport.Direction)
                        {
                            throw new BadRequestException("Choose a schedule with the same direction");
                        }
                    }
                    else {
                        throw new BadRequestException("Active transport not found");
                    }
                }
                else{
                    throw new BadRequestException("Current schedule not found");
                }


            }


        }

        #endregion


        private async Task<int?> ChangeEmployeeStatusMultipleSkipped(int employeeId, DateTime oldEventDate, DateTime newEventDate, int ShiftId, string Direction, int? departmentId, int? costCodeId)
        {

            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime endDate = DateTime.Today;
                if (oldEventDate <= newEventDate)
                {
                    currentDate = oldEventDate;
                    endDate = newEventDate;
                }
                else
                {
                    currentDate = newEventDate;
                    endDate = oldEventDate;
                }
                if (oldEventDate < newEventDate)
                {


                    if (Direction == "OUT")
                    {
                        int? RoomId = null;
                        List<int> RoomBedIds = new List<int>();
                        bool VirtualRoom = false;
                        var currentRoom = await Context.Employee.AsNoTracking().Where(x => x.Id == employeeId && x.RoomId != null)
                            .Select(x => new { x.RoomId }).FirstOrDefaultAsync();
                        if (currentRoom != null)
                        {
                            var RoomStatus = await CheckRoomDate(currentRoom.RoomId.Value, currentDate.Date, endDate.Date.AddDays(-1), employeeId);
                            if (!RoomStatus)
                            {
                                return employeeId;
                            }
                            else
                            {
                                RoomId = currentRoom.RoomId;
                                RoomBedIds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == currentRoom.RoomId).Select(x => x.Id).ToListAsync();
                                VirtualRoom = false;
                            }
                        }
                        else
                        {

                            var beforeRoomData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.EventDate < oldEventDate && x.RoomId != null).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync(); ;

                            if (beforeRoomData != null)
                            {
                                var beforeRoom = await Context.Room.AsNoTracking().Where(x => x.Id == beforeRoomData.RoomId).FirstOrDefaultAsync();
                                if (beforeRoom != null)
                                {
                                    if (beforeRoom.VirtualRoom > 0)
                                    {
                                        RoomId = beforeRoom.Id;
                                        VirtualRoom = true;
                                    }
                                    else
                                    {
                                        var RoomStatus = await CheckRoomDate(beforeRoomData.RoomId.Value, currentDate.Date, endDate.Date.AddDays(-1), employeeId); //await GetRescheduleStatusRoomId(currentDate, endDate, beforeRoomData.RoomId.Value);
                                        if (!RoomStatus)
                                        {
                                            return employeeId;
                                        }
                                        else
                                        {
                                            RoomId = beforeRoomData.RoomId;
                                            RoomBedIds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == beforeRoomData.RoomId).Select(x => x.Id).ToListAsync();
                                            VirtualRoom = false;
                                        }
                                    }

                                }
                                else
                                {
                                    return employeeId;
                                }



                            }
                            else
                            {
                                return employeeId;
                            }
                        }


                        while (currentDate < endDate.Date)
                        {
                            var currentStatus = await Context.EmployeeStatus
                                  .Where(x => x.EmployeeId == employeeId &&
                                  x.EventDate.Value.Date == currentDate.Date).FirstOrDefaultAsync();
                            var NonActiveBeds = await Context.EmployeeStatus
                                .Where(x => x.EmployeeId != employeeId && x.RoomId == RoomId)
                                .Select(x => x.BedId).FirstOrDefaultAsync();

                            if (currentStatus != null)
                            {
                                currentStatus.ShiftId = ShiftId;
                                currentStatus.RoomId = RoomId;
                                currentStatus.BedId = VirtualRoom == false ? await GetDateBedId(RoomId.Value, currentDate, RoomBedIds) : null;
                                currentStatus.DateUpdated = DateTime.Now;
                                currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;

                                currentStatus.ChangeRoute = "Reschedule from TAS";
                                Context.EmployeeStatus.Update(currentStatus);
                            }
                            else
                            {

                                var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeId);
                                var newRecord = new EmployeeStatus
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = employeeId,
                                    ShiftId = ShiftId,
                                    RoomId = RoomId,
                                    BedId = VirtualRoom == false ? await GetDateBedId(RoomId.Value, currentDate, RoomBedIds) : null,
                                    EventDate = currentDate,
                                    EmployerId = currentEmployee?.EmployerId,
                                    PositionId = currentEmployee?.PositionId,
                                    DepId = currentEmployee?.DepartmentId,
                                    CostCodeId = currentEmployee?.CostCodeId,

                                    ChangeRoute = "Reschedule from TAS",
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                };

                                Context.EmployeeStatus.Add(newRecord);
                            }

                            currentDate = currentDate.AddDays(1);
                        }

                        return null;
                    }
                    else
                    {


                        var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeId);
                        while (currentDate < endDate.Date)
                        {
                            var currentStatus = await Context.EmployeeStatus
                                  .Where(x => x.EmployeeId == employeeId &&
                                  x.EventDate.Value.Date == currentDate.Date).FirstOrDefaultAsync();

                            if (currentStatus != null)
                            {
                                currentStatus.ShiftId = ShiftId;
                                currentStatus.RoomId = null;
                                currentStatus.BedId = null;
                                currentStatus.DateUpdated = DateTime.Now;
                                currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;

                                currentStatus.ChangeRoute = "Reschedule from TAS";
                                Context.EmployeeStatus.Update(currentStatus);
                            }
                            else
                            {


                                var newRecord = new EmployeeStatus
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = employeeId,
                                    ShiftId = ShiftId,
                                    RoomId = null,
                                    BedId = null,
                                    DepId = currentEmployee?.DepartmentId,
                                    CostCodeId = currentEmployee?.CostCodeId,
                                    EventDate = currentDate,
                                    EmployerId = currentEmployee?.EmployerId,
                                    PositionId = currentEmployee?.PositionId,
                                    ChangeRoute = "Reschedule from TAS",
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                };

                                Context.EmployeeStatus.Add(newRecord);
                            }

                            currentDate = currentDate.AddDays(1);

                        }

                        return null;
                    }


                }
                else
                {

                    if (Direction == "OUT")
                    {
                        var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeId);
                        while (currentDate < endDate)
                        {
                            var currentStatus = await Context.EmployeeStatus
                                  .Where(x => x.EmployeeId == employeeId &&
                                  x.EventDate.Value.Date == currentDate.Date).FirstOrDefaultAsync();

                            if (currentStatus != null)
                            {
                                currentStatus.ShiftId = ShiftId;
                                currentStatus.RoomId = null;
                                currentStatus.BedId = null;
                                currentStatus.DepId = departmentId;
                                currentStatus.CostCodeId = costCodeId;
                                currentStatus.DateUpdated = DateTime.Now;
                                currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;

                                currentStatus.ChangeRoute = "Reschedule from TAS";
                                Context.EmployeeStatus.Update(currentStatus);
                            }
                            else
                            {


                                var newRecord = new EmployeeStatus
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = employeeId,
                                    ShiftId = ShiftId,
                                    RoomId = null,
                                    BedId = null,
                                    DepId = departmentId,
                                    CostCodeId = costCodeId,
                                    EventDate = currentDate,
                                    EmployerId = currentEmployee?.EmployerId,
                                    PositionId = currentEmployee?.PositionId,
                                    ChangeRoute = "Reschedule from TAS",
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                };

                                Context.EmployeeStatus.Add(newRecord);
                            }

                            currentDate = currentDate.AddDays(1);


                        }

                        return null;
                    }
                    else
                    {
                        int? RoomId = null;
                        List<int> RoomBedIds = new List<int>();
                        bool VirtualRoom = false;
                        var currentRoom = await Context.Employee.AsNoTracking().Where(x => x.Id == employeeId && x.RoomId != null)
                            .Select(x => new { x.RoomId }).FirstOrDefaultAsync();
                        if (currentRoom != null)
                        {
                            // var RoomStatus = await GetRescheduleStatusRoomId(currentDate, endDate, currentRoom.RoomId.Value);

                            var RoomStatus = await CheckRoomDate(currentRoom.RoomId.Value, currentDate.Date, endDate.Date.AddDays(-1), employeeId);
                            if (!RoomStatus)
                            {
                                return employeeId;
                            }
                            else
                            {
                                RoomId = currentRoom.RoomId;
                                RoomBedIds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == currentRoom.RoomId).Select(x => x.Id).ToListAsync();
                                VirtualRoom = false;
                            }
                        }
                        else
                        {

                            var beforeRoomData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.EventDate < oldEventDate && x.RoomId != null).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync(); ;

                            if (beforeRoomData != null)
                            {
                                var beforeRoom = await Context.Room.AsNoTracking().Where(x => x.Id == beforeRoomData.RoomId).FirstOrDefaultAsync();
                                if (beforeRoom != null)
                                {
                                    if (beforeRoom.VirtualRoom > 0)
                                    {
                                        RoomId = beforeRoom.Id;
                                        VirtualRoom = true;
                                    }
                                    else
                                    {
                                        var RoomStatus = await CheckRoomDate(beforeRoomData.RoomId.Value, currentDate.Date, endDate.Date.AddDays(-1), employeeId); //await GetRescheduleStatusRoomId(currentDate, endDate, beforeRoomData.RoomId.Value);
                                        if (!RoomStatus)
                                        {
                                            return employeeId;
                                        }
                                        else
                                        {
                                            RoomId = beforeRoomData.RoomId;
                                            RoomBedIds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == beforeRoomData.RoomId).Select(x => x.Id).ToListAsync();
                                            VirtualRoom = false;
                                        }
                                    }

                                }
                                else
                                {
                                    return employeeId;
                                }



                            }
                            else
                            {
                                return employeeId;
                            }
                        }


                        while (currentDate < endDate.Date)
                        {
                            var currentStatus = await Context.EmployeeStatus
                                  .Where(x => x.EmployeeId == employeeId &&
                                  x.EventDate.Value.Date == currentDate.Date).FirstOrDefaultAsync();
                            var NonActiveBeds = await Context.EmployeeStatus
                                .Where(x => x.EmployeeId != employeeId && x.RoomId == RoomId)
                                .Select(x => x.BedId).FirstOrDefaultAsync();

                            if (currentStatus != null)
                            {
                                currentStatus.ShiftId = ShiftId;
                                currentStatus.RoomId = RoomId;
                                currentStatus.DepId = departmentId;
                                currentStatus.CostCodeId = costCodeId;
                                currentStatus.BedId = VirtualRoom == false ? await GetDateBedId(RoomId.Value, currentDate, RoomBedIds) : null;
                                currentStatus.DateUpdated = DateTime.Now;
                                currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;

                                currentStatus.ChangeRoute = "Reschedule from TAS";
                                Context.EmployeeStatus.Update(currentStatus);
                            }
                            else
                            {

                                var currentEmployee = await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeId);
                                var newRecord = new EmployeeStatus
                                {
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = employeeId,
                                    ShiftId = ShiftId,
                                    RoomId = RoomId,
                                    BedId = VirtualRoom == false ? await GetDateBedId(RoomId.Value, currentDate, RoomBedIds) : null,
                                    DepId = departmentId,
                                    CostCodeId = costCodeId,
                                    EventDate = currentDate,
                                    EmployerId = currentEmployee?.EmployerId,
                                    PositionId = currentEmployee?.PositionId,
                                    ChangeRoute = "Reschedule from TAS",
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id

                                };

                                Context.EmployeeStatus.Add(newRecord);
                            }

                            currentDate = currentDate.AddDays(1);
                        }

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return employeeId; 
            }

            
        }


    }
}
