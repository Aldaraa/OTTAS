using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeProfileStatusFeature.GetDateRangeProfileStatus;
using tas.Application.Features.EmployeeStatusFeature.CalendarBookingEmployee;
using tas.Application.Features.EmployeeStatusFeature.CalendarBookingRoomAssign;
using tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDate;
using tas.Application.Features.EmployeeStatusFeature.ChangeRoomByDates;
using tas.Application.Features.EmployeeStatusFeature.DateLastEmployeeStatus;
using tas.Application.Features.EmployeeStatusFeature.GetDateRangeStatus;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingByRoom;
using tas.Application.Features.EmployeeStatusFeature.RoomBookingEmployee;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusBulkChange;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChange;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusDateChangeBulk;
using tas.Application.Features.EmployeeStatusFeature.VisualStatusGetEmployee;
using tas.Application.Features.RoomFeature.DateProfileRoomDetail;
using tas.Application.Features.RoomFeature.MonthStatusRoom;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;
using static tas.Persistence.Repositories.RosterExecuteRepository;

namespace tas.Persistence.Repositories
{


    public class EmployeeStatusRepository : BaseRepository<EmployeeStatus>, IEmployeeStatusRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;

        public EmployeeStatusRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
        }


        public async Task<RoomBookingByRoomResponse> RoombookingByRoom(RoomBookingByRoomRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 100 : request.pageSize;
            int pageIndex = request.pageIndex;



            var returnDat = new List<RoomBookingByRoomResponseData>();
            var occupiedRooms = await Context.EmployeeStatus.AsNoTracking()
             .Where(e => e.EventDate >= request.model.StartDate && e.EventDate <= request.model.EndDate && e.RoomId == request.model.RoomId)
             .OrderBy(e => e.EventDate)
             .Select(e => new { e.EmployeeId, e.EventDate, e.RoomId, e.Id, e.CostCodeId })
             .ToListAsync(cancellationToken);

             var occupiedRoomsPlus = await Context.EmployeeStatus.AsNoTracking().FirstOrDefaultAsync(e => e.EventDate >= request.model.EndDate && e.EmployeeId == request.model.RoomId && e.RoomId != null);


            var continuousRooms = new List<int>();
            DateTime? previousEventDate = null;
            int? previousEmpId = null;
            int? previousRoomId = null;
            int id = 0;

            var empIds = occupiedRooms.Select(x => x.EmployeeId).Distinct();


            var pageEmpIds = empIds.Skip(pageIndex * pageSize)
              .Take(pageSize)
              .ToList();


            foreach (var empId in pageEmpIds) 
            {
                var occupiedRoomsEmp = occupiedRooms.Where(x => x.EmployeeId == empId).ToList();
                foreach (var roomEvent in occupiedRoomsEmp)
                {
                    

                    if (previousEmpId.HasValue && previousEventDate.HasValue)
                    {
                        if (roomEvent.EmployeeId == previousEmpId.Value && roomEvent.EventDate.Value.AddDays(-1) == previousEventDate.Value)
                        {
                            previousEmpId = roomEvent.EmployeeId;
                            previousEventDate = roomEvent.EventDate;
                            previousRoomId = roomEvent.RoomId;

                        }
                        else
                        {
                            var oldata = returnDat.FirstOrDefault(x => x.Id == id);
                            if (oldata != null)
                            {
                                oldata.LastNight = previousEventDate.Value.Date.ToString("yyyy-MM-dd");
                            }
                            previousEventDate = null!;
                            previousEmpId = null;
                            previousRoomId = null;


                        }
                    }
                    else
                    {
                        id++;
                        var newData = new RoomBookingByRoomResponseData
                        {
                            Id = id,
                            DateIn = id == 1 ? roomEvent.EventDate.Value.Date.ToString("yyyy-MM-dd") : roomEvent.EventDate.Value.AddDays(-1).Date.ToString("yyyy-MM-dd"),
                            EmployeeId = roomEvent.EmployeeId,
                            RoomId = roomEvent.RoomId,
                            CostCodeId = roomEvent.CostCodeId

                        };

                        returnDat.Add(newData);

                        previousEmpId = roomEvent.EmployeeId;
                        previousEventDate = roomEvent.EventDate;
                        previousRoomId = roomEvent.RoomId;
                    }
                }
            }

           

            foreach (var item in returnDat)
            {
                var currentRoom =await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.RoomId);
                var currentEmployee =await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.EmployeeId);


                var currentEmployeePeopleType = currentEmployee != null
                ? await Context.PeopleType.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PeopleTypeId)
                : null;

                var currentEmployeeEmployer = currentEmployee != null
                    ? await Context.Employer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.EmployerId)
                    : null;

                var currentEmployeeDepartment = currentEmployee != null
                    ? await Context.Department.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.DepartmentId)
                    : null;

                var currentCostCode =await Context.CostCodes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.CostCodeId);
                item.RoomNumber = currentRoom?.Number;

                var currentCamp = await  Context.Camp.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentRoom.CampId);
                var currentRoomType = await Context.RoomType.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentRoom.RoomTypeId);

                item.Camp =currentCamp?.Description;
                item.RoomType = currentRoomType?.Description;
                item.SAPID = currentEmployee?.SAPID;
                item.FullName = string.Format("{0} {1}",currentEmployee?.Firstname, currentEmployee?.Lastname);
                item.Lastname = currentEmployee?.Lastname;
                item.Firstname = currentEmployee?.Firstname;
                item.Gender = currentEmployee?.Gender;
                item.CostCodeDescription = currentCostCode?.Code;
                item.VirtualRoom = currentRoom?.VirtualRoom;
                item.HotelCheck = currentEmployee?.HotelCheck;
                item.DepartmentName = currentEmployeeDepartment?.Name;
                item.DepartmentId = currentEmployee?.DepartmentId;
                item.EmployerName = currentEmployeeEmployer?.Description;
                item.PeopleTypeCode = currentEmployeePeopleType?.Code;
                item.RoomOwner = request.model.RoomId == currentEmployee?.RoomId;
                item.PersonalMobile = currentEmployee?.PersonalMobile;
                
                if (item.LastNight == null)
                {
                    item.LastNight = occupiedRooms.Last().EventDate.Value.ToString("yyyy-MM-dd");
                }


                try
                {
                    DateTime LastNightdateValue;
                    DateTime DateIndateValue;


                    if (DateTime.TryParseExact(item.LastNight, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out LastNightdateValue))
                    {

                        if (DateTime.TryParseExact(item.DateIn, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateIndateValue))
                        {
                            TimeSpan difference = LastNightdateValue - DateIndateValue;
                            item.Days = (int)difference.TotalDays;
                        }

                    }
                }
                catch (Exception)
                {

                    throw;
                }


              //  item.Days = (int)difference.TotalDays;
            }

            var returnData = new RoomBookingByRoomResponse
            {
                data = returnDat,
                pageSize = pageSize,
                currentPage = pageIndex + 1,
                totalcount = empIds.Count()
            };

            return returnData;

        }

        public async Task<List<RoomBookingEmployeeResponse>> EmployeeRoombooking(RoomBookingEmployeeRequest request, CancellationToken cancellationToken) 
        {


            var startDate = request.StartDate;
            var endDate = request.EndDate;

            var startDateOnsite = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == startDate && x.RoomId != null)
                .FirstOrDefaultAsync();

                            var roomEvents = await Context.EmployeeStatus.AsNoTracking()
                 .Where(e => e.EventDate >= request.StartDate && e.EventDate <= request.EndDate && e.EmployeeId == request.EmployeeId && e.RoomId != null)
                 .OrderBy(e => e.EventDate)
                 .Select(e => new { e.RoomId, e.EventDate, e.Id })
                 .ToListAsync(cancellationToken);



            var roomEventSummaries = new List<RoomBookingEmployeeResponse>();


            if (roomEvents.Any())
            {
                var currentRoomId = roomEvents.First().RoomId;
                var continuousNights = 1;
                var firstNight = roomEvents.First().EventDate;

                for (int i = 1; i < roomEvents.Count; i++)
                {
                    var previousEvent = roomEvents[i - 1];
                    var currentEvent = roomEvents[i];

                    if (currentEvent.RoomId == currentRoomId && (currentEvent.EventDate - previousEvent.EventDate).Value.Days == 1)
                    {
                        continuousNights++;
                    }
                    else
                    {
                        roomEventSummaries.Add(new RoomBookingEmployeeResponse
                        {
                            RoomId = currentRoomId,
                            DateIn = firstNight,
                            LastNight = previousEvent.EventDate,
                            Days = continuousNights
                        });

                        currentRoomId = currentEvent.RoomId;
                        firstNight = currentEvent.EventDate;
                        continuousNights = 1;
                    }
                }

                // Add the last room event summary
                roomEventSummaries.Add(new RoomBookingEmployeeResponse
                {
                    RoomId = currentRoomId,
                    DateIn = firstNight,
                    LastNight = roomEvents.Last().EventDate,
                    Days = continuousNights
                });
            }


            foreach (var item in roomEventSummaries)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                    .Where(x => x.Id == request.EmployeeId).Select(x => new { x.RoomId }).FirstOrDefaultAsync();

                var CurrentRoom = await (from room in Context.Room.AsNoTracking().Where(x => x.Id == item.RoomId)
                                     join camp in Context.Camp.AsNoTracking() on room.CampId equals camp.Id into campData
                                     from camp in campData.DefaultIfEmpty()
                                     join roomtype in Context.RoomType.AsNoTracking() on room.RoomTypeId equals roomtype.Id into roomtypeData
                                     from roomtype in roomtypeData.DefaultIfEmpty()
                                     select new {
                                        roomNumber = room.Number,
                                        CampName = camp.Description,
                                        RoomTypeName = roomtype.Description,
                                        VirtualRoom = room.VirtualRoom,
                                        

                                     }).FirstOrDefaultAsync();
                item.RoomNumber = CurrentRoom?.roomNumber;
                item.Camp = CurrentRoom?.CampName;
                item.RoomType = CurrentRoom?.RoomTypeName;
                item.RoomOwner = currentEmployee?.RoomId == item.RoomId;
                item.VirtualRoom = CurrentRoom?.VirtualRoom;

            }

            return roomEventSummaries;

       


        }


        public async Task<List<VisualStatusGetEmployeeResponse>> VisualStatusGetEmployee(VisualStatusGetEmployeeRequest request, CancellationToken cancellationToken)
        {
            var visualDateRanges = GetVisualStatuDateRange(request.StartDate);
            var startDate = visualDateRanges.First();
            var endDate = visualDateRanges.Last();
            var EmployeeStatusData =await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate >= startDate.Date && x.EventDate <= endDate && x.EmployeeId == request.EmployeeId).ToListAsync();
            var returnData = new List<VisualStatusGetEmployeeResponse>();


            foreach (var currentDate in visualDateRanges)
            {
                string? color = null;
                string? shiftCode = null;
                string? shifDescription = null;
                int? shifId = null;
                var currentEmployeeStatus = EmployeeStatusData.FirstOrDefault(x => x.EventDate == currentDate.Date);
                if (currentEmployeeStatus != null) {
                    if (currentEmployeeStatus.ShiftId != null) {
                        shifId = currentEmployeeStatus.ShiftId;
                        var currentShift =await Context.Shift.FirstOrDefaultAsync(x => x.Id == currentEmployeeStatus.ShiftId);
                        if (currentShift != null)
                        {
                            shiftCode = currentShift.Code;
                            shifDescription = currentShift.Description;
                            if (currentShift.ColorId != null) {
                                 color = await Context.Color.Where(x => x.Id == currentShift.ColorId)
                                  .Select(x => x.Code)
                                  .FirstOrDefaultAsync();
                            }
                        }
                    }
                }

                var newData = new VisualStatusGetEmployeeResponse
                {
                    EventDate = currentDate,
                    ShiftId = shifId,
                    Color = color,
                    ShiftCode = shiftCode,
                    ShiftDescription = shifDescription

                };
                returnData.Add(newData);
            }
            return returnData;
        }


        private List<DateTime> GetVisualStatuDateRange(DateTime startDate) 
        {
            List<DateTime> dateRange = new List<DateTime>();

            DateTime nearestMonday = startDate.AddDays(-(int)startDate.DayOfWeek + (int)DayOfWeek.Monday);
            for (int i = 0; i < 35; i++)
            {
                dateRange.Add(nearestMonday.AddDays(i));
            }

            return dateRange;
        }

        public async Task VisualStatusDateChange(VisualStatusDateChangeRequest request, CancellationToken cancellationToken)
        {
            var currentEmployee =await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId && x.Active == 1).Select(x => new { x.CostCodeId, x.DepartmentId, x.PositionId, x.EmployerId }).FirstOrDefaultAsync();
            if (currentEmployee != null) {
                foreach (var item in request.StatusDates)
                {
                    var currentStatus = await Context.EmployeeStatus.FirstOrDefaultAsync(x => x.EventDate.Value.Date == item.EventDate.Date && x.EmployeeId == request.EmployeeId);
                    var newRequestShift = await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.ShiftId);
                    if (newRequestShift != null) {
                        if (currentStatus != null)
                        {
                            var currentShift = await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentStatus.ShiftId);
                            if (currentShift?.OnSite == newRequestShift.OnSite)
                            {
                                currentStatus.ShiftId = item.ShiftId;
                                currentStatus.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                currentStatus.DateUpdated = DateTime.Now;
                                currentStatus.ChangeRoute = $"status shift change";
                                Context.EmployeeStatus.Update(currentStatus);

                            }
                        }
                        else
                        {
                            if (newRequestShift.OnSite != 1)
                            {
                                var newData = new EmployeeStatus
                                {
                                    Active = 1,
                                    DepId = currentEmployee.DepartmentId,
                                    CostCodeId = currentEmployee.CostCodeId,
                                    PositionId = currentEmployee.PositionId,
                                    ShiftId = item.ShiftId,
                                    EventDate = item.EventDate.Date,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = request.EmployeeId,
                                    ChangeRoute = $"status shift change",
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                                };

                                Context.EmployeeStatus.Add(newData);
                            }
                        }
                    }
                  
                }
            }

            await Task.CompletedTask;

            
        }

        public async Task VisualStatusBulkChange(VisualStatusBulkChangeRequest request, CancellationToken cancellationToken)
        {
            DateTime currentDate = request.StartDate;
            var currentEmployee =await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId && x.Active == 1).Select(x => new { x.CostCodeId, x.DepartmentId, x.PositionId, x.EmployerId }).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                var currentShift = await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ShiftId);
                if (currentShift != null)
                {
                    while (currentDate <= request.EndDate)
                    {
                        var currentStatus = await Context.EmployeeStatus.FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId
                            && x.EventDate.Value.Date == currentDate.Date);
                        if (currentStatus != null)
                        {
                            var currentStatusShift =await Context.Shift.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentStatus.ShiftId);
                            if (currentStatusShift?.OnSite == currentShift.OnSite)
                            {
                                currentStatus.ShiftId = request.ShiftId;
                                currentStatus.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                currentStatus.DateUpdated = DateTime.Now;
                                currentStatus.ChangeRoute = $"status shift bulk change";
                                Context.EmployeeStatus.Update(currentStatus);

                            }
                        }
                        else {
                            if (currentShift.OnSite != 1) 
                            {
                                var newData = new EmployeeStatus
                                {
                                    Active = 1,
                                    DepId = currentEmployee.DepartmentId,
                                    CostCodeId = currentEmployee.CostCodeId,
                                    PositionId = currentEmployee.PositionId,
                                    ShiftId = request.ShiftId,
                                    EmployeeId = request.EmployeeId,
                                    EventDate = currentDate,
                                    DateCreated = DateTime.Now,
                                    ChangeRoute = $"status shift bulk change",
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                                };

                                Context.EmployeeStatus.Add(newData);

                            }
                        }

                        currentDate = currentDate.AddDays(1);
                    }

                }
            }

        }






        public async Task<List<CalendarBookingEmployeeResponse>> EmployeeBookingViewCalendar(CalendarBookingEmployeeRequest request, CancellationToken cancellationToken)
        {
            var visualDateRanges = GetVisualStatuDateRange(request.CurrentDate);
            var startDate = visualDateRanges.First();
            var endDate = visualDateRanges.Last();
            var EmployeeStatusData = await Context.EmployeeStatus.Where(x => x.EventDate >= startDate.Date && x.EventDate <= endDate && x.EmployeeId == request.EmployeeId && x.RoomId != null).ToListAsync();
            var returnData = new List<CalendarBookingEmployeeResponse>();

            foreach (var currentDate in visualDateRanges)
            {
                string? color = null;
                int? campId = null;
                string? campName  = null;
                string? CampCode = null;
                string? RoomTypeName = null;
                string? RoomNumber =  null;
                int VirtualRoom = 0;
                var currentEmployeeStatus = EmployeeStatusData.FirstOrDefault(x => x.EventDate == currentDate.Date);
                if (currentEmployeeStatus != null)
                {
                    var currentRoom =await Context.Room.FirstOrDefaultAsync(x => x.Id == currentEmployeeStatus.RoomId);
                    if (currentRoom != null) {
                        var currentCamp =await Context.Camp.FirstOrDefaultAsync(x => x.Id == currentRoom.CampId);
                        var currentRoomType =await Context.RoomType.FirstOrDefaultAsync(x => x.Id == currentRoom.RoomTypeId);
                        RoomNumber = currentRoom.Number;
                        VirtualRoom = currentRoom.VirtualRoom;
                        if (currentCamp != null)
                        {
                            campName = currentCamp.Description;
                            CampCode = currentCamp.Code;

                        }
                        if (currentRoomType != null) { 
                            RoomTypeName = currentRoomType.Description;
                        }
                    }

                    var newData = new CalendarBookingEmployeeResponse
                    {
                        EventDate = currentDate,
                        RoomId = currentEmployeeStatus.RoomId,
                        Id = currentEmployeeStatus.Id,
                        CampName = campName,
                        RoomNumber = RoomNumber,
                        VirtualRoom = VirtualRoom,
                        RoomTypeName = RoomTypeName
                    };
                    returnData.Add(newData);


                }


            }
            return returnData;


        }

        public async Task CalendarRoomAssign(CalendarBookingRoomAssignRequest request, CancellationToken cancellationToken)
        {

            foreach (var item in request.RoomDateStatus)
            {
                var currentData =await Context.EmployeeStatus.FirstOrDefaultAsync(x =>
                x.EmployeeId == request.EmployeeId &&
                x.EventDate == item.EventDate.Date &&
                x.RoomId != null
                );

                if (currentData != null) {

                    if (await CheckRoomDate(item.RoomId, item.EventDate.Date, item.EventDate.Date))
                    {
                        currentData.RoomId = item.RoomId;
                        Context.EmployeeStatus.Update(currentData);
                    }
                    else {
                        throw new BadRequestException($"Sorry, the operation failed. On {item.EventDate.ToShortDateString()}, " +
                               $"it is not possible to enter because the room is full. Please choose another room");
                    }
                }

            }
          await  Task.CompletedTask;
        }


        public async Task ChangeRoomByDateRangeAssign(ChangeRoomByDateRequest request, CancellationToken cancellationToken)
        {

            if (await CheckRoomDate(request.RoomId, request.StartDate.Date, request.EndDate))
            {
                if (request.StartDate <= request.EndDate)
                {

                    var datetData = await Context.EmployeeStatus
                          .Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date >= request.StartDate.Date && x.EventDate <= request.EndDate && x.RoomId != null)
                          .ToListAsync();


                    foreach (var currentData in datetData)
                    {
                        currentData.RoomId = request.RoomId;
                        currentData.ChangeRoute = $"Room assign change";
                        currentData.BedId = await GetBedId(request.RoomId, currentData.EventDate.Value.Date);
                        Context.EmployeeStatus.Update(currentData);

                    }
                }
            }
            else {
                throw new BadRequestException($"Sorry, the operation failed. " +
        $"it is not possible to enter because the room is full. Please choose another room");
            }
        }

        public async Task<ChangeRoomByDatesResponse> ChangeRoomByDatesAssign(ChangeRoomByDatesRequest request, CancellationToken cancellationToken)
        {
            if (request.Dates.Count > 0)
            {

                var currentRoom = await
                    Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId).FirstOrDefaultAsync();

                if (currentRoom != null)
                {
                    var bedList = new List<ChangeRoomByDatesBedData>();
                    foreach (var currentDate in request.Dates)
                    {
                        var currentData = await Context.EmployeeStatus
                          .Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date == currentDate.Date && x.RoomId != null)
                          .FirstOrDefaultAsync();


                        if (currentData != null)
                        {
                            var newBedData = new ChangeRoomByDatesBedData();
                            newBedData.EventDate = currentDate;

                            if (await CheckRoomDate(request.RoomId, currentDate, currentDate ))
                            {
                                currentData.RoomId = request.RoomId;
                                int? bedId = await GetBedId(request.RoomId, currentDate);
                                if (bedId.HasValue)
                                {
                                    if (bedId > 0)
                                    {
                                        currentData.BedId = bedId;

                                        newBedData.BedId = bedId;
                                    }
                                }
                                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                currentData.ChangeRoute = $"Room assign change";
                                Context.EmployeeStatus.Update(currentData);
                                bedList.Add(newBedData);
                            }
                            else
                            {
                                throw new BadRequestException($"Sorry, the operation failed. On {currentDate.ToShortDateString()}," +
                                    $" it is not possible to enter because the room is full. Please choose another room");
                            }

                        }

                    }

                    var returnData = await (from employee in Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId)
                                            join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into DepartmentData
                                            from department in DepartmentData.DefaultIfEmpty()
                                            join employer in Context.Department.AsNoTracking() on employee.EmployerId equals employer.Id into EmployerData
                                            from employer in EmployerData.DefaultIfEmpty()
                                            join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into PeopletypeData
                                            from peopletype in PeopletypeData.DefaultIfEmpty()

                                            select new ChangeRoomByDatesResponse
                                            {
                                                EmployeeId = employee.Id,
                                                Id = employee.Id,
                                                FullName = $"{employee.Firstname} {employee.Lastname}",
                                                RoomOwner = request.RoomId == employee.RoomId,
                                                Firstname = employee.Firstname,
                                                Lastname = employee.Lastname,
                                                Gender = employee.Gender,
                                                HotelCheck = employee.HotelCheck,
                                                SAPID = employee.SAPID,
                                                DepartmentName = department.Name,
                                                EmployerName = employer.Name,
                                                PeopleTypeCode = peopletype.Code

                                            }).FirstOrDefaultAsync();



                    foreach (var item in bedList)
                    {
                        var currentBed = await Context.Bed.AsNoTracking().Where(x => x.Id == item.BedId).FirstOrDefaultAsync();

                        item.RoomNumber = currentRoom.Number;
                        item.BedDescr = currentBed != null ? currentBed.Description : null;
                    }
                    if (returnData != null)
                    {
                        returnData.BedInfo = bedList;
                    }

                    return returnData;
                }
                else
                {
                    throw new BadRequestException($"Sorry, Room not avialable");
                }

            }
            else {

                throw new BadRequestException($"A date must be selected for room movement");
            }
        }

        //private async Task<bool> CheckRoomDate(int roomId, DateTime eventDate)
        //{
        //    var currentRoom =await Context.Room.Where(x => x.Id == roomId)
        //        .Select(x => new { x.BedCount, x.VirtualRoom }).FirstOrDefaultAsync();
        //    if (currentRoom.VirtualRoom != 1)
        //    {
        //        if (currentRoom != null)
        //        {

        //            var dateCountRoom = await Context.EmployeeStatus
        //                  .Where(x => x.EventDate.Value.Date == eventDate.Date
        //                  && x.RoomId == roomId).CountAsync();
        //            if (currentRoom.BedCount > dateCountRoom)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else {
        //        return true;
        //    }
        //}


        #region checkRoomDate


        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, DateTime endDate)
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
                while (currentDate <= endDate)
                {
                    var dateCountRoom = await Context.EmployeeStatus.AsNoTracking()
                        .Where(x => x.EventDate.Value.Date == currentDate.Date && x.RoomId == roomId)
                        .CountAsync();

                    if (currentDate == startDate)
                    {
                        if (currentRoom.BedCount <= dateCountRoom)
                        {
                            var onsiteEmployees = await Context.EmployeeStatus.AsNoTracking().Where(c => c.RoomId == roomId && c.EventDate.Value.Date == currentDate.Date).Select(x => x.EmployeeId).ToListAsync();

                            var datelockCount = await GetDateLockedCount(roomId, currentDate);

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


        private async Task<int> GetDateLockedCount(int RoomId, DateTime currentDate)
        {

            var activeDocumentActions = new List<string> { "Submitted", "Approved" };


            var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                    && x.DocumentTag == "ADD" && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();


            var requestDocumentRoomDate = await (from requestAddList in Context.RequestSiteTravelAdd.AsNoTracking().Where(x => requestDocumentIds.Contains(x.DocumentId) && x.RoomId == RoomId)
                                                 join inschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.inScheduleId equals inschedule.Id /*into inscheduleData
                                                 from inschedule in inscheduleData.DefaultIfEmpty()*/

                                                 join doc in Context.RequestDocument.AsNoTracking() on requestAddList.DocumentId equals doc.Id/*into docData
                                                 from doc in docData.DefaultIfEmpty()*/

                                                 join outschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.outScheduleId equals outschedule.Id /*into outScheduleData
                                                 from outschedule in outScheduleData.DefaultIfEmpty()*/
                                                 select new
                                                 {
                                                     roomId = requestAddList.RoomId,
                                                     EmployeeId = requestAddList.EmployeeId,
                                                     startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
                                                     endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate,
                                                     DocumentId = requestAddList.DocumentId,
                                                     DocumentTag = doc.DocumentTag

                                                 }
                                                ).Where(x => x.startDate.Date <= currentDate && x.endDate > currentDate.Date).ToListAsync();

            return requestDocumentRoomDate.Count();
        }


        #endregion






        private async Task<int?> GetBedId(int RoomId, DateTime EventDate)
        {
          var BedIds =await Context.EmployeeStatus
                .Where(x => x.RoomId == RoomId && x.EventDate.Value.Date == EventDate.Date)
                .Select(x => x.BedId).ToListAsync();

            var bedId =await Context.Bed.Where(x => x.RoomId == RoomId && !BedIds.Contains(x.Id))
                .OrderBy(x => x.Id).Select(x=> x.Id).FirstOrDefaultAsync();
            return bedId;
        }




        public async Task<List<GetDateRangeStatusResponse>> GetDateRangeStatus(GetDateRangeStatusRequest request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate.Date;
            var endDate = startDate.AddMonths(request.DurationMonth);
            var currentEmployee = await Context.Employee.Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
            var returnData = new List<GetDateRangeStatusResponse>();
            if (currentEmployee != null)
            {
                if (currentEmployee.RosterId.HasValue)
                {
                    var rosterDetails = await Context.RosterDetail
                        .Where(x => x.RosterId == currentEmployee.RosterId).ToListAsync();

                    if (!rosterDetails.Any())
                        return returnData;

                    var itms = await EmployeeActiveDates(startDate, endDate, rosterDetails);
                    var employeeOnSiteDates = EmployeeOnsiteDates(itms);
                    var v_startDate = request.StartDate;
                    var v_endDate = request.StartDate.AddMonths(1);
                    if (employeeOnSiteDates.Count > 0)
                    {
                        List<DateTime> sortedDates = employeeOnSiteDates.OrderBy(d => d.Date).ToList();
                        v_startDate = sortedDates.First().Date;
                        v_endDate = sortedDates.Last().Date;

                    }


                    var data = await Context.EmployeeStatus
                      .Where(x => x.RoomId == null && employeeOnSiteDates.Contains(x.EventDate.Value.Date) && x.EmployeeId == request.EmployeeId)
                      .ToListAsync();

                    foreach (var item in data)
                    {
                        //var currentShift = await Context.Shift
                        //    .Where(x => x.Id == item.ShiftId)
                        //    .Select(x => new { x.Id, x.Code, x.OnSite, x.Description })
                        //    .FirstOrDefaultAsync();

                        var currentShift = await (from shift in Context.Shift.Where(c => c.Id == item.ShiftId)
                                                  join shiftColor in Context.Color on shift.ColorId equals shiftColor.Id into shiftColorData
                                                  from shiftColor in shiftColorData.DefaultIfEmpty()
                                                  select new { 
                                                    Code = shift.Code,
                                                    description = shift.Description,
                                                    color = shiftColor.Code
                                                  }).FirstOrDefaultAsync();



                        if (currentShift?.Code == "AN")
                        {
                            var newData = new GetDateRangeStatusResponse
                            {
                                Id = item.Id,
                                EvenDate = item.EventDate.Value.Date,
                                ShiftCode = currentShift.Code,
                                Description = currentShift.description,
                                Color = currentShift.color
                            };
                            returnData.Add(newData);
                        }

                    }
                }
                else {
                    var data = await Context.EmployeeStatus
                      .Where(x => x.RoomId == null 
                        && x.EventDate >= startDate
                        && x.EventDate <= endDate 
                        && x.EmployeeId == request.EmployeeId)
                      .ToListAsync();

                    foreach (var item in data)
                    {
                        var currentShift = await Context.Shift
                            .Where(x => x.Id == item.ShiftId)
                            .Select(x => new { x.Id, x.Code, x.OnSite, x.Description })
                            .FirstOrDefaultAsync();

                        if (currentShift?.Code == "AN")
                        {
                            var newData = new GetDateRangeStatusResponse
                            {
                                Id = item.Id,
                                EvenDate = item.EventDate.Value.Date,
                                ShiftCode = currentShift.Code,
                                Description = currentShift.Description
                            };
                            returnData.Add(newData);
                        }

                    }
                }
            }

            return returnData;
        }

        #region ProfileBYDates


        public async Task<List<GetDateRangeProfileStatusResponse>> GetDateRangeProfileStatus(GetDateRangeProfileStatusRequest request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate;
            var returnData =new List<GetDateRangeProfileStatusResponse>();
            var result = await (from employee in Context.EmployeeStatus.AsNoTracking()
                                                .Where(es => es.EmployeeId == request.EmployeeId &&
                                                 es.EventDate >= startDate &&
                                                 es.EventDate <= endDate &&
                                                 es.RoomId != null)
                                join department in Context.Department.AsNoTracking() on employee.DepId equals department.Id into departmentData
                                from department in departmentData.DefaultIfEmpty()
                                join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                from employer in employerData.DefaultIfEmpty()
                                join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                from position in positionData.DefaultIfEmpty()
                                join costcode in Context.CostCodes.AsNoTracking() on employee.CostCodeId equals costcode.Id into costcodeData
                                from costcode in costcodeData.DefaultIfEmpty()
                                join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into RoomData
                                from room in RoomData.DefaultIfEmpty()
                                join camp in Context.Camp.AsNoTracking() on room.CampId equals camp.Id into campData
                                from camp in campData.DefaultIfEmpty()
                                select new GetDateRangeProfileStatusResponse
                                {
                                    Id = employee.Id,
                                    CostCode = $"{costcode.Number} {costcode.Description}",
                                    Department = department.Name,
                                    Employer = employer.Description,
                                    EventDate = employee.EventDate,
                                    EmployeeId = employee.EmployeeId,
                                    Location = string.Empty,
                                    Position = position.Description,
                                    CampRoom = $"{camp.Description} {room.Number}"
                                }).OrderBy(x=> x.EventDate).ToListAsync(cancellationToken);


            GetDateRangeProfileStatusResponse beforeRow = null;
            foreach (var item in result)
            {
                if (beforeRow == null)
                {
                    returnData.Add(item);
                    beforeRow = item;
                }
                else {

                    if (beforeRow.EventDate.HasValue)
                    {
                        var beforerowEventDate = beforeRow.EventDate.Value.Date;
                        if (item.EventDate.HasValue)
                        {
                            if (beforerowEventDate.AddDays(1) == item.EventDate.Value.Date)
                            {
                                if (beforeRow.CostCode != item.CostCode || beforeRow.CampRoom != item.CampRoom || beforeRow.Department != item.Department
                                    || beforeRow.Employer != item.Employer || beforeRow.Location != item.Location || beforeRow.Position != item.Position)
                                {
                                    returnData.Add(item);
                                    
                                }
                            }
                            else {
                                returnData.Add(item);   
                            }
                        }
                    }

                    beforeRow = item;

                }
            }

            return returnData;

            
            //var groupedResult = result.GroupBy(item => new
            //{
            //    item.EventDate,
            //    item.Department,
            //    item.Employer,
            //    item.Location,
            //    item.Position,
            //    item.CampRoom,
            //    item.CostCode
            //})
            //    .OrderBy(group => group.Key.EventDate)
            //    .Select(group => new GetDateRangeProfileStatusResponse
            //    {
            //       EventDate = group.Key.EventDate,
            //       Department = group.Key.Department,
            //       Employer = group.Key.Employer,
            //       Location = group.Key.Location,
            //       Position = group.Key.Position,
            //       CampRoom = group.Key.CampRoom,
            //      CostCode = group.Key.CostCode
            //    });



           // return result;
        }

        #endregion


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

        private async Task<List<BulkRosterActiveDate>> EmployeeActiveDates(DateTime startDate, DateTime endDate, List<RosterDetail> rosterDetails)
        {
            var cycleDates = new List<BulkRosterActiveDate>();
            DateTime currentDate = startDate;

            var DateCycles = rosterDetails.Select(x => new { x.DaysOn.Value, x.ShiftId }).ToList();

            if (DateCycles.Count > 0)
            {
                int cycleIndex = 0;
                while (currentDate <= endDate)
                {
                    string Direction = "IN";
                    int onsite = 1;
                    var currentShift =await Context.Shift.Where(x => x.Id == DateCycles[cycleIndex].ShiftId).Select(x => new { x.OnSite }).FirstOrDefaultAsync();
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


                while (cycleIndex <= DateCycles.Count)
                {
                    cycleIndex++;
                    if (cycleIndex <= DateCycles.Count)
                    {
                        string Direction = "IN";
                        int onsite = 1;
                        var currentShift =await Context.Shift.Where(x => x.Id == DateCycles[cycleIndex - 1].ShiftId).Select(x => new { x.OnSite }).FirstOrDefaultAsync();
                        if (currentShift?.OnSite != 1)
                        {
                            Direction = "OUT";
                            onsite = 0;
                        }
                        if (cycleIndex == DateCycles.Count)
                        {
                            cycleDates.Add(new BulkRosterActiveDate { EventDate = currentDate, Direction = Direction, ShiftId = DateCycles[cycleIndex - 1].ShiftId.Value, OnSite = onsite });
                        }
                        else
                        {
                            int cycle = DateCycles[cycleIndex - 1].Value;
                            cycleDates.Add(new BulkRosterActiveDate { EventDate = currentDate, Direction = Direction, ShiftId = DateCycles[cycleIndex - 1].ShiftId.Value, OnSite = onsite });
                            currentDate = currentDate.AddDays(cycle);

                        }
                    }


                }
            }

            for (int i = 0; i < cycleDates.Count; i++)
            {

                if (i + 1 < cycleDates.Count)
                {
                    var cycleStartDate = cycleDates[i].EventDate;
                    var cycleEndDate = cycleDates[i + 1].EventDate;
                    var itemDates = new List<BulkRosterActiveDateDetail>();
                    while (cycleStartDate < cycleEndDate)
                    {
                        var newData = new BulkRosterActiveDateDetail
                        {
                            EventDate = cycleStartDate,
                            OnSite = cycleDates[i].OnSite,
                            ShifId = cycleDates[i].ShiftId
                        };
                        itemDates.Add(newData);
                        cycleStartDate = cycleStartDate.AddDays(1);
                    }

                    cycleDates[i].details = itemDates;
                }

            }
            return cycleDates;
        }



        #region BulkShiftChange
      public  async Task<List<VisualStatusDateChangeBulkResponse>> VisualStatusDateChangeBulk(VisualStatusDateChangeBulkRequest request, CancellationToken cancellationToken)
        {

            var employeeData = await Context.Employee.AsNoTracking().Where(x => request.EmployeeIds.Contains(x.Id))
                .Select(x => new { x.Id, x.CostCodeId, x.Lastname, x.Firstname, x.DepartmentId, x.PositionId, x.EmployerId, x.Active }).ToListAsync(cancellationToken);
            var returnData = new List<VisualStatusDateChangeBulkResponse>();
            List<int> skippedEmployeesIds = new List<int>();

            var newRequestShift = await Context.Shift.AsNoTracking().Where(x => x.Id == request.ShiftId).FirstOrDefaultAsync();
            foreach (var itemEmployee in employeeData)
            {
                var currentEmployee = employeeData.Where(x => x.Id == itemEmployee.Id).Select(x => new { x.Id, x.Lastname, x.Firstname, x.CostCodeId, x.DepartmentId, x.Active, x.PositionId, x.EmployerId }).FirstOrDefault();
                var skippedDates = new List<DateTime>();
                if (currentEmployee != null && currentEmployee.Active == 1)
                {
                    DateTime currentDate = request.startDate;
                    while (currentDate <= request.endDate)
                    {

                        if (newRequestShift.OnSite == 1)
                        {
                            var currentStatus = await Context.EmployeeStatus.FirstOrDefaultAsync(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == currentEmployee.Id && x.RoomId != null);
                            if (currentStatus != null)
                            {
                                currentStatus.ShiftId = request.ShiftId;
                                currentStatus.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                currentStatus.DateUpdated = DateTime.Now;
                                currentStatus.ChangeRoute = $"Bulk shift change from profile";
                                Context.EmployeeStatus.Update(currentStatus);
                            }
                            else
                            {
                                skippedDates.Add(currentDate);
                            }

                        }
                        else
                        {
                            var currentStatus = await Context.EmployeeStatus.FirstOrDefaultAsync(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == currentEmployee.Id);
                            if (currentStatus == null)
                            {
                                var newData = new EmployeeStatus
                                {
                                    Active = 1,
                                    DepId = currentEmployee.DepartmentId,
                                    CostCodeId = currentEmployee.CostCodeId,
                                    PositionId = currentEmployee.PositionId,
                                    ShiftId = request.ShiftId,
                                    EventDate = currentDate.Date,
                                    DateCreated = DateTime.Now,
                                    EmployeeId = currentEmployee.Id,
                                    ChangeRoute = $"Bulk shift change from profile",
                                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                                };

                                Context.EmployeeStatus.Add(newData);
                            }
                            else
                            {
                                if (currentStatus.RoomId == null)
                                {
                                    currentStatus.ShiftId = request.ShiftId;
                                    currentStatus.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    currentStatus.DateUpdated = DateTime.Now;
                                    currentStatus.ChangeRoute = $"Bulk shift change from profile";
                                    Context.EmployeeStatus.Update(currentStatus);
                                }
                                else
                                {
                                    skippedDates.Add(currentDate);
                                }
                            }
                        }
                        currentDate = currentDate.AddDays(1);
                    }

                    if (skippedDates.Count > 0)
                    {
                        if (newRequestShift?.OnSite == 1)
                        {


                            List<string> continuousRanges = FindContinuousRanges(skippedDates);




                            VisualStatusDateChangeBulkResponseReson reson = new VisualStatusDateChangeBulkResponseReson
                            {
                                Name = "Off site days",
                                Days = continuousRanges
                            };


                            returnData.Add(new VisualStatusDateChangeBulkResponse { Id = currentEmployee.Id, FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}", SkippedReason = reson });

                        }
                        else
                        {


                            List<string> continuousRanges = FindContinuousRanges(skippedDates);


                            VisualStatusDateChangeBulkResponseReson reson = new VisualStatusDateChangeBulkResponseReson
                            {
                                Name = "On site days",
                                Days = continuousRanges
                            };


                            //  string dates = string.Join(", ", continuousRanges.Select(date => date.Item1.ToString("yyyy-MM-dd")) );
                            returnData.Add(new VisualStatusDateChangeBulkResponse { Id = currentEmployee.Id, FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}", SkippedReason = reson });
                        }
                    }



                    else
                    {
                        returnData.Add(new VisualStatusDateChangeBulkResponse { Id = currentEmployee.Id, FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}", SkippedReason = new VisualStatusDateChangeBulkResponseReson { Name = "Completed" } });
                    }

                }

            }


            return returnData;



        }


        private  List<string> FindContinuousRanges(List<DateTime> dates)
        {
            dates.Sort(); // Ensure dates are sorted
            var ranges = new List<string>();
            DateTime start = dates[0];
            DateTime end = start;

            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i].Date == end.Date.AddDays(1))
                {
                    end = dates[i];
                }
                else
                {
                    ranges.Add(start.ToString("yyyy-MM-dd") + " - " +   end.ToString("yyyy-MM-dd"));
                    start = dates[i];
                    end = start;
                }
            }
            ranges.Add(start.ToString("yyyy-MM-dd") + " - " + end.ToString("yyyy-MM-dd"));

            return ranges;
        }




        public async Task<DateLastEmployeeStatusResponse> DateLastEmployeeStatus(DateLastEmployeeStatusRequest request, CancellationToken cancellationToken)
        {
            var shiftData =await Context.Shift.AsNoTracking().ToListAsync();
            var returnData = new DateLastEmployeeStatusResponse();
            if (request.Onsite == 1)
            {
                if (request.laststatus)
                {

                    var lasteEmployeeOnsiteStatus = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date <= request.EventDate.Date && x.RoomId != null).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();
                    if (lasteEmployeeOnsiteStatus != null)
                    {
                        var datass = lasteEmployeeOnsiteStatus.EventDate;
                        var currentShift = shiftData.Where(x => x.Id == lasteEmployeeOnsiteStatus.ShiftId && x.OnSite == 1).FirstOrDefault();
                        returnData.EmployerId = lasteEmployeeOnsiteStatus.EmployerId;
                        returnData.ShiftId = currentShift?.Id;
                        returnData.DepartmentId = lasteEmployeeOnsiteStatus.DepId;
                        returnData.CostCodeId = lasteEmployeeOnsiteStatus.CostCodeId;

                        return returnData;

                    }
                    else
                    {
                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
                        if (currentEmployee != null)
                        {
                            var currentShift = shiftData.Where(x => x.Code == "DS").FirstOrDefault();
                            returnData.EmployerId = currentEmployee.EmployerId;
                            returnData.ShiftId = currentShift?.Id;
                            returnData.DepartmentId = currentEmployee.DepartmentId;
                            returnData.CostCodeId = currentEmployee.CostCodeId;
                            return returnData;

                        }
                        else
                        {
                            throw new BadRequestException("Employee not found");
                        }
                    }
                }
                else {
                    var lasteEmployeeOnsiteStatus = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date >= request.EventDate.Date && x.RoomId != null).OrderBy(x => x.EventDate).FirstOrDefaultAsync();
                    if (lasteEmployeeOnsiteStatus != null)
                    {
                        var datass = lasteEmployeeOnsiteStatus.EventDate;
                        var currentShift = shiftData.Where(x => x.Id == lasteEmployeeOnsiteStatus.ShiftId && x.OnSite == 1).FirstOrDefault();
                        returnData.EmployerId = lasteEmployeeOnsiteStatus.EmployerId;
                        returnData.ShiftId = currentShift?.Id;
                        returnData.DepartmentId = lasteEmployeeOnsiteStatus.DepId;
                        returnData.CostCodeId = lasteEmployeeOnsiteStatus.CostCodeId;

                        return returnData;

                    }
                    else
                    {
                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
                        if (currentEmployee != null)
                        {
                            var currentShift = shiftData.Where(x => x.Code == "DS").FirstOrDefault();
                            returnData.EmployerId = currentEmployee.EmployerId;
                            returnData.ShiftId = currentShift?.Id; 
                            returnData.CostCodeId = currentEmployee.CostCodeId;
                            return returnData;

                        }
                        else
                        {
                            throw new BadRequestException("Employee not found");
                        }
                    }
                }


            }
            else {

                if (request.laststatus)
                {
                    var lasteEmployeeOffSiteStatus = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date <= request.EventDate.Date && x.RoomId == null).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();
                    if (lasteEmployeeOffSiteStatus != null)
                    {
                        var currentShift = shiftData.Where(x => x.Id == lasteEmployeeOffSiteStatus.ShiftId && x.OnSite != 1).FirstOrDefault();
                        returnData.EmployerId = lasteEmployeeOffSiteStatus.EmployerId;
                        returnData.ShiftId = currentShift?.Id;
                        returnData.DepartmentId = lasteEmployeeOffSiteStatus.DepId;
                        returnData.CostCodeId = lasteEmployeeOffSiteStatus.CostCodeId;

                        return returnData;
                    }
                    else
                    {
                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
                        if (currentEmployee != null)
                        {
                            var currentShift = shiftData.Where(x => x.Code == "RR").FirstOrDefault();
                            returnData.EmployerId = currentEmployee.EmployerId;
                            returnData.ShiftId = currentShift?.Id;
                            returnData.DepartmentId = currentEmployee.DepartmentId;
                            returnData.CostCodeId = currentEmployee.CostCodeId;
                            return returnData;

                        }
                        else
                        {
                            throw new BadRequestException("Employee not found");
                        }
                    }
                }
                else {
                    var lasteEmployeeOffSiteStatus = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date >= request.EventDate.Date && x.RoomId == null).OrderBy(x => x.EventDate).FirstOrDefaultAsync();
                    if (lasteEmployeeOffSiteStatus != null)
                    {
                        var currentShift = shiftData.Where(x => x.Id == lasteEmployeeOffSiteStatus.ShiftId && x.OnSite != 1).FirstOrDefault();
                        returnData.EmployerId = lasteEmployeeOffSiteStatus.EmployerId;
                        returnData.ShiftId = currentShift?.Id;
                        returnData.DepartmentId = lasteEmployeeOffSiteStatus.DepId;
                        returnData.CostCodeId = lasteEmployeeOffSiteStatus.CostCodeId;

                        return returnData;
                    }
                    else
                    {
                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
                        if (currentEmployee != null)
                        {
                            var currentShift = shiftData.Where(x => x.Code == "RR").FirstOrDefault();
                            returnData.EmployerId = currentEmployee.EmployerId;
                            returnData.ShiftId = currentShift?.Id;
                            returnData.DepartmentId = currentEmployee.DepartmentId;
                            returnData.CostCodeId = currentEmployee.CostCodeId;
                            return returnData;

                        }
                        else
                        {
                            throw new BadRequestException("Employee not found");
                        }
                    }
                }


            }
        }




        #endregion



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
        }


    }
}
