using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.MultipleBookingFeature.MultipleBookingAddTransport;
using tas.Application.Features.MultipleBookingFeature.MultipleBookingPreviewTransport;
using tas.Application.Features.TransportFeature.AddExternalTravel;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tas.Persistence.Repositories
{
    public partial class TransportRepository
    {
     //   private readonly int NO_ROOM_EMPLOYEE_ALLOW_DAYS = 2;
    //   private readonly int ROOM_OWNER_EMPLOYEE_ALLOW_DAYS = 3;


        #region AddTravel

        public async Task<List<MultipleBookingAddTransportResponse>> MultipleBookingAddTransport(MultipleBookingAddTransportRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<MultipleBookingAddTransportResponse>();

            var firtschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.firsScheduleId)
                                      join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                      select new
                                      {
                                          EventDate = schedule.EventDate,
                                          Direction = activetransport.Direction,

                                      }).FirstOrDefaultAsync();

            if (firtschedule != null)
            {
                if (firtschedule.Direction == "EXTERNAL")
                {
                    returnData = await MultipleBookingAddExternal(request, cancellationToken);
                }
                else if (firtschedule.Direction == "IN" || firtschedule.Direction == "OUT")
                {
                    if (request.lastSheduleId.HasValue)
                    {
                        var lastschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.lastSheduleId)
                                                  join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                  select new
                                                  {
                                                      EventDate = schedule.EventDate,
                                                      Direction = activetransport.Direction,

                                                  }).FirstOrDefaultAsync();
                        if (lastschedule != null)
                        {
                            returnData = await MultipleBookingAddSiteTrave(request, cancellationToken);


                        }
                        else
                        {
                            throw new BadRequestException("Last Schedule not found");
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Last Schedule not found");

                    }
                }
                else
                {
                    throw new BadRequestException("Invalid direction active transport");
                }

            }
            else
            {
                throw new BadRequestException("Schedule not found");
            }

            return returnData;
        }


        private async Task<List<MultipleBookingAddTransportResponse>> MultipleBookingAddExternal(MultipleBookingAddTransportRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<MultipleBookingAddTransportResponse>();
            foreach (var emp in request.EmployeeData)
            {

                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == emp.EmployerId).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (currentEmployee.Active == 1)
                    {
                        var newData = new MultipleBookingAddTransportResponse { EmployeeId = currentEmployee.Id };
                        try
                        {
                            await _transportCheckerRepository.TransportExternalAddValidCheck(emp.EmployeeId, request.firsScheduleId, request.lastSheduleId);

                            await  ExternalTraveCreate(request.firsScheduleId, request.lastSheduleId, emp, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            newData.Error = ex.Message;

                        }
                        finally
                        {

                            newData.FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}";
                            returnData.Add(newData);
                        }
                    }
                    else
                    {
                        returnData.Add(new MultipleBookingAddTransportResponse
                        {
                            EmployeeId = emp.EmployeeId,
                            Error = "Inactive Employee",
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}"
                        });
                    }

                }

            }
            return returnData.Where(x => x.Error != null).ToList();
        }






        private async Task ExternalTraveCreate(int firstScheduleId, int? lastScheduleId, MultipleBookingAddTransportEmployee request, CancellationToken cancellationToken)
        {

            var currentSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == firstScheduleId);
            if (currentSchedule != null)
            {
                var currentActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentSchedule.ActiveTransportId);

                if (currentActiveTransport != null)
                {
                    DateTime time = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                    int hours = time.Hour;
                    int minutes = time.Minute;

                    var transportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == currentSchedule.Id);

                    var transport = new Transport
                    {
                        EmployeeId = request.EmployeeId,
                        PositionId = request.PositionId,
                        DepId = request.DepartmentId,
                        CostCodeId = request.CostCodeId,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                        ActiveTransportId = currentSchedule.ActiveTransportId,
                        EventDate = currentSchedule.EventDate.Date,
                        EventDateTime = currentSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                        ScheduleId = currentSchedule.Id,
                        Active = 1,
                        Direction = currentActiveTransport.Direction,
                        EmployerId = request.EmployerId,
                        ChangeRoute = "Add travel profile",
                        Status = currentSchedule.Seats > transportcount ? "Confirmed" : "Over booked"


                    };

                    Context.Transport.Add(transport);

                    if (lastScheduleId.HasValue)
                    {
                        var currentLastSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == firstScheduleId);
                        if (currentLastSchedule != null)
                        {
                            var currentLastActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentLastSchedule.ActiveTransportId);

                            if (currentLastActiveTransport != null)
                            {
                                DateTime lasttime = DateTime.ParseExact(currentLastSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int lasthours = time.Hour;
                                int lastminutes = time.Minute;

                                var lasttransportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == currentLastSchedule.Id);

                                var Lasttransport = new Transport
                                {
                                    EmployeeId = request.EmployeeId,
                                    PositionId = request.PositionId,
                                    DepId = request.DepartmentId,
                                    CostCodeId = request.CostCodeId,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    ActiveTransportId = currentLastSchedule.ActiveTransportId,
                                    EventDate = currentLastSchedule.EventDate.Date,
                                    EventDateTime = currentLastSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                                    ScheduleId = currentLastSchedule.Id,
                                    Active = 1,
                                    Direction = currentLastActiveTransport.Direction,
                                    EmployerId = request.EmployerId,
                                    ChangeRoute = "Add travel profile",
                                    Status = currentSchedule.Seats > transportcount ? "Confirmed" : "Over booked"
                                };


                                Context.Transport.Add(Lasttransport);

                            }

                        }
                    }
                }
            }
        }




        private async Task<List<MultipleBookingAddTransportResponse>> MultipleBookingAddSiteTrave(MultipleBookingAddTransportRequest request, CancellationToken cancellationToken)
        {

            var returnData = new List<MultipleBookingAddTransportResponse>();

            // Fetch Virtual Room upfront
            var virtualRoom = await Context.Room.AsNoTracking()
                                 .Where(x => x.VirtualRoom == 1)
                                 .FirstOrDefaultAsync(cancellationToken);
            if (virtualRoom == null)
                throw new BadRequestException("Please register virtual room");

            // Pre-fetch Employee Data

            foreach (var empdata in request.EmployeeData)
            {
                int employeeRoomId = virtualRoom.Id;
                var currentEmployee = await Context.Employee.AsNoTracking().Where(c => c.Id == empdata.EmployeeId).FirstOrDefaultAsync();
                    

                if (currentEmployee != null && currentEmployee.Active == 1)
                {
                    var newData = new MultipleBookingAddTransportResponse { EmployeeId = currentEmployee.Id };
                    try
                    {
                        if (currentEmployee.RoomId.HasValue)
                        {
                            var employeeRoom = await Context.Room.AsNoTracking()
                                                .Where(x => x.Id == currentEmployee.RoomId.Value)
                                                .FirstOrDefaultAsync(cancellationToken);
                            if (employeeRoom != null)
                            {
                                employeeRoomId = employeeRoom.Id;
                            }
                        }

                        await _transportCheckerRepository.TransportAddValidDirectionSequenceCheck(
                            currentEmployee.Id, request.firsScheduleId, request.lastSheduleId ?? 0);
                        await ValidateAddTravelShort(
                            currentEmployee.Id, request.firsScheduleId, request.lastSheduleId ?? 0, employeeRoomId, cancellationToken);


                        var lastschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.lastSheduleId)
                                                  join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                  select new
                                                  {
                                                      EventDate = schedule.EventDate,
                                                      Direction = activetransport.Direction,

                                                  }).FirstOrDefaultAsync();


                        var firtschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.firsScheduleId)
                                                  join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                  select new
                                                  {
                                                      EventDate = schedule.EventDate,
                                                      Direction = activetransport.Direction,

                                                  }).FirstOrDefaultAsync();
                        if (firtschedule.Direction == "IN" && lastschedule.Direction == "OUT")
                        {
                            if (firtschedule.EventDate.Date <= lastschedule.EventDate.Date)
                            {
                                await MultipleBookingPreviewSiteTravelRoomCheck(employeeRoomId, firtschedule.EventDate, lastschedule.EventDate, virtualRoom.Id);

                                await SiteTravelCreate(request.firsScheduleId, request.lastSheduleId.Value, employeeRoomId, empdata, cancellationToken);

                            }
                        }



                    }
                    catch (Exception ex)
                    {
                        newData.Error = ex.Message;
                    }
                    finally
                    {
                        newData.FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}";
                        returnData.Add(newData);
                    }
                }
                else
                {
                    returnData.Add(new MultipleBookingAddTransportResponse
                    {
                        EmployeeId = currentEmployee.Id,
                        Error = "Inactive Employee",
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}"
                    });
                }
            }

            return returnData.Where(x => x.Error != null).ToList() ;
        }

        private async Task SiteTravelCreate(int firstScheduleId, int lastScheduleId, int EmployeeRoomId,  MultipleBookingAddTransportEmployee request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {


                    var inSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == firstScheduleId);
                    var outSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == lastScheduleId);
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
                                        GoShow = request.firsScheduleGoShow


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
                                        ChangeRoute = "Multiple booking",
                                        EmployerId = request.EmployerId,
                                        Status = outSchedule.Seats > outtransportcount ? "Confirmed" : "Over booked",
                                        GoShow = request.lastSheduleGoShow

                                    };

                                    Context.Transport.Add(transportin);
                                    Context.Transport.Add(transportout);


                                    Stopwatch stopWatch = new Stopwatch();
                                    stopWatch.Start();

                                    await NoGoShowSave(request.EmployeeId, firstScheduleId, request.firsScheduleGoShow, string.Empty, false);
                                    await NoGoShowSave(request.EmployeeId, lastScheduleId, request.lastSheduleGoShow, string.Empty, false);

                                    await AddTravelSetRoomMultiple(transportin.EventDate.Value.Date,  transportout.EventDate.Value.Date, EmployeeRoomId, request);

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
                        else if (inSchedule.EventDate.Date == outSchedule.EventDate.Date.Date)
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
                                        GoShow = request.firsScheduleGoShow


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
                                        GoShow = request.lastSheduleGoShow

                                    };

                                    Context.Transport.Add(transportin);
                                    Context.Transport.Add(transportout);

                                    await NoGoShowSave(request.EmployeeId, firstScheduleId, request.firsScheduleGoShow, string.Empty, false);
                                    await NoGoShowSave(request.EmployeeId, lastScheduleId, request.lastSheduleGoShow, string.Empty, false);
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


        public async Task AddTravelSetRoomMultiple(DateTime StartDate, DateTime EndDate, int EmployeeRoomId, MultipleBookingAddTransportEmployee request)
        {

            if (StartDate.Date == EndDate.Date)
            {


                return;


            }
            else
            {
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == EmployeeRoomId);
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
                                    RoomId = currentShift?.OnSite == 1 ? EmployeeRoomId : null,
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
                                currentStatus.RoomId = currentShift?.OnSite == 1 ? EmployeeRoomId : null;
                                currentStatus.Active = 1;
                                currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentStatus.ChangeRoute = $"Add travel Profile";
                                Context.EmployeeStatus.Update(currentStatus);

                            }

                            currentDate = currentDate.AddDays(1);

                        }
                        else
                        {
                            int? roomId = currentShift?.OnSite == 1 ? EmployeeRoomId : null;
                            int? bedId = null;
                            if (roomId != null)
                            {
                                bedId = await getRoomBedId((int)roomId, currentDate);
                            }
                            int dateGuestCount = await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.RoomId == EmployeeRoomId &&
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
                                        currentStatus.RoomId = currentShift?.OnSite == 1 ? EmployeeRoomId : null;
                                        currentStatus.Active = 1;
                                        currentStatus.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                        currentStatus.ChangeRoute = $"Add travel Profile";
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
                                        RoomId = currentShift?.OnSite == 1 ? EmployeeRoomId : null,
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
                                    currentStatus.RoomId = currentShift?.OnSite == 1 ? EmployeeRoomId : null;
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

            await Task.CompletedTask;
        }


        #endregion


        #region PreviewData

        public async Task<List<MultipleBookingPreviewTransportResponse>> MultipleBookingPreviewTransport(MultipleBookingPreviewTransportRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<MultipleBookingPreviewTransportResponse>();

            var firtschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.firsScheduleId)
                                      join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                        select new { 
                                        EventDate = schedule.EventDate,
                                        Direction = activetransport.Direction,

                                     }).FirstOrDefaultAsync();

            if (firtschedule != null)
            {
                if (firtschedule.Direction == "EXTERNAL")
                {
                    returnData = await MultipleBookingPreviewExternalCheck(request, cancellationToken);
                }
                else if (firtschedule.Direction == "IN" || firtschedule.Direction == "OUT")
                {
                    if (request.lastSheduleId.HasValue)
                    {
                        var lastschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.lastSheduleId)
                                                  join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                  select new
                                                  {
                                                      EventDate = schedule.EventDate,
                                                      Direction = activetransport.Direction,

                                                  }).FirstOrDefaultAsync();
                        if (lastschedule != null)
                        {
                            returnData = await MultipleBookingPreviewSiteTravelCheck(request, cancellationToken);
            

                        }
                        else {
                            throw new BadRequestException("Last Schedule not found");
                        }
                    }
                    else {
                        throw new BadRequestException("Last Schedule not found");

                    }
                }
                else {
                    throw new BadRequestException("Invalid direction active transport");
                }

            }
            else
            {
                throw new BadRequestException("Schedule not found");
            }

            return returnData;

        }

        private async Task<List<MultipleBookingPreviewTransportResponse>> MultipleBookingPreviewExternalCheck(MultipleBookingPreviewTransportRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<MultipleBookingPreviewTransportResponse>();
            foreach (var empId in request.EmployeeIds)
            {

                var currentEmployee =await Context.Employee.AsNoTracking().Where(x => x.Id == empId).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (currentEmployee.Active == 1)
                    {
                        var newData = new MultipleBookingPreviewTransportResponse { EmployeeId = currentEmployee.Id };
                        try
                        {
                            await _transportCheckerRepository.TransportExternalAddValidCheck(empId, request.firsScheduleId, request.lastSheduleId);
                        }
                        catch (Exception ex)
                        {
                            newData.Error = ex.Message;

                        }
                        finally
                        {

                            newData.FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}";
                            returnData.Add(newData);
                        }
                    }
                    else {
                        returnData.Add(new MultipleBookingPreviewTransportResponse { 
                            EmployeeId = empId,
                            Error = "Inactive Employee",
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}"
                        });
                    }

                }

            }
            return returnData.Where(x=> x.Error != null).ToList();
        }

        private async Task<List<MultipleBookingPreviewTransportResponse>> MultipleBookingPreviewSiteTravelCheck(MultipleBookingPreviewTransportRequest request, CancellationToken cancellationToken)
        {

            var returnData = new List<MultipleBookingPreviewTransportResponse>();

            // Fetch Virtual Room upfront
            var virtualRoom = await Context.Room.AsNoTracking()
                                 .Where(x => x.VirtualRoom == 1)
                                 .FirstOrDefaultAsync(cancellationToken);
            if (virtualRoom == null)
                throw new BadRequestException("Please register virtual room");

            // Pre-fetch Employee Data
            var employeeList = await Context.Employee.AsNoTracking()
                                .Where(e => request.EmployeeIds.Contains(e.Id))
                                .ToListAsync(cancellationToken);

            foreach (var currentEmployee in employeeList)
            {
                int employeeRoomId = virtualRoom.Id;
                if (currentEmployee != null && currentEmployee.Active == 1)
                {
                    var newData = new MultipleBookingPreviewTransportResponse { EmployeeId = currentEmployee.Id };
                    try
                    {
                        if (currentEmployee.RoomId.HasValue)
                        {
                            var employeeRoom = await Context.Room.AsNoTracking()
                                                .Where(x => x.Id == currentEmployee.RoomId.Value)
                                                .FirstOrDefaultAsync(cancellationToken);
                            if (employeeRoom != null)
                            {
                                employeeRoomId = employeeRoom.Id;
                            }
                        }


                        if (currentEmployee.Id == 443971)
                        {
                            var aa = 0;
                        }

                        var aac = 100;

                        await _transportCheckerRepository.TransportAddValidDirectionSequenceCheck(
                            currentEmployee.Id, request.firsScheduleId, request.lastSheduleId ?? 0);
                        await ValidateAddTravelShort(
                            currentEmployee.Id, request.firsScheduleId, request.lastSheduleId ?? 0, employeeRoomId, cancellationToken);


                        var lastschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.lastSheduleId)
                                                  join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                  select new
                                                  {
                                                      EventDate = schedule.EventDate,
                                                      Direction = activetransport.Direction,

                                                  }).FirstOrDefaultAsync();


                        var firtschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.firsScheduleId)
                                                  join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id
                                                  select new
                                                  {
                                                      EventDate = schedule.EventDate,
                                                      Direction = activetransport.Direction,

                                                  }).FirstOrDefaultAsync();
                        if (firtschedule.Direction == "IN" && lastschedule.Direction == "OUT")
                        {
                            if (firtschedule.EventDate.Date <= lastschedule.EventDate.Date)
                            {
                                await MultipleBookingPreviewSiteTravelRoomCheck(employeeRoomId, firtschedule.EventDate, lastschedule.EventDate, virtualRoom.Id);
                            }
                        }



                    }
                    catch (Exception ex)
                    {
                        newData.Error = ex.Message;
                    }
                    finally
                    {
                        newData.FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}";
                        returnData.Add(newData);
                    }
                }
                else
                {
                    returnData.Add(new MultipleBookingPreviewTransportResponse
                    {
                        EmployeeId = currentEmployee.Id,
                        Error = "Inactive Employee",
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}"
                    });
                }
            }

            return returnData.Where(x=> x.Error != null).ToList();

        }





        public async Task<int> MultipleBookingPreviewSiteTravelRoomCheck(int roomId, DateTime startDate, DateTime endDate, int virtualRoomId)
        {
            if (virtualRoomId == roomId)
            {

                return virtualRoomId;
                //if (startDate.Date >= DateTime.Today.AddDays(NO_ROOM_EMPLOYEE_ALLOW_DAYS).Date)
                //{
                //    return virtualRoomId;

                //}
                //else {
                //    throw new BadRequestException($"You can book for an employee who is not the room owner, but only for dates starting {NO_ROOM_EMPLOYEE_ALLOW_DAYS} days from today or later.");
                //}
            }
            else {
               // if (startDate.Date >= DateTime.Today.AddDays(ROOM_OWNER_EMPLOYEE_ALLOW_DAYS).Date)
             //   {
                    DateTime currentDate = startDate.Date;  
                    var currentRoom = await Context.Room.AsNoTracking().Where(x=> x.Id == roomId).FirstOrDefaultAsync();
                    bool roomFullStatus = false;
                    if (currentRoom != null)
                    {
                        while (currentDate.AddDays(1) <= endDate.Date)
                        {

                            int dateGuestCount = await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.RoomId == roomId &&
                                 x.EventDate.Value.Date == currentDate.Date
                             );

                            if (dateGuestCount > currentRoom?.BedCount)
                            {
                                roomFullStatus = true;
                            }
                            else
                            {
                                currentDate = currentDate.AddDays(1);
                            }
                        }
                        if (roomFullStatus)
                        {
                            return virtualRoomId;
                        }
                        else {
                            return roomId;
                        }

                    }
                    else {
                        return virtualRoomId;
                    }


                //}
                //else
                //{
                //    throw new BadRequestException($"You can book for an employee who is  the room owner, but only for dates starting {ROOM_OWNER_EMPLOYEE_ALLOW_DAYS} days from today or later.");
                //}
            }
            

        }

        #endregion

    }
}
