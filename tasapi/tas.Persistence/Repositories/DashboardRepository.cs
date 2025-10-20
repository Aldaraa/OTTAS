using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DashboardFeature.EmployeeDashboard;
using tas.Application.Features.DashboardFeature.RoomDashboard;
using tas.Application.Features.DashboardFeature.TransportDashboard;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{


    public class DashboardRepository : BaseRepository<Employee>, IDashboardRepository
    {
        public DashboardRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }


        #region Transport dahboard


        public async Task<TransportDashboardResponse> TransportData(TransportDashboardRequest request, CancellationToken cancellationToken)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();



            var todayTransportIds =await Context.TransportSchedule
                .Where(x => x.EventDate.Date == DateTime.Today && x.Active == 1 )
                .Select(x => x.ActiveTransportId).Distinct().ToArrayAsync();


            stopWatch.Stop();
            Console.WriteLine($"todayTransportIds ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();

            var transportweekData = await TransportWeekData(DateTime.Today);



            stopWatch.Stop();
            Console.WriteLine($"transportweekData ==================> {stopWatch.ElapsedMilliseconds}");



            var returnData = new TransportDashboardResponse
            {
                TodayINTransport = await Context.ActiveTransport
                .CountAsync(x => x.Active == 1 && x.Direction == "IN" && todayTransportIds.Contains(x.Id)),
                TodayOUTTransport = await Context.ActiveTransport
                .CountAsync(x => x.Active == 1 && x.Direction == "OUT" && todayTransportIds.Contains(x.Id)),
                SeatWeek = transportweekData,
                TodayINEmployeeCount = await Context.Transport.CountAsync(x=> x.Direction == "IN" && x.EventDate.Value.Date == DateTime.Today ),
                TodayOUTEmployeeCount = await Context.Transport.CountAsync(x => x.Direction == "OUT" && x.EventDate.Value.Date == DateTime.Today),



            };

            stopWatch.Restart();



            stopWatch.Stop();
            Console.WriteLine($"returnData ==================> {stopWatch.ElapsedMilliseconds}");

            return returnData;
        }

        //private async Task<List<SeatCountWeekData>> TransportWeekData(DateTime startDate)
        //{

        //    var returnData = new List<SeatCountWeekData>();
        //    var currentDate = startDate;
        //    var endDate = currentDate.AddDays(7);
        //    while (currentDate <= endDate)
        //    {

        //        var InConfirmedCount =  await Context.Transport
        //            .CountAsync(x => x.EventDate.Value.Date == currentDate.Date 
        //            && x.Status == "Confirmed" 
        //            && x.Direction == "IN"
        //            );
        //        var OutConfirmedCount = await Context.Transport
        //            .CountAsync(x => x.EventDate.Value.Date == currentDate.Date
        //            && x.Status == "Confirmed"
        //            && x.Direction == "OUT");
        //        var OutOverBookedCount = await Context.Transport
        //            .CountAsync(x => x.EventDate.Value.Date == currentDate.Date
        //            && x.Status == "Over Booked"
        //            && x.Direction == "OUT");
        //        var InOverBookedCount = await Context.Transport
        //            .CountAsync(x => x.EventDate.Value.Date == currentDate.Date
        //            && x.Status == "Over Booked"
        //            && x.Direction == "IN"
        //            );
        //        var newData = new SeatCountWeekData
        //        {
        //            date = currentDate,
        //            InConfirmed = InConfirmedCount,
        //            OutConfirmed = OutConfirmedCount,
        //            InOverBook = InOverBookedCount,
        //            OutOverBook = OutOverBookedCount,
        //            InSeatBlock = await Context.Transport.CountAsync(x => x.Direction == "IN" && x.EventDate.Value.Date == currentDate.Date && x.SeatBlock == 1),
        //            OutSeatBlock = await Context.Transport.CountAsync(x => x.Direction == "OUT" && x.EventDate.Value.Date == currentDate.Date && x.SeatBlock == 1),
        //            InEmloyeeCount = await Context.Transport.CountAsync(x => x.Direction == "IN" && x.EventDate.Value.Date == currentDate.Date),
        //            OutEmloyeeCount = await Context.Transport.CountAsync(x => x.Direction == "OUT" && x.EventDate.Value.Date == currentDate.Date),
        //        };

        //        returnData.Add(newData);
        //        currentDate = currentDate.AddDays(1);
        //    }

        //    return returnData;

        //}


        private async Task<List<SeatCountWeekData>> TransportWeekData(DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            var returnData = await Context.Transport
                .Where(x => x.EventDate >= startDate && x.EventDate < endDate)
                .GroupBy(x => x.EventDate.Value.Date)
                .Select(group => new SeatCountWeekData
                {
                    date = group.Key,
                    InConfirmed = group.Count(x => x.Status == "Confirmed" && x.Direction == "IN"),
                    OutConfirmed = group.Count(x => x.Status == "Confirmed" && x.Direction == "OUT"),
                    InOverBook = group.Count(x => x.Status == "Over Booked" && x.Direction == "IN"),
                    OutOverBook = group.Count(x => x.Status == "Over Booked" && x.Direction == "OUT"),
                    InSeatBlock = group.Count(x => x.Direction == "IN" && x.SeatBlock == 1),
                    OutSeatBlock = group.Count(x => x.Direction == "OUT" && x.SeatBlock == 1),
                    InEmloyeeCount = group.Count(x => x.Direction == "IN"),
                    OutEmloyeeCount = group.Count(x => x.Direction == "OUT"),
                })
                .ToListAsync();

            return returnData;
        }

        #endregion


        #region Room Dashboard



        public async Task<RoomDashboardResponse> RoomData(RoomDashboardRequest request, CancellationToken cancellationToken)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();




            var todayActiveBedRoomIds =await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.Active == 1 && x.RoomId != null && x.EventDate.Value.Date == DateTime.Today)
                .Select(x => x.RoomId).ToArrayAsync();
            int todayVirtualRoomEmployeeCount = 0;
            var todayEmptyRoom = await Context.Room.AsNoTracking().CountAsync(x => !todayActiveBedRoomIds.Contains(x.Id) && x.VirtualRoom != 1);
            var virtualRoomIds = await Context.Room
                .Where(x => x.VirtualRoom == 1)
                .Select(x => x.Id)
                .ToListAsync();
            if (virtualRoomIds.Count > 0)
            {

                var EmployeeRoomStatus = await Context.EmployeeStatus.AsNoTracking()
                    .Where(x => x.EventDate.Value.Date == DateTime.Today && x.RoomId != null).Select(x => new { x.RoomId }).ToListAsync();
                todayVirtualRoomEmployeeCount = EmployeeRoomStatus.AsEnumerable().Count(x => virtualRoomIds.Contains(Convert.ToInt32(x.RoomId)));
            }





            stopWatch.Stop();
            Console.WriteLine($"todayActiveBedRoomIds ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();



            var VirtualRoomWeek = await VirtualRoomWeekData(DateTime.Today);



            stopWatch.Stop();
            Console.WriteLine($"VirtualRoomWeekData ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();




            var ActiveRoom =new List<ActiveRoomWeekData>();// await ActiveRoomWeek(DateTime.Today);


            stopWatch.Stop();
            Console.WriteLine($"watchActiveRoom ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            var EmptyRoom =  await EmptyRoomWeek(DateTime.Today);

            stopWatch.Stop();
            Console.WriteLine($"EmptyRoom ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();


            var returnData = new RoomDashboardResponse
            {
                TotalRooms = await Context.Room.AsNoTracking().CountAsync(x => x.VirtualRoom != 1 && x.Active == 1),
                TodayActiveBed =  0,//await GetDateAvailableBed(DateTime.Today),
                TodayEmptyRoom = todayEmptyRoom,
                TodayVirtualRoomEmloyees = todayVirtualRoomEmployeeCount,
                VirtualRoomWeek = VirtualRoomWeek,
                ActiveRoomWeek = ActiveRoom,
                EmptyRoomWeek = EmptyRoom


            };



            stopWatch.Stop();
            Console.WriteLine($"returnData ==================> {stopWatch.ElapsedMilliseconds}");
            // var returnData = new RoomDashboardResponse();
            return returnData; 
        }

        private async Task<List<VirtualRoomWeekData>> VirtualRoomWeekData(DateTime startDate)
        {
            //var currentDate = startDate;
            //var endDate = currentDate.AddDays(7);
            //var returnData = new List<VirtualRoomWeekData>();
            //var virtualRoomIds = await Context.Room.AsNoTracking()
            //    .Where(x => x.VirtualRoom == 1)
            //    .Select(x => x.Id)
            //    .ToListAsync();
            //while (currentDate <= endDate)
            //{
            //    int dateVirtualRoomEmployeeCount = 0;
            //    var EmployeeRoomStatus = await Context.EmployeeStatus.AsNoTracking()
            //                     .Where(x => x.EventDate.Value.Date == currentDate.Date && x.RoomId != null).Select(x => new { x.RoomId }).ToListAsync();
            //    dateVirtualRoomEmployeeCount = EmployeeRoomStatus.Count(x => virtualRoomIds.Contains(Convert.ToInt32(x.RoomId)));

            //    var newData = new VirtualRoomWeekData
            //    {
            //        date = currentDate,
            //        VirtualRoomEmployeeCount = dateVirtualRoomEmployeeCount,

            //    };

            //    returnData.Add(newData);

            //    currentDate = currentDate.AddDays(1);
            //}

            //return returnData;

            var currentDate = startDate;
            var endDate = currentDate.AddDays(7);
            var returnData = new List<VirtualRoomWeekData>();

            var virtualRoomIds = await Context.Room.AsNoTracking()
                .Where(x => x.VirtualRoom == 1)
                .Select(x => x.Id)
                .ToListAsync();

            var employeeStatusData = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EventDate >= currentDate.Date && x.EventDate < endDate.Date && x.RoomId != null)
                .ToListAsync();

            // Group the employeeStatusData by EventDate
            var groupedData = employeeStatusData
                .Where(x => virtualRoomIds.Contains(Convert.ToInt32(x.RoomId)))
                .GroupBy(x => x.EventDate.Value.Date)
                .ToList();

            while (currentDate <= endDate)
            {
                // Find the group that matches the currentDate.Date
                var group = groupedData.FirstOrDefault(g => g.Key == currentDate.Date);

                int dateVirtualRoomEmployeeCount = group?.Count() ?? 0;

                var newData = new VirtualRoomWeekData
                {
                    date = currentDate,
                    VirtualRoomEmployeeCount = dateVirtualRoomEmployeeCount,
                };

                returnData.Add(newData);

                currentDate = currentDate.AddDays(1);
            }

            return returnData;

        }

        private async Task<List<ActiveRoomWeekData>> ActiveRoomWeek(DateTime startDate)
        {
            var currentDate = startDate;
            var endDate = currentDate.AddDays(7);
            var returnData = new List<ActiveRoomWeekData>();

            var statusRoomDate = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EventDate.Value.Date >= currentDate.Date && x.EventDate >= endDate && x.RoomId != null)
                .ToListAsync();


            var AllRoomRoomIds = await Context.Room.AsNoTracking().Where(x => x.Active == 1 && x.VirtualRoom != 1)
                .Select(x => new { x.Id, x.BedCount }).ToListAsync();

            var EmptyRooms = AllRoomRoomIds.Where(x => !statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x=>new { x.Id, x.BedCount});
            var NonEmptyRooms = AllRoomRoomIds.Where(x => statusRoomDate.Select(y => x.Id).Contains(x.Id)).Select(x => new { x.Id, x.BedCount });




            while (currentDate <= endDate)
            {
                int ActiveRoomCount = EmptyRooms.Count();

                var DateRooms = statusRoomDate.Where(x => x.EventDate.Value.Date == currentDate.Date); 
                foreach (var item in NonEmptyRooms)
                {
                    var dateRoomEmloyeeCoount = statusRoomDate.Count(x => x.EventDate.Value.Date == currentDate.Date && x.RoomId == item.Id);
                    if (item.BedCount > dateRoomEmloyeeCoount)
                    {
                        ActiveRoomCount++;
                    }
                }
                var newData = new ActiveRoomWeekData
                {
                    date = currentDate,
                    ActiveRoomCount =ActiveRoomCount,

                };

                returnData.Add(newData);

                currentDate = currentDate.AddDays(1);
            }

            return returnData;
        }




        private async Task<List<EmptyRoomWeekData>> EmptyRoomWeek(DateTime startDate)
        {
            var currentDate = startDate;
            var endDate = currentDate.AddDays(7);

            var statusRoomDate = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EventDate.Value.Date >= currentDate.Date && x.EventDate <= endDate && x.RoomId != null)
                .ToListAsync();

            var allRoomRoomIds = await Context.Room.AsNoTracking()
                .Where(x => x.Active == 1 && x.VirtualRoom != 1)
                .Select(x => new { x.Id, x.BedCount })
                .ToListAsync();

            var nonEmptyRooms = statusRoomDate.Select(x => x.RoomId).ToArray();

            var returnData = Enumerable.Range(0, (int)(endDate - currentDate).TotalDays + 1)
                .Select(offset => currentDate.AddDays(offset))
                .Select(date => new EmptyRoomWeekData
                {
                    date = date,
                    EmptyRoomCount = allRoomRoomIds.Count(x => !nonEmptyRooms.Contains(x.Id))
                })
                .ToList();

            return returnData;



            //var currentDate = startDate;
            //var endDate = currentDate.AddDays(7);
            //var returnData = new List<EmptyRoomWeekData>();

            //var statusRoomDate = await Context.EmployeeStatus.AsNoTracking()
            //    .Where(x => x.EventDate.Value.Date >= currentDate.Date && x.EventDate <= endDate && x.RoomId != null)
            //    .ToListAsync();


            //var AllRoomRoomIds = await Context.Room.AsNoTracking().Where(x => x.Active == 1 && x.VirtualRoom != 1)
            //    .Select(x => new { x.Id, x.BedCount }).ToListAsync();

            //var  NonEmptyRooms = statusRoomDate.Select(x => x.RoomId).ToArray();
            //var EmptyRooms = AllRoomRoomIds.Where(x => !NonEmptyRooms.Contains(x.Id)).Select(x => new { x.Id, x.BedCount });
            //while (currentDate <= endDate)
            //{

            //    var newData = new EmptyRoomWeekData
            //    {
            //        date = currentDate,
            //        EmptyRoomCount = EmptyRooms.Count(),

            //    };

            //    returnData.Add(newData);

            //    currentDate = currentDate.AddDays(1);
            //}

            //return returnData;


            //var currentDate = startDate;
            //var endDate = currentDate.AddDays(7);
            //var returnData = new List<EmptyRoomWeekData>();
            //while (currentDate <= endDate)
            //{
            //    var dateActiveBedRoomIds = Context.EmployeeStatus
            //        .Where(x => x.Active == 1 && x.RoomId != null && x.EventDate.Value.Date == currentDate.Date)
            //        .Select(x => x.RoomId).ToArray();
            //    var dateEmptyRoom = await Context.Room
            //        .CountAsync(x => dateActiveBedRoomIds
            //        .Contains(x.Id) && x.VirtualRoom != 1);


            //    var newData = new EmptyRoomWeekData
            //    {
            //        date = currentDate,
            //        EmptyRoomCount = dateEmptyRoom
            //    };

            //    returnData.Add(newData);

            //    currentDate = currentDate.AddDays(1);
            //}

            //return returnData;
        }



        private async Task<int?> GetDateAvailableBed(DateTime eventDate)
        {
            var roomIds = await Context.Room
                .AsNoTracking()
                .Where(x => x.Active == 1 && x.VirtualRoom != 1)
                .Select(x => x.Id)
                .ToListAsync();

            var availableBedsTasks = roomIds.Select(item => RoomAvailableBeds(item, eventDate));
            var availableBeds = await Task.WhenAll(availableBedsTasks);

            return availableBeds.Sum();
        }

        private async Task<int> RoomAvailableBeds(int roomId, DateTime eventDate)
        {
            var statusRoomDate = await Context.EmployeeStatus
                .AsNoTracking()
                .Where(x => x.EventDate.Value.Date == eventDate.Date && x.RoomId == roomId)
                .ToListAsync().ConfigureAwait(false);

            if (statusRoomDate != null) {
                var currentRoom = await Context.Room
             .AsNoTracking()
             .Where(x => x.Id == roomId)
             .Select(x => new { x.VirtualRoom, x.BedCount })
             .FirstOrDefaultAsync();

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
                return 0;
            }
            return 0;
        }



        //private async Task<int?> GetDateAvailableBed(DateTime eventDate)
        //{
        //    int returnData = 0;
        //    var RoomIds = await Context.Room.AsNoTracking().Where(x => x.Active == 1 && x.VirtualRoom != 1).Select(x => x.Id).ToListAsync();
        //    foreach (var item in RoomIds)
        //    {
        //        returnData += 1;//await RoomAvailableBeds((int)item, eventDate);
        //    }
        //    return returnData;
        //}



        //private async Task<int> RoomAvailableBeds(int roomId, DateTime eventDate)
        //{
        //    var statusRoomDate = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate.Value.Date == eventDate.Date && x.RoomId == roomId).ToListAsync();

        //    if (statusRoomDate != null)
        //    {
        //        var currentRoom = await Context.Room.AsNoTracking().Where(x => x.Id == roomId)
        //            .Select(x=> new { x.VirtualRoom, x.BedCount})
        //            .FirstOrDefaultAsync();
        //        if (currentRoom != null)
        //        {
        //            if (currentRoom.VirtualRoom == 1)
        //            {
        //                return currentRoom.BedCount;
        //            }
        //            else
        //            {
        //                return currentRoom.BedCount - statusRoomDate.Count;
        //            }
        //        }
        //    }
        //    return 0; 
        //}

        #endregion


        #region Employee Dashboard



        public async Task<EmployeeDashboardResponse> EmployeeData(EmployeeDashboardRequest request, CancellationToken cancellationToken)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var totalEmployeesTask =await Context.Employee.CountAsync();
            var totalActiveEmployeesTask = await Context.Employee.CountAsync(x=> x.Active == 1);
            var totalDeActiveEmployeesTask = await Context.Employee.CountAsync(x => x.Active == 0);



            stopWatch.Stop();
            Console.WriteLine($"totalEmployees ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
            var employeeMNTask =await Context.Employee.CountAsync(x => x.Active == 1 && x.NationalityId == 144);

            stopWatch.Stop();
            Console.WriteLine($"employeeMNT ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();

            var employeeOtherCountriesTask =await Context.Employee.AsNoTracking().CountAsync(x => x.Active == 1 && x.NationalityId != 144);

            stopWatch.Stop();
            Console.WriteLine($"employeeOtherCountries ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
            var onsiteEmployeesTask =await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.EventDate.Value.Date == DateTime.Today && x.RoomId != null);

            stopWatch.Stop();
            Console.WriteLine($"onsiteEmployees ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
            var offsiteEmployeesTask =await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.EventDate.Value.Date == DateTime.Today && x.RoomId == null);
            stopWatch.Stop();
            Console.WriteLine($"offsiteEmployees ==================> {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
            var weekData = await GetSiteStatusEmployees(DateTime.Today);
            stopWatch.Stop();
            Console.WriteLine($"weekData ==================> {stopWatch.ElapsedMilliseconds}");


            //     await Task.WhenAll(totalEmployeesTask, employeeMNTask, employeeOtherCountriesTask, onsiteEmployeesTask, offsiteEmployeesTask);

            var returnData = new EmployeeDashboardResponse
            {
                TotalEmployees =  totalEmployeesTask,
                TotalActiveEmployees = totalActiveEmployeesTask,
                TotalDeActiveEmployees = totalDeActiveEmployeesTask,
                EmployeeMN =  employeeMNTask,
                EmployeeOtherCountries =  employeeOtherCountriesTask,
                OnsiteEmployees =  onsiteEmployeesTask,
                OffSiteEmployees =  offsiteEmployeesTask,
                WeekData = weekData
            };



            //var returnData = new EmployeeDashboardResponse
            //{
            //    TotalEmployees = await Context.Employee.CountAsync(x => x.Active == 1),
            //    EmployeeMN = await Context.Employee.CountAsync(x => x.Active == 1 && x.NationalityId == 144),
            //    EmployeeOtherCountries = await Context.Employee.AsNoTracking().CountAsync(x => x.Active == 1 && x.NationalityId != 144),
            //    OnsiteEmployees = await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.EventDate.Value.Date == DateTime.Today && x.RoomId != null),
            //    OffSiteEmployees = await Context.EmployeeStatus.AsNoTracking().CountAsync(x => x.EventDate.Value.Date == DateTime.Today && x.RoomId == null),
            //    WeekData =await GetSiteStatusEmployees(DateTime.Today)


            //};


            return returnData;
        }


        //private async Task<List<SiteStatusEmployee>> GetSiteStatusEmployees(DateTime startDate)
        //{
        //    var currentDate = startDate;
        //    var endDate = currentDate.AddDays(7);
        //    var returnData = new List<SiteStatusEmployee>();
        //    while (currentDate <= endDate)
        //    {

        //        var onsiteEmployees =await Context.EmployeeStatus.AsNoTracking()
        //            .CountAsync(x => x.EventDate.Value.Date == currentDate.Date && x.Active == 1 && x.RoomId != null);
        //        var offSiteEmployees = await Context.EmployeeStatus
        //            .CountAsync(x => x.EventDate.Value.Date == currentDate.Date 
        //            && x.Active == 1 && x.RoomId == null);

        //        var newData = new SiteStatusEmployee
        //        {
        //            date = currentDate,
        //            OnsiteEmployee = onsiteEmployees,
        //            OffsiteEmployee = offSiteEmployees
        //        };

        //        returnData.Add(newData);

        //       currentDate = currentDate.AddDays(1);
        //    }

        //    return returnData;
        //}

        private async Task<List<SiteStatusEmployee>> GetSiteStatusEmployees(DateTime startDate)
        {
            var endDate = startDate.AddDays(7);
            var returnData = new List<SiteStatusEmployee>();

            var statusData = await Context.EmployeeStatus.AsNoTracking()
                .Where(x => x.EventDate.Value.Date >= startDate.Date && x.EventDate.Value.Date <= endDate.Date && x.Active == 1)
                .GroupBy(x => x.EventDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    OnsiteEmployeeCount = g.Count(e => e.RoomId != null),
                    OffsiteEmployeeCount = g.Count(e => e.RoomId == null)
                })
                .ToListAsync();

            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                var dataForDate = statusData.FirstOrDefault(d => d.Date == currentDate.Date);

                var newData = new SiteStatusEmployee
                {
                    date = currentDate,
                    OnsiteEmployee = dataForDate?.OnsiteEmployeeCount ?? 0,
                    OffsiteEmployee = dataForDate?.OffsiteEmployeeCount ?? 0
                };

                returnData.Add(newData);
            }

            return returnData;
        }


        #endregion


    }

}

