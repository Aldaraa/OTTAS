using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DashboardFeature.RoomDashboard;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignment;
using tas.Application.Features.RoomAssignmentFeature.CreateRoomAssignmentOwnership;
using tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment;
using tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze;
using tas.Application.Features.RoomFeature.CreateRoomAssignment;
using tas.Application.Features.RoomFeature.CreateRoomAssignmentOwnership;
using tas.Application.Features.RoomFeature.FindAvailableByDates;
using tas.Application.Features.RoomFeature.GetAllActiveRoomAssignment;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Features.RoomFeature.GetRoomAssignAvialable;
using tas.Application.Features.RoomFeature.RemoveRoomAssignmentOwnership;
using tas.Application.Features.RoomFeature.RoomAssignmentEmployeeInfo;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class RoomAssignmentRepository : BaseRepository<RoomAssignment>, IRoomAssignmentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RoomAssignmentRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }

        


        public async Task<List<GetAllActiveRoomAssignmentResponse>> GetAllActiveRooms(GetAllActiveRoomAssignmentRequest request, CancellationToken cancellationToken)
        {
            var result = await Context.Room
                    .Join(Context.Camp,
                        room => room.CampId,
                        camp => camp.Id,
                        (room, Camp) => new { room, Camp }
                        ).Join(
                         Context.RoomType,
                         room => room.room.RoomTypeId,
                         roomtype => roomtype.Id,
                         (room, roomtype) => new GetAllActiveRoomAssignmentResponse
                         {
                             Id = room.room.Id,
                             Number = room.room.Number,
                             CampName = room.Camp.Description,
                             BedCount = room.room.BedCount,
                             Private = room.room.Private,
                             Active = room.room.Active,
                             RoomTypeName = roomtype.Description,
                             VirtualRoom = room.room.VirtualRoom,
                             CampId = room.Camp.Id,
                             DateCreated = room.room.DateCreated,
                             DateUpdated = room.room.DateUpdated,
                             RoomTypeId = room.room.RoomTypeId,
                             EmployeeCount = Context.Employee.Count(x => x.RoomId == room.room.Id && x.Active == 1)
                         }).Where(x=> x.BedCount > x.EmployeeCount && x.VirtualRoom != 1).ToListAsync(cancellationToken);

            return result;

        }



        public async Task<List<CreateRoomAssignmentOwnershipResponse>> SaveOwnershipRoom(CreateRoomAssignmentOwnershipRequest request, CancellationToken cancellationToken)
        {
            var assignEmployee = await Context.Employee.Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync(cancellationToken);
            var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId).FirstOrDefaultAsync(cancellationToken);
            var employees = await Context.Employee.AsNoTracking().Where(x => x.RoomId == request.RoomId).ToListAsync(cancellationToken);
            var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
            var empIds = employees.Select(x => x.Id).ToList();
            var guestList = new List<AssignRoomGuests>();
            var guestEmpIds = new List<int>();
            var returnData = new List<CreateRoomAssignmentOwnershipResponse>();
            if (virtualRoom == null) {
                throw new BadRequestException("Please register virtual room");
            }

            if (assignEmployee != null)
            {

                if (currentRoom != null)
                {

                    if (assignEmployee.RoomId == currentRoom.Id)
                    {
                        throw new BadRequestException("The ownership of the room has already been assigned to this employee.");
                    }

                    var employeeOnsiteDates =await Context.EmployeeStatus.Where(x => x.EmployeeId == request.EmployeeId && x.RoomId != null && x.EventDate.Value.Date >= request.startDate.Date).ToListAsync();
                    foreach (var item in employeeOnsiteDates)
                    {

                        var dateRoomStatus = await CheckRoomDate(request.RoomId, item.EventDate.Value, request.EmployeeId);
                        if (dateRoomStatus) {
                            item.RoomId = request.RoomId;

                            var bedId = await GetBedId(item.EventDate.Value.Date, request.RoomId);
                            item.BedId = bedId == 0 ? null : bedId;
                            item.ChangeRoute = "Room assign change ownership";
                            item.DateUpdated = DateTime.Now;
                            item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            Context.EmployeeStatus.Update(item);
                        }
                        else { 
                            item.RoomId = virtualRoom.Id;
                            item.BedId = null;
                            item.DateUpdated = DateTime.Now;
                            item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            item.ChangeRoute = "Room assign change ownership";
                            Context.EmployeeStatus.Update(item);
                        }


                       // var eventDateOccupancy = await Context.EmployeeStatus.Where(x => x.RoomId == request.RoomId && x.EventDate.Value.Date == item.EventDate.Value.Date).ToListAsync();





                    }

                    assignEmployee.RoomId = request.RoomId;
                    Context.Employee.Update(assignEmployee);
                    return new List<CreateRoomAssignmentOwnershipResponse>();
                    //return returnData;


                    //if (guestList.Count == 0)
                    //{
                    //    foreach (var item in employeeOnsiteDates)
                    //    {

                    //        var bedId = await GetBedId(item.EventDate.Value.Date, request.RoomId);


                    //        item.RoomId = request.RoomId;
                    //        item.BedId =  bedId == 0 ? null : bedId ; 
                    //        item.ChangeRoute = "Room assign change ownership";
                    //        item.DateUpdated = DateTime.Now;
                    //        item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    //        Context.EmployeeStatus.Update(item);
                    //    }

                    //    assignEmployee.RoomId = request.RoomId;
                    //    assignEmployee.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    //    assignEmployee.DateUpdated = DateTime.Now;
                    //    Context.Employee.Update(assignEmployee);
                    //    return returnData;
                    //}
                    //else
                    //{

                    //    foreach (var item in guestList)
                    //    {
                    //        var newRecord = new CreateRoomAssignmentOwnershipResponse();
                    //        newRecord.EventDate = item.EventDate.Value.Date;



                    //        var retData = await (from employee in Context.Employee.Where(c => item.GuestIds.Contains(c.Id))
                    //                             join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                    //                             from department in departmentData.DefaultIfEmpty()
                    //                             join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                    //                             from employer in employerData.DefaultIfEmpty()
                    //                             join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                    //                             from position in positionData.DefaultIfEmpty()
                    //                             join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                    //                             from peopletype in peopletypeData.DefaultIfEmpty()
                    //                             select new CreateRoomAssignmentOwnershipGuest
                    //                             {
                    //                                 EmployeeId = employee.Id,
                    //                                 Firstname = employee.Firstname,
                    //                                 Lastname = employee.Lastname,
                    //                                 Gender = employee.Gender,
                    //                                 SAPID = employee.SAPID,
                    //                                 DepartmentName = department.Name,
                    //                                 EmployerName = employer.Description,
                    //                                 PeopleTypeName = peopletype.Code,
                    //                                 PositionName = position.Description

                    //                             }).ToListAsync();

                    //        newRecord.Guests = retData;

                    //        returnData.Add(newRecord);

                    //    }


                    //    return returnData;

                    //}
                }
                else {
                    throw new BadRequestException("There are no records for the Room.");
                }

            }
            else
            {
                throw new BadRequestException("There are no records for the specified employee.");

            }
        }


        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, int EmployeeId)
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
                        .Where(x => x.EventDate.Value.Date == startDate.Date && x.RoomId == roomId && x.EmployeeId != EmployeeId)
                        .CountAsync();

                if (sessionRoomCount == 0)
                {
                    return true;
                }
                else {

                    var dateCountRoom = await Context.EmployeeStatus.Where(x => x.EmployeeId != EmployeeId).AsNoTracking()
                        .Where(x => x.EventDate.Value.Date == currentDate.Date && x.RoomId == roomId)
                        .CountAsync();


                    if (currentRoom.BedCount <= dateCountRoom)
                    {
                        var onsiteEmployees = await Context.EmployeeStatus.AsNoTracking().Where(c => c.RoomId == roomId && c.EventDate.Value.Date == currentDate.Date && c.EmployeeId != EmployeeId).Select(x => x.EmployeeId).ToListAsync();
                        if (onsiteEmployees.Count > 0)
                        {
                            var tomorrowOut = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date == currentDate.AddDays(1).Date && /*(*/onsiteEmployees.Contains(x.EmployeeId)/* || x.EmployeeId == EmployeeId)*/ && x.Direction == "OUT").ToListAsync();
                            if (tomorrowOut.Count == 0)
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
                        return true;
                    }

                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }






        private async Task<int?> GetAvailableBeds(int roomId, DateTime eventDate)
        {
            var statusRoomDate = await Context.EmployeeStatus.Where(x => x.EventDate == eventDate.Date && x.RoomId == roomId).ToListAsync();

            if (statusRoomDate != null)
            {
                var currentRoom = await Context.Room.FirstOrDefaultAsync(x => x.Id == roomId);
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
        public async Task<List<FindAvailableByDatesAssignmentResponse>> FindAvailableByDatesAssignment(FindAvailableByDatesAssignmentRequest request, CancellationToken cancellationToken)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            if (!request.CampId.HasValue && string.IsNullOrEmpty(request.RoomNumber))
            {
                throw new BadRequestException("Please ensure both 'Camp' and 'Room Number' fields are filled out.");
            }
            var campRooms = await Context.Room.AsNoTracking().Where(x=> x.Active == 1).ToListAsync(cancellationToken);


            stopWatch.Stop();
            Console.WriteLine($"CAMP ROOMS END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            if (request.CampId.HasValue)
            {

                campRooms = campRooms
                   .Where(x => x.CampId == request.CampId && x.Active == 1 && x.VirtualRoom != 1)
                  .ToList();

            }
            if (request.RoomTypeId.HasValue)
            {
                campRooms = campRooms.Where(x => x.RoomTypeId == request.RoomTypeId).ToList();
            }
            if (request.Private.HasValue)
            {
                campRooms = campRooms.Where(x => x.Private == request.Private).ToList();
            }
            if (!string.IsNullOrEmpty(request.RoomNumber))
            {
                campRooms = campRooms.Where(x => x.Number.ToLower().Contains(request.RoomNumber.Trim().ToLower())).ToList();
            }
            if (request.BedCount.HasValue)
            {
                if (request.BedCount > 0)
                {
                    campRooms = campRooms.Where(x => x.BedCount == request.BedCount).ToList();
                }
            }

            var campRoomIds = campRooms.Select(x => x.Id);



            //var NotHoteRoomIds = Context.Employee
            //    .Where(e => e.HotelCheck != 1 && e.RoomId != null)
            //    .Select(e => e.RoomId.Value)
            //    .Distinct()
            //    .ToList();


            var NotHotelRoomIds = await (from r in Context.Room
                                         join e in Context.Employee on r.Id equals e.RoomId
                                         where e.HotelCheck != 1
                                         select r.Id
                                        ).ToListAsync();


            var statusRoomDate = await Context.EmployeeStatus.AsNoTracking().AsNoTracking()
                .Where(x => x.EventDate >= request.startDate && x.EventDate <= request.endDate && x.RoomId != null && campRoomIds.Contains(x.RoomId.Value))
                .ToListAsync();

            var EmptyRooms = campRooms.Where(x => !statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x => new { x.Id, x.BedCount });
            var NonEmptyRooms = campRooms.Where(x => statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x => new { x.Id, x.BedCount });


            List<int> RemoveRoomIds = new List<int>();
            var returnData = new List<FindAvailableByDatesAssignmentResponse>();



            var activeDocumentActions = new List<string> { "Submitted", "Approved" };

            stopWatch.Stop();
            Console.WriteLine($"FILTER END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
            var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                    && x.DocumentTag == "ADD" && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();


            //var requestDocumentRoomDate = await (from requestAddList in Context.RequestSiteTravelAdd.AsNoTracking().Where(x => requestDocumentIds.Contains(x.DocumentId) && campRoomIds.Contains(x.RoomId.Value))
            //                                     join inschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.inScheduleId equals inschedule.Id into inscheduleData
            //                                     from inschedule in inscheduleData.DefaultIfEmpty()

            //                                     join doc in Context.RequestDocument on requestAddList.DocumentId equals doc.Id into docData
            //                                     from doc in docData.DefaultIfEmpty()

            //                                     join outschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.outScheduleId equals outschedule.Id into outScheduleData
            //                                     from outschedule in outScheduleData.DefaultIfEmpty()
            //                                     select new
            //                                     {
            //                                         roomId = requestAddList.RoomId,
            //                                         EmployeeId = requestAddList.EmployeeId,
            //                                         startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
            //                                         endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate,
            //                                         DocumentId = requestAddList.DocumentId,
            //                                         DocumentTag = doc.DocumentTag

            //                                     }
            //                                    ).ToListAsync();
            //stopWatch.Stop();
            //Console.WriteLine($"REQUEST DOCUMENT ROOMD END ==================> {stopWatch.ElapsedMilliseconds}");
            //stopWatch.Restart();

            foreach (var room in campRooms)
            {
                DateTime currentDate = request.startDate;

                var currentRoomStatus = await CheckRoomDate(room.Id, request.startDate, request.endDate);
                if (!currentRoomStatus)
                {
                    if (!RemoveRoomIds.Any(x => x == room.Id))
                    {
                        RemoveRoomIds.Add(room.Id);
                    }
                }
                else {
                    var newRecord = new FindAvailableByDatesAssignmentResponse
                    {
                        BedCount = room.BedCount,
                        RoomId = room.Id,
                        roomNumber = room.Number,
                        VirtulRoom = 0,

                    };

                    returnData.Add(newRecord);
                    currentDate = currentDate.AddDays(1);
                }


                //while (currentDate <= request.endDate)
                //{
                //    int activeBed = room.BedCount;

                //    if (!EmptyRooms.Any(x => x.Id == room.Id))
                //    {
                //        var lockedCount2 = requestDocumentRoomDate.Where(x => x.roomId == room.Id && x.startDate <= currentDate && x.endDate > currentDate).Count();
                //        var DateEmployeeCount = statusRoomDate.Count(x => x.RoomId == room.Id && x.EventDate.Value.Date == currentDate.Date);
                //        var lockedCount = 0; //await GetDateLockedCount(room.Id, currentDate.Date);
                //        activeBed = room.BedCount - DateEmployeeCount - lockedCount2;
                //        if (activeBed < 1)
                //        {
                //            if (!RemoveRoomIds.Any(x => x == room.Id))
                //            {
                //                RemoveRoomIds.Add(room.Id);
                //            }
                //        }

                //    }
                //    var newRecord = new FindAvailableByDatesAssignmentResponse
                //    {
                //        BedCount = room.BedCount,
                //        RoomId = room.Id,
                //        roomNumber = room.Number,
                //        VirtulRoom = 0,
                        
                //    };

                //    returnData.Add(newRecord);
                //    currentDate = currentDate.AddDays(1);
                //}
            }


            stopWatch.Stop();
            Console.WriteLine($"SEARC FOREACH  END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();




            if (RemoveRoomIds.Count > 0)
            {
                returnData = returnData.Where(x => !RemoveRoomIds.Contains(x.RoomId.Value)).ToList();
            }

            if (NotHotelRoomIds.Count > 0)
            {
                returnData = returnData.Where(x => x.RoomId.HasValue && !NotHotelRoomIds.Contains(x.RoomId.Value)).ToList();
            }

            returnData = returnData.Distinct().ToList();


            foreach (var item in returnData)
            {
                var ownerIds = await Context.Employee.AsNoTracking().Where(x => x.RoomId == item.RoomId).Select(x=> x.Id).ToListAsync();
                DateTime? ownerDateIn =null;


                if (ownerIds.Count > 0)
                {
                    var dates = await Context.Transport.Where(x => x.EventDate >= request.startDate && ownerIds.Contains(x.EmployeeId.Value) && x.Direction == "IN").OrderBy(x => x.EventDate).FirstOrDefaultAsync();

                    if (dates != null)
                    {
                        ownerDateIn = dates.EventDateTime;
                    }


                }
                var roomData = await (from roomD in Context.Room.Where(x => x.Id == item.RoomId && x.VirtualRoom != 1)
                                      join camp in Context.Camp on roomD.CampId equals camp.Id into campData
                                      from camp in campData.DefaultIfEmpty()
                                      join roomtype in Context.RoomType on roomD.CampId equals roomtype.Id into roomTypeData
                                      from roomtype in roomTypeData.DefaultIfEmpty()
                                      select new FindAvailableByDatesAssignmentResponse
                                      {
                                          RoomId = item.RoomId,
                                          BedCount = item.BedCount,
                                          VirtulRoom = 0,
                                          Employees = 0,
                                          roomNumber = item.roomNumber,
                                          RoomOwners = ownerIds.Count,
                                          OwnerInDate = ownerDateIn,  
                                          Descr = $"{item.roomNumber}({item.BedCount})/{roomtype.Description}/{camp.Description}/"
                                      }).FirstOrDefaultAsync();

                if (roomData != null)
                { 
                    item.Descr = roomData.Descr;
                    item.roomNumber = item.roomNumber;
                    item.BedCount = roomData.BedCount;
                    item.VirtulRoom = 0;
                    item.RoomOwners = roomData.RoomOwners;
                    item.OwnerInDate = roomData.OwnerInDate;
                 




                }

                

                int EmployeeCount = await Context.EmployeeStatus.AsNoTracking().Where(x => x.RoomId == item.RoomId && x.EventDate.Value.Date >= request.startDate.Date
                   && x.EventDate.Value.Date <= request.endDate).Select(x => x.EmployeeId).Distinct().CountAsync();
                item.Employees = EmployeeCount;
            }

            var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
            if (virtualRoom != null)
            {

                var vRoomOnsiteEmployees = await Context.EmployeeStatus.AsNoTracking().Where(x => x.RoomId == virtualRoom.Id && x.EventDate.Value.Date >= request.startDate.Date
                && x.EventDate.Value.Date <= request.endDate).Select(x => x.EmployeeId).Distinct().CountAsync();
                returnData.Insert(0, new FindAvailableByDatesAssignmentResponse { RoomId = virtualRoom.Id, roomNumber = virtualRoom.Number, VirtulRoom = 1, Employees = vRoomOnsiteEmployees, BedCount = virtualRoom.BedCount, Descr = virtualRoom.Number });
            }

            stopWatch.Stop();
            Console.WriteLine($"---------------------------------- END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            return await Task.FromResult(returnData.Distinct().ToList());





























            //IQueryable<Room> campRoomsQuery = Context.Room.AsQueryable();

            //if (!string.IsNullOrEmpty(request.RoomNumber))
            //{
            //    campRoomsQuery = campRoomsQuery.Where(x => x.Number.ToLower().Contains(request.RoomNumber.ToLower()));
            //}

            //if (request.CampId.HasValue)
            //{
            //    campRoomsQuery = campRoomsQuery.Where(x => x.CampId == request.CampId && x.Active == 1 && x.VirtualRoom != 1);
            //}

            //if (request.RoomTypeId.HasValue)
            //{
            //    campRoomsQuery = campRoomsQuery.Where(x => x.RoomTypeId == request.RoomTypeId);
            //}

            //if (request.Private.HasValue)
            //{
            //    campRoomsQuery = campRoomsQuery.Where(x => x.Private == request.Private);
            //}

            //if (request.BedCount.HasValue)
            //{
            //    if (request.BedCount > 0)
            //    {
            //        campRoomsQuery = campRoomsQuery.Where(x => x.BedCount == request.BedCount);
            //    }
                
            //}



            //var campRoomIds = await campRoomsQuery.Select(x => x.Id).ToListAsync(cancellationToken);

            //var statusRoomDate = await Context.EmployeeStatus.AsNoTracking()
            //    .Where(x => x.EventDate >= request.startDate && x.EventDate <= request.endDate &&
            //                x.RoomId.HasValue && campRoomIds.Contains(x.RoomId.Value))
            //    .ToListAsync(cancellationToken);


            //var returnData = new List<FindAvailableByDatesAssignmentResponse>();

            //foreach (var roomId in campRoomIds)
            //{
            //    var room = await Context.Room.FindAsync(roomId);
            //    if (room == null)
            //        continue;

            //    var dateList = statusRoomDate.Where(x => x.RoomId == roomId).Select(x => x.EventDate).ToList();
            //    var employeeCount =await Context.EmployeeStatus.AsNoTracking().Where(x => x.RoomId == roomId && x.EventDate.Value.Date == request.startDate.Date).CountAsync();

            //    var owners = await Context.Employee.AsNoTracking().Where(x => x.RoomId == roomId).CountAsync();
            //    if (dateList.Count > 0)
            //    {


            //        foreach (DateTime currentDate in dateList)
            //        {
            //            int activeBed = room.BedCount;

            //            var dateEmployeeCount = statusRoomDate.Count(x => x.RoomId == roomId && x.EventDate.Value.Date == currentDate.Date);
            //            var lockedCount2 = 0;//requestDocumentRoomDate.Count(x => x.RoomId == roomId && x.StartDate <= currentDate && x.EndDate > currentDate);
            //            activeBed = room.BedCount - dateEmployeeCount - lockedCount2;

            //            if (activeBed >= 1)
            //            {
            //                var roomData = await (from roomD in Context.Room.Where(x => x.Id == room.Id && x.VirtualRoom != 1)
            //                                      join camp in Context.Camp on room.CampId equals camp.Id into campData
            //                                      from camp in campData.DefaultIfEmpty()
            //                                      join roomtype in Context.RoomType on room.RoomTypeId equals roomtype.Id into roomTypeData
            //                                      from roomtype in roomTypeData.DefaultIfEmpty()
            //                                      select new FindAvailableByDatesAssignmentResponse
            //                                      {
            //                                          RoomId = room.Id,
            //                                          BedCount = room.BedCount,
            //                                          VirtulRoom = 0,
            //                                          Employees = employeeCount,
            //                                          roomNumber = room.Number,
            //                                          RoomOwners = owners,
            //                                          Descr = $"{room.Number}({room.BedCount})/{roomtype.Description}/{camp.Description}/"
            //                                      }).FirstOrDefaultAsync();


            //                if (roomData != null)
            //                {

            //                    if (returnData.Where(x => x.RoomId == roomData.RoomId).Count() == 0)
            //                    {
            //                        returnData.Add(roomData);
            //                    }

            //                }
            //            }
            //        }
            //    }
            //    else {
            //        var roomData = await (from roomD in Context.Room.Where(x => x.Id == room.Id && x.VirtualRoom != 1)
            //                              join camp in Context.Camp on room.CampId equals camp.Id into campData
            //                              from camp in campData.DefaultIfEmpty()
            //                              join roomtype in Context.RoomType on room.RoomTypeId equals roomtype.Id into roomTypeData
            //                              from roomtype in roomTypeData.DefaultIfEmpty()
            //                              select new FindAvailableByDatesAssignmentResponse
            //                              {
            //                                  RoomId = room.Id,
            //                                  BedCount = room.BedCount,
            //                                  VirtulRoom = 0,
            //                                  Employees = 0,
            //                                  roomNumber = room.Number,
            //                                  RoomOwners = owners,
            //                                  Descr = $"{room.Number}({room.BedCount})/{roomtype.Description}/{camp.Description}/"
            //                              }).FirstOrDefaultAsync();


            //        if (roomData != null)
            //        {

            //            if (returnData.Where(x => x.RoomId == roomData.RoomId).Count() == 0)
            //            {
            //                returnData.Add(roomData);
            //            }

            //        }
            //    }

            //}


            //var virtualRoom = await Context.Room.FirstOrDefaultAsync(x => x.VirtualRoom == 1);
            //if (virtualRoom != null)
            //{
            //    var vRoomOnsiteEmployees = await Context.EmployeeStatus.AsNoTracking()
            //        .Where(x => x.RoomId == virtualRoom.Id && x.EventDate >= request.startDate &&
            //                    x.EventDate <= request.endDate)
            //        .Select(x => x.EmployeeId)
            //        .Distinct()
            //        .CountAsync(cancellationToken);

            //    returnData.Insert(0, new FindAvailableByDatesAssignmentResponse
            //    {
            //        RoomId = virtualRoom.Id,
            //        roomNumber = virtualRoom.Number,
            //        VirtulRoom = 1,
            //        Employees = vRoomOnsiteEmployees,
            //        BedCount = virtualRoom.BedCount
            //    });
            //}

            //return returnData;

      


        }




        public async Task<CreateRoomAssignmentResponse> SaveTemporaryRoom(CreateRoomAssignmentRequest request, CancellationToken cancellationToken)
        {

            var assignEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync(cancellationToken);
            var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId).FirstOrDefaultAsync(cancellationToken);
            var employees = await Context.Employee.AsNoTracking().Where(x => x.RoomId == request.RoomId).ToListAsync(cancellationToken);
            var empIds = employees.Select(x => x.Id).ToList();
            var guestList = new List<AssignRoomGuests>();
            var guestEmpIds = new List<int>();
            var returnData = new CreateRoomAssignmentResponse();
            if (assignEmployee != null)
            {

                if (currentRoom != null)
                {

                    var employeeOnsiteDates = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.EmployeeId && x.RoomId != null && x.EventDate.Value.Date >= request.StartDate.Date && x.EventDate.Value.Date <= request.EndDate.Date).ToListAsync();

                    var roomAvailableCheck = await CheckRoomDate(request.RoomId, request.StartDate, request.EndDate);
                    if (roomAvailableCheck)
                    {

                        var retData = new List<CreateRoomAssignmentRoomByDatesBedData>(); 

                        foreach (var item in employeeOnsiteDates)
                        {
                            var bedId = await GetBedId(item.EventDate.Value.Date, request.RoomId);
                            item.RoomId = request.RoomId;
                            item.BedId = bedId == 0 ? null : bedId;
                            item.ChangeRoute = "Room assign change temp";
                            item.DateUpdated = DateTime.Now;
                            item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            Context.EmployeeStatus.Update(item);

                            var BedInfo = await (from bed in Context.Bed.Where(x => x.Id == bedId)
                                                 join room in Context.Room on bed.RoomId equals room.Id into roomData
                                                 from room in roomData.DefaultIfEmpty()
                                                 select new CreateRoomAssignmentRoomByDatesBedData
                                                 {
                                                     BedId = bedId, 
                                                     BedDescr = bed.Description,
                                                     EventDate = item.EventDate,
                                                     RoomNumber = room.Number
                                                 }).FirstOrDefaultAsync();
                            if (BedInfo != null) 
                            {
                                retData.Add(BedInfo);   
                            }

                        }


                        returnData.BedInfo = retData;


                        return returnData;
                    }
                    else
                    {

                        foreach (var item in employeeOnsiteDates)
                        {
                            var eventDateOccupancy = await Context.EmployeeStatus.Where(x => x.RoomId == request.RoomId && x.EventDate.Value.Date == item.EventDate.Value.Date).ToListAsync();

                            if (eventDateOccupancy.Count >= currentRoom.BedCount)
                            {
                                var newGuestData = new AssignRoomGuests();
                                newGuestData.EventDate = item.EventDate.Value.Date;
                                newGuestData.GuestIds = new List<int>();

                                var guestEmployee = eventDateOccupancy.Where(x => !empIds.Contains(x.EmployeeId.Value)).Select(x => x.EmployeeId).ToList();
                                foreach (var item2 in guestEmployee)
                                {
                                    if (item2.HasValue)
                                    {
                                        newGuestData.GuestIds.Add(item2.Value);
                                    }

                                }

                                guestList.Add(newGuestData);
                            }

                        }



                        var retGuestData = new List<CreateRoomAssignmentResponseGuest>();
                        foreach (var item in guestList)
                        {
   
                          //  newRecord.EventDate = item.EventDate.Value.Date;



                            var retData = await (from employee in Context.Employee.AsNoTracking().Where(c => item.GuestIds.Contains(c.Id))
                                                 join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                                 from department in departmentData.DefaultIfEmpty()
                                                 join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                                 from employer in employerData.DefaultIfEmpty()
                                                 join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                                 from position in positionData.DefaultIfEmpty()
                                                 join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                                 from peopletype in peopletypeData.DefaultIfEmpty()
                                                 select new CreateRoomAssignmentResponseGuest
                                                 {
                                                     EmployeeId = employee.Id,
                                                     Firstname = employee.Firstname,
                                                     Lastname = employee.Lastname,
                                                     Gender = employee.Gender,
                                                     SAPID = employee.SAPID,
                                                     DepartmentName = department.Name,
                                                     EmployerName = employer.Description,
                                                     PeopleTypeName = peopletype.Code,
                                                     PositionName = position.Description,
                                                     EventDate = item.EventDate.Value.Date

                                                 }).ToListAsync();

                            retGuestData.AddRange(retData);
                        }

                        returnData.Guests = retGuestData;
                        return returnData;
                    }
                }
                else
                {
                    throw new BadRequestException("There are no records for the Room.");
                }
            }
            else
            {
                throw new BadRequestException("There are no records for the specified employee.");

            }
        }





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
                                if (tomorrowOut.Count  == 0)
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


            var requestDocumentIds = await Context.RequestDocument.Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                    && x.DocumentTag == "ADD" && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();


            var requestDocumentRoomDate = await (from requestAddList in Context.RequestSiteTravelAdd.Where(x => requestDocumentIds.Contains(x.DocumentId) && x.RoomId == RoomId)
                                                 join inschedule in Context.TransportSchedule on requestAddList.inScheduleId equals inschedule.Id into inscheduleData
                                                 from inschedule in inscheduleData.DefaultIfEmpty()

                                                 join doc in Context.RequestDocument on requestAddList.DocumentId equals doc.Id into docData
                                                 from doc in docData.DefaultIfEmpty()

                                                 join outschedule in Context.TransportSchedule on requestAddList.outScheduleId equals outschedule.Id into outScheduleData
                                                 from outschedule in outScheduleData.DefaultIfEmpty()
                                                 select new
                                                 {
                                                     roomId = requestAddList.RoomId,
                                                     EmployeeId = requestAddList.EmployeeId,
                                                     startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
                                                     endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate,
                                                     DocumentId = requestAddList.DocumentId,
                                                     DocumentTag = doc.DocumentTag

                                                 }
                                                ).Where(x => x.startDate <= currentDate && x.endDate > currentDate).ToListAsync();

            return requestDocumentRoomDate.Count();
        }



        private async Task<int> GetBedId(DateTime eventDate, int roomId)
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


        public async Task<RoomAssignmentEmployeeInfoResponse> EmployeeInfo(RoomAssignmentEmployeeInfoRequest request, CancellationToken cancellationToken)
        {
          var data = await  Context.RoomAssignment
                .Where(x => x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
            if (data != null)
            {
                var returnData = new RoomAssignmentEmployeeInfoResponse
                {
                    EndDate = data.EndDate,
                    Id = data.Id,
                    RoomId = data.RoomId,
                    StartDate = data.StartDate
                };

                return returnData;
            }
            else {
                return new RoomAssignmentEmployeeInfoResponse();
            }

        }



        #region RemoveOwnerShip
        public async Task RemoveOwnershipRoom(RemoveRoomAssignmentOwnershipRequest request, CancellationToken cancellationToken)
        {
            var currentEmployee = await  Context.Employee.Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                if (currentEmployee.RoomId != null)
                {
                    var  futureRoomData =await  Context.EmployeeStatus.Where(x => x.RoomId == currentEmployee.RoomId && x.EmployeeId == request.EmployeeId && x.EventDate >= request.StartDate).ToListAsync();
                    var virtualRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
                    if (virtualRoom == null) {
                        throw new BadRequestException("Please register virtual room");
                    }
                    foreach (var item in futureRoomData)
                    {
                        item.BedId = null;
                        item.RoomId = virtualRoom.Id;
                        item.DateUpdated = DateTime.Now;
                        item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        item.ChangeRoute = $"Remove ownership accomodation {currentEmployee.RoomId}";

                        Context.EmployeeStatus.Update(item);
                    }

                    currentEmployee.RoomId = null;
                    currentEmployee.DateUpdated = DateTime.Now;
                    currentEmployee.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    Context.Employee.Update(currentEmployee); 
                    
                }
            }

        }
        #endregion



    }


    #region ClassesAssign
    public class AssignRoomGuests
    { 
        public DateTime? EventDate { get; set; }
        public List<int> GuestIds { get; set; }

    }

    #endregion

}
