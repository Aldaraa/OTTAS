using AutoMapper.Execution;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RoomFeature.ActiveSearchRoom;
using tas.Application.Features.RoomFeature.CreateRoom;
using tas.Application.Features.RoomFeature.DateProfileRoom;
using tas.Application.Features.RoomFeature.DateStatusRoom;
using tas.Application.Features.RoomFeature.EmployeeRoomProfile;
using tas.Application.Features.RoomFeature.FindAvailableByDates;
using tas.Application.Features.RoomFeature.FindAvailableByRoomId;
using tas.Application.Features.RoomFeature.FindAvailableRoom;
using tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Features.RoomFeature.GetRoom;
using tas.Application.Features.RoomFeature.GetVirtualRoom;
using tas.Application.Features.RoomFeature.MonthStatusRoom;
using tas.Application.Features.RoomFeature.SearchRoom;
using tas.Application.Features.RoomFeature.UpdateRoom;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.ComponentModel;
using System.Security.Cryptography.Xml;
using tas.Domain.Enums;
using System.Diagnostics;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.AspNetCore.Mvc.Formatters;
using tas.Application.Features.RoomFeature.GetRoomAssignAvialable;
using tas.Application.Features.RoomFeature.AssignRoomDateOccupancyAnalyze;
using tas.Application.Features.RoomFeature.DateProfileRoomDetail;
using tas.Application.Features.RoomOwnerAndLockFeature.DateProfileRoomOwnerAndLock;
using tas.Application.Features.RoomFeature.MonthStatusRoomOwner;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Xml;
using tas.Application.Features.RoomFeature.DateProfileRoomExport;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using System.Drawing.Printing;
using LicenseContext = OfficeOpenXml.LicenseContext;
using tas.Application.Features.AuditFeature.GetTransportAudit;
using tas.Application.Features.RoomAssignmentFeature.FindAvailableByDatesAssignment;
using Newtonsoft.Json.Bson;
using Microsoft.EntityFrameworkCore.Metadata;
using OfficeOpenXml.Drawing.Chart;
using Microsoft.IdentityModel.Logging;

