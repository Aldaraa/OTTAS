
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using OfficeOpenXml.Drawing.Chart;
using System.Drawing;
using System.Formats.Asn1;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.EmployeeFeature.CreateEmployee;
using tas.Application.Features.EmployeeFeature.CreateEmployeeRequest;
using tas.Application.Features.EmployeeFeature.DeActiveEmployee;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.EmployeeFeature.GetEmployeeAccountHistory;
using tas.Application.Features.EmployeeFeature.GetProfileTransport;
using tas.Application.Features.EmployeeFeature.RemovePassportImageEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Application.Features.EmployeeFeature.SearchEmployeeAccommodation;
using tas.Application.Features.EmployeeFeature.SearchShortEmployee;
using tas.Application.Features.EmployeeFeature.StatusEmployee;
using tas.Application.Features.EmployeeFeature.UpdateEmployee;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace tas.Persistence.Repositories
{

    public partial class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly IConfiguration _configuration;

        private readonly HTTPUserRepository _hTTPUserRepository;

        private readonly BulkImportExcelService _bulkImportExcelService;
        private readonly SignalrHub _signalrhub;

        private readonly IMemoryCache _memoryCache;
        public EmployeeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, BulkImportExcelService bulkImportExcelService, IMemoryCache memoryCache, SignalrHub signalrHub) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
            _bulkImportExcelService = bulkImportExcelService;
            _memoryCache = memoryCache;
            _signalrhub = signalrHub;

        }

        #region AccountHistory
        public async Task<List<GetEmployeeAccountHistoryResponse>> GetAccountHistory(GetEmployeeAccountHistoryRequest request, CancellationToken cancellationToken)
        {
            var employeeHistory = await (from history in Context.EmployeeHistory.AsNoTracking().Where(e => e.EmployeeId == request.Id)
                                         join DemobType in Context.RequestDeMobilisationType on
                                         history.TerminationTypeId equals DemobType.Id into demobData
                                         from Demobtype in demobData.DefaultIfEmpty()
                                         select new GetEmployeeAccountHistoryResponse
                                         {
                                             Id = history.Id,
                                             Action = history.Action,
                                             Comment = history.Comment,
                                             EventDate = history.EventDate,
                                             TerminationTypeName = $"{Demobtype.Code} {Demobtype.Description}",
                                             EmployeeId = history.EmployeeId
                                              
                                         }).AsNoTracking().Where(x => x.EmployeeId == request.Id).ToListAsync();

            return employeeHistory;
        }


        #endregion


        #region EmployeeProfile


        public async Task<GetEmployeeResponse?> GetProfileAdmin(int Id, CancellationToken cancellationToken)
        {

            var userId = _hTTPUserRepository.LogCurrentUser()?.Id;

            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            var employeeStatuses = await GetEmployeeStatusDates(Id, DateTime.Now);


            watch.Stop();

            Console.WriteLine($"**************************************----------------------********************************Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();

            //var employeeTransports =await EmployeeTransports(Id, DateTime.Now);


            watch.Stop();

            Console.WriteLine($"**********************************************************************Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();

            //  var employeeTransport =await Context.Transport.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == Id);

            var activeGroups = await Context.GroupMaster.AsNoTracking()
                .Where(x => x.Active == 1 && x.ShowOnProfile == 1)
                .Select(x => x.Id)
                .ToListAsync();


            var employeeGroupData = await Context.GroupMembers.AsNoTracking()
                .Where(x => x.EmployeeId == Id && (x.GroupMasterId == null || activeGroups.Contains(x.GroupMasterId.Value)))
                .Select(x => new EmployeeInfoGroup
                {
                    Id = x.Id,
                    GroupDetailId = x.GroupDetailId,
                    GroupMasterId = x.GroupMasterId
                })
                .ToListAsync();

            DateTime today = DateTime.Today;
            var employeeNextTransport = await Context.Transport.AsNoTracking()
                    .Where(t => t.EventDate.HasValue && t.EventDate.Value.Date >= today && t.EmployeeId == Id)
                    .OrderBy(t => t.EventDate)
                    .FirstOrDefaultAsync();

            var returndata = await (from employee in Context.Employee.AsNoTracking()
                                    join state in Context.State.AsNoTracking() on employee.StateId equals state.Id into stateData
                                    from state in stateData.DefaultIfEmpty()
                                    join shift in Context.Shift.AsNoTracking() on employee.Shiftid equals shift.Id into shiftData
                                    from shift in shiftData.DefaultIfEmpty()
                                    join costcode in Context.CostCodes.AsNoTracking() on employee.CostCodeId equals costcode.Id into costcodeData
                                    from costcode in costcodeData.DefaultIfEmpty()
                                    join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                    from department in departmentData.DefaultIfEmpty()
                                    join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                    from position in positionData.DefaultIfEmpty()
                                    join roster in Context.Roster.AsNoTracking() on employee.RosterId equals roster.Id into rosterData
                                    from roster in rosterData.DefaultIfEmpty()
                                    join location in Context.Location.AsNoTracking() on employee.LocationId equals location.Id into locationData
                                    from location in locationData.DefaultIfEmpty()
                                    join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                    from peopletype in peopletypeData.DefaultIfEmpty()
                                    join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into roomData
                                    from room in roomData.DefaultIfEmpty()
                                    join flightgroupmaster in Context.FlightGroupMaster.AsNoTracking() on employee.FlightGroupMasterId equals flightgroupmaster.Id into flightgroupmasterData
                                    from flightgroupmaster in flightgroupmasterData.DefaultIfEmpty()
                                    join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                    from employer in employerData.DefaultIfEmpty()
                                    join nationality in Context.Nationality.AsNoTracking() on employee.NationalityId equals nationality.Id into nationalityData
                                    from nationality in nationalityData.DefaultIfEmpty()
                                    join sitecontactempployee in Context.Employee.AsNoTracking() on employee.SiteContactEmployeeId equals sitecontactempployee.Id into employeeData
                                    from sitecontactempdeployee in employeeData.DefaultIfEmpty()
                                    join camp in Context.Camp.AsNoTracking() on room.CampId equals camp.Id into employeeCampData
                                    from camp in employeeCampData.DefaultIfEmpty()
                                    join roomtype in Context.RoomType.AsNoTracking() on room.RoomTypeId equals roomtype.Id into employeeRoomTypeData
                                    from roomtype in employeeRoomTypeData.DefaultIfEmpty()


                                    select new GetEmployeeResponse
                                    {
                                        Id = employee.Id,
                                        Lastname = employee.Lastname,
                                        Firstname = employee.Firstname,
                                        MLastname = employee.MLastname,
                                        MFirstname = employee.MFirstname,
                                        Mobile = employee.Mobile,
                                        Email = employee.Email,
                                        EmployerName = employer.Description,
                                        EmployerCurrentStatus = 0,
                                        StateId = employee.StateId,
                                        StateName = state.Description,
                                        ShiftId = employee.Shiftid,
                                        ShiftName = shift.Description,
                                        CostCodeName = $"{costcode.Number} - {costcode.Description}",
                                        CostCodeNumber = costcode.Number,
                                        CostCodeDescription = costcode.Description,
                                        CostCodeCurrentStatus = 0,
                                        DepartmentId = department.Id,
                                        DepartmentName = department.Name,
                                        DepartmentCurrentStatus = 0,
                                        PositionName = position.Description,
                                        PositionCurrentStatus = 0,
                                        RosterName = roster.Name,
                                        LocationName = location.Description,
                                        PeopleTypeName = peopletype.Code,
                                        RoomNumber = room.Number,
                                        FlightGroupMasterName = flightgroupmaster.Description,
                                        SAPID = employee.SAPID,
                                        CostCodeId = employee.CostCodeId,
                                        FlightGroupMasterId = employee.FlightGroupMasterId,
                                        LocationId = employee.LocationId,
                                        PeopleTypeId = employee.PeopleTypeId,
                                        RosterId = employee.RosterId,
                                        NRN = employee.NRN,
                                        Active = employee.Active == null ? 0 : employee.Active,
                                        ADAccount = employee.ADAccount,
                                        CommenceDate = employee.CommenceDate,
                                        ContractNumber = employee.ContractNumber,
                                        Dob = employee.Dob,
                                        Gender = employee.Gender,
                                        HotelCheck = employee.HotelCheck,
                                        NationalityId = employee.NationalityId,
                                        SiteContactEmployeeId = employee.SiteContactEmployeeId,
                                        DateCreated = employee.DateCreated,
                                        LoginEnabled = employee.LoginEnabled,
                                        CompletionDate = employee.CompletionDate,
                                        NationalityName = nationality.Description,
                                        DateUpdated = employee.DateUpdated,
                                        PassportExpiry = employee.PassportExpiry,
                                        PassportImage = employee.PassportImage,
                                        PassportNumber = employee.PassportNumber,
                                        PositionId = employee.PositionId,
                                        PersonalMobile = employee.PersonalMobile,
                                        EmergencyContactMobile = employee.EmergencyContactMobile,
                                        EmergencyContactName = employee.EmergencyContactName,
                                        PassportName = employee.PassportName,
                                        PickUpAddress = employee.PickUpAddress,
                                        SiteContactEmployeeLastname = sitecontactempdeployee.Lastname,
                                        SiteContactEmployeeFirstname = sitecontactempdeployee.Firstname,
                                        SiteContactEmployeeMobile = sitecontactempdeployee.Mobile,
                                        EmployerId = employee.EmployerId,
                                        RoomId = employee.RoomId,
                                        FrequentFlyer = employee.FrequentFlyer,
                                        NextRosterDate = employeeNextTransport == null ? null : employeeNextTransport.EventDate,
                                        employeeStatusDates = employeeStatuses,
                                        //    employeeTransports = employeeTransports,
                                        GroupData = employeeGroupData,
                                        CreateRequest = employee.CreateRequest,
                                        RoomTypeId = employee.RoomTypeId,
                                        CampId = employee.CampId,
                                        CampRoomType = $"{camp.Description}-{roomtype.Description}",
                                        OnSiteStatus = 0,
                                        Hometown = employee.Hometown,
                                        RosterExecutedDate = employee.RosterExecutedDate,
                                        RosterExecuteLastDate = employee.RosterExecuteLastDate,
                                        RosterExecuteMonthDuration = employee.RosterExecuteMonthDuration
                                    }).Where(x => x.Id == Id).FirstOrDefaultAsync(cancellationToken);

            // var currentEmployee =await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            watch.Stop();

            Console.WriteLine($"----------------------------------------------------------Execution total Time: {watch.ElapsedMilliseconds} ms");


            if (returndata == null)
            {
                throw new NotFoundNoDataException("");

            }
            else
            {
                returndata = await CurrrentOnsiteData(returndata);


                if (_hTTPUserRepository.LogCurrentUser()?.Role == "AccomAdmin" || _hTTPUserRepository.LogCurrentUser()?.Role == "SystemAdmin" || _hTTPUserRepository.LogCurrentUser()?.Role == "DataApproval" || _hTTPUserRepository.LogCurrentUser()?.Role == "TravelAdmin" || _hTTPUserRepository.LogCurrentUser()?.Role == "DepartmentAdmin" || _hTTPUserRepository.LogCurrentUser()?.Role == "DepartmentManager")
                {
                    return returndata;
                }
                else
                {
                    if (_hTTPUserRepository.LogCurrentUser()?.Id == Id)
                    {
                        return returndata;
                    }
                    else
                    {

                        if (_hTTPUserRepository.LogCurrentUser()?.Role == "DepartmentAdmin" || _hTTPUserRepository.LogCurrentUser()?.Role == "Supervisor")
                        {

                            if (returndata.Active == 0)
                            {
                                return returndata;
                            }
                            else {
                                var roleEmployeeIds = await GetRoleEmployeeIds();

                                if (roleEmployeeIds.IndexOf(returndata.Id) > -1)
                                {
                                    return returndata;
                                }
                                else
                                {
                                    throw new ForBiddenException(" You do not have the required permissions to access this employee's information.");
                                }
                            }
                            
                        }
                        if (_hTTPUserRepository.LogCurrentUser()?.Role == "DepartmentManager")
                        {
                            if (returndata.Active == 0)
                            {
                                return returndata;
                            }
                            else {




                                var roleEmployeeIds = await GetRoleEmployeeIds();

                                if (roleEmployeeIds.IndexOf(returndata.Id) > -1)
                                {
                                    return returndata;
                                }
                                else
                                {
                                    throw new ForBiddenException(" You do not have the required permissions to access this employee's information.");
                                }
                            }

                        }
                        else
                        {
                            throw new ForBiddenException(" You do not have the required permissions to access this employee's information.");
                        }


                    }
                }
            }
        }
        private async Task<GetEmployeeResponse> CurrrentOnsiteData(GetEmployeeResponse currentData)
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            var todayOnsiteData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == currentData.Id && x.EventDate.Value.Date == DateTime.Today.Date /*&& x.RoomId != null*/).FirstOrDefaultAsync();

            if (todayOnsiteData != null)
            {
                if (todayOnsiteData.RoomId.HasValue)
                { 
                    currentData.OnSiteStatus = 1;
                }
                
                if (todayOnsiteData.DepId != null)
                {
                    var todayDepartment = await Context.Department.AsNoTracking().Where(x => x.Id == todayOnsiteData.DepId).FirstOrDefaultAsync();
           
                    currentData.DepartmentCurrentStatus = todayDepartment?.Name == currentData.DepartmentName ? 0 : 1;
                    currentData.DepartmentName = todayDepartment == null ? currentData.DepartmentName : todayDepartment.Name;
                    currentData.DepartmentId = todayDepartment == null ? currentData.DepartmentId : todayDepartment.Id;
                }
                if (todayOnsiteData.PositionId != null)
                {
                    var todayPosition = await Context.Position.AsNoTracking().Where(x => x.Id == todayOnsiteData.PositionId).FirstOrDefaultAsync();
                    currentData.PositionCurrentStatus = todayPosition?.Description == currentData.PositionName ? 0 : 1;
                    currentData.PositionName = todayPosition == null ? currentData.PositionName : todayPosition.Description;
                    currentData.PositionId = todayPosition == null ? currentData.PositionId : todayPosition.Id;

                }
                if (todayOnsiteData.EmployerId != null)
                {
                    var todayEmployer = await Context.Employer.AsNoTracking().Where(x => x.Id == todayOnsiteData.EmployerId).FirstOrDefaultAsync();
                    currentData.EmployerCurrentStatus = todayEmployer?.Description == currentData.EmployerName ? 0 : 1;
                    currentData.EmployerName = todayEmployer == null ? currentData.EmployerName : todayEmployer.Description;
                    currentData.EmployerId = todayEmployer == null ? currentData.EmployerId : todayEmployer.Id;


                }
                if (todayOnsiteData.CostCodeId != null)
                {
                    var todayCostcode = await Context.CostCodes.AsNoTracking().Where(x => x.Id == todayOnsiteData.CostCodeId).FirstOrDefaultAsync();
                    currentData.CostCodeCurrentStatus = todayCostcode?.Id == currentData.CostCodeId ? 0 : 1;
                    currentData.CostCodeName = todayCostcode == null ? currentData.CostCodeName : $"{todayCostcode.Number} - {todayCostcode.Description}";
                    currentData.CostCodeId = todayCostcode == null ? currentData.CostCodeId : todayCostcode.Id;
                    currentData.CostCodeDescription =  todayCostcode == null ? currentData.CostCodeDescription : todayCostcode.Description;
                    currentData.CostCodeNumber = todayCostcode == null ? currentData.CostCodeNumber : todayCostcode.Number;


                }

            }


            var todayOnsiteRoom = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == currentData.Id && x.EventDate.Value.Date == DateTime.Today.Date && x.RoomId != null).FirstOrDefaultAsync();
            if (todayOnsiteRoom != null)
            {
                var roomInfo = await Context.Room.AsNoTracking().Where(x => x.Id == todayOnsiteRoom.RoomId).FirstOrDefaultAsync();
                if (roomInfo != null)
                {
                    currentData.CurrentRoomNumber = roomInfo.Number;
                    currentData.CurrentRoomId = roomInfo.Id;

                }
            }
            else
            {
                var yesterOnsiteRoom = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == currentData.Id && x.EventDate.Value.Date == DateTime.Today.Date.AddDays(-1) && x.RoomId != null).FirstOrDefaultAsync();
                if (yesterOnsiteRoom != null)
                {
                    var roomInfo = await Context.Room.AsNoTracking().Where(x => x.Id == yesterOnsiteRoom.RoomId).FirstOrDefaultAsync();
                    if (roomInfo != null)
                    {
                        currentData.CurrentRoomNumber = roomInfo.Number;
                        currentData.CurrentRoomId = roomInfo.Id;
                    }
                }
            }

            Console.WriteLine($"**************************************----------ONSITE DATA----------********************************Execution Time: {watch.ElapsedMilliseconds} ms");

            return currentData;
        }



        #endregion






        private async Task<List<EmployeeStatusDate>> GetEmployeeStatusDates(int EmployeeId, DateTime currentDate)
        {




            var sdate = currentDate.AddDays(-7); // Subtract 4 weeks
            sdate = sdate.AddDays(-(int)sdate.DayOfWeek); // Move to Monday
            var edate = currentDate.AddMonths(1).AddDays(7);
            //  edate = edate.AddDays(6 - (int)edate.DayOfWeek);

            DateTime startOfMonth = sdate;//sdate.AddDays(1);
            DateTime endOfMonth = edate;//edate.AddDays(1);
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();



            var statusDate = await Context.EmployeeStatus.AsNoTracking().
                                         Where(es => es.EmployeeId == EmployeeId
                            && es.EventDate >= startOfMonth
                            && es.EventDate <= endOfMonth).Select(x => x.EventDate.Value.Date).ToListAsync();



            var transportDates = await Context.Transport.AsNoTracking().
                  Where(es => es.EmployeeId == EmployeeId
                            && es.EventDate >= startOfMonth
                            && es.EventDate <= endOfMonth).Select(x => x.EventDate.Value.Date).ToListAsync();


            var dateSet = new HashSet<DateTime>(statusDate);
            dateSet.UnionWith(transportDates);
            var mergedDates = dateSet.ToList();

            var returnData = new List<EmployeeStatusDate>();


            foreach (var item in dateSet)
            {

                var dateStatusInfo = await (from es in Context.EmployeeStatus.AsNoTracking()
                                            join s in Context.Shift on es.ShiftId equals s.Id into shiftGroup

                                            from shift in shiftGroup.DefaultIfEmpty()
                                            join color in Context.Color on shift.ColorId equals color.Id into ColorData
                                            from color in ColorData.DefaultIfEmpty()
                                            where es.EmployeeId == EmployeeId && es.EventDate == item
                                            select new
                                            {
                                                es.EventDate,
                                                ShiftCode = shift.Code,
                                                ColorCode = color.Code
                                            }).FirstOrDefaultAsync();




                var tCode = new List<string>();
                var directions = new List<string>();
                var Modes = new List<string>();

                var transportInfo = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EmployeeId == EmployeeId && x.EventDate.Value.Date == item)
                                           join transportSchedule in Context.TransportSchedule.AsNoTracking() on transport.ScheduleId equals transportSchedule.Id

                                           select new
                                           {
                                               employeeId = transport.EmployeeId,
                                               eventDate = transport.EventDate,
                                               direction = transport.Direction,
                                               Code = transportSchedule.Description,
                                               eventDateTime = transport.EventDateTime,
                                               activeTransportId = transportSchedule.ActiveTransportId
                                           }).OrderBy(x => x.eventDateTime).ToListAsync();




                foreach (var item2 in transportInfo)
                {
                    if (item2?.activeTransportId != null)
                    {
                        var transportModeData = await (from
                                        activeTransport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == item2.activeTransportId)
                                                       join transportMode in Context.TransportMode.AsNoTracking() on activeTransport.TransportModeId equals transportMode.Id
                                                       select new
                                                       {
                                                           Mode = transportMode.Code

                                                       }).FirstOrDefaultAsync();
                        if (transportModeData != null)
                        {
                            Modes.Add(transportModeData.Mode);
                        }


                    }



                    if (!string.IsNullOrWhiteSpace(item2.direction) && !directions.Contains(item2.direction))
                    {
                        directions.Add(item2.direction);
                    }
                    if (!string.IsNullOrWhiteSpace(item2.Code) && !tCode.Contains(item2.Code))
                    {
                        tCode.Add(item2.Code);
                    }
                }

                string transportCode = string.Join("/", tCode);

                string Direction = string.Join("/", directions);

                string transportModes = string.Join("/", Modes);

                var newRecord = new EmployeeStatusDate
                {
                    Color = dateStatusInfo?.ColorCode,
                    ShiftCode = dateStatusInfo?.ShiftCode,
                    EventDate = item,
                    Schedule = tCode.Count > 0 ? transportCode : null,
                    Direction = directions.Count > 0 ? Direction : null,
                    TransportMode = Modes.Count > 0 ? transportModes : null,
                };

                returnData.Add(newRecord);

            }


            watch.Stop();


            return returnData;
        }


        private async Task<List<EmployeeStatusDate>> GetEmployeeStatusDates(int EmployeeId, DateTime startOfMonth, DateTime endOfMonth)
        {
            var query = from es in Context.EmployeeStatus.AsNoTracking()
                        join s in Context.Shift on es.ShiftId equals s.Id into shiftGroup
                        from shift in shiftGroup.DefaultIfEmpty()
                        where es.EmployeeId == EmployeeId
                            && es.EventDate >= startOfMonth
                            && es.EventDate <= endOfMonth
                        select new EmployeeStatusDate
                        {
                            EventDate = es.EventDate.Value.Date,
                            ShiftCode = shift.Code,
                            Direction = Context.Transport.AsNoTracking().FirstOrDefault(x => x.EmployeeId == EmployeeId && x.EventDate == es.EventDate).Direction,
                            Color = Context.Color.AsNoTracking().FirstOrDefault(x => x.Id == shift.ColorId).Code
                            //Schedule = GetStatusDateSchedule(EmployeeId, es.EventDate)

                        };

            List<EmployeeStatusDate> dates = query.ToList();
            foreach (var item in dates)
            {
                if (item.Direction != null)
                {
                    item.Schedule = await GetStatusDateSchedule(EmployeeId, item.EventDate, item.Direction);
                }

            }


            return dates;

        }




        private async Task<string> GetStatusDateSchedule(int employeeId, DateTime? eventDate, string direction)
        {
            try
            {
                if (!eventDate.HasValue)
                {
                    return string.Empty;
                }

                var currentTransport = await Context.Transport.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Direction == direction
                    && x.EventDate == eventDate);

                var currentSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.ActiveTransportId == currentTransport.ActiveTransportId
                    && x.EventDate.Date == currentTransport.EventDate.Value.Date);

                var currentActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentTransport.ActiveTransportId);

                var currentCarrier = await Context.Carrier.FirstOrDefaultAsync(x => x.Id == currentActiveTransport.CarrierId);

                if (currentTransport != null && currentSchedule != null)
                {
                    return string.Format("{0} {1} {2}", currentSchedule.Description, currentCarrier.Description, currentActiveTransport.Seats);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }





        public async Task<StatusEmployeeResponse> GetStatusDates(StatusEmployeeRequest request, CancellationToken cancellationToken)
        {

            if (request.employeeId.HasValue)
            {
                var employeeStatuses = await GetEmployeeStatusDates(request.employeeId.Value, request.currentDate);
                var employeeTransports = await EmployeeTransports(request.employeeId.Value, request.currentDate);
                var returnData = new StatusEmployeeResponse
                {
                    employeeStatusDates = employeeStatuses,
                    employeeTransports = employeeTransports

                };
                return returnData;
            }
            else
            {


                var employeeStatuses = await GetEmployeeStatusDates(_hTTPUserRepository.LogCurrentUser().Id, request.currentDate);
                var employeeTransports = await EmployeeTransports(_hTTPUserRepository.LogCurrentUser().Id, request.currentDate);
                var returnData = new StatusEmployeeResponse
                {
                    employeeStatusDates = employeeStatuses,
                    employeeTransports = employeeTransports

                };
                return returnData;
            }

        }

        private async Task<List<EmployeeTransport>> EmployeeTransports(int employeeId, DateTime currentDate)
        {

            var sdate = currentDate.AddDays(-28); // Subtract 4 weeks
            sdate = sdate.AddDays(-(int)sdate.DayOfWeek); // Move to Monday
            var edate = currentDate.AddDays(49);
            edate = edate.AddDays(6 - (int)edate.DayOfWeek);

            DateTime startOfMonth = sdate.AddDays(1);
            DateTime endOfMonth = edate.AddDays(1);
            var watch = new System.Diagnostics.Stopwatch();



            var query = from t in Context.Transport.AsNoTracking()
                        join activeTransport in Context.ActiveTransport.AsNoTracking() on t.ActiveTransportId equals activeTransport.Id into activeTransportGroup
                        from activeTransport in activeTransportGroup.DefaultIfEmpty()
                        join ts in Context.TransportSchedule.AsNoTracking() on t.ActiveTransportId equals ts.ActiveTransportId into activeTransportSchedule
                        from ts in activeTransportSchedule.DefaultIfEmpty()
                        join tmode in Context.TransportMode on activeTransport.TransportModeId equals tmode.Id into activeTransportMode
                        from tmode in activeTransportMode.DefaultIfEmpty()


                        where t.EmployeeId == employeeId && t.EventDate >= startOfMonth && t.ScheduleId == ts.Id
                        && t.EventDate <= endOfMonth
                        && ts.EventDate.Date == t.EventDate
                        select new EmployeeTransport
                        {
                            Description = ts.Description,
                            Direction = t.Direction,
                            EventDate = t.EventDate,
                            TransportMode = tmode.Code,
                            TransportCode = activeTransport.Code,
                            EventDateTime = t.EventDateTime,


                        };



            return await query.OrderBy(x => x.EventDate).AsNoTracking().ToListAsync();
        }


        private async Task<List<EmployeeTransport>> EmployeeTransports(int employeeId, DateTime startOfMonth, DateTime endOfMonth)
        {
            var query = from t in Context.Transport
                        join at in Context.ActiveTransport.AsNoTracking() on t.ActiveTransportId equals at.Id into activeTransportGroup
                        from activeTransport in activeTransportGroup.DefaultIfEmpty()
                        join ts in Context.TransportSchedule.AsNoTracking() on t.ActiveTransportId equals ts.ActiveTransportId into activeTransportSchedule
                        from ts in activeTransportSchedule.DefaultIfEmpty()
                        where t.EmployeeId == employeeId && t.EventDate >= startOfMonth
                        && t.EventDate <= endOfMonth
                        && ts.EventDate.Date == t.EventDate
                        select new EmployeeTransport
                        {
                            Description = ts.Description,
                            Direction = t.Direction,
                            EventDate = t.EventDate,
                            EventDateTime = t.EventDateTime,
                           

                        };

            return await query.OrderBy(x => x.EventDate).ToListAsync();
        }

        #region SearchEmployee



        public async Task<SearchEmployeeResponse> SearchAdmin(SearchEmployeeRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;

            bool? SAPSearchStatus = false;
            List<int> SearchSAPIds = new List<int>();

            List<int> NotFoundSAPIDs = new List<int>();




            IQueryable<Employee> empFilter = Context.Employee.AsNoTracking().Where(x => x.Active != 2);


            if (request.model.Active.HasValue)
            {
                if (request.model.Active.Value == 1)
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.Active == 1);
                }
                else
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.Active == 0);
                }

            }
            if (!string.IsNullOrWhiteSpace(request.model?.Firstname))
            {
                string trimmedFirstname = request.model.Firstname.Trim();
                empFilter = empFilter.AsNoTracking().Where(e => e.Firstname.Contains(trimmedFirstname));
            }

            if (!string.IsNullOrWhiteSpace(request.model?.Lastname))
            {
                string trimmedLastname = request.model.Lastname.Trim();
                empFilter = empFilter.AsNoTracking().Where(e =>  e.Lastname.Contains(trimmedLastname));
            }

            if (!string.IsNullOrWhiteSpace(request.model.Id))
            {

                string numericInput = Regex.Replace(request.model.Id, "[^0-9]+", ",");
                if (!string.IsNullOrWhiteSpace(numericInput))
                {
                    var Ids = numericInput.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();


                    if (Ids.Count > 0)
                    {
                        empFilter = empFilter.AsNoTracking().Where(x => Ids.Contains(x.Id));
                    }

                }

                //     empFilter = empFilter.AsNoTracking().Where(e => e.Id == request.model.Id);


            }
            if (request.model.CostCodeId.HasValue && request.model.CostCodeId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.CostCodeId == request.model.CostCodeId);
            }
            if (request.model.FlightGroupMasterId.HasValue && request.model.FlightGroupMasterId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.FlightGroupMasterId == request.model.FlightGroupMasterId);
            }
            if (request.model.LocationId.HasValue && request.model.LocationId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.LocationId == request.model.LocationId);
            }
            if (request.model.PeopleTypeId.HasValue && request.model.PeopleTypeId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.PeopleTypeId == request.model.PeopleTypeId);
            }
            if (request.model.RosterId.HasValue && request.model.RosterId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.RosterId == request.model.RosterId);
            }
            if (request.model.EmployerId.HasValue && request.model.EmployerId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.EmployerId == request.model.EmployerId);
            }

            if (request.model.Departmentid.HasValue && request.model.Departmentid.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.DepartmentId == request.model.Departmentid);
            }

            if (request.model.PositionId.HasValue && request.model.PositionId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.PositionId == request.model.PositionId);
            }

            if (request.model?.Firstname != null && !string.IsNullOrWhiteSpace(request.model.Firstname))
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.Firstname.StartsWith(request.model.Firstname.Trim()));
            }


            if (request.model?.Lastname != null && !string.IsNullOrWhiteSpace(request.model.Lastname))
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.Lastname.StartsWith(request.model.Lastname.Trim()));
            }

            if (request.model?.NRN != null && !string.IsNullOrWhiteSpace(request.model.NRN))
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.NRN.Contains(request.model.NRN.Trim()));
            }

            if (request.model?.Mobile != null && !string.IsNullOrWhiteSpace(request.model.Mobile))
            {
                //string trimmedMobile = request.model.Mobile.Trim();
                //empFilter = empFilter.Where(e => e.Mobile.Contains(trimmedMobile) || e.PersonalMobile.Contains(trimmedMobile));

                string trimmedMobile = request.model.Mobile.Trim();
                empFilter = empFilter.Where(e =>
                    (e.Mobile != null && e.Mobile.Contains(trimmedMobile)) ||
                    (e.PersonalMobile != null && e.PersonalMobile.Contains(trimmedMobile))
                );
            }
            if (request.model.HasRoom.HasValue)
            {
                if (request.model.HasRoom.Value == 1)
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.RoomId != null && e.Active == 1);
                }
                else
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.RoomId == null && e.Active == 1);
                }
            }


            if (!string.IsNullOrWhiteSpace(request.model.RoomNumber))
            {
                var RoomIds = await Context.Room.AsNoTracking()
                    .Where(x => x.Number.ToLower()
                    .Contains(request.model.RoomNumber.ToLower()))
                    .Select(x => x.Id).ToArrayAsync();
                if (RoomIds.Length > 0)
                {
                    empFilter = empFilter.Where(x => RoomIds.Contains(x.RoomId.Value));
                }
            }



            if (request.model.CampId.HasValue && request.model.CampId > 0)
            {

                int[] campRooms = Context.Room
                 .Where(r => r.CampId == request.model.CampId)
                 .Select(r => r.Id)
                 .ToArray();
                if (campRooms.Length > 0)
                {

                    empFilter = empFilter.AsNoTracking().Where(x => campRooms.Any(y => y == x.RoomId.Value));
                }

            }


            if (request.model.RoomTypeId.HasValue && request.model.RoomTypeId > 0)
            {

                int[] roomTypeRooms = Context.Room.AsNoTracking()
                 .Where(r => r.RoomTypeId == request.model.RoomTypeId)
                 .Select(r => r.Id)
                 .ToArray();
                if (roomTypeRooms.Length > 0)
                {

                    empFilter = empFilter.AsNoTracking().Where(x => roomTypeRooms.Any(y => y == x.RoomId.Value));
                }

            }

            if (!string.IsNullOrWhiteSpace(request.model.SAPID))
            {
                string numericInput = Regex.Replace(request.model.SAPID, "[^0-9]+", ",");
                if (!string.IsNullOrWhiteSpace(numericInput))
                {
                    var sapIds = numericInput.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();

                    if (sapIds != null)
                    {
                        if (sapIds.Count > 0)
                        {
                            SAPSearchStatus = true;
                            SearchSAPIds = sapIds;
                            empFilter = empFilter.AsNoTracking().Where(x => sapIds.Contains(x.SAPID.Value));
                        }
                    }


                }
            }


            if (request.model.FutureBooking.HasValue)
            {
                DateTime currentDate = DateTime.Today.Date;

                IQueryable<int> bookingEmployeeIds = Context.Transport
               .AsNoTracking()
               .Where(x => x.EventDate >= currentDate)
               .Select(x => x.EmployeeId.Value)
               .Distinct();





                if (request.model.FutureBooking.Value == 1)
                {
                    empFilter = empFilter
                    .Join(bookingEmployeeIds, e => e.Id, id => id, (e, id) => e)
                    .Where(e => e.Active == 1);
                }
                else
                {

                    Console.WriteLine(bookingEmployeeIds.Count().ToString(), "----------------------sdsdd----------");

                    empFilter = from e in empFilter
                                join b in bookingEmployeeIds on e.Id equals b into gj
                                from subb in gj.DefaultIfEmpty()
                                where subb == null && e.Active == 1
                                select e;
                }
            }




            empFilter = await RoleEmployeeEmfilter(empFilter);
            int totalEmployees = await empFilter.CountAsync();

            if (SAPSearchStatus.HasValue && SAPSearchStatus.Value)
            {
                if (SAPSearchStatus.Value)
                {
                    var existingSAPIDs = await empFilter.ToListAsync();
                    var existData = existingSAPIDs
                        .Where(x => x.SAPID.HasValue)
                        .Select(x => x.SAPID.Value)
                        .ToList();
                    if (SearchSAPIds != null)
                    {
                        if (SearchSAPIds != null && SearchSAPIds.Count > 0)
                        {

                            //    NotFoundSAPIDs = SearchSAPIds.Except(existData).ToList();
                            NotFoundSAPIDs = SearchSAPIds.Except(existData).ToList();
                        }
                    }

                    

                }
                 
            }


            var empfilterQuery = empFilter.OrderBy(e => e.Id) // Add an appropriate ordering here
                    .Skip((pageIndex) * pageSize)
                    .Take(pageSize);

            var result = await (from employee in Context.Employee.AsNoTracking()
                                where empfilterQuery.AsNoTracking().Contains(employee)
                                join state in Context.State on employee.StateId equals state.Id into stateData
                                from state in stateData.DefaultIfEmpty()
                                join shift in Context.Shift.AsNoTracking() on employee.Shiftid equals shift.Id into shiftData
                                from shift in shiftData.DefaultIfEmpty()
                                join costcode in Context.CostCodes.AsNoTracking() on employee.CostCodeId equals costcode.Id into costcodeData
                                from costcode in costcodeData.DefaultIfEmpty()
                                join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                from department in departmentData.DefaultIfEmpty()
                                join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                from position in positionData.DefaultIfEmpty()
                                join roster in Context.Roster.AsNoTracking() on employee.RosterId equals roster.Id into rosterData
                                from roster in rosterData.DefaultIfEmpty()
                                join location in Context.Location.AsNoTracking() on employee.LocationId equals location.Id into locationData
                                from location in locationData.DefaultIfEmpty()
                                join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                from peopletype in peopletypeData.DefaultIfEmpty()
                                join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into roomData
                                from room in roomData.DefaultIfEmpty()
                                join flightgroupmaster in Context.FlightGroupMaster.AsNoTracking() on employee.FlightGroupMasterId equals flightgroupmaster.Id into flightgroupmasterData
                                from flightgroupmaster in flightgroupmasterData.DefaultIfEmpty()
                                join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                from employer in employerData.DefaultIfEmpty()
                                select new EmployeeSearchResult
                                {
                                    Id = employee.Id,
                                    Lastname = employee.Lastname,
                                    Firstname = employee.Firstname,
                                    MLastname = employee.MLastname,
                                    MFirstname = employee.MFirstname,
                                    Mobile = employee.Mobile,
                                    Email = employee.Email,
                                    EmployerName = employer.Description,
                                    StateId = employee.StateId,
                                    StateName = state.Description,
                                    ShiftId = employee.Shiftid,
                                    ShiftName = shift.Description,
                                    CostCodeName = $"{costcode.Number}-{costcode.Description}",
                                    DepartmentName = department.Name,
                                    PositionName = position.Description,
                                    RosterName = roster.Name,
                                    LocationName = location.Description,
                                    PeopleTypeName = peopletype.Code,
                                    RoomNumber = room.Number,
                                    FlightGroupMasterName = flightgroupmaster.Description,
                                    SAPID = employee.SAPID,
                                    CostCodeId = employee.CostCodeId,
                                    FlightGroupMasterId = employee.FlightGroupMasterId,
                                    LocationId = employee.LocationId,
                                    PositionId = employee.PositionId,
                                    PeopleTypeId = employee.PeopleTypeId,
                                    RosterId = employee.RosterId,
                                    Departmentid = employee.DepartmentId,
                                    NRN = employee.NRN,
                                    RoomId = employee.RoomId,
                                    EmployerId = employee.EmployerId,
                                    Gender = employee.Gender,
                                    HasFutureRoomBooking = 0,
                                    HasFutureTransport = 0,
                                    RoomTypeId = null,
                                    Active = employee.Active
                                }).ToListAsync();


            var retData = result.ToList();


            var employeeIds = retData.Select(item => item.Id).ToList();



            DateTime todayDate = DateTime.Today.Date;


            var transportData = await Context.Transport.AsNoTracking()
            .Where(x => employeeIds.Contains((int)x.EmployeeId) && x.EventDate >= DateTime.Today.Date)
            .ToListAsync();

            var groupedTransportData = transportData
                .GroupBy(x => x.EmployeeId)
                .Select(group => new
                {
                    EmployeeId = group.Key ?? 0, // Default value for nullable EmployeeId
                    Count = group.Count()
                });


            var groupedBookingData = await Context.EmployeeStatus.AsNoTracking()
                        .Where(e => employeeIds.Contains((int)e.EmployeeId) && e.EventDate >= DateTime.Today.Date)
                        .GroupBy(e => e.EmployeeId)
                        .Select(group => new
                        {
                            EmployeeId = group.Key,
                            Count = group.Count()
                        })
                        .ToListAsync(cancellationToken);

            foreach (var item in retData)
            {
                int hasFutureTransportCount = groupedTransportData.Where(e => e.EmployeeId == item.Id).Count();
                int hasFutureRoomBookingCount = groupedBookingData.Where(e => e.EmployeeId == item.Id).Count();
                item.RoomTypeName = await Context.RoomType.AsNoTracking().Where(r => r.Id == item.RoomTypeId).Select(r => r.Description).FirstOrDefaultAsync();
                item.HasFutureRoomBooking = hasFutureRoomBookingCount > 0 ? 1 : 0;
                item.HasFutureTransport = hasFutureTransportCount > 0 ? 1 : 0;
            }


            //retData = _hTTPUserRepository.GetRoleEmpoyee(retData, "Id");


            var returnData = new SearchEmployeeResponse
            {
                data = retData.OrderBy(x => x.Id)
                     .ToList<EmployeeSearchResult>(),
                pageSize = pageSize,
                NotFoundSAPIDs = NotFoundSAPIDs,
                currentPage = pageIndex,
                totalcount = totalEmployees
            };

            return returnData;
        }


        private async Task<IQueryable<Employee>> RoleEmployeeEmfilter(IQueryable<Employee> employees)
        {
            var role = _hTTPUserRepository.LogCurrentUser()?.Role;
            var userId = _hTTPUserRepository.LogCurrentUser()?.Id;
            if (role == "DepartmentAdmin" || role == "DepartmentManager")
            {
                if (role == "DepartmentAdmin")
                {

                    var departmentIds = await Context.DepartmentAdmin.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();

                    var roleEmployerIds = await Context.EmployerAdmin.AsNoTracking().Where(x =>  x.EmployeeId == userId).Select(x => x.EmployerId).ToListAsync();


                    List<int> RoleDepartmenIds = new List<int>();
                    var deleageDepIds = await DelegateDepartmentIds(userId.Value);
                    if (deleageDepIds.Count > 0)
                    {
                        RoleDepartmenIds.AddRange(deleageDepIds);
                    }

                    var supervisordepartmentIds = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();

                    foreach (var item in supervisordepartmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds = await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }


                    foreach (var item in departmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds =await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }

                    if (RoleDepartmenIds.Count > 0 || roleEmployerIds.Count > 0)
                    {
                        employees = employees.Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value) || x.Id == userId || (x.Active == 0) || roleEmployerIds.Contains(x.EmployerId.Value));
                    }
                    else
                    {
                        employees = employees.Where(x => userId == x.Id || x.Active == 0);
                    }


                }

                if (role == "DepartmentManager")
                {


                    List<int> RoleDepartmenIds = new List<int>();
                    var departmentIds = await Context.DepartmentManager.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();


                    var deleageDepIds =  await DelegateDepartmentIds(userId.Value);
                    if (deleageDepIds.Count > 0) {
                        RoleDepartmenIds.AddRange(deleageDepIds);
                    }


                    var supervisordepartmentIds = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();

                    foreach (var item in supervisordepartmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds = await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }


                    foreach (var item in departmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds =await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }

                    if (RoleDepartmenIds.Count > 0)
                    {
                        employees = employees.Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value) ||(x.Id == userId) || (x.Active == 0));
                    }
                    else
                    {
                        employees = employees.Where(x => userId == x.Id || x.Id == userId || x.Active == 0);
                    }


                }

            }
            if (role == "Guest")
            {
                employees = employees.Where(x => userId == x.Id);

                if (employees == null)
                {
                    throw new ForBiddenException("You do not have access rights. Contact the administrator. Forbidden");
                }
            }
            if (role == "Supervisor")
            {
                List<int> RoleDepartmenIds = new List<int>();
                var departmentIds = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();

                foreach (var item in departmentIds)
                {
                    RoleDepartmenIds.Add(item);
                    var retIds = await GetAllChildDepartmentIds(item);
                    RoleDepartmenIds.AddRange(retIds);
                }

                if (RoleDepartmenIds.Count > 0)
                {
                    employees = employees.Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value) || x.Id == userId || x.Active == 0  );
                }
                else
                {
                    employees = employees.Where(x => userId == x.Id || x.Active == 0);
                }

            }



            return employees;
        }

        public async Task<List<int>> DelegateDepartmentEmployeeIds(int userId)
        {
            var requestDelegateIds = await Context.RequestDelegates.Where(c => c.EndDate > DateTime.Now && c.ToEmployeeId == userId).Select(x => x.FromEmployeeId).ToListAsync();
            var returnData = new List<int>();
            if (requestDelegateIds.Count > 0)
            {
                


                var departmentIds = await Context.DepartmentAdmin.AsNoTracking().Where(x => requestDelegateIds.Contains(x.EmployeeId)).Select(x => x.DepartmentId).ToListAsync();

                List<int> RoleDepartmenIds = new List<int>();
                foreach (var item in departmentIds)
                {
                    RoleDepartmenIds.Add(item);
                    var retIds = await GetAllChildDepartmentIds(item);
                    RoleDepartmenIds.AddRange(retIds);
                }



                var ManagerDepartmentIds = await Context.DepartmentManager.AsNoTracking().Where(x => requestDelegateIds.Contains(x.EmployeeId)).Select(x => x.DepartmentId).ToListAsync();

                foreach (var item in ManagerDepartmentIds)
                {
                    RoleDepartmenIds.Add(item);
                    var retIds = await GetAllChildDepartmentIds(item);
                    RoleDepartmenIds.AddRange(retIds);
                }

                if (RoleDepartmenIds.Count > 0)
                {
                    returnData = await Context.Employee.AsNoTracking().Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value)).Select(x => x.Id).ToListAsync();
                }
                else
                {
                    returnData.AddRange(requestDelegateIds);
                }
            }

            return returnData;
        }



        public async Task<List<int>> DelegateDepartmentIds(int userId)
        {
            var requestDelegateIds = await Context.RequestDelegates.Where(c => c.EndDate > DateTime.Now && c.ToEmployeeId == userId).Select(x => x.FromEmployeeId).ToListAsync();
            var returnData = new List<int>();
            if (requestDelegateIds.Count > 0)
            {



                var ManangerDepartmentIds = await Context.DepartmentManager.AsNoTracking().Where(x => requestDelegateIds.Contains(x.EmployeeId)).Select(x => x.DepartmentId).ToListAsync();

                List<int> RoleDepartmenIds = new List<int>();
                foreach (var item in ManangerDepartmentIds)
                {
                    RoleDepartmenIds.Add(item);
                    var retIds = await GetAllChildDepartmentIds(item);
                    RoleDepartmenIds.AddRange(retIds);
                }


                var AdminDepartmentIds = await Context.DepartmentAdmin.AsNoTracking().Where(x => requestDelegateIds.Contains(x.EmployeeId)).Select(x => x.DepartmentId).ToListAsync();
                foreach (var item in AdminDepartmentIds)
                {
                    RoleDepartmenIds.Add(item);
                    var retIds = await GetAllChildDepartmentIds(item);
                    RoleDepartmenIds.AddRange(retIds);
                }

                return RoleDepartmenIds;
            }
            else {
                return new List<int>();
            }
        }




        public async Task<List<int>> GetRoleEmployeeIds()
        {
            var role = _hTTPUserRepository.LogCurrentUser()?.Role;
            var userId = _hTTPUserRepository.LogCurrentUser()?.Id;
            var returnData = new List<int>();
            if (role == "DepartmentAdmin" || role == "DepartmentManager" || role == "Supervisor")
            {
                if (role == "DepartmentAdmin")
                {

                    var departmentIds = await Context.DepartmentAdmin.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.DepartmentId).ToListAsync();
                    var RoleEmloyerIds = await Context.EmployerAdmin.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.EmployerId).ToListAsync();
                  
                    
                    var delegateEmployeeIds = await DelegateDepartmentEmployeeIds(userId.Value);

                    List<int> RoleDepartmenIds = new List<int>();
                    foreach (var item in departmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds = await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }

                    if (RoleDepartmenIds.Count > 0 || RoleEmloyerIds.Count > 0)
                    {
                        returnData = await Context.Employee.AsNoTracking().Where(x => x.Id == userId || RoleDepartmenIds.Contains(x.DepartmentId.Value) || RoleEmloyerIds.Contains(x.EmployerId.Value)).Select(x => x.Id).ToListAsync();
                    }
                    else
                    {
                        // returnData.Add(userId.Value);
                        returnData = await Context.Employee.AsNoTracking().Where(x => x.Id == userId || x.Active == 0).Select(x => x.Id).ToListAsync();
                    }


                    if (delegateEmployeeIds.Count > 0)
                    {
                        returnData.AddRange(delegateEmployeeIds);
                    }


                }

                if (role == "Supervisor")
                {
                    List<int> RoleDepartmenIds = new List<int>();
                    var departmentIds = await Context.DepartmentSupervisor.AsNoTracking().Where(x => x.EmployeeId == userId || x.Active == 0).Select(x => x.DepartmentId).ToListAsync();
                    foreach (var item in departmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds = await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }

                    if (RoleDepartmenIds.Count > 0)
                    {
                        returnData = await Context.Employee.AsNoTracking().Where(x => x.Id == userId || RoleDepartmenIds.Contains(x.DepartmentId.Value)).Select(x => x.Id).ToListAsync();
                    }
                    else
                    {
                        returnData = await Context.Employee.AsNoTracking().Where(x => x.Id == userId || x.Active == 0).Select(x => x.Id).ToListAsync();
                    }



                }

                if (role == "DepartmentManager")
                {
                    List<int> RoleDepartmenIds = new List<int>();
                    var departmentIds = await Context.DepartmentManager.AsNoTracking().Where(x => x.EmployeeId == userId || x.Active == 0).Select(x => x.DepartmentId).ToListAsync();

                    var delegateEmployeeIds =  await DelegateDepartmentEmployeeIds(userId.Value);


                    foreach (var item in departmentIds)
                    {
                        RoleDepartmenIds.Add(item);
                        var retIds = await GetAllChildDepartmentIds(item);
                        RoleDepartmenIds.AddRange(retIds);
                    }

                    if (RoleDepartmenIds.Count > 0)
                    {
                        returnData = await Context.Employee.AsNoTracking().Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value) || x.Active == 0).Select(x => x.Id).ToListAsync();
                    }
                    else
                    {
                        //returnData.Add(userId.Value);
                        returnData = await Context.Employee.AsNoTracking().Where(x => x.Id == userId ||  x.Active == 0).Select(x => x.Id).ToListAsync();
                    }

                    if (delegateEmployeeIds.Count > 0) {
                        returnData.AddRange(delegateEmployeeIds);
                    }



                }

            }
            if (role == "Guest")
            {
                returnData.Add(userId.Value);
            }

            if (userId.HasValue) { 
                returnData.Add(userId.Value);  
            }

            return returnData;
        }

        public async Task<List<int>> GetAllChildDepartmentIds(int parentDepartmentId)
        {
            var ids = new HashSet<int>(); // Using HashSet to avoid duplicates
            await foreach (var id in GetChildDepartmentIdsRecursive(parentDepartmentId))
            {
                ids.Add(id);
            }
            return ids.ToList();
        }

        private async IAsyncEnumerable<int> GetChildDepartmentIdsRecursive(int departmentId)
        {
            var childDepartments = await Context.Department.AsNoTracking()
                .Where(d => d.ParentDepartmentId == departmentId)
                .Select(d => d.Id)
                .ToListAsync(); // Executing the query asynchronously

            foreach (var deptId in childDepartments)
            {
                yield return deptId;
                await foreach (var childDeptId in GetChildDepartmentIdsRecursive(deptId))
                {
                    yield return childDeptId;
                }
            }
        }


        #endregion


        #region SearchShortEmployee


        public async Task<SearchShortEmployeeResponse> SearchShortAdmin(SearchShortEmployeeRequest request, CancellationToken cancellationToken)
        {
            // Use meaningful variable names
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;

            // Use async/await for database operations
            var query = Context.Employee.AsQueryable();

            // Filter by keyword if provided
            if (!string.IsNullOrWhiteSpace(request.model.keyWord))
            {
                string keyword = request.model.keyWord.Trim().ToLower();
                query = query.Where(e => e.Lastname.Contains(keyword)
                                     || e.Firstname.Contains(keyword)
                                     || e.NRN.Contains(keyword)
                                     || e.Email.Contains(keyword)
                                     || e.Mobile.Contains(keyword)
                                     || e.SAPID.ToString().Contains(keyword));
            }

            // Calculate total count before pagination
            int totalCount = await query.CountAsync();

            // Apply pagination
            var results = await query.OrderBy(x => x.Id)
                                    .Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .Select(employee => new EmployeeSearchShortResult
                                    {
                                        Id = employee.Id,
                                        Lastname = employee.Lastname,
                                        Firstname = employee.Firstname,
                                        MLastname = employee.MLastname,
                                        MFirstname = employee.MFirstname,
                                        Mobile = employee.Mobile,
                                        Email = employee.Email,
                                        NRN = employee.NRN,
                                        SAPID = employee.SAPID
                                    })
                                    .ToListAsync();

            // Create and return the response
            var response = new SearchShortEmployeeResponse
            {
                data = results,
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = totalCount
            };

            return response;
        }

        #endregion

        #region EmployeeActiveCheckWith400
        public async Task EmployeeActiveCheck(int EmployeeId)
        {
            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                if (currentEmployee.Active != 1)
                {
                    throw new BadRequestException("This employee is inactive. Unable to act");
                }
            }
            else
            {
                throw new BadRequestException("Employee not found");
            }
        }



        #endregion

        #region Create And Update Database Validation
        public async Task CreateEmployeeValidateDB(CreateEmployeeRequest request, CancellationToken cancellation)
        {
            List<string> errors = new List<string>();
            var ErrorData = new List<EmployeeRegisterValidate>();
            if (request.ADAccount != null)
            {
                var currentEmployee = await Context.Employee
                    .Where(x => x.ADAccount == request.ADAccount)
                    .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            ADAccount = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).ADAccount = false;
                    }
                }
            }
            if (request.NRN != null)
            {
                var currentEmployee = await Context.Employee
                  .Where(x => x.NRN == request.NRN)
                  .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            NRN = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).NRN = false;
                    }
                }

            }
            if (request.SAPID != null)
            {
                var currentEmployee = await Context.Employee
                  .Where(x => x.SAPID == request.SAPID)
                  .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            SAPID = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).SAPID = false;
                    }
                }

            }
            if (request.Mobile != null)
            {
                var currentEmployee = Context.Employee
                .Where(x => x.Mobile == request.Mobile)
                .Select(x => new { x.Lastname, x.Firstname, x.Id })
                .FirstOrDefault();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            Mobile = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).Mobile = false;
                    }
                }
            }

            if (ErrorData.Count > 0)
            {
                string validationErrorData = JsonSerializer.Serialize(ErrorData);
                throw new BadRequestException(validationErrorData);
            }

        }



        public async Task CreateEmployeeRequestValidateDB(CreateEmployeeRequestRequest request, CancellationToken cancellation)
        {
            List<string> errors = new List<string>();
            var ErrorData = new List<EmployeeRegisterValidate>();
            if (request.ADAccount != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                    .Where(x => x.ADAccount == request.ADAccount)
                    .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            ADAccount = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).ADAccount = false;
                    }
                }
            }
            if (request.NRN != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                  .Where(x => x.NRN == request.NRN)
                  .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            NRN = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).NRN = false;
                    }
                }

            }
            if (request.SAPID != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                  .Where(x => x.SAPID == request.SAPID)
                  .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            SAPID = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).SAPID = false;
                    }
                }

            }
            if (request.Mobile != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                .Where(x => x.Mobile == request.Mobile)
                .Select(x => new { x.Lastname, x.Firstname, x.Id })
                .FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            Mobile = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).Mobile = false;
                    }
                }
            }

            if (ErrorData.Count > 0)
            {
                string validationErrorData = JsonSerializer.Serialize(ErrorData);
                throw new BadRequestException(validationErrorData);
            }

        }

        private async Task ChangeProfileStatus(UpdateEmployeeRequest request, CancellationToken cancellationToken)
        {
            if (request.Active == 0 || request.Active == null)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                if (currentEmployee != null)
                {
                    if (currentEmployee.Active == 1)
                    {
                        var complenceDataCheck = await Context.EmployeeStatus
                            .Where(x => x.EmployeeId == request.Id && x.EventDate.Value.Date == DateTime.Today && x.RoomId != null)
                            .FirstOrDefaultAsync();


                        var complenceTransportCheck = await Context.Transport
                                    .Where(x => x.EmployeeId == request.Id && x.EventDate.Value.Date == DateTime.Today)
                                    .FirstOrDefaultAsync();
                        if (complenceDataCheck == null && complenceTransportCheck == null)
                        {

                            var item = new DeActiveEmployee
                            {
                                Comment = "Deactive from profile",
                                EmployeeId = request.Id,
                                EventDate = DateTime.Today
                            };




                            await DeActiveEmployeeDelete(item.EmployeeId, cancellationToken);
                            //  await DeleteMoreData(item);
                            currentEmployee.Active = 0;
                            currentEmployee.RoomId = null;
                            currentEmployee.CompletionDate = DateTime.Now;
                            var empHisDat = new EmployeeHistory
                            {
                                Comment = item.Comment,
                                Active = 1,
                                DateCreated = DateTime.Now,
                                EventDate = item.EventDate.Date,
                                TerminationTypeId = item.DemobTypeTypeId,
                                
                                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                EmployeeId = item.EmployeeId,
                                Action = "Termination"
                            };
                            Context.EmployeeHistory.Add(empHisDat);

                            Context.Employee.Update(currentEmployee);

                            var currentEmployeeAdInfo = await Context.Employee.AsNoTracking().Where(x => x.Id == request.Id).Select(x => new { x.ADAccount }).FirstOrDefaultAsync(cancellationToken);
                            if (!string.IsNullOrWhiteSpace(currentEmployeeAdInfo?.ADAccount))
                            {
                                _hTTPUserRepository.ClearRoleCache(currentEmployeeAdInfo.ADAccount);
                            }
                            await _signalrhub.RoleChange(Convert.ToString(request.Id));
                            await Context.SaveChangesAsync();

                        }
                        else
                        {
                            throw new BadRequestException("This is on the employee's site. Cannot be deactive.");
                        }
                    }

                }
            }
            else
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                if (currentEmployee != null)
                {
                    if (currentEmployee.Active != 1) {
                        currentEmployee.Active = 1;
                        currentEmployee.CommenceDate = DateTime.Now;
                        Context.Employee.Update(currentEmployee);
                        await Context.SaveChangesAsync();

                    }

                }

            }

        }

        public async Task UpdateEmployeeValidateDB(UpdateEmployeeRequest request, CancellationToken cancellation)
        {

            await ChangeProfileStatus(request, cancellation);
            List<string> errors = new List<string>();
            var ErrorData = new List<EmployeeRegisterValidate>();

            if (request.ADAccount != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                    .Where(x => x.ADAccount == request.ADAccount && x.Id != request.Id)
                    .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            ADAccount = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).ADAccount = false;
                    }

                }
            }
            if (request.NRN != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                   .Where(x => x.NRN == request.NRN && x.Id != request.Id)
                   .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            NRN = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).NRN = false;
                    }
                }
            }
            if (request.SAPID != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
                  .Where(x => x.SAPID == request.SAPID && x.Id != request.Id)
                  .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            SAPID = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).SAPID = false;
                    }
                }

            }
            if (request.Mobile != null)
            {
                var currentEmployee = await Context.Employee.AsNoTracking()
              .Where(x => x.Mobile == request.Mobile && x.Id != request.Id)
              .Select(x => new { x.Lastname, x.Firstname, x.Id }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    if (ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id) == null)
                    {
                        var newData = new EmployeeRegisterValidate
                        {
                            EmployeeId = currentEmployee.Id,
                            FullName = $" {currentEmployee.Firstname} {currentEmployee.Lastname}",
                            Mobile = false,
                        };

                        ErrorData.Add(newData);
                    }
                    else
                    {
                        ErrorData.FirstOrDefault(x => x.EmployeeId == currentEmployee.Id).Mobile = false;
                    }
                }
            }



            if (ErrorData.Count > 0)
            {
                string validationErrorData = JsonSerializer.Serialize(ErrorData);
                throw new BadRequestException(validationErrorData);
            }

        }

        #endregion

        #region GetProfileTransport

        public async Task<List<GetProfileTransportResponse>> GetProfileTransport(GetProfileTransportRequest request, CancellationToken cancellationToken)
        {

            try
            {
                DateTime startDate = DateTime.Today.AddMonths(-2);
                DateTime endDate = DateTime.Today.AddMonths(6);

                var returnData = new List<GetProfileTransportResponse>();

                var transportInfo = await (from t in Context.Transport.AsNoTracking()
                                           join activeTransport in Context.ActiveTransport.AsNoTracking() on t.ActiveTransportId equals activeTransport.Id into activeTransportGroup
                                           from activeTransport in activeTransportGroup.DefaultIfEmpty()
                                           join ts in Context.TransportSchedule.AsNoTracking() on t.ActiveTransportId equals ts.ActiveTransportId into activeTransportSchedule
                                           from ts in activeTransportSchedule.DefaultIfEmpty()
                                           join tmode in Context.TransportMode on activeTransport.TransportModeId equals tmode.Id into activeTransportMode
                                           from tmode in activeTransportMode.DefaultIfEmpty()
                                           where t.EmployeeId == request.EmployeeId && (t.EventDate.Value.Date >= startDate.Date) && t.ScheduleId == ts.Id
                                                 && t.EventDate <= endDate
                                                 && ts.EventDate.Date == t.EventDate
                                           select new
                                           {
                                               Description = ts.Description,
                                               Direction = t.Direction,
                                               EventDate = t.EventDate,
                                               EventDateTime = t.EventDateTime,
                                               TransportMode = tmode.Code,
                                               TransportCode = activeTransport.Code
                                           }).OrderBy(c => c.EventDateTime).ToListAsync();

                if (transportInfo.Count == 0)
                {
                    return new List<GetProfileTransportResponse>();
                }

                var inData = transportInfo.Where(x => x.Direction == "IN" || x.Direction == "EXTERNAL").Select(x => new GetProfileTransportResponse
                {
                    InDescription = x.Description,
                    InDirection = x.Direction,
                    InEventDate = x.EventDate,
                    InTransportMode = x.TransportMode,
                    InTransportCode = x.TransportCode,
                    InEventDateTime = x.EventDateTime,


                }).ToList();

                var outData = transportInfo.Where(x => x.Direction == "OUT").Select(x => new GetProfileTransportResponse
                {
                    OutDescription = x.Description,
                    OutDirection = x.Direction,
                    OutEventDate = x.EventDate,
                    OutTransportMode = x.TransportMode,
                    OutTransportCode = x.TransportCode,
                    OutEventDateTime = x.EventDateTime
                }).ToList();




                foreach (var inItem in inData)
                {
                    var outItem = outData.FirstOrDefault(x => x.OutEventDateTime >= inItem.InEventDateTime);

                    if (inItem.InEventDate.Value.Date.Month == 7 && inItem.InEventDate.Value.Date.Day == 25)
                    {
                        var aa = 0;
                    }

                    var inDailyTripdata = outData.FirstOrDefault(x => x.OutEventDate.Value.Date == inItem.InEventDate.Value.Date && x.OutDirection != "EXTERNAL");

                  //  if (outItem != null)
                  //  {
                     //   var outDailyTrip = inData.FirstOrDefault(x => x.InEventDate.Value.Date == outItem.OutEventDate.Value.Date && x.OutDirection != "EXTERNAL");


                        //if (inDailyTripdata == null && outDailyTrip == null)
                        //{
                            if (inItem.InDirection == "IN" && outItem != null)
                            {
                                returnData.Add(new GetProfileTransportResponse
                                {
                                    InDescription = inItem.InDescription,
                                    InDirection = inItem.InDirection,
                                    InEventDate = inItem.InEventDate,
                                    InTransportMode = inItem.InTransportMode,
                                    InTransportCode = inItem.InTransportCode,
                                    InEventDateTime = inItem.InEventDateTime,

                                    OutDescription = outItem?.OutDescription,
                                    OutDirection = outItem?.OutDirection,
                                    OutEventDate = outItem?.OutEventDate,
                                    OutEventDateTime = outItem?.OutEventDateTime,
                                    OutTransportMode = outItem?.OutTransportMode,
                                    OutTransportCode = outItem?.OutTransportCode,
                                    
                                });
                            }
                            if (inItem.InDirection == "EXTERNAL")
                            {
                                returnData.Add(new GetProfileTransportResponse
                                {
                                    InDescription = inItem.InDescription,
                                    InDirection = inItem.InDirection,
                                    InEventDate = inItem.InEventDate,
                                    InEventDateTime = inItem.InEventDateTime,
                                    InTransportMode = inItem.InTransportMode,
                                    InTransportCode = inItem.InTransportCode
                                });
                            }
                    //    }
                        //else
                        //{
                        //    var outItemDaily = outData.FirstOrDefault(x => x.OutEventDate.Value.Date == inItem.InEventDate.Value.Date);


                        //    if (outItemDaily != null)
                        //    {

                        //        returnData.Add(new GetProfileTransportResponse
                        //        {
                        //            InDescription = inItem.InDescription,
                        //            InDirection = inItem.InDirection,
                        //            InEventDate = inItem.InEventDate,
                        //            InEventDateTime = inItem.InEventDateTime,

                        //            InTransportMode = inItem.InTransportMode,
                        //            InTransportCode = inItem.InTransportCode,
                        //            OutDescription = outItemDaily?.OutDescription,
                        //            OutDirection = outItemDaily?.OutDirection,
                        //            OutEventDate = outItemDaily?.OutEventDate,
                        //            OutEventDateTime = outItemDaily?.OutEventDateTime,
                        //            OutTransportMode = outItemDaily?.OutTransportMode,
                        //            OutTransportCode = outItemDaily?.OutTransportCode
                        //        });
                        //    }
                        //    else
                        //    {
                        //        var outItemNext = outData.FirstOrDefault(x => x.OutEventDate > outDailyTrip.InEventDate);

                        //        returnData.Add(new GetProfileTransportResponse
                        //        {
                        //            InDescription = inItem.InDescription,
                        //            InDirection = inItem.InDirection,
                        //            InEventDate = inItem.InEventDate,
                        //            InEventDateTime = inItem.InEventDateTime,
                        //            InTransportMode = inItem.InTransportMode,
                        //            InTransportCode = inItem.InTransportCode,
                        //            OutDescription = outItemNext?.OutDescription,
                        //            OutDirection = outItemNext?.OutDirection,
                        //            OutEventDate = outItemNext?.OutEventDate,
                        //            OutTransportMode = outItemNext?.OutTransportMode,
                        //            OutTransportCode = outItemNext?.OutTransportCode,
                        //            OutEventDateTime = outItemNext?.OutEventDateTime
                        //        }) ;
                        //    }


                        //}
                 //   }





                }


                return returnData.OrderBy(x => x.InEventDate).ToList();

            }
            catch (Exception)
            {
                return new List<GetProfileTransportResponse>();
            }


            #endregion




        }


        #region SearchAccomodation


        public async Task<SearchEmployeeAccommodationResponse> SearchAccommodation(SearchEmployeeAccommodationRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;

            bool? SAPSearchStatus = false;
            List<int> SearchSAPIds = new List<int>();
            List<int> NotFoundSAPIDs = new List<int>();


            IQueryable<Employee> empFilter = Context.Employee.AsNoTracking().Where(x => x.Active != 2);


            if (request.model.Active.HasValue)
            {
                if (request.model.Active.Value == 1)
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.Active == 1);
                }
                else
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.Active == 0);
                }

            }
            if (!string.IsNullOrWhiteSpace(request.model?.Firstname))
            {
                string trimmedFirstname = request.model.Firstname.Trim();
                empFilter = empFilter.AsNoTracking().Where(e => e.Firstname.Contains(trimmedFirstname));
            }

            if (!string.IsNullOrWhiteSpace(request.model?.Lastname))
            {
                string trimmedLastname = request.model.Lastname.Trim();
                empFilter = empFilter.AsNoTracking().Where(e => e.Lastname.Contains(trimmedLastname));
            }

            if (!string.IsNullOrWhiteSpace(request.model.Id))
            {

                string numericInput = Regex.Replace(request.model.Id, "[^0-9]+", ",");
                if (!string.IsNullOrWhiteSpace(numericInput))
                {
                    var Ids = numericInput.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();


                    if (Ids.Count > 0)
                    {
                        empFilter = empFilter.AsNoTracking().Where(x => Ids.Contains(x.Id));
                    }

                }

            }
            if (request.model.CostCodeId.HasValue && request.model.CostCodeId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.CostCodeId == request.model.CostCodeId);
            }
            if (request.model.FlightGroupMasterId.HasValue && request.model.FlightGroupMasterId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.FlightGroupMasterId == request.model.FlightGroupMasterId);
            }
            if (request.model.LocationId.HasValue && request.model.LocationId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.LocationId == request.model.LocationId);
            }
            if (request.model.PeopleTypeId.HasValue && request.model.PeopleTypeId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.PeopleTypeId == request.model.PeopleTypeId);
            }
            if (request.model.RosterId.HasValue && request.model.RosterId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.RosterId == request.model.RosterId);
            }
            if (request.model.EmployerId.HasValue && request.model.EmployerId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.EmployerId == request.model.EmployerId);
            }

            if (request.model.Departmentid.HasValue && request.model.Departmentid.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.DepartmentId == request.model.Departmentid);
            }
            if (request.model?.Firstname != null && !string.IsNullOrWhiteSpace(request.model.Firstname))
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.Firstname.StartsWith(request.model.Firstname.Trim()));
            }


            if (request.model?.Lastname != null && !string.IsNullOrWhiteSpace(request.model.Lastname))
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.Lastname.StartsWith(request.model.Lastname.Trim()));
            }

            if (request.model?.NRN != null && !string.IsNullOrWhiteSpace(request.model.NRN))
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.NRN.Contains(request.model.NRN.Trim()));
            }
            if (request.model.PositionId.HasValue && request.model.PositionId.Value > 0)
            {
                empFilter = empFilter.AsNoTracking().Where(e => e.PositionId == request.model.PositionId);
            }

            if (request.model?.Mobile != null && !string.IsNullOrWhiteSpace(request.model.Mobile))
            {
              //  empFilter = empFilter.AsNoTracking().Where(e => e.Mobile.Contains(request.model.Mobile.Trim()));


                string trimmedMobile = request.model.Mobile.Trim();
                empFilter = empFilter.Where(e =>
                    (e.Mobile != null && e.Mobile.Contains(trimmedMobile)) ||
                    (e.PersonalMobile != null && e.PersonalMobile.Contains(trimmedMobile))
                );
            }
            if (request.model.HasRoom.HasValue)
            {
                if (request.model.HasRoom.Value == 1)
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.RoomId != null && e.Active == 1);
                }
                else
                {
                    empFilter = empFilter.AsNoTracking().Where(e => e.RoomId == null && e.Active == 1);
                }
            }


            if (!string.IsNullOrWhiteSpace(request.model.RoomNumber))
            {
                var RoomIds = await Context.Room.AsNoTracking()
                    .Where(x => x.Number.ToLower()
                    .Contains(request.model.RoomNumber.ToLower()))
                    .Select(x => x.Id).ToArrayAsync();
                if (RoomIds.Length > 0)
                {
                    empFilter = empFilter.Where(x => RoomIds.Contains(x.RoomId.Value));
                }
            }



            if (request.model.CampId.HasValue && request.model.CampId > 0)
            {

                int[] campRooms = Context.Room
                 .Where(r => r.CampId == request.model.CampId)
                 .Select(r => r.Id)
                 .ToArray();
                if (campRooms.Length > 0)
                {

                    empFilter = empFilter.AsNoTracking().Where(x => campRooms.Any(y => y == x.RoomId.Value));
                }

            }


            if (request.model.RoomTypeId.HasValue && request.model.RoomTypeId > 0)
            {

                int[] roomTypeRooms = Context.Room.AsNoTracking()
                 .Where(r => r.RoomTypeId == request.model.RoomTypeId)
                 .Select(r => r.Id)
                 .ToArray();
                if (roomTypeRooms.Length > 0)
                {

                    empFilter = empFilter.AsNoTracking().Where(x => roomTypeRooms.Any(y => y == x.RoomId.Value));
                }

            }

            if (!string.IsNullOrWhiteSpace(request.model.SAPID))
            {
                string numericInput = Regex.Replace(request.model.SAPID, "[^0-9]+", ",");
                if (!string.IsNullOrWhiteSpace(numericInput))
                {
                    var sapIds = numericInput.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse)
                      .ToList();


                    if (sapIds.Count > 0)
                    {
                        SAPSearchStatus = true;
                        SearchSAPIds = sapIds;

                        empFilter = empFilter.AsNoTracking().Where(x => sapIds.Contains(x.SAPID.Value));
                    }

                }
            }


            if (request.model.FutureBooking.HasValue)
            {
                DateTime today = DateTime.Today.Date;
                IQueryable<int> bookingEmployeeIds = Context.EmployeeStatus
                    .AsNoTracking()
                    .Where(x => x.EventDate >= today && x.RoomId != null)
                    .Select(x => x.EmployeeId.Value)
                    .Distinct();


                if (request.model.FutureBooking.Value == 1)
                {
                    empFilter = empFilter
                    .Join(bookingEmployeeIds, e => e.Id, id => id, (e, id) => e)
                    .Where(e => e.Active == 1);
                }
                else
                {

                    empFilter = from e in empFilter
                                join b in bookingEmployeeIds on e.Id equals b into gj
                                from subb in gj.DefaultIfEmpty()
                                where subb == null && e.Active == 1
                                select e;
                }
            }


            empFilter = await RoleEmployeeEmfilter(empFilter);
            int totalEmployees = await empFilter.CountAsync();


            if (SAPSearchStatus.HasValue && SAPSearchStatus.Value)
            {
                if (SAPSearchStatus.Value)
                {
                    var existingSAPIDs = await empFilter.ToListAsync();
                    var existData = existingSAPIDs
                        .Where(x => x.SAPID.HasValue)
                        .Select(x => x.SAPID.Value)
                        .ToList();
                    if (SearchSAPIds != null)
                    {
                        if (SearchSAPIds != null && SearchSAPIds.Count > 0)
                        {

                            //    NotFoundSAPIDs = SearchSAPIds.Except(existData).ToList();
                            NotFoundSAPIDs = SearchSAPIds.Except(existData).ToList();
                        }
                    }



                }

            }


            var empfilterQuery = empFilter.OrderBy(e => e.Id) // Add an appropriate ordering here
                    .Skip((pageIndex) * pageSize)
                    .Take(pageSize);

            var result = await (from employee in Context.Employee.AsNoTracking()
                                where empfilterQuery.AsNoTracking().Contains(employee)
                                join state in Context.State on employee.StateId equals state.Id into stateData
                                from state in stateData.DefaultIfEmpty()
                                join shift in Context.Shift.AsNoTracking() on employee.Shiftid equals shift.Id into shiftData
                                from shift in shiftData.DefaultIfEmpty()
                                join costcode in Context.CostCodes.AsNoTracking() on employee.CostCodeId equals costcode.Id into costcodeData
                                from costcode in costcodeData.DefaultIfEmpty()
                                join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                from department in departmentData.DefaultIfEmpty()
                                join position in Context.Position.AsNoTracking() on employee.PositionId equals position.Id into positionData
                                from position in positionData.DefaultIfEmpty()
                                join roster in Context.Roster.AsNoTracking() on employee.RosterId equals roster.Id into rosterData
                                from roster in rosterData.DefaultIfEmpty()
                                join location in Context.Location.AsNoTracking() on employee.LocationId equals location.Id into locationData
                                from location in locationData.DefaultIfEmpty()
                                join peopletype in Context.PeopleType.AsNoTracking() on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                from peopletype in peopletypeData.DefaultIfEmpty()
                                join room in Context.Room.AsNoTracking() on employee.RoomId equals room.Id into roomData
                                from room in roomData.DefaultIfEmpty()
                                join flightgroupmaster in Context.FlightGroupMaster.AsNoTracking() on employee.FlightGroupMasterId equals flightgroupmaster.Id into flightgroupmasterData
                                from flightgroupmaster in flightgroupmasterData.DefaultIfEmpty()
                                join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                from employer in employerData.DefaultIfEmpty()
                                select new SearchEmployeeAccommodationSearchResult
                                {
                                    Id = employee.Id,
                                    Lastname = employee.Lastname,
                                    Firstname = employee.Firstname,
                                    MLastname = employee.MLastname,
                                    MFirstname = employee.MFirstname,
                                    Mobile = employee.Mobile,
                                    Email = employee.Email,
                                    EmployerName = employer.Description,
                                    StateId = employee.StateId,
                                    StateName = state.Description,
                                    ShiftId = employee.Shiftid,
                                    ShiftName = shift.Description,
                                    CostCodeName = $"{costcode.Number}-{costcode.Description}",
                                    DepartmentName = department.Name,
                                    PositionName = position.Description,
                                    RosterName = roster.Name,
                                    LocationName = location.Description,
                                    PeopleTypeName = peopletype.Code,
                                    RoomNumber = room.Number,
                                    FlightGroupMasterName = flightgroupmaster.Description,
                                    SAPID = employee.SAPID,
                                    CostCodeId = employee.CostCodeId,
                                    FlightGroupMasterId = employee.FlightGroupMasterId,
                                    LocationId = employee.LocationId,
                                    PositionId = employee.PositionId,
                                    PeopleTypeId = employee.PeopleTypeId,
                                    RosterId = employee.RosterId,
                                    Departmentid = employee.DepartmentId,
                                    NRN = employee.NRN,
                                    RoomId = employee.RoomId,
                                    EmployerId = employee.EmployerId,
                                    Gender = employee.Gender,
                                    HasFutureRoomBooking = 0,
                                    HasFutureTransport = 0,
                                    RoomTypeId = null,
                                    Active = employee.Active
                                }).ToListAsync();


            var retData = result.ToList();


            var employeeIds = retData.Select(item => item.Id).ToList();



            DateTime todayDate = DateTime.Today.Date;


            var transportData = await Context.Transport.AsNoTracking()
            .Where(x => employeeIds.Contains((int)x.EmployeeId) && x.EventDate >= DateTime.Today.Date)
            .ToListAsync();

            var groupedTransportData = transportData
                .GroupBy(x => x.EmployeeId)
                .Select(group => new
                {
                    EmployeeId = group.Key ?? 0, // Default value for nullable EmployeeId
                    Count = group.Count()
                });


            var groupedBookingData = await Context.EmployeeStatus.AsNoTracking()
                        .Where(e => employeeIds.Contains((int)e.EmployeeId) && e.EventDate >= DateTime.Today.Date)
                        .GroupBy(e => e.EmployeeId)
                        .Select(group => new
                        {
                            EmployeeId = group.Key,
                            Count = group.Count()
                        })
                        .ToListAsync(cancellationToken);

            foreach (var item in retData)
            {
                int hasFutureTransportCount = groupedTransportData.Where(e => e.EmployeeId == item.Id).Count();
                int hasFutureRoomBookingCount = groupedBookingData.Where(e => e.EmployeeId == item.Id).Count();
                item.RoomTypeName = await Context.RoomType.AsNoTracking().Where(r => r.Id == item.RoomTypeId).Select(r => r.Description).FirstOrDefaultAsync();
                item.HasFutureRoomBooking = hasFutureRoomBookingCount > 0 ? 1 : 0;
                item.TodayOnsite =await Context.EmployeeStatus.AsNoTracking().Where(e => e.EmployeeId == item.Id && e.EventDate.Value.Date == DateTime.Today && e.RoomId != null).AnyAsync();
                item.HasFutureTransport = hasFutureTransportCount > 0 ? 1 : 0;
            }


          //  retData = _hTTPUserRepository.GetRoleEmpoyee(retData, "Id");


            var returnData = new SearchEmployeeAccommodationResponse
            {
                data = retData.OrderBy(x => x.Id)
                     .ToList<SearchEmployeeAccommodationSearchResult>(),
                pageSize = pageSize,
                NotFoundSAPIDs = NotFoundSAPIDs,
                currentPage = pageIndex,
                totalcount = totalEmployees
            };

            return returnData;
        }

        #endregion


        #region RemovePassportiMAGE

        public async Task RemovePassportImage(RemovePassportImageEmployeeRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.Employee.Where(x => x.Id == request.employeeId).FirstOrDefaultAsync();
            if (currentData != null)
            {
                if (currentData.PassportImage != null)
                {
                    currentData.PassportImage = null;
                    currentData.DateUpdated = DateTime.Now;
                    currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    Context.Employee.Update(currentData);

                }
            }
        }



        #endregion



    }


    public sealed class EmployeeRegisterValidate
    {
        public int? EmployeeId { get; set; }
        public string? FullName { get; set; }
    
        public bool? NRN { get; set; }

        public bool? SAPID { get; set; }

        public bool? ADAccount { get; set; }
        public bool? Mobile { get; set; }

    }

}


        

   