namespace tas.Persistence.Repositories
{
    public partial class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly BulkImportExcelService _bulkImportExcelService;
        public RoomRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, BulkImportExcelService bulkImportExcelService) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _bulkImportExcelService = bulkImportExcelService;
        }


        public async Task<EmployeeRoomProfileResponse> GetEmployeeRoomProfile(EmployeeRoomProfileRequest request, CancellationToken cancellationToken)
        {
            var returnData = new EmployeeRoomProfileResponse();
            var currentEmployee = await Context.Employee.Where(x => x.Id == request.Id)
                .Select(x => new { x.Id, x.Lastname, x.Firstname, x.EmployerId, x.RoomId, x.DepartmentId, x.PeopleTypeId, x.SAPID, x.HotelCheck, x.PositionId,   x.Gender }).FirstOrDefaultAsync();
            if (currentEmployee != null) {

                var currentEmployeePeopleType = currentEmployee != null
                    ? await Context.PeopleType.FirstOrDefaultAsync(x => x.Id == currentEmployee.PeopleTypeId)
                    : null;

                var currentEmployeeEmployer = currentEmployee != null
                    ? await Context.Employer.FirstOrDefaultAsync(x => x.Id == currentEmployee.EmployerId)
                    : null;

                var currentEmployeeDepartment = currentEmployee != null
                    ? await Context.Department.FirstOrDefaultAsync(x => x.Id == currentEmployee.DepartmentId)
                    : null;

                var currentEmployeePosition = currentEmployee != null
                    ? await Context.Position.FirstOrDefaultAsync(x => x.Id == currentEmployee.PositionId)
                    : null;

                var currentEmployeeRoom = currentEmployee?.RoomId != null
                    ? await Context.Room.FirstOrDefaultAsync(x => x.Id == currentEmployee.RoomId)
                    : null;


                returnData.Id = currentEmployee.Id;
                returnData.Firstname = currentEmployee.Firstname;
                returnData.Lastname = currentEmployee.Lastname;
                returnData.Gender = currentEmployee.Gender;
                returnData.HotelCheck = currentEmployee.HotelCheck;
                returnData.SAPID = currentEmployee.SAPID;
                returnData.DepartmentName = currentEmployeeDepartment?.Name;
                returnData.PositionName = currentEmployeePosition?.Description;
                returnData.PeopleTypeName = currentEmployeePeopleType?.Code;
                returnData.EmployerName = currentEmployeeEmployer?.Description;
                returnData.RoomNumber = currentEmployeeRoom?.Number;


                var roomOccupancyInfo =await Context.EmployeeStatus.AsNoTracking()
                        .Where(x => x.EmployeeId == request.Id && x.EventDate < DateTime.Today && x.RoomId != null)
                        .OrderByDescending(es => es.EventDate)
                        .ThenBy(es => es.EventDate)
                        .GroupBy(es => new { es.EmployeeId, es.RoomId })
                        .Select(group => new EmployeeRoomProfileRoomHistory
                        {

                            RoomId = group.Key.RoomId.Value,
                            StartDate = group.Min(es => es.EventDate.Value),
                            EndDate = group.Max(es => es.EventDate.Value)
                        })
                        .ToListAsync();

                foreach (var item in roomOccupancyInfo)
                {
                    string? campName = "";
                    var currentRoom = await Context.Room.AsNoTracking().Where(c => c.Id == item.RoomId).Select(x => new { x.Number, x.CampId }).FirstOrDefaultAsync();
                    if (currentRoom != null) {
                        campName = await Context.Camp.AsNoTracking().Where(x => x.Id == currentRoom.CampId).Select(x => x.Description).FirstOrDefaultAsync();
                    }

                    item.RoomNumber = currentRoom?.Number;
                    item.Camp = campName;
                }
                returnData.employeeRoomProfileRoomHistories = roomOccupancyInfo;






            }
            return returnData;
        }



        public async Task<FindRoomDateOccupancyAnalyzeResponse> FindRoomDateOccupancyAnalyze(FindRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken)
        {
            var returnData = new FindRoomDateOccupancyAnalyzeResponse();
            var currentInfoList = new List<FindRoomDateOccupancyAnalyzeCurrentInfo>();
            var employeeStatusList = await Context.EmployeeStatus.AsNoTracking()
                 .Where(x => x.RoomId == request.RoomId && x.EventDate >= request.StartDate && x.EventDate <= request.EndDate)
                 .ToListAsync(cancellationToken);
            var currentRoom = await Context.Room.Where(x => x.Id == request.RoomId && x.VirtualRoom == 0).FirstOrDefaultAsync();
            if (currentRoom != null) {
                var shiftquery = from shift in Context.Shift
                                 join shiftColor in Context.Color on shift.ColorId equals shiftColor.Id
                                 select new
                                 {
                                     shiftId = shift.Id,
                                     Code = shift.Code,
                                     ColorCode = shiftColor.Code
                                 };

                var empIds = employeeStatusList.Select(x => x.EmployeeId).Distinct().ToArray();

                foreach (var empId in empIds)
                {
                    var currentEmployee = await Context.Employee.Where(x => x.Id == empId)
                            .Select(x => new { x.Id, x.Lastname, x.Firstname, x.EmployerId, x.RoomId, x.DepartmentId, x.PeopleTypeId, x.SAPID, x.HotelCheck, x.PositionId, x.Gender }).FirstOrDefaultAsync();
                    if (currentEmployee != null)
                    {

                        var currentEmployeePeopleType = currentEmployee != null
                            ? await Context.PeopleType.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PeopleTypeId)
                            : null;

                        var currentEmployeeEmployer = currentEmployee != null
                            ? await Context.Employer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.EmployerId)
                            : null;

                        var currentEmployeeDepartment = currentEmployee != null
                            ? await Context.Department.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.DepartmentId)
                            : null;

                        var currentEmployeePosition = currentEmployee != null
                             ? await Context.Position.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PositionId)
                             : null;


                        var currentEmployeeRoom = currentEmployee?.RoomId != null
                            ? await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.RoomId)
                            : null;


                        var outRoomDate = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == currentEmployee.Id && x.EventDate >
                        request.StartDate && x.RoomId != request.RoomId).OrderBy(x => x.EventDate).FirstOrDefaultAsync();

                        var currentInfo = new FindRoomDateOccupancyAnalyzeCurrentInfo
                        {
                            Id = empId,
                            Firstname = currentEmployee?.Firstname,
                            Lastname = currentEmployee?.Lastname,
                            Gender = currentEmployee?.Gender,
                            HotelCheck = currentEmployee?.HotelCheck,
                            SAPID = currentEmployee?.SAPID,
                            DepartmentName = currentEmployeeDepartment?.Name,
                            PeopleTypeName = currentEmployeePeopleType?.Code,
                            PositionName = currentEmployeePosition?.Description,
                            EmployerName = currentEmployeeEmployer?.Description,
                            RoomNumber = currentEmployeeRoom?.Number,
                            RoomOwner = currentEmployee?.RoomId == request.RoomId,
                            OutDate = outRoomDate?.EventDate
                        };
                        currentInfoList.Add(currentInfo);
                    }
                }


                var returnHistoryInfo = new List<FindRoomDateOccupancyAnalyzeHistoryInfo>();
                DateTime endDate = DateTime.Today;
                DateTime startDate = endDate.AddMonths(-6);


                var roomOccupancyInfo = await Context.EmployeeStatus.AsNoTracking()
                        .Where(x => x.RoomId == request.RoomId && x.EventDate >= startDate && x.EventDate.Value.Date <= endDate)
                        .OrderByDescending(es => es.EventDate)
                        .ToListAsync();


                var ocupancyEmpIds = roomOccupancyInfo.Select(x => x.EmployeeId).Distinct().ToArray();

                foreach (var item in ocupancyEmpIds)
                {

                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item)
                          .Select(x => new { x.Id, x.Lastname, x.Firstname, x.EmployerId, x.RoomId, x.DepartmentId, x.PeopleTypeId, x.SAPID, x.HotelCheck, x.PositionId, x.Gender }).FirstOrDefaultAsync();


                    var currentEmployeePeopleType = currentEmployee != null
                        ? await Context.PeopleType.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PeopleTypeId)
                        : null;

                    var currentEmployeeEmployer = currentEmployee != null
                        ? await Context.Employer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.EmployerId)
                        : null;

                    var currentEmployeeDepartment = currentEmployee != null
                        ? await Context.Department.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.DepartmentId)
                        : null;

                    var currentEmployeePosition = currentEmployee != null
                        ? await Context.Position.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PositionId)
                        : null;

                    var currentEmployeeRoom = currentEmployee?.RoomId != null
                        ? await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.RoomId)
                        : null;


                    var newItem = new FindRoomDateOccupancyAnalyzeHistoryInfo();
                    newItem.Id = item;
                    newItem.Firstname = currentEmployee.Firstname;
                    newItem.Lastname = currentEmployee.Lastname;
                    newItem.Gender = currentEmployee.Gender;
                    newItem.HotelCheck = currentEmployee.HotelCheck;
                    newItem.PositionName = currentEmployeePosition?.Description;
                    newItem.SAPID = currentEmployee.SAPID;
                    newItem.DepartmentName = currentEmployeeDepartment?.Name;
                    newItem.PeopleTypeName = currentEmployeePeopleType?.Code;
                    newItem.EmployerName = currentEmployeeEmployer?.Description;
                    newItem.RoomNumber = currentEmployeeRoom?.Number;
                    newItem.RoomOwner = currentEmployee.RoomId == request.RoomId;
                //    newItem.RoomDateLog = roomOccupancyInfo.Where(x => x.EmployeeId == item.Value).Select(x => x.EventDate).ToList();
                    returnHistoryInfo.Add(newItem);
                }

                returnData.HistoryInfo = returnHistoryInfo;
                returnData.currentInfo = currentInfoList;
            }


            return returnData;


        }



        public async Task<AssignRoomDateOccupancyAnalyzeResponse> AssignRoomDateOccupancyAnalyze(AssignRoomDateOccupancyAnalyzeRequest request, CancellationToken cancellationToken)
        {

            var employees =await Context.Employee.AsNoTracking().Where(x => x.RoomId == request.RoomId).ToListAsync(cancellationToken);

            var empIds = employees.Select(x => x.Id).ToArray();
            var currentOwnerList = new List<AssignRoomDateOccupancyAnalyzeOwnerInfo>();

            var guestList = new List<AssignRoomDateOccupancyAnalyzeGuestInfo>();


            var currentRoom =await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId).FirstOrDefaultAsync(cancellationToken);
            if (currentRoom != null)
            {
                foreach (var empId in empIds)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == empId)
                            .Select(x => new { x.Id, x.Lastname, x.Firstname, x.EmployerId, x.RoomId, x.DepartmentId, x.PeopleTypeId, x.SAPID, x.HotelCheck, x.PositionId, x.Gender }).FirstOrDefaultAsync();
                    if (currentEmployee != null)
                    {

                        var currentEmployeePeopleType = currentEmployee != null
                            ? await Context.PeopleType.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PeopleTypeId)
                            : null;

                        var currentEmployeeEmployer = currentEmployee != null
                            ? await Context.Employer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.EmployerId)
                            : null;

                        var currentEmployeeDepartment = currentEmployee != null
                            ? await Context.Department.AsNoTracking().AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.DepartmentId)
                            : null;

                        var currentEmployeePosition = currentEmployee != null
                             ? await Context.Position.AsNoTracking().AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PositionId)
                             : null;


                        var currentEmployeeRoom = currentEmployee?.RoomId != null
                            ? await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.RoomId)
                            : null;


                      var futureInInfo = await  Context.Transport.AsNoTracking().Where(x => x.EmployeeId == currentEmployee.Id && x.EventDate >= request.StartDate.Date && x.Direction == "IN").OrderBy(c => c.EventDate).FirstOrDefaultAsync();

                        var currentInfo = new AssignRoomDateOccupancyAnalyzeOwnerInfo
                        {
                            Id = empId,
                            Firstname = currentEmployee?.Firstname,
                            Lastname = currentEmployee?.Lastname,
                            Gender = currentEmployee?.Gender,
                            HotelCheck = currentEmployee?.HotelCheck,
                            SAPID = currentEmployee?.SAPID,
                            DepartmentName = currentEmployeeDepartment?.Name,
                            DepartmentId = currentEmployee.DepartmentId,
                            PeopleTypeName = currentEmployeePeopleType?.Code,
                            PositionName = currentEmployeePosition?.Description,
                            EmployerName = currentEmployeeEmployer?.Description,
                            RoomNumber = currentEmployeeRoom?.Number,
                            InDate = futureInInfo != null ? futureInInfo.EventDate : null,
                        };

                        currentOwnerList.Add(currentInfo);
                    }
                }



                guestList = await Context.EmployeeStatus.AsNoTracking().Where(x => x.RoomId == request.RoomId && x.EventDate.Value.Date >= request.StartDate.Date && x.EventDate.Value.Date <= request.EndDate.Date).Select(x => new AssignRoomDateOccupancyAnalyzeGuestInfo
                {

                            Id = x.EmployeeId
                }).Distinct().ToListAsync();

                foreach (var guest in guestList)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == guest.Id)
                            .Select(x => new { x.Id, x.Lastname, x.Firstname, x.EmployerId, x.RoomId, x.DepartmentId, x.PeopleTypeId, x.SAPID, x.HotelCheck, x.PositionId, x.Gender }).FirstOrDefaultAsync();
                    if (currentEmployee != null)
                    {

                        var currentEmployeePeopleType = currentEmployee != null
                            ? await Context.PeopleType.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PeopleTypeId)
                            : null;

                        var currentEmployeeEmployer = currentEmployee != null
                            ? await Context.Employer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.EmployerId)
                            : null;

                        var currentEmployeeDepartment = currentEmployee != null
                            ? await Context.Department.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.DepartmentId)
                            : null;

                        var currentEmployeePosition = currentEmployee != null
                             ? await Context.Position.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.PositionId)
                             : null;


                        var currentEmployeeRoom = currentEmployee?.RoomId != null
                            ? await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentEmployee.RoomId)
                            : null;



                        var roomMinDate =await  Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate >= request.StartDate && x.EmployeeId == guest.Id && x.RoomId != null).OrderBy(x => x.EventDate).FirstOrDefaultAsync();


                        var roomMaxDate = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate >= request.StartDate && x.EmployeeId == guest.Id && x.RoomId == request.RoomId).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();





                        var outRoomDate = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == guest.Id && 
                       x.EventDate > roomMinDate.EventDate  && x.RoomId != request.RoomId).OrderBy(x => x.EventDate).FirstOrDefaultAsync();


                        var RoomINRoomDate = await Context.EmployeeStatus.Where(x => x.EmployeeId == currentEmployee.Id && x.EventDate >
                                request.StartDate && x.RoomId == request.RoomId).OrderBy(x => x.EventDate).FirstOrDefaultAsync();

                        if (RoomINRoomDate != null) {
                             guest.ShiftCode = await   Context.Shift.AsNoTracking().Where(x => x.Id == RoomINRoomDate.ShiftId).Select(z=> z.Code).FirstOrDefaultAsync();
                        }

                        guest.Firstname = currentEmployee?.Firstname;
                        guest.Lastname = currentEmployee?.Lastname;
                        guest.Gender = currentEmployee?.Gender;
                        guest.SAPID = currentEmployee?.SAPID;
                        
                        guest.DepartmentName = currentEmployeeDepartment?.Name;
                        guest.DepartmentId = currentEmployee.DepartmentId;
                        guest.PeopleTypeName = currentEmployeePeopleType?.Code;
                        guest.PositionName = currentEmployeePosition?.Description;
                        guest.EmployerName = currentEmployeeEmployer?.Description;
                        guest.RoomOwner = request.RoomId == currentEmployee?.RoomId;

                        if (outRoomDate != null)
                        {
                            guest.OutDate = outRoomDate.EventDate;
                        }
                        else if (roomMaxDate != null)
                        {
                            guest.OutDate = roomMaxDate.EventDate.Value.AddDays(1);
                        }
                        else
                        {
                            guest.OutDate = null;
                        }

                    }
                }


             //   }
            }


            var returnData = new AssignRoomDateOccupancyAnalyzeResponse
            {
                ownerInfo = currentOwnerList,
                GuestFutureInfo = guestList
            };


            return returnData;


        }


        public async Task<FindAvailableByRoomIdResponse> FindAvailableByRoomId(FindAvailableByRoomIdRequest request, CancellationToken cancellationToken)
        {

            var roomStatus = await  CheckRoomDate(request.RoomId, request.startDate, request.endDate, request.EmployeeId);

            var room = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.RoomId);
            if (room != null)
            {

                DateTime currentDate = request.startDate;

                FindAvailableByRoomIdResponse result = new FindAvailableByRoomIdResponse();

                var RoomData = new List<FindAvailableByRoomIdRoomData>();

                while (currentDate <= request.endDate)
                {
 
                        var newRecord = new FindAvailableByRoomIdRoomData
                        {
                            BedCount = room.BedCount,
                            roomNumber = room.Number,
                            ActiveBeds = roomStatus ? room.BedCount : 0,
                            EventDate = currentDate,
                            RoomId = room.Id

                        };

                        RoomData.Add(newRecord);
                   


                    currentDate = currentDate.AddDays(1);
                }

                result.status = roomStatus;
                result.RoomData = RoomData;
                return result;
            }
            else
            {
                return null;
            }




        }


        private async Task<bool> CheckRoomDate(int roomId, DateTime startDate, DateTime endDate, int employeeId)
        {
            try
            {

                
                var currentRoom = await Context.Room.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roomId);

                //var endDateTransport =await Context.Transport.AsNoTracking()
                //    .Where(x => x.EventDate.Value.Date == endDate.Date && x.EmployeeId == employeeId && x.Direction == "OUT").FirstOrDefaultAsync();

            //    if (endDateTransport != null) 
             //   {
                    endDate = endDate.AddDays(-1);
                //}


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
                        .Where(x => x.EventDate.Value.Date >= startDate.Date && x.EventDate <= endDate.Date && x.RoomId == roomId && x.EmployeeId != employeeId)
                        .CountAsync();

                if (sessionRoomCount == 0)
                {
                    return true;
                }

                while (currentDate <= endDate)
                {
                    var dateCountRoom = await Context.EmployeeStatus
                        .Where(x => x.EventDate.Value.Date == currentDate.Date && x.RoomId == roomId && x.EmployeeId != employeeId)
                        .CountAsync();



                    if (currentDate == startDate)
                    {
                        if (currentRoom.BedCount <= dateCountRoom)
                        {
                            var onsiteEmployees = await Context.EmployeeStatus.AsNoTracking().Where(c => c.RoomId == roomId && c.EventDate.Value.Date == currentDate.Date && c.EmployeeId != employeeId).Select(x => x.EmployeeId).ToListAsync();
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


        public async Task<List<GetAllRoomResponse>> GetAll(GetAllRoomRequest request, CancellationToken cancellationToken)
        {

            var result = await Context.Room.AsNoTracking()
                .Join(Context.Camp.AsNoTracking(),
                    room => room.CampId,
                    camp => camp.Id,
                    (room, Camp) => new { room, Camp }
                    ).Join(
                     Context.RoomType,
                     room => room.room.RoomTypeId,
                     roomtype => roomtype.Id,
                     (room, roomtype) => new GetAllRoomResponse
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
                         EmployeeCount = Context.Employee.AsNoTracking().Count(x => x.RoomId == room.room.Id && x.Active == 1)
                     }).ToListAsync(cancellationToken);

            if (request.status.HasValue)
            {
                return result.Where(x => x.Active == request.status).ToList();
            }
            else {

            }
            return result;
        }






        public async Task<GetRoomResponse?> Get(int Id, CancellationToken cancellationToken)
        {

            return await Context.Room.AsNoTracking().Where(x => x.Id == Id)
                .Join(Context.Camp,
                    room => room.CampId,
                    camp => camp.Id,
                    (room, Camp) => new { room, Camp }
                    ).Join(
                     Context.RoomType,
                     room => room.room.RoomTypeId,
                     roomtype => roomtype.Id,
                     (room, roomtype) => new GetRoomResponse
                     {
                         Id = room.room.Id,
                         Number = room.room.Number,
                         CampName = room.Camp.Description,
                         BedCount = room.room.BedCount,
                         Private = room.room.Private,
                         Active = room.room.Active,
                         VirtualRoom = room.room.VirtualRoom,
                         RoomTypeName = roomtype.Description,
                         CampId = room.room.Id,
                         DateCreated = room.room.DateCreated,
                         DateUpdated = room.room.DateUpdated,
                         RoomTypeId = room.room.RoomTypeId,
                         BedList = Context.Bed.AsNoTracking().Where(x => x.RoomId == room.room.Id).ToList(),
                     }).FirstOrDefaultAsync(cancellationToken);

        }
         
        public async Task GenerateBed(int roomId)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var currentRoom = await Context.Room.FirstOrDefaultAsync(x => x.Id == roomId);
                var oldBedCount = Context.Bed.Count(x => x.RoomId == roomId);
            if (currentRoom != null)
            {

                if (currentRoom.BedCount < oldBedCount)
                {

                    var bedList =await Context.Bed.Where(x => x.RoomId == currentRoom.Id)
                         .Select(x => new { x.Description, x.Id })
                         .OrderByDescending(x => x.Description).ToListAsync();
                    int removeBedCount = oldBedCount - currentRoom.BedCount;
                    int removedIndex = 0;
                    foreach (var item in bedList)
                    {
                        var bedstatus = await Context.EmployeeStatus.AsNoTracking().AnyAsync(x => x.BedId == item.Id);
                        if (!bedstatus)
                        {
                            if (removedIndex < removeBedCount)
                            {
                                var currentBed = await Context.Bed.FirstOrDefaultAsync(x => x.Id == item.Id);
                                Context.Bed.Remove(currentBed);
                                removedIndex++;
                            }

                        }
                    }

                    if (removeBedCount - removedIndex > 0)
                    {
                        currentRoom.BedCount = currentRoom.BedCount + (removeBedCount - removedIndex);
                        Context.Room.Update(currentRoom);
                    }

                }
                if (currentRoom.BedCount > oldBedCount)
                {

                    for (int i = oldBedCount + 1; i <= currentRoom.BedCount; i++)
                    {
                        var indexStr = alphabets[i - 1].ToString();
                        var newBed = new Bed
                        {
                            DateCreated = DateTime.Now,
                            UserIdCreated = 1,
                            RoomId = roomId,
                            Active = 1,
                            Description = string.Format("{0}-{1}", currentRoom.Number, indexStr)
                        };

                        Context.Bed.Add(newBed);
                    }
                }

                await Context.SaveChangesAsync();

            }
            

        }


        public async Task<List<FindAvailableRoomResponse>> FindAvailableRooms(FindAvailableRoomRequest request, CancellationToken cancellationToken)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var campRooms = await Context.Room.AsNoTracking()
                .Where(x => x.CampId == request.CampId && x.Active == 1 && x.VirtualRoom != 1)
                .Select(x => new { x.Id, x.BedCount, x.RoomTypeId, x.Number, x.Private }).ToListAsync(cancellationToken);

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
                campRooms = campRooms.Where(x => x.Number.ToLower().Contains(request.RoomNumber.ToLower())).ToList();
            }
            var campRoomIds = campRooms.Select(x => x.Id);

            stopWatch.Stop();
            Console.WriteLine($"CAMPROOM ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            var statusRoomDate = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EventDate.Value.Date >= request.startDate.Date && x.EventDate <= request.endDate.Date && x.RoomId != null && campRoomIds.Contains(x.RoomId.Value))
                .ToListAsync();
            var EmptyRooms = campRooms.Where(x => !statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x => new { x.Id, x.BedCount });
            var NonEmptyRooms = campRooms.Where(x => statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x => new { x.Id, x.BedCount });



            stopWatch.Stop();
            Console.WriteLine($"EMPTY ROOM CALCULATION ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
            List<int> RemoveRoomIds = new List<int>();
            var returnData = new List<FindAvailableRoomResponse>();
            foreach (var room in campRooms)
            {
                DateTime currentDate = request.startDate;
                while (currentDate <= request.endDate)
                {

                    int activeBed = room.BedCount;

                    if (!EmptyRooms.Any(x => x.Id == room.Id))
                    {
                        var DateEmployeeCount = statusRoomDate.Count(x => x.RoomId == room.Id && x.EventDate.Value.Date == currentDate.Date);
                        var lockedCount = await GetDateLockedCount(room.Id, currentDate.Date);

                        activeBed = room.BedCount - DateEmployeeCount - lockedCount;
                        if (activeBed < 1)
                        {
                            if (!RemoveRoomIds.Any(x => x == room.Id))
                            {
                                RemoveRoomIds.Add(room.Id);
                            }
                        }

                    }



                    var newRecord = new FindAvailableRoomResponse
                    {
                        BedCount = room.BedCount,
                        RoomId = room.Id,
                        roomNumber = room.Number,
                        VirtulRoom = 0,

                    };

                    returnData.Add(newRecord);

                    currentDate = currentDate.AddDays(1);
                }
            }



            stopWatch.Stop();
            Console.WriteLine($"FOREACH CALCULATION ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();

            if (RemoveRoomIds.Count > 0)
            {
                returnData = returnData.Where(x => !RemoveRoomIds.Contains(x.RoomId.Value)).ToList();
            }

            returnData = returnData.Distinct().ToList();

            var virtualRoom =await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).FirstOrDefaultAsync();
            if (virtualRoom != null) {
                returnData.Insert(0, new FindAvailableRoomResponse { RoomId = virtualRoom.Id, roomNumber = virtualRoom.Number, VirtulRoom = 1, BedCount = virtualRoom.BedCount });
            }


            stopWatch.Stop();
            Console.WriteLine($"END ==================> {stopWatch.ElapsedMilliseconds}");
            return await Task.FromResult(returnData.Distinct().ToList());


        }


        public async Task<List<FindAvailableByDatesResponse>> FindAvailableByDates(FindAvailableByDatesRequest request, CancellationToken cancellationToken)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();



            if (!request.CampId.HasValue && string.IsNullOrEmpty(request.RoomNumber))
            {
                throw new BadRequestException("Please ensure both 'Camp' and 'Room Number' fields are filled out.");  
            }
            var campRooms =await Context.Room.AsNoTracking().Where(x=> x.Active == 1).ToListAsync(cancellationToken);


            stopWatch.Stop();
            Console.WriteLine($"CAMP ROOMS END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            if (request.CampId.HasValue)
            {

                 campRooms = campRooms
                    .Where(x => x.CampId == request.CampId && x.Active == 1)
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
                campRooms = campRooms.Where(x => x.Number.ToLower().Contains(request.RoomNumber.ToLower())).ToList();
            }
            if (request.bedCount.HasValue)
            {
                if (request.bedCount > 0)
                {
                    campRooms = campRooms.Where(x => x.BedCount == request.bedCount).ToList();
                }
            }   

            var campRoomIds = campRooms.Select(x => x.Id);



            var NotHotelRoomIds = await (from r in Context.Room.AsNoTracking()
                                        join e in Context.Employee.AsNoTracking() on r.Id equals e.RoomId
                                        where e.HotelCheck != 1
                                        select r.Id 
                                        ).ToListAsync();


            var statusRoomDate = await Context.EmployeeStatus.AsNoTracking().AsNoTracking()
                .Where(x => x.EventDate >= request.startDate && x.EventDate <= request.endDate && x.RoomId != null && campRoomIds.Contains(x.RoomId.Value))
                .ToListAsync();

            var EmptyRooms = campRooms.Where(x => !statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x => new { x.Id, x.BedCount });
            var NonEmptyRooms = campRooms.Where(x => statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x => new { x.Id, x.BedCount });


            List<int> RemoveRoomIds = new List<int>();
            var returnData = new List<FindAvailableByDatesResponse>();



            var activeDocumentActions = new List<string> { "Submitted", "Approved" };

            stopWatch.Stop();
            Console.WriteLine($"FILTER END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
            var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                    && x.DocumentTag == "ADD" && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();

            stopWatch.Stop();
            Console.WriteLine($"REQUEST DOCUMENT ROOMD END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();

            foreach (var room in campRooms)
            {
                DateTime currentDate = request.startDate;

                var ownerIds = await Context.Employee.AsNoTracking().Where(x => x.RoomId == room.Id).Select(x => x.Id).ToListAsync();
                DateTime? ownerDateIn = null;


                var currentRoomStatus = await CheckRoomDate(room.Id, request.startDate, request.endDate, request.employeeId);


                if (!currentRoomStatus)
                {
                    if (!RemoveRoomIds.Any(x => x == room.Id))
                    {
                        RemoveRoomIds.Add(room.Id);
                    }
                }
                else
                {


                    if (ownerIds.Count > 0)
                    {
                        var dates = await Context.Transport.AsNoTracking().Where(x => x.EventDate >= request.startDate && ownerIds.Contains(x.EmployeeId.Value) && x.Direction == "IN").OrderBy(x => x.EventDate).FirstOrDefaultAsync();

                        if (dates != null)
                        {
                            var currentSchedule =await Context.TransportSchedule.AsNoTracking().Where(c => c.Id == dates.ScheduleId).FirstOrDefaultAsync();
                            if (currentSchedule != null)
                            {
                                // Parse hour and minute from ETD
                                if (int.TryParse(currentSchedule.ETD?.Substring(0, 2), out int hour) &&
                                    int.TryParse(currentSchedule.ETD?.Substring(2, 2), out int minute))
                                {
                                    ownerDateIn = dates.EventDate.Value.Date.AddHours(hour).AddMinutes(minute);
                                }
                                else
                                {
                                    // Handle parsing failure if necessary
                                    ownerDateIn = dates.EventDateTime;
                                }

                            }
                            else {
                                ownerDateIn = dates.EventDateTime;
                            }
                            
                        }


                    }


                    var newRecord = new FindAvailableByDatesResponse
                    {
                        BedCount = room.BedCount,
                        RoomId = room.Id,
                        roomNumber = room.Number,
                        VirtulRoom = 0,
                        OwnerInDate = ownerDateIn,
                        RoomOwners = ownerIds.Count()

                    };

                        returnData.Add(newRecord);
                }


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

                var employeeMaxCountByDate = await Context.EmployeeStatus
                            .AsNoTracking()
                            .Where(x => x.RoomId == item.RoomId
                                        && x.EventDate.Value.Date >= request.startDate.Date
                                        && x.EventDate.Value.Date <= request.endDate)
                            .GroupBy(x => x.EventDate.Value.Date)
                            .Select(g => new
                            {
                                Date = g.Key,
                                MaxEmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count()
                            })
                            .OrderByDescending(x => x.MaxEmployeeCount)
                            .FirstOrDefaultAsync();
                item.Employees = employeeMaxCountByDate?.MaxEmployeeCount;
            }


            stopWatch.Stop();
            Console.WriteLine($"---------------------------------- END ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            return await Task.FromResult(returnData.Distinct().ToList());



        }




        public async Task<List<GetRoomAssignAvialableResponse>> GetRoomAssignAvialable(GetRoomAssignAvialableRequest request, CancellationToken cancellationToken)
        {
           if (!request.CampId.HasValue && string.IsNullOrEmpty(request.RoomNumber))
            {
                throw new BadRequestException("Please ensure both 'Camp' and 'Room Number' fields are filled out.");
            }
            var campRooms = await Context.Room.AsNoTracking().Where(c=> c.Active == 1).ToListAsync(cancellationToken);

            if (request.CampId.HasValue)
            {

                campRooms = campRooms
                   .Where(x => x.CampId == request.CampId && x.Active == 1)
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
                    campRooms = campRooms.Where(x=> x.BedCount == request.BedCount).ToList();
                }
            }

            var campRoomIds = campRooms.Select(x => x.Id);


            var avialableRooms = await (
                                        from r in Context.Room
                                        .Where(r => campRoomIds.Contains(r.Id))
                                      
                                        select new GetRoomAssignAvialableResponse
                                        {
                                            RoomId = r.Id,
                                            BedCount = r.BedCount,
                                            VirtulRoom = 0,
                                            roomNumber = r.Number,


                                        }).ToListAsync();

            if (campRoomIds.Count() > 0 && avialableRooms.Count > 0)
            {
                foreach (var item in avialableRooms)
                {

                    var ownerCount =await Context.Employee.AsNoTracking().Where(x => x.RoomId == item.RoomId).CountAsync();
                    var descrdata = await (from room in Context.Room.AsNoTracking().Where(x => x.Id == item.RoomId)
                                           join camp in Context.Camp.AsNoTracking() on room.CampId equals camp.Id into campData
                                           from camp in campData.DefaultIfEmpty()
                                           join roomtype in Context.RoomType.AsNoTracking() on room.RoomTypeId equals roomtype.Id into roomTypeData
                                           from roomtype in roomTypeData.DefaultIfEmpty()
                                           select new
                                           {
                                               
                                               Descr = $"{room.Number}({room.BedCount})/{roomtype.Description}/{camp.Description}/"
                                           }).FirstOrDefaultAsync();
                    item.Employees = await Context.EmployeeStatus.AsNoTracking().Where(x => x.RoomId == item.RoomId && x.EventDate.Value.Date == request.startDate.Date).CountAsync();
                    item.Descr = descrdata != null ? descrdata.Descr : item.roomNumber;
                    item.OwnerCount = ownerCount;

                }
                return avialableRooms;
            }
            if (campRoomIds.Count() > 0 && avialableRooms.Count() == 0)
            {
                var roomData = await (from room in Context.Room.AsNoTracking().Where(x => campRoomIds.Contains(x.Id) && x.VirtualRoom != 1)
                                      join camp in Context.Camp.AsNoTracking() on room.CampId equals camp.Id into campData
                                      from camp in campData.DefaultIfEmpty()
                                      join roomtype in Context.RoomType.AsNoTracking() on room.RoomTypeId equals roomtype.Id into roomTypeData
                                      from roomtype in roomTypeData.DefaultIfEmpty()
                                      select new GetRoomAssignAvialableResponse { 
                                          RoomId = room.Id,
                                          BedCount = room.BedCount,
                                          VirtulRoom = 0,
                                          Employees = 0,
                                          roomNumber = room.Number,
                                          Descr = $"{room.Number}({room.BedCount})/{roomtype.Description}/{camp.Description}/"
                                      }).ToListAsync();


                foreach (var item in roomData)
                {
                    item.Employees =await Context.EmployeeStatus.AsNoTracking().Where(x => x.RoomId == item.RoomId && x.EventDate.Value.Date == request.startDate.Date).CountAsync();
                    var ownerCount = await Context.Employee.AsNoTracking().Where(x => x.RoomId == item.RoomId).CountAsync();
                    item.OwnerCount = ownerCount;
                }
                return roomData;
            }


            return new List<GetRoomAssignAvialableResponse>();

        }



        #region StatusBetweenDates

        public async Task<ActiveSearchRoomResponse> StatusBetweenDates(ActiveSearchRoomRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;


            IQueryable<Room> campRoomFilter = Context.Room.Where(x=> x.Active == 1);


            if (!string.IsNullOrWhiteSpace(request.model.RoomNumber))
            {
                campRoomFilter = campRoomFilter.Where(e => e.Number.ToLower().Contains(request.model.RoomNumber.ToLower()));
            }
            else
            {

                if (request.model.CampId.HasValue)
                {
                    campRoomFilter = campRoomFilter.Where(x => x.CampId == request.model.CampId);
                }

                if (request.model.bedCount.HasValue)
                {
                    campRoomFilter = campRoomFilter.Where(x => x.BedCount == request.model.bedCount.Value);
                }

                if (request.model.RoomTypeId.HasValue)
                {
                    campRoomFilter = campRoomFilter.Where(e => e.RoomTypeId == request.model.RoomTypeId);
                }



                if (request.model.Private.HasValue)
                {
                    campRoomFilter = campRoomFilter.Where(e => e.Private == request.model.Private);
                }

                if (request.model.hasOwner.HasValue)
                {
                    var hasOwnerRoomIds = await Context.Employee
                                                        .AsNoTracking()
                                                        .Where(x => x.RoomId != null)
                                                        .Select(x => x.RoomId.Value)
                                                        .Distinct()
                                                        .ToListAsync();

                    if (request.model.hasOwner.Value == 1)
                    {
                        campRoomFilter = campRoomFilter.Where(x => hasOwnerRoomIds.Contains(x.Id));
                    }
                    else
                    {
                        campRoomFilter = campRoomFilter.Where(x => !hasOwnerRoomIds.Contains(x.Id));
                    }
                }



            }

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }


            int recordsToSkip = (pageIndex - 1) * pageSize;
            int recordsToTake = pageSize;

            var activeDocumentActions = new List<string> { "Submitted", "Approved" };

            var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                    && x.DocumentTag == "ADD" && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();


            var requestDocumentRoomDate = await (from requestAddList in Context.RequestSiteTravelAdd.AsNoTracking().Where(x => requestDocumentIds.Contains(x.DocumentId) && (x.RoomId != null && x.RoomId != 0))
                                                 join inschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.inScheduleId equals inschedule.Id /* into inscheduleData*/
                                                 /*from inschedule in inscheduleData.DefaultIfEmpty()*/

                                                 join outschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.outScheduleId equals outschedule.Id /* into outScheduleData
                                                    /* from outschedule in outScheduleData.DefaultIfEmpty()*/
                                                 select new
                                                 {
                                                     roomId = requestAddList.RoomId,
                                                     startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
                                                     endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate
                                                 }
                                                ).ToListAsync();

            if (request.model.Locked.HasValue)
            {
                var lockedRoomIds = requestDocumentRoomDate.Select(c => c.roomId).ToList();
                if (request.model.Locked.Value == 1)
                {
                    campRoomFilter = campRoomFilter.Where(x => lockedRoomIds.Contains(x.Id));
                }
                else
                {
                    campRoomFilter = campRoomFilter.Where(x => !lockedRoomIds.Contains(x.Id));
                }

            }
            var paginatedData = campRoomFilter.AsEnumerable();

            if (campRoomFilter.Count() > recordsToSkip)
            {
                paginatedData = campRoomFilter.Skip(recordsToSkip).Take(recordsToTake).AsEnumerable();

            }

            var pCo8unt = paginatedData.Count();


            int skip = (pageIndex - 1) * pageSize;

            var paginatedRooms = await campRoomFilter.Skip(skip).Take(pageSize).ToListAsync();
            var paginatedRoomIds = paginatedRooms.Select(room => room.Id).ToList();

            var statusRoomData = await Context.EmployeeStatus.AsNoTracking()
                                                             .Where(status => paginatedRoomIds.Contains(status.RoomId.Value) &&
                                                                              status.EventDate >= request.model.startDate &&
                                                                              status.EventDate <= request.model.endDate &&
                                                                              status.RoomId != null)
                                                             .Select(status => new
                                                             {
                                                                 status.RoomId,
                                                                 status.EventDate
                                                             }).ToListAsync();



            var retData = new List<ActiveSearchRoom>();
            if (paginatedData != null)
            {

                var FilterRoom = paginatedData.ToList();
                foreach (var room in FilterRoom)
                {
                    int occupancyCount = 0;
                    var newRecord = new ActiveSearchRoom
                    {
                        RoomId = room.Id,
                        Private = room.Private,
                        RoomNumber = room.Number,
                        VirtualRoom = room.VirtualRoom,
                        BedCount = room.BedCount,
                        RoowOwners = await Context.Employee.AsNoTracking().Where(x => x.RoomId == room.Id).CountAsync(),
                        OccupancyData = new Dictionary<string, ActiveSearchRoomResponseDateInfo>()
                    };

                    for (var currentDate = request.model.startDate.Date; currentDate <= request.model.endDate.Date; currentDate = currentDate.AddDays(1))
                    {


                        var activeBed = room.BedCount;

                        var roomStatus = statusRoomData
                            .Where(x => x.RoomId == room.Id && x.EventDate == currentDate)
                            .ToList();

                        var lockStatus = requestDocumentRoomDate.Where(x => x.roomId == room.Id && x.startDate.Date <= currentDate && x.endDate.Date > currentDate).ToList();
                        if (lockStatus.Count > 0)
                        {
                            var aa = 0;
                        }

                        if (room.VirtualRoom > 0)
                        {
                            activeBed = roomStatus.Count;
                            occupancyCount = roomStatus.Count;
                        }
                        else
                        {
                            activeBed = room.BedCount - roomStatus.Count - lockStatus.Count;
                            occupancyCount += room.BedCount - activeBed;

                        }

                        newRecord.OccupancyData.Add(currentDate.ToString("yyyy-MM-dd"), new ActiveSearchRoomResponseDateInfo { ActiveBeds = activeBed, BedCount = room.BedCount });
                    }

                    newRecord.MonthOccupancy = occupancyCount;

                    retData.Add(newRecord);
                }
            }


            if (request.sort != null && !string.IsNullOrWhiteSpace(request.sort.SortBy))
            {
                switch (request.sort.SortBy.ToLower())
                {
                    case "roowowners":
                        retData = request.sort.SortDirection.ToLower() == "asc" ?
                                  retData.OrderBy(r => r.RoowOwners).ToList() :
                                  retData.OrderByDescending(r => r.RoowOwners).ToList();
                        break;
                    case "roomnumber":
                        retData = request.sort.SortDirection.ToLower() == "asc" ?
                                  retData.OrderBy(r => r.RoomNumber).ToList() :
                                  retData.OrderByDescending(r => r.RoomNumber).ToList();
                        break;
                    case "bedcount":
                        retData = request.sort.SortDirection.ToLower() == "asc" ?
                                  retData.OrderBy(r => r.BedCount).ToList() :
                                  retData.OrderByDescending(r => r.BedCount).ToList();

                        break;
                }
            }


            var returnData = new ActiveSearchRoomResponse
            {
                data = retData.OrderByDescending(x => x.VirtualRoom).ToList(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = campRoomFilter.Count()
            };

            return returnData;

        }





        #endregion






        public async Task<ActiveSearchRoomResponse> StatusBetweenDates2(ActiveSearchRoomRequest request, CancellationToken cancellationToken)
        {

            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;



            var campRoomFilter = Context.Room.AsQueryable();


            if (request.model.CampId.HasValue)
            {
                campRoomFilter = Context.Room
                    .Where(x => x.CampId == request.model.CampId || x.VirtualRoom == 1);
            }


            if (request.model.bedCount.HasValue)
            {
                campRoomFilter = Context.Room.Where(x => x.BedCount == request.model.bedCount.Value);
            }

            if (request.model.RoomTypeId.HasValue)
            {
                campRoomFilter = campRoomFilter.Where(e => e.RoomTypeId == request.model.RoomTypeId || e.VirtualRoom == 1);
            }
            if (request.model.Private.HasValue)
            {
                campRoomFilter = campRoomFilter.Where(e => e.Private == request.model.Private || e.VirtualRoom == 1);
            }
            if (!string.IsNullOrWhiteSpace(request.model.RoomNumber))
            {
                campRoomFilter = campRoomFilter.Where(e => e.Number.ToLower().Contains(request.model.RoomNumber.ToLower()) || e.VirtualRoom == 1);
            }

            var campRoomIds = await campRoomFilter.Select(e => e.Id).ToListAsync();
            var statusRoomDate = await Context.EmployeeStatus
                .Where(x => x.EventDate >= request.model.startDate.Date && x.EventDate <= request.model.endDate.Date &&
                            x.RoomId.HasValue && campRoomIds.Contains(x.RoomId.Value))
                .ToListAsync();

            var retData = new List<ActiveSearchRoom>();

            foreach (var room in campRoomFilter)
            {




                var newRecord = new ActiveSearchRoom
                {
                    RoomId = room.Id,
                    Private = room.Private,
                    RoomNumber = room.Number,
                    VirtualRoom = room.VirtualRoom,
                    BedCount = room.BedCount,
                    OccupancyData = new Dictionary<string, ActiveSearchRoomResponseDateInfo>()
                };

                for (var currentDate = request.model.startDate.Date; currentDate <= request.model.endDate.Date; currentDate = currentDate.AddDays(1))
                {
                    var activeBed = room.BedCount;

                    var roomStatus = statusRoomDate
                        .Where(x => x.RoomId == room.Id && x.EventDate == currentDate)
                        .ToList();

                    if (room.VirtualRoom == 1)
                    {
                        activeBed = roomStatus.Count;
                    }
                    else if (roomStatus.Count > 0)
                    {
                        activeBed = room.BedCount - roomStatus.Count;
                    }
                    else {
                        activeBed = room.BedCount;
                    }

                    newRecord.OccupancyData.Add(currentDate.ToString("yyyy-MM-dd"), new ActiveSearchRoomResponseDateInfo { ActiveBeds = activeBed, BedCount = room.BedCount });
                }

                retData.Add(newRecord);
            }





            var returnData = new ActiveSearchRoomResponse
            {
                data = retData.OrderByDescending(x=> x.VirtualRoom)
                     .ToList(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = retData.Count()
            };

            return returnData;
        }





        #region RoomDetailDateProfile

        public async Task<List<DateProfileRoomDetailResponse>> GetRoomDetailDateProfile(DateProfileRoomDetailRequest request, CancellationToken cancellationToken)
        {

            var returnData = await (from es in Context.EmployeeStatus
                                    where es.RoomId == request.RoomId && es.EventDate.Value.Date == request.CurrentDate.Date
                                    join employee in Context.Employee on es.EmployeeId equals employee.Id
                                    join department in Context.Department on employee.DepartmentId equals department.Id into DepartmentData
                                    from department in DepartmentData.DefaultIfEmpty()
                                    join employer in Context.Department on employee.EmployerId equals employer.Id into EmployerData
                                    from employer in EmployerData.DefaultIfEmpty()
                                    join peopletype in Context.PeopleType on employee.PeopleTypeId equals peopletype.Id into PeopletypeData
                                    from peopletype in PeopletypeData.DefaultIfEmpty()



                                    select new DateProfileRoomDetailResponse
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
                                        DepartmentId = employee.DepartmentId,
                                        EmployerName = employer.Name,
                                        PeopleTypeCode = peopletype.Code

                                    }).ToListAsync();

            return returnData;


        }





        #endregion


        #region RoomDateProfile



       public async Task<DateProfileRoomOwnerAndLockResponse> GetRoomOwnerAndLockDateProfile(DateProfileRoomOwnerAndLockRequest request, CancellationToken cancellationToken)
        {
            var ownerhistoryData = await GetRoomOwnerFutureBooking(request.RoomId, request.CurrentDate.Date);
            var roomLockedData = await GetRoomLockedBooking(request.RoomId, request.CurrentDate.Date);
            var retData = new DateProfileRoomOwnerAndLockResponse
            {
                OwnerFutureBooking = ownerhistoryData,
                LockedEmployees = roomLockedData

            };


            return retData;
        }



        public async Task<DateProfileRoomResponse> GetRoomDateProfile(DateProfileRoomRequest request, CancellationToken cancellationToken)
        {

            int pageSize = request.pageSize <= 0 ? 100 : request.pageSize;
            int pageIndex = request.pageIndex <= 0 ? 0 : request.pageIndex - 1;

            var filteredEmployees = from es in Context.EmployeeStatus.AsNoTracking()
                                    where es.RoomId == request.model.RoomId && es.EventDate.Value.Date == request.model.CurrentDate.Date
                                    join employee in Context.Employee.AsNoTracking() on es.EmployeeId equals employee.Id
                                    join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into DepartmentData
                                    from department in DepartmentData.DefaultIfEmpty()
                                    join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into EmployerData
                                    from employer in EmployerData.DefaultIfEmpty()
                                    join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into PeopletypeData
                                    from peopletype in PeopletypeData.DefaultIfEmpty()
                                    select new
                                    {
                                        Employee = employee,
                                        DepartmentName = department.Name,
                                        EmployerName = employer.Description,
                                        PeopleTypeCode = peopletype.Code
                                    };

            var pagedEmployees = await filteredEmployees.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            List<DateProfileRoomResponseDateEmployee> employeeDetails = new List<DateProfileRoomResponseDateEmployee>();
            foreach (var emp in pagedEmployees)
            {
                DateTime? latesDate = null;
                DateTime? earliestDate = null;
                string? shiftCode = null;

  
                var latestStatus = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == emp.Employee.Id && x.EventDate >
                                    request.model.CurrentDate && x.RoomId != request.model.RoomId).OrderBy(x => x.EventDate).FirstOrDefaultAsync();

                if (latestStatus == null)
                {
                    latestStatus = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == emp.Employee.Id && x.EventDate >
                        request.model.CurrentDate && x.RoomId == request.model.RoomId).OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();
                    if (latestStatus != null)
                    {
                        latesDate = latestStatus.EventDate?.AddDays(1);

                    }
                }
                else {

                    latesDate = latestStatus?.EventDate;

                }



                var earliestStatus = await Context.EmployeeStatus.AsNoTracking()
                                      .Where(es => es.EmployeeId == emp.Employee.Id && es.RoomId != request.model.RoomId && es.EventDate <= request.model.CurrentDate)
                                      .OrderByDescending(es => es.EventDate)
                                      .FirstOrDefaultAsync();
                if (earliestStatus == null)
                {
                    earliestStatus = await Context.EmployeeStatus.AsNoTracking()
                      .Where(es => es.EmployeeId == emp.Employee.Id && es.RoomId == request.model.RoomId && es.EventDate <= request.model.CurrentDate)
                      .OrderBy(es => es.EventDate)
                      .FirstOrDefaultAsync();
                    earliestDate = earliestStatus.EventDate;
                }
                else {
                    earliestDate = earliestStatus.EventDate.Value.AddDays(1);
                }
                if (earliestStatus != null) {

                  var currentEmployeeStatus = await  Context.EmployeeStatus.Where(x => x.EmployeeId == emp.Employee.Id && x.EventDate.Value.Date == earliestDate.Value.Date && x.RoomId != null).FirstOrDefaultAsync();
                    if (currentEmployeeStatus != null) {
                        shiftCode = await Context.Shift.AsNoTracking().Where(x => x.Id == currentEmployeeStatus.ShiftId).Select(x => x.Code).FirstOrDefaultAsync();
                    }
                
                   
                }

                employeeDetails.Add(new DateProfileRoomResponseDateEmployee
                {
                    EmployeeId = emp.Employee.Id,
                    Id = emp.Employee.Id,
                    FullName = $"{emp.Employee.Firstname} {emp.Employee.Lastname}",
                    RoomOwner = request.model.RoomId == emp.Employee.RoomId,
                    Firstname = emp.Employee.Firstname,
                    Lastname = emp.Employee.Lastname,
                    Gender = emp.Employee.Gender,
                    HotelCheck = emp.Employee.HotelCheck,
                    SAPID = emp.Employee.SAPID,
                    DepartmentName = emp.DepartmentName,
                    DepartmentId = emp.Employee.DepartmentId,
                    EmployerName = emp.EmployerName,
                    PeopleTypeCode = emp.PeopleTypeCode,
                    StartDate = earliestStatus?.EventDate,
                    EndDate = latesDate?.Date,
                    ShiftCode = shiftCode
                }) ;
            }

            int totalRecords = await filteredEmployees.CountAsync(); // This ensures we count all potential records that match the criteria

            var returnData = new DateProfileRoomResponse
            {
                data = employeeDetails,
                pageSize = pageSize,
                currentPage = pageIndex + 1, // Page index presented as 1-based to the client
                totalcount = totalRecords
            };

            return returnData;

        }

        private async Task<List<DateProfileRoomOwnerAndLockLockResponseDateEmployee>> GetRoomLockedBooking(int RoomId, DateTime currentDate)
        {

            var activeDocumentActions = new List<string> { "Submitted", "Approved" };


            var requestDocumentIds = await Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == RequestDocumentType.SiteTravel
                     && activeDocumentActions.Contains(x.CurrentAction)).Select(x => x.Id).ToListAsync();


            var requestDocumentRoomDateAdd = await (from requestAddList in Context.RequestSiteTravelAdd.AsNoTracking().Where(x => requestDocumentIds.Contains(x.DocumentId) && x.RoomId == RoomId)
                                                 join inschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.inScheduleId equals inschedule.Id /*into inscheduleData
                                                 from inschedule in inscheduleData.DefaultIfEmpty()*/

                                                 join doc in Context.RequestDocument.AsNoTracking() on requestAddList.DocumentId equals doc.Id /*into docData
                                                 from doc in docData.DefaultIfEmpty()*/

                                                 join outschedule in Context.TransportSchedule.AsNoTracking() on requestAddList.outScheduleId equals outschedule.Id /*into outScheduleData
                                                 from outschedule in outScheduleData.DefaultIfEmpty()*/
                                                 select new
                                                 {
                                                     roomId = requestAddList.RoomId,
                                                     EmployeeId =   requestAddList.EmployeeId,
                                                     startDate = inschedule.EventDate < outschedule.EventDate ? inschedule.EventDate : outschedule.EventDate,
                                                     endDate = inschedule.EventDate < outschedule.EventDate ? outschedule.EventDate : inschedule.EventDate,
                                                     DocumentId = requestAddList.DocumentId,
                                                     DocumentTag = doc.DocumentTag

                                                 }
                                                ).Where(x=>  x.startDate.Date <= currentDate.Date &&   x.endDate.Date >= currentDate.Date).ToListAsync();


            


            if (requestDocumentRoomDateAdd.Count > 0)
            {
                var lockedEmplIds = requestDocumentRoomDateAdd.Select(x => x.EmployeeId).ToList();
                var documentEmployees = requestDocumentRoomDateAdd.Select(x => new { x.DocumentId, x.EmployeeId, x.DocumentTag, x.startDate, x.endDate }).ToList();
                var lockedData = await (from employee in Context.Employee.AsNoTracking().Where(x => lockedEmplIds.Contains(x.Id))
                                        join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into RoomData
                                        from room in RoomData.DefaultIfEmpty()
                                        join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into DepartmentData
                                        from department in DepartmentData.DefaultIfEmpty()
                                        join employer in Context.Department.AsNoTracking() on employee.EmployerId equals employer.Id into EmployerData
                                        from employer in EmployerData.DefaultIfEmpty()
                                        join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into PeopletypeData
                                        from peopletype in PeopletypeData.DefaultIfEmpty()

                                        select new DateProfileRoomOwnerAndLockLockResponseDateEmployee
                                        {
                                            EmployeeId = employee.Id,
                                            Id = employee.Id,
                                            FullName = $"{employee.Firstname} {employee.Lastname}",
                                            Firstname = employee.Firstname,
                                            Lastname = employee.Lastname,
                                            Gender = employee.Gender,
                                            HotelCheck = employee.HotelCheck,
                                            SAPID = employee.SAPID,
                                            DepartmentName = department.Name,
                                            DepartmentId = employee.DepartmentId,
                                            EmployerName = employer.Name,
                                            PeopleTypeCode = peopletype.Code,
                                            RoomOwner = RoomId == employee.RoomId
                                        }).ToListAsync();



                foreach (var item in lockedData)
                {
                    item.DocumentId = documentEmployees.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.DocumentId).First();
                    item.DocumentTag = documentEmployees.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.DocumentTag).First();
                    item.StartDate = documentEmployees.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.startDate).First();
                    item.EndDate = documentEmployees.Where(x => x.EmployeeId == item.EmployeeId).Select(x => x.endDate).First();


                }



                return lockedData;
            }
            else{
                return new List<DateProfileRoomOwnerAndLockLockResponseDateEmployee>();
            }




        }


        private async Task<List<DateProfileRoomOwnerAndLockResponseOwnerEmployee>> GetRoomOwnerFutureBooking(int RoomId, DateTime currentDate)
        {

            var ownerdData = await (from employee in Context.Employee.AsNoTracking().Where(x => x.RoomId == RoomId)
                                    join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into RoomData
                                    from room in RoomData.DefaultIfEmpty()
                                    join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into DepartmentData
                                    from department in DepartmentData.DefaultIfEmpty()
                                    join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into EmployerData
                                    from employer in EmployerData.DefaultIfEmpty()
                                    join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into PeopletypeData
                                    from peopletype in PeopletypeData.DefaultIfEmpty()

                                    select new DateProfileRoomOwnerAndLockResponseOwnerEmployee
                                    {
                                        EmployeeId = employee.Id,
                                        Id = employee.Id,
                                        FullName = $"{employee.Firstname} {employee.Lastname}",
                                        RoomNumber = room.Number,
                                        Firstname = employee.Firstname,
                                        Lastname = employee.Lastname,
                                        Gender = employee.Gender,
                                        HotelCheck = employee.HotelCheck,
                                        SAPID = employee.SAPID,
                                        DepartmentName = department.Name,
                                        DepartmentId = employee.DepartmentId,
                                        EmployerName = employer.Description,
                                        PeopleTypeCode = peopletype.Code,
                                    }).ToListAsync();






            foreach (var item in ownerdData)
            {
                var queryStart = await Context.Transport.AsNoTracking()
                    .Where(es => es.EventDate >= currentDate.Date && es.EmployeeId == item.EmployeeId && es.Direction == "IN")
                    .OrderBy(es => es.EventDate)
                    .Select(es => new
                    {
                        Id = es.Id,
                        EmployeeId = es.EmployeeId,
                        EventDate = es.EventDate
                    })
                    .FirstOrDefaultAsync();


                if (queryStart != null)
                {
                    item.StartDate = queryStart.EventDate;

                    var startDateShiftInfo = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == item.EmployeeId && x.EventDate.Value.Date == queryStart.EventDate.Value.Date && x.RoomId != null).FirstOrDefaultAsync();
                    if (startDateShiftInfo != null) {
                      item.ShiftCode =await  Context.Shift.AsNoTracking().Where(x => x.Id == startDateShiftInfo.ShiftId).Select(x => x.Code).FirstOrDefaultAsync();
                    }

                }
                if (queryStart != null)
                {
                    var queryEnd = await Context.Transport.AsNoTracking()
                        .Where(es => es.EmployeeId == item.EmployeeId && es.Direction == "OUT" && es.EventDate.Value.Date >= queryStart.EventDate)
                        .OrderBy(es => es.EventDate)
                        .Select(es => new
                        {
                            Id = es.Id,
                            EmployeeId = es.EmployeeId,
                            EventDate = es.EventDate
                        }).FirstOrDefaultAsync();
                    if (queryEnd != null) 
                    {
                        item.EndDate = queryEnd.EventDate;
                    }


                }
            }

            return ownerdData;

        }




        #endregion



        private async Task<DateTime?> GetEmployeeContinuesDate(DateTime currentDate, int? EmpId, bool startDate = true)
        {
            if (startDate)
            {
                var p_startDate =await Context.EmployeeStatus
                  .Where(x => x.EmployeeId == EmpId.Value && x.EventDate.Value.Date < currentDate.Date.Date && x.RoomId == null)
                  .OrderByDescending(x => x.EventDate).Select(x => x.EventDate).FirstOrDefaultAsync();

                if (p_startDate.HasValue)
                {
                    var pp_date =await Context.EmployeeStatus
                        .Where(x => x.EmployeeId == EmpId.Value && x.EventDate.Value.Date > p_startDate && x.RoomId != null)
                        .OrderBy(x => x.EventDate).Select(x => x.EventDate).FirstOrDefaultAsync();

                    return pp_date;
                }
                else
                {
                    var pp_startDate =await Context.EmployeeStatus.AsNoTracking()
                      .Where(x => x.EmployeeId == EmpId.Value && x.EventDate.Value.Date <= currentDate.Date && x.RoomId != null)
                      .OrderBy(x => x.EventDate).Select(x => x.EventDate).FirstOrDefaultAsync();
                    return pp_startDate;
                }

            }
            else
            {


                var p_date =await Context.EmployeeStatus.AsNoTracking()
                    .Where(x => x.EmployeeId == EmpId.Value && x.EventDate.Value.Date > currentDate.Date && x.RoomId == null)
                    .OrderBy(x => x.EventDate).Select(x => x.EventDate).FirstOrDefaultAsync();
                if (p_date.HasValue)
                {

                    var pp_date =await Context.EmployeeStatus.AsNoTracking()
                        .Where(x => x.EmployeeId == EmpId.Value && x.EventDate.Value.Date < p_date && x.RoomId != null)
                        .OrderByDescending(x => x.EventDate).Select(x => x.EventDate).FirstOrDefaultAsync();

                    return pp_date;
                }
                else {
                    var pp_endDate =await Context.EmployeeStatus
                      .Where(x => x.EmployeeId == EmpId.Value && x.EventDate.Value.Date >= currentDate.Date && x.RoomId != null)
                      .OrderByDescending(x => x.EventDate).Select(x => x.EventDate).FirstOrDefaultAsync();
                    return pp_endDate;
                }

            }


        }


        public async Task<DateStatusRoomResponse?> GetRoomDateStatus(DateStatusRoomRequest request, CancellationToken cancellationToken)
        {
            var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId && x.Active == 1).ToListAsync(cancellationToken);
            if (currentRoom != null)
            {
                var beds = await Context.Bed.AsNoTracking().Where(x => x.RoomId == request.RoomId).ToListAsync();
                var beditems = new List<DateStatusRoomBed>();
                var statusRoomDates = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate == request.CurrentDate.Date && x.RoomId == request.RoomId).ToListAsync();

                foreach (var item in beds)
                {

                    var currentStatusDate = statusRoomDates.FirstOrDefault(x => x.BedId == item.Id && x.EmployeeId != null); ;

                    if (currentStatusDate != null)
                    {
                        var currentEmployee =await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentStatusDate.EmployeeId);
                        var guest = new DateStatusRoomBedGuest
                        {
                            Id = currentEmployee.Id,
                            Lastname = currentEmployee.Lastname,
                            Firstname = currentEmployee.Firstname
                        };

                        var itemBed = new DateStatusRoomBed
                        {
                            Description = item.Description,
                            Id = item.Id,
                            Guest = guest

                        };
                        beditems.Add(itemBed);

                    }
                    else
                    {
                        var itemBed = new DateStatusRoomBed
                        {
                            Description = item.Description,
                            Id = item.Id

                        };
                        beditems.Add(itemBed);
                    }
                }

                var returnData = new DateStatusRoomResponse
                {
                    beds = beditems

                };

                return returnData;


            }
            else
            {
                var aa = new DateStatusRoomResponse { };
                return null;
            }


        }



        public async Task<int?> GetAvailableBeds(int roomId, DateTime eventDate)
        {
            var statusRoomDate = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate == eventDate.Date && x.RoomId == roomId).ToListAsync();

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
                else {
                    return null;
                }



            }
            else {
                return null;
            }

        }

        public async Task CreateValidateRoom(CreateRoomRequest request, CancellationToken cancellationToken)
        {
            List<string> errors = new List<string>();
            if (request.VirtualRoom  >0 )
            {
                var roomData = await Context.Room.AnyAsync(x=> x.VirtualRoom == 1);
                if (!roomData)
                {
                    errors.Add("Sorry, only one virtual room can be created");
                }
            }
            if (errors.Count > 0) {
                throw new BadRequestException(errors.ToArray());
            }
            await Task.CompletedTask;
        }


        public async Task UpdateValidateRoom(UpdateRoomRequest request, CancellationToken cancellationToken)
        {
            var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.Id).FirstOrDefaultAsync();

            if (currentRoom?.VirtualRoom > 0) {
                throw new BadRequestException("Virtual type room Unable to edit");
            }


        }


        public async Task<GetVirtualRoomResponse> GetVirtualRoomId(CancellationToken cancellationToken)
        {
            var CurrentRoom =await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 1).Select(x=> new { x.Id}).FirstOrDefaultAsync();
            var NoAccomdationRoom =await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 2).Select(x=> new { x.Id}).FirstOrDefaultAsync();
            var KhanbogdRoom = await Context.Room.AsNoTracking().Where(x => x.VirtualRoom == 3).Select(x=> new { x.Id}).FirstOrDefaultAsync();


            return new GetVirtualRoomResponse
            {
                Id = CurrentRoom.Id,
                NoAccommdationId = NoAccomdationRoom?.Id,
                KhanbogdRoomId = KhanbogdRoom?.Id
            };
        }


        #region GetRoomMonthStatus

        public async Task<MonthStatusRoomResponse> GetRoomMonthStatus(MonthStatusRoomRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;
            var startDate = request.model.CurrentDate.Date.AddDays((-1) * (request.model.CurrentDate.Day - 1));
            //  var endDate = startDate.AddMonths(1).AddDays(-1);
            var endDate = startDate.AddMonths(2).AddDays(-1);

            var returnDataEmployees = new List<MonthStatusRoomEmployees>();

            var ownerEmployeeIds = new List<int>();

            var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == request.model.RoomId && x.Active == 1).FirstOrDefaultAsync();
            if (currentRoom == null) {
                throw new BadRequestException("Room not found");
            } 



            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var filteredEmployeeIds = new List<int>();

            if (currentRoom.VirtualRoom > 0)
            {

                if (!string.IsNullOrWhiteSpace(request.model.keyword))
                {
                    if (Transliterator.IsNumeric(request.model.keyword))
                    {
                        var activeData = await Context.EmployeeStatus.AsNoTracking()
                               .Where(es => es.EventDate.Value.Date >= startDate.Date &&
                                            es.EventDate.Value.Date <= endDate.Date &&
                                            es.RoomId == request.model.RoomId &&
                                            es.EmployeeId == Convert.ToInt32(request.model.keyword)).Select(x => x.EmployeeId).Distinct()
                               .FirstOrDefaultAsync(cancellationToken);
                        if (activeData != null)
                        {
                            filteredEmployeeIds.Add(activeData.Value);
                        }
                        else
                        {
                            var employeeStatusListEmployeeIds = await Context.EmployeeStatus.AsNoTracking()
                           .Where(es => es.EventDate.Value.Date >= startDate.Date &&
                                        es.EventDate.Value.Date <= endDate.Date &&
                                        es.RoomId == request.model.RoomId).Select(x => x.EmployeeId).Distinct()
                           .ToListAsync(cancellationToken);

                            var employees = await Context.Employee.AsNoTracking()
                                .Where(e => employeeStatusListEmployeeIds.Contains(e.Id))
                                .ToListAsync(cancellationToken);


                            var keyword = request.model.keyword?.ToLower();
                            var searchDataResult = employees.Where(c => (c.SAPID != null && c.SAPID.ToString().Contains(keyword))
                               ).ToList();
                            if (searchDataResult.Count > 0)
                            {
                                filteredEmployeeIds = searchDataResult.Select(x => x.Id).Distinct().ToList();
                            }

                        }
                    }
                    else
                    {
                        var employeeStatusListEmployeeIds = await Context.EmployeeStatus.AsNoTracking()
                               .Where(es => es.EventDate.Value.Date >= startDate.Date &&
                                            es.EventDate.Value.Date <= endDate.Date &&
                                            es.RoomId == request.model.RoomId).Select(x => x.EmployeeId).Distinct()
                               .ToListAsync(cancellationToken);

                        var employees = await Context.Employee.AsNoTracking()
                            .Where(e => employeeStatusListEmployeeIds.Contains(e.Id))
                            .ToListAsync(cancellationToken);


                        var keyword = request.model.keyword?.ToLower();
                        var searchDataResult = employees.Where(c =>
                               (c.Firstname != null && c.Firstname.ToLower().StartsWith(keyword))
                               || (c.Lastname != null && c.Lastname.ToLower().StartsWith(keyword))
                           ).ToList();

                        if (searchDataResult.Count > 0)
                        {
                            filteredEmployeeIds = searchDataResult.Select(x => x.Id).Distinct().ToList();
                        }


                    }
                }
                else
                {

                    filteredEmployeeIds = await Context.EmployeeStatus.AsNoTracking()
                       .Where(es => es.EventDate.Value.Date >= startDate.Date &&
                                    es.EventDate.Value.Date <= endDate.Date &&
                                    es.RoomId == request.model.RoomId).OrderByDescending(x => x.EmployeeId).Select(x => x.EmployeeId.Value).Distinct()
                       .ToListAsync(cancellationToken);


                }

            }
           else {



                ownerEmployeeIds = await Context.Employee.AsNoTracking().Where(x => x.RoomId == request.model.RoomId).Select(c => c.Id).ToListAsync();


                var employeeStatusList = await Context.EmployeeStatus
                .AsNoTracking()
                .Where(es => es.EventDate.HasValue &&
                            es.EventDate.Value.Date >= startDate.Date &&
                            es.EventDate.Value.Date <= endDate.Date &&
                            es.RoomId == request.model.RoomId)
                .ToListAsync(cancellationToken);

                // Fetch all the necessary employees
                var employees = await Context.Employee.AsNoTracking()
                                        .Where(e => employeeStatusList.Select(es => es.EmployeeId).Contains(e.Id))
                                        .ToListAsync(cancellationToken);

                // Perform the join in memory
                var DateEmployeeStatus = (from employee in employees
                                          join es in employeeStatusList on employee.Id equals es.EmployeeId
                                          select new
                                          {
                                              EmployeeId = es.EmployeeId,
                                              ShiftId = es.ShiftId,
                                              SAPID = employee.SAPID,
                                              Firstname = employee.Firstname,
                                              Lastname = employee.Lastname,
                                              HasOrder = 0
                                          }).ToList();
                if (!string.IsNullOrWhiteSpace(request.model.keyword))
                {

                    var keyword = request.model.keyword?.ToLower();
                    var searchDataResult = DateEmployeeStatus.Where(c =>
                           (c.Firstname != null && c.Firstname.ToLower().StartsWith(keyword))
                           || (c.Lastname != null && c.Lastname.ToLower().StartsWith(keyword))
                           || (c.SAPID != null && c.SAPID.ToString().Contains(keyword))
                           || c.EmployeeId.ToString().Contains(keyword)
                       ).ToList();

                    if (searchDataResult.Count > 0)
                    {
                        filteredEmployeeIds = searchDataResult.Select(x => x.EmployeeId.Value).Distinct().ToList();
                    }
                }


                var empIds = DateEmployeeStatus.Select(x => x.EmployeeId.Value).Distinct().ToList();

                if (!filteredEmployeeIds.Any())
                {
                    foreach (var item in empIds)
                    {
                        if (filteredEmployeeIds.IndexOf(item) == -1)
                        {
                            filteredEmployeeIds.Add(item);
                        }




                    }
                }
                else{ 
                    filteredEmployeeIds = DateEmployeeStatus.Select(x => x.EmployeeId.Value).Distinct().ToList();
                }
            }



            //
            var pageEmpIds = new List<int>();
            if (currentRoom?.VirtualRoom > 0)
            {
                pageEmpIds = filteredEmployeeIds.Skip(pageIndex * pageSize)
                          .Take(pageSize)
                          .ToList();
            }
            else
            {
                pageEmpIds = filteredEmployeeIds;
            }


            stopwatch.Restart();


            var AnotherRoomDateStatus = await (from employeeStatus in Context.EmployeeStatus.AsNoTracking()
                                               where pageEmpIds.Contains(employeeStatus.EmployeeId.Value) && employeeStatus.EventDate.Value.Date >= startDate.Date && employeeStatus.EventDate.Value.Date <= endDate.Date
                                               && employeeStatus.RoomId != request.model.RoomId && employeeStatus.RoomId != null
                                               join employee in Context.Employee.AsNoTracking() on employeeStatus.EmployeeId equals employee.Id
                                               join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                               from department in departmentData.DefaultIfEmpty()
                                               join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                               from position in positionData.DefaultIfEmpty()

                                               join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                               from employer in employerData.DefaultIfEmpty()
                                               join room in Context.Room.AsNoTracking() on employeeStatus.RoomId equals room.Id into roomData
                                               from room in roomData.DefaultIfEmpty()
                                               join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                               from peopletype in peopletypeData.DefaultIfEmpty()
                                               select new
                                               {
                                                   EmployeeId = employee.Id,
                                                   Id = employee.Id,
                                                   SAPID = employee.SAPID,
                                                   Firstname = employee.Firstname,
                                                   Lastname = employee.Lastname,
                                                   PeopleTypeCode = peopletype.Code,
                                                   DepartmentName = department.Name,
                                                   PositionName = position.Description,
                                                   Gender = employee.Gender,
                                                   RoomOwner = employee.RoomId == request.model.RoomId,
                                                   HotelCheck = employee.HotelCheck,
                                                   RoomNumber = room.Number,
                                                   RoomId = room.Id,
                                                   EmployerName = employer.Description,
                                                   EventDate = employeeStatus.EventDate,
                                                   ShiftId = employeeStatus.ShiftId
                                               }).ToArrayAsync();

            Console.WriteLine("AnotherRoomDateStatus Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();

            var dateStatusShifts = await (from shift in Context.Shift.AsNoTracking()
                                          join color in Context.Color.AsNoTracking() on shift.ColorId equals color.Id into colorData
                                          from color in colorData.DefaultIfEmpty()
                                          select new
                                          {
                                              Id = shift.Id,
                                              Description = shift.Description,
                                              Code = shift.Code,
                                              ColorCode = color.Code
                                          }).ToListAsync();

            Console.WriteLine("dateStatusShifts Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();




            var transportQuery = await (from transport in Context.Transport.AsNoTracking().Where(x => pageEmpIds.Contains(x.EmployeeId.Value) && x.EventDate.Value.Date >= startDate && x.EventDate.Value.Date <= endDate)
                                        join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id
                                        select new
                                        {
                                            employeeId = transport.EmployeeId,
                                            eventDate = transport.EventDate,
                                            direction = transport.Direction,
                                            Code = transportSchedule.Description,
                                            eventDateTime = transport.EventDateTime
                                        }).ToListAsync();

            Console.WriteLine("transportQuery Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Restart();



            

            foreach (var empId in pageEmpIds)
            {

                var currentEmployee = await (from employeeStatus in Context.EmployeeStatus.AsNoTracking()
                                             where employeeStatus.EmployeeId == empId && employeeStatus.EventDate.Value.Date >= startDate.Date && employeeStatus.EventDate.Value.Date <= endDate.Date &&  employeeStatus.RoomId == request.model.RoomId
                                             join employee in Context.Employee.AsNoTracking() on employeeStatus.EmployeeId equals employee.Id
                                             join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                             from department in departmentData.DefaultIfEmpty()
                                             join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                             from employer in employerData.DefaultIfEmpty()

                                             join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                             from position in positionData.DefaultIfEmpty()

                                             join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into roomData
                                             from room in roomData.DefaultIfEmpty()
                                             join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                             from peopletype in peopletypeData.DefaultIfEmpty()
                                             select new
                                             {
                                                 EmployeeId = employee.Id,
                                                 Id = employee.Id,
                                                 SAPID = employee.SAPID,
                                                 Firstname = employee.Firstname,
                                                 Lastname = employee.Lastname,
                                                 PeopleTypeCode = peopletype.Code,
                                                 DepartmentName = department.Name,
                                                 DepartmentId = employee.DepartmentId,
                                                 PositionName = position.Description,
                                                 Gender = employee.Gender,
                                                 RoomOwner = employee.RoomId == request.model.RoomId,
                                                 HotelCheck = employee.HotelCheck,
                                                 RoomNumber = room.Number,
                                                 RoomId = employee.RoomId,
                                                 EmployerName = employer.Description,
                                                 EventDate = employeeStatus.EventDate,
                                                 ShiftId = employeeStatus.ShiftId,
                                                 HasOrder = 0
                                             }).OrderBy(x => x.Firstname).ToListAsync();

                    var newData = new MonthStatusRoomEmployees
                    {
                        Id = currentEmployee[0].Id,
                        SAPID = currentEmployee[0].SAPID,
                        Firstname = currentEmployee[0].Firstname,
                        Lastname = currentEmployee[0].Lastname,
                        PeopleTypeCode = currentEmployee[0].PeopleTypeCode,
                        DepartmentId = currentEmployee[0].DepartmentId,
                        DepartmentName = currentEmployee[0].DepartmentName,
                        PositionName = currentEmployee[0].PositionName,
                        Gender = currentEmployee[0].Gender,
                        RoomOwner = currentEmployee[0].RoomOwner,
                        HotelCheck = currentEmployee[0].HotelCheck,
                        OccupancyData = new Dictionary<string, MonthStatusRoomResponseDateInfo>(),
                        AnotherRoomData = new Dictionary<string, MonthStatusAnotherRoomResponseDateInfo>(),
                        RoomId = currentEmployee[0].RoomId,
                        RoomNumber = currentEmployee[0].RoomNumber,
                        EmployerName = currentEmployee[0].EmployerName

                    };
                    DateTime currentDate = startDate;
                    while (currentDate <= endDate)
                    {

                        Console.WriteLine(currentDate.ToLongDateString() + "-------------------------" + empId);
                        var currentDateData = currentEmployee
                            .Where(x => x.EventDate == currentDate.Date)
                            .FirstOrDefault();
                        if (currentDateData != null)
                        {
                            var currentShift = dateStatusShifts.Where(x => x.Id == currentDateData.ShiftId).FirstOrDefault();
                            var currentTransport = transportQuery.Where(x => x.employeeId == empId && (x.eventDate.Value.Date == currentDate.Date || x.eventDate.Value.Date == currentDate.Date.AddDays(1))).OrderByDescending(x => x.eventDateTime).ToList();


                            var tCode = new List<string>();
                            var directions = new List<string>();

                            foreach (var item in currentTransport)
                            {

                                if (!string.IsNullOrWhiteSpace(item.direction) && !directions.Contains(item.direction))
                                {
                                    directions.Add(item.direction);
                                }
                                if (!string.IsNullOrWhiteSpace(item.Code) && !tCode.Contains(item.Code))
                                {

                                    tCode.Add($"{item.eventDate?.ToString("MM/dd") ?? "NA"} {item.Code}");
                                }
                            }

                            string transportDirection = string.Join("/", directions);
                            string transportCode = string.Join("/", tCode);


                            if (currentShift != null)
                            {
                                var color = dateStatusShifts.FirstOrDefault(c => c.Id == currentDateData.ShiftId);

                                if (color != null)
                                {
                                    string? colorCode = color?.ColorCode;
                                    newData.OccupancyData?.Add(currentDate.ToString("yyyy-MM-dd"), new MonthStatusRoomResponseDateInfo { ShiftCode = currentShift?.Code, ShiftDescription = currentShift?.Description, ShiftColor = colorCode, transporScheduleCode = transportCode, transportDirection = transportDirection });
                                }
                                else
                                {
                                    newData.OccupancyData?.Add(currentDate.ToString("yyyy-MM-dd"), new MonthStatusRoomResponseDateInfo { ShiftCode = currentShift?.Code, ShiftDescription = currentShift?.Description, ShiftColor = "green", transporScheduleCode = transportCode, transportDirection = transportDirection });
                                }
                            }
                            else
                            {
                                newData.OccupancyData?.Add(currentDate.ToString("yyyy-MM-dd"), new MonthStatusRoomResponseDateInfo { ShiftCode = currentShift?.Code, ShiftDescription = currentShift?.Description, ShiftColor = "green", transporScheduleCode = transportCode, transportDirection = transportDirection });
                            }

                        }
                        else
                        {
                            var currentAnotherRoomDateData = AnotherRoomDateStatus
                            .Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == empId)
                            .FirstOrDefault();
                            if (currentAnotherRoomDateData != null)
                            {
                                //var currentAnotherRoom = await Context.Room.Where(c => c.Id == currentAnotherRoomDateData.RoomId).Select(x => new { x.Number }).FirstOrDefaultAsync();
                                //  if (currentAnotherRoom != null) {
                                newData.AnotherRoomData.Add(currentDate.ToString("yyyy-MM-dd"), new MonthStatusAnotherRoomResponseDateInfo { RoomId = currentAnotherRoomDateData.RoomId, RoomNumber = currentAnotherRoomDateData.RoomNumber });
                                //   }

                            }

                        }
                        currentDate = currentDate.AddDays(1);
                    }
                    returnDataEmployees.Add(newData);
            }


            foreach (var item in ownerEmployeeIds)
            {


                if (returnDataEmployees.Where(c => c.Id == item).FirstOrDefault() == null) 
                {


                    var AnotherRoomDateStatusOwner = await (from employeeStatus in Context.EmployeeStatus.AsNoTracking()
                                                       where employeeStatus.EmployeeId == item && employeeStatus.EventDate.Value.Date >= startDate.Date && employeeStatus.EventDate.Value.Date <= endDate.Date
                                                       && employeeStatus.RoomId != request.model.RoomId && employeeStatus.RoomId != null
                                                       join employee in Context.Employee.AsNoTracking() on employeeStatus.EmployeeId equals employee.Id
                                                       join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                                       from department in departmentData.DefaultIfEmpty()
                                                       join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                                       from position in positionData.DefaultIfEmpty()

                                                       join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                                       from employer in employerData.DefaultIfEmpty()
                                                       join room in Context.Room.AsNoTracking() on employeeStatus.RoomId equals room.Id into roomData
                                                       from room in roomData.DefaultIfEmpty()
                                                       join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                                       from peopletype in peopletypeData.DefaultIfEmpty()
                                                       select new
                                                       {
                                                           EmployeeId = employee.Id,
                                                           Id = employee.Id,
                                                           SAPID = employee.SAPID,
                                                           Firstname = employee.Firstname,
                                                           Lastname = employee.Lastname,
                                                           PeopleTypeCode = peopletype.Code,
                                                           DepartmentName = department.Name,
                                                           PositionName = position.Description,
                                                           Gender = employee.Gender,
                                                           RoomOwner = employee.RoomId == request.model.RoomId,
                                                           HotelCheck = employee.HotelCheck,
                                                           RoomNumber = room.Number,
                                                           RoomId = room.Id,
                                                           EmployerName = employer.Description,
                                                           EventDate = employeeStatus.EventDate,
                                                           ShiftId = employeeStatus.ShiftId
                                                       }).ToArrayAsync();



                    var currentEmployeeAnotherRoom = await (from employee in Context.Employee.AsNoTracking().Where(e => e.Id == item)
                                                            join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                                            from department in departmentData.DefaultIfEmpty()
                                                            join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                                            from employer in employerData.DefaultIfEmpty()

                                                            join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                                            from position in positionData.DefaultIfEmpty()

                                                            join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into roomData
                                                            from room in roomData.DefaultIfEmpty()
                                                            join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                                            from peopletype in peopletypeData.DefaultIfEmpty()
                                                            select new
                                                            {
                                                                EmployeeId = employee.Id,
                                                                Id = employee.Id,
                                                                SAPID = employee.SAPID,
                                                                Firstname = employee.Firstname,
                                                                Lastname = employee.Lastname,
                                                                PeopleTypeCode = peopletype.Code,
                                                                DepartmentName = department.Name,
                                                                DepartmentId = employee.DepartmentId,
                                                                PositionName = position.Description,
                                                                Gender = employee.Gender,
                                                                RoomOwner = employee.RoomId == request.model.RoomId,
                                                                HotelCheck = employee.HotelCheck,
                                                                RoomNumber = room.Number,
                                                                RoomId = employee.RoomId,
                                                                EmployerName = employer.Description,
                                                                HasOrder = 0
                                                            }).OrderBy(x => x.Firstname).FirstOrDefaultAsync();


                    if (currentEmployeeAnotherRoom != null && AnotherRoomDateStatusOwner.Count() > 0)
                    {
                        var newData = new MonthStatusRoomEmployees
                        {
                            Id = currentEmployeeAnotherRoom.Id,
                            SAPID = currentEmployeeAnotherRoom.SAPID,
                            Firstname = currentEmployeeAnotherRoom.Firstname,
                            Lastname = currentEmployeeAnotherRoom.Lastname,
                            PeopleTypeCode = currentEmployeeAnotherRoom.PeopleTypeCode,
                            DepartmentId = currentEmployeeAnotherRoom.DepartmentId,
                            DepartmentName = currentEmployeeAnotherRoom.DepartmentName,
                            PositionName = currentEmployeeAnotherRoom.PositionName,
                            Gender = currentEmployeeAnotherRoom.Gender,
                            RoomOwner = currentEmployeeAnotherRoom.RoomOwner,
                            HotelCheck = currentEmployeeAnotherRoom.HotelCheck,
                            OccupancyData = new Dictionary<string, MonthStatusRoomResponseDateInfo>(),
                            AnotherRoomData = new Dictionary<string, MonthStatusAnotherRoomResponseDateInfo>(),
                            RoomId = currentEmployeeAnotherRoom.RoomId,
                            RoomNumber = currentEmployeeAnotherRoom.RoomNumber,
                            EmployerName = currentEmployeeAnotherRoom.EmployerName

                        };
                        DateTime currentDate = startDate;
                        while (currentDate <= endDate)
                        {
                            var currentAnotherRoomDateData = AnotherRoomDateStatusOwner
                            .Where(x => x.EventDate.Value.Date == currentDate.Date && x.EmployeeId == item)
                            .FirstOrDefault();
                            if (currentAnotherRoomDateData != null)
                            {
                                //var currentAnotherRoom = await Context.Room.Where(c => c.Id == currentAnotherRoomDateData.RoomId).Select(x => new { x.Number }).FirstOrDefaultAsync();
                                //  if (currentAnotherRoom != null) {
                                newData.AnotherRoomData.Add(currentDate.ToString("yyyy-MM-dd"), new MonthStatusAnotherRoomResponseDateInfo { RoomId = currentAnotherRoomDateData.RoomId, RoomNumber = currentAnotherRoomDateData.RoomNumber });
                                //   }

                            }
                            currentDate = currentDate.AddDays(1);
                        }
                        returnDataEmployees.Add(newData);
                    }



                }

            }


            returnDataEmployees = returnDataEmployees
                .OrderByDescending(x => x.RoomOwner.HasValue ? x.RoomOwner.Value : false)
                .ThenBy(x => string.IsNullOrEmpty(x.Firstname) ? "" : x.Firstname).ToList();


            Console.WriteLine("pageEmpIds foreach Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
            stopwatch.Stop();

            var returnData = new MonthStatusRoomResponse
            {
                data = returnDataEmployees,
            pageSize = pageSize,
                currentPage = pageIndex + 1,
                totalcount = filteredEmployeeIds.Count
            };



           

            return returnData;

        }





        public async Task<List<MonthStatusRoomOwnerResponse>> GetRoomMonthStatusOwner(MonthStatusRoomOwnerRequest request, CancellationToken cancellationToken)
        {

            var roomOwnerList = new List<MonthStatusRoomOwnerResponse>();

            var ownerEmployees = await (from employee in Context.Employee.AsNoTracking()
                                        where employee.RoomId == request.RoomId
                                        join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                        from department in departmentData.DefaultIfEmpty()
                                        join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                        from employer in employerData.DefaultIfEmpty()
                                        join position in Context.Position on employee.PositionId equals position.Id into positionData
                                        from position in positionData.DefaultIfEmpty()


                                        join room in Context.Room on employee.RoomId equals room.Id into roomData
                                        from room in roomData.DefaultIfEmpty()
                                        join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                        from peopletype in peopletypeData.DefaultIfEmpty()
                                        select new
                                        {
                                            EmployeeId = employee.Id,
                                            Id = employee.Id,
                                            SAPID = employee.SAPID,
                                            Firstname = employee.Firstname,
                                            Lastname = employee.Lastname,
                                            PeopleTypeCode = peopletype.Code,
                                            DepartmentName = department.Name,
                                            DepartmemntId = employee.DepartmentId,
                                            PositionName = position.Description,
                                            Gender = employee.Gender,
                                            RoomOwner = employee.RoomId == request.RoomId,
                                            HotelCheck = employee.HotelCheck,
                                            RoomNumber = room.Number,
                                            RoomId = employee.RoomId,
                                            EmployerName = employer.Description,
                                        }).ToArrayAsync();


            var owneremployeeIds = ownerEmployees.Select(x => x.EmployeeId).ToList();


            foreach (var ownerEmployee in ownerEmployees)
            {

                DateTime? ownerTransportINDate = null;
                string? ownerTransportINDescr = "";

                DateTime? ownerTransportOUTDate = null;
                string? ownerTransportOUTDescr = "";
                var transportStartDate = DateTime.Today;

                if (request.CurrentDate.Year == DateTime.Today.Year && request.CurrentDate.Month == DateTime.Today.Month)
                {
                    var todayOnsite = await Context.EmployeeStatus.AsNoTracking()
                           .Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.RoomId != null && x.EventDate.Value.Date == DateTime.Today)
                           .FirstOrDefaultAsync();
                    if (todayOnsite != null)
                    {
                        var transportINData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date < DateTime.Today && x.Direction == "IN").OrderByDescending(x => x.EventDate)
                                                     join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                     from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         EventDate = transport.EventDate,
                                                         Description = transportSchedule.Description
                                                     }).FirstOrDefaultAsync();
                        if (transportINData != null)
                        {

                            ownerTransportINDate = transportINData.EventDate;
                            ownerTransportINDescr = transportINData.Description;
                            var transportOUTData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date > transportINData.EventDate.Value.Date && x.Direction == "OUT").OrderBy(x => x.EventDate)
                                                          join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                          from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              EventDate = transport.EventDate,
                                                              Description = transportSchedule.Description
                                                          }).FirstOrDefaultAsync();
                            if (transportOUTData != null)
                            {
                                ownerTransportOUTDate = transportOUTData.EventDate;
                                ownerTransportOUTDescr = transportOUTData.Description;

                            }
                        }
                    }
                    else {
                        var transportINData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date >= DateTime.Today && x.Direction == "IN").OrderByDescending(x => x.EventDate)
                                                     join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                     from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         EventDate = transport.EventDate,
                                                         Description = transportSchedule.Description
                                                     }).OrderBy(x=> x.EventDate).FirstOrDefaultAsync();
                        if (transportINData != null)
                        {

                            ownerTransportINDate = transportINData.EventDate;
                            ownerTransportINDescr = transportINData.Description;
                            var transportOUTData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date > transportINData.EventDate.Value.Date && x.Direction == "OUT").OrderBy(x => x.EventDate)
                                                          join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                          from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              EventDate = transport.EventDate,
                                                              Description = transportSchedule.Description
                                                          }).FirstOrDefaultAsync();
                            if (transportOUTData != null)
                            {
                                ownerTransportOUTDate = transportOUTData.EventDate;
                                ownerTransportOUTDescr = transportOUTData.Description;

                            }
                        }
                    }
                }
                else
                {

                    var currentOnsite = await Context.EmployeeStatus.AsNoTracking()
                       .Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.RoomId != null && x.EventDate.Value.Date == request.CurrentDate)
                       .FirstOrDefaultAsync();
                    if (currentOnsite != null)
                    {
                        var transportINData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date >= request.CurrentDate && x.Direction == "IN").OrderBy(x => x.EventDate)
                                                     join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                     from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         EventDate = transport.EventDate,
                                                         Description = transportSchedule.Description
                                                     }).FirstOrDefaultAsync();
                        if (transportINData != null)
                        {
                            ownerTransportINDate = transportINData.EventDate;
                            ownerTransportINDescr = transportINData.Description;
                            var transportOUTData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date > transportINData.EventDate.Value.Date && x.Direction == "OUT").OrderBy(x => x.EventDate)
                                                          join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                          from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              EventDate = transport.EventDate,
                                                              Description = transportSchedule.Description
                                                          }).FirstOrDefaultAsync();
                            if (transportOUTData != null)
                            {
                                ownerTransportOUTDate = transportOUTData.EventDate;
                                ownerTransportOUTDescr = transportOUTData.Description;

                            }
                        }
                    }
                    else {
                        var transportINData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date >= request.CurrentDate && x.Direction == "IN").OrderBy(x => x.EventDate)
                                                     join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                     from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         EventDate = transport.EventDate,
                                                         Description = transportSchedule.Description
                                                     }).FirstOrDefaultAsync();
                        if (transportINData != null)
                        {
                            ownerTransportINDate = transportINData.EventDate;
                            ownerTransportINDescr = transportINData.Description;
                            var transportOUTData = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == ownerEmployee.EmployeeId && x.EventDate.Value.Date > transportINData.EventDate.Value.Date && x.Direction == "OUT").OrderBy(x => x.EventDate)
                                                          join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id into transportScheduleData
                                                          from transportSchedule in transportScheduleData.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              EventDate = transport.EventDate,
                                                              Description = transportSchedule.Description
                                                          }).FirstOrDefaultAsync();
                            if (transportOUTData != null)
                            {
                                ownerTransportOUTDate = transportOUTData.EventDate;
                                ownerTransportOUTDescr = transportOUTData.Description;

                            }
                        }
                    }
                }



                var newRecord = new MonthStatusRoomOwnerResponse
                {
                    Id = ownerEmployee.Id,
                    Lastname = ownerEmployee.Lastname,
                    Firstname = ownerEmployee.Firstname,
                    Gender = ownerEmployee.Gender,
                    HotelCheck = ownerEmployee.HotelCheck,
                    PositionName = ownerEmployee.PositionName,
                    EmployerName = ownerEmployee?.EmployerName,
                    DepartmentName = ownerEmployee?.DepartmentName,
                    DepartmentId = ownerEmployee?.DepartmemntId,
                    SAPID = ownerEmployee?.SAPID,
                    PeopleTypeName = ownerEmployee?.PeopleTypeCode,
                    futureTransportDate = ownerTransportINDate,
                    futureTransportScheduleDescription = ownerTransportINDescr,
                    futureTransportOUTDate = ownerTransportOUTDate,
                    futureTransportOUTScheduleDescription = ownerTransportOUTDescr,


                };

                roomOwnerList.Add(newRecord);
            }


            return roomOwnerList;
        }


        #endregion

        #region SearchRoom



        public async Task<SearchRoomResponse> SearchRoom(SearchRoomRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;

            IQueryable<Room> roomFilter = Context.Room;
            if (request.model.CampId.HasValue)
            {
                roomFilter = roomFilter.Where(x => x.CampId == request.model.CampId);
            }
            if (request.model.RoomTypeId.HasValue)
            {
                roomFilter = roomFilter.Where(e => e.RoomTypeId == request.model.RoomTypeId);
            }
            if (request.model.Private.HasValue) {
                roomFilter = roomFilter.Where(e => e.Private == request.model.Private);
            }
            if (!string.IsNullOrWhiteSpace(request.model.RoomNumber))
            {
             //   roomFilter = roomFilter.Where(x => x.Number.ToLower().Contains(request.model.RoomNumber.ToLower()));
                roomFilter= roomFilter.Where(x => x.Number.StartsWith(request.model.RoomNumber));
            }


            IQueryable<Room> vRoom = Context.Room.Where(c => c.VirtualRoom == 1);

            // Combine the filtered rooms with virtual rooms
            IQueryable<Room> combinedRooms = roomFilter.Concat(vRoom).Distinct();


        var result = from room in combinedRooms
                     join roomtype in Context.RoomType on room.RoomTypeId equals roomtype.Id into roomTypeData
                     from roomtype in roomTypeData.DefaultIfEmpty()
                     join camp in Context.Camp on room.CampId equals camp.Id into campData
                     from camp in campData.DefaultIfEmpty()
                     select new RoomSearchResult
                     {
                         Id = room.Id,
                         Number = room.Number,
                         BedCount = room.BedCount,
                         CampName = camp.Description,
                         RoomTypeName = roomtype.Description,
                         Private = room.Private,
                         CampId = room.CampId,
                         RoomTypeId = room.RoomTypeId,
                         VirtualRoom = room.VirtualRoom
                     };


            var retData =await result.ToListAsync();

            var returnData = new SearchRoomResponse
            {
                data = retData
                     .Skip(pageIndex * pageSize)
                     .Take(pageSize)
                     .ToList<RoomSearchResult>(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = retData.Count()
            };

            return returnData;
        }


        #endregion

        #region UpdateRoomBedCount

        public async Task UpdateRoomBedCount(int roomId)
        {
            var currentRoom =await Context.Room.FirstOrDefaultAsync();
            if (currentRoom != null)
            {
                var bedCount = await Context.Bed.Where(x => x.RoomId == roomId).CountAsync();
                currentRoom.BedCount = bedCount;
                currentRoom.DateUpdated = DateTime.Now;
                currentRoom.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                Context.Room.Update(currentRoom);
            }

        }


        #endregion


        public async Task<DateProfileRoomExportResponse> GetRoomDateProfileExport(DateProfileRoomExportRequest request, CancellationToken cancellationToken)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var currentRoom =await Context.Room.AsNoTracking().Where(x => x.Id == request.RoomId).FirstOrDefaultAsync();
            if (currentRoom != null)
            {
                var filteredEmployees =await (from es in Context.EmployeeStatus.AsNoTracking()
                                         where es.RoomId == request.RoomId && es.EventDate.Value.Date == request.CurrentDate.Date
                                         join employee in Context.Employee.AsNoTracking() on es.EmployeeId equals employee.Id
                                         join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into DepartmentData
                                         from department in DepartmentData.DefaultIfEmpty()
                                         join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into EmployerData
                                         from employer in EmployerData.DefaultIfEmpty()
                                         join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into PeopletypeData
                                         from peopletype in PeopletypeData.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = employee.Id,
                                             FirstName = employee.Firstname,
                                             LastName = employee.Lastname,
                                             SAPID = employee.SAPID,
                                             EmployeeRoomId = employee.RoomId,
                                             Gender = employee.Gender,
                                             DepartmentName = department != null ? department.Name : null,
                                             EmployerName = employer != null ? employer.Description : null,
                                             PeopleTypeCode = peopletype != null ? peopletype.Code : null,
                                             PeopleTypeId = peopletype != null ? peopletype.Id : (int?)null,
                                             Mobile = employee.PersonalMobile
                                         }).ToListAsync();



                List<DateProfileRoomExportResponseDateEmployee> employeeDetails = new List<DateProfileRoomExportResponseDateEmployee>();

            //    var allEmployees =await filteredEmployees.ToListAsync();

                foreach (var emp in filteredEmployees)
                {
                    DateTime? latestDate = null;
                    DateTime? earlistDate = null;
                    var latestStatus = await Context.EmployeeStatus.AsNoTracking()
                                         .Where(es => es.EmployeeId == emp.Id && es.RoomId != request.RoomId)
                                         .OrderByDescending(es => es.EventDate)
                                         .FirstOrDefaultAsync();
                    if (latestStatus != null)
                    {
                        latestDate = latestStatus.EventDate;
                    }
                    else {
                        var latestStatus2 = await Context.EmployeeStatus.AsNoTracking()
                        .Where(es => es.EmployeeId == emp.Id && es.RoomId == request.RoomId)
                        .OrderByDescending(es => es.EventDate)
                        .FirstOrDefaultAsync();
                        latestDate = latestStatus2.EventDate;
                    }

                    var earliestStatus = await Context.EmployeeStatus.AsNoTracking()
                                          .Where(es => es.EmployeeId == emp.Id && es.RoomId == request.RoomId)
                                          .OrderBy(es => es.EventDate)
                                          .FirstOrDefaultAsync();

                    employeeDetails.Add(new DateProfileRoomExportResponseDateEmployee
                    {
                        EmployeeId = emp.Id,
                        FullName = $"{emp.FirstName} {emp.LastName}",
                        RoomOwner = request.RoomId == emp.EmployeeRoomId ? "Yes" : "No",
                        Gender = emp.Gender == 1 ? "Male" : "Female",
                        SAPID = emp.SAPID,
                        DepartmentName = emp.DepartmentName,
                        EmployerName = emp.EmployerName,
                        PeopleTypeCode = emp.PeopleTypeCode,
                        StartDate = earliestStatus != null ? earliestStatus.EventDate?.Date.ToString("yyyy-MM-dd") : null,
                        EndDate = latestStatus != null ? latestStatus?.EventDate?.Date.ToString("yyyy-MM-dd") : null,

                        PeopleTypeId = emp.PeopleTypeId,
                        Mobile = emp.Mobile
                    });
                }


                if (request.PeopleTypeIds.Count > 0)
                {

                    var data = employeeDetails.Where(x =>  x.PeopleTypeId.HasValue && request.PeopleTypeIds.Contains(x.PeopleTypeId.Value)).ToList<dynamic>();
                    if (data.Count > 0)
                    {
                        return new DateProfileRoomExportResponse
                        {
                            ExcelFile = ExcelExport($"{currentRoom.Number}-{request.CurrentDate.Date.ToString("yyyy-MM-dd")}", data)
                        };
                    }
                    else
                    {
                        throw new BadRequestException("Room Employee not found");
                    }

                }
                else
                {
                    if (employeeDetails.Count > 0)
                    {
                        return new DateProfileRoomExportResponse
                        {
                            ExcelFile = ExcelExport($"{currentRoom.Number}-{request.CurrentDate.Date.ToString("yyyy-MM-dd")}", employeeDetails.ToList<dynamic>())
                        };
                    }
                    else {
                        throw new BadRequestException($"Room {currentRoom.Number} is currently unoccupied on {request.CurrentDate.ToString("yyy-MM-dd")}");
                    }
                    
                }

            }
            else {
                throw new BadRequestException("Room  not found");
            }

        }



        private List<IDictionary<string, object>> ConvertToDictionaryList(List<dynamic> dynamicList)
        {
            var dictionaryList = new List<IDictionary<string, object>>();

            foreach (var item in dynamicList)
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var property in item.GetType().GetProperties())
                {
                    dictionary[property.Name] = property.GetValue(item, null);
                }
                dictionaryList.Add(dictionary);
            }

            return dictionaryList;
        }






        private byte[] ExcelExport(string sheetName, List<dynamic> objectData)
        {
            using (var package = new ExcelPackage())
            {

                var data = ConvertToDictionaryList(objectData);
                var headerProps = ((IDictionary<string, object>)data[0]).Keys;
                var auditParams = new List<ExcelAuditParam>();
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Room Profile Executed date : ", FieldValueCaption = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") });
                auditParams.Add(new ExcelAuditParam { FieldName = "#MetaData", FieldCaption = "Result Count : ", FieldValueCaption = data.Count.ToString("N") });

                var worksheet = package.Workbook.Worksheets.Add(sheetName);

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
                List<string> mainColumns = new List<string>();
                mainColumns.Add("EmployeeId");
                mainColumns.Add("FullName");
                mainColumns.Add("Gender");
                mainColumns.Add("SAPID`");
                mainColumns.Add("DepartmentName");
                mainColumns.Add("EmployerName");
                mainColumns.Add("PeopleTypeCode");
                mainColumns.Add("StartDate");
                mainColumns.Add("EndDate");
                mainColumns.Add("Reason");
                mainColumns.Add("Mobile");
                
                foreach (var header in headerProps)
                {
                    worksheet.Cells[row, column].Value = AddSpacesToSentence(header, true);
                    var headerCells = worksheet.Cells[row, column];
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Font.Size = 13;
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    if (mainColumns.IndexOf(header) > -1)
                    {
                        headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e37222"));

                        headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFF"));
                    }
                    else
                    {
                        headerCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ACBDCB"));
                        headerCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#201b2e"));

                    }


                    headerCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    headerCells.AutoFilter = true;
                    column++;
                }

                // To set AutoFilter on the entire column range
                worksheet.Cells[row, 1, row, headerProps.Count].AutoFilter = true;
                row++;

                // Loop through each dynamic object and fill in the cells
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


        public async Task DeActiveRoomOwnersRemove(int RoomId, CancellationToken cancellationToken)
        {
          var orwnerEmployees = await Context.Employee.Where(x => x.RoomId == RoomId).ToListAsync();
            foreach (var item in orwnerEmployees)
            {
                item.RoomId = null;
                item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                item.DateUpdated = DateTime.Now;
            }
        }




    }



}
