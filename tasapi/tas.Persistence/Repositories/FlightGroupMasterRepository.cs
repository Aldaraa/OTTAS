using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.FlightGroupMasterFeature.CreateFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.GetAllFlightGroupMaster;
using tas.Application.Features.FlightGroupMasterFeature.GetFlightGroupMaster;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class FlightGroupMasterRepository : BaseRepository<FlightGroupMaster>, IFlightGroupMasterRepository
    {
        private readonly IConfiguration _configuration;
        public FlightGroupMasterRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
        }

        public async Task<List<GetAllFlightGroupMasterResponse>> GetAllData(GetAllFlightGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<GetAllFlightGroupMasterResponse>();
            if (request.status.HasValue)
            {

                var masters =await Context.FlightGroupMaster.AsNoTracking().Where(x => x.Active == request.status).ToListAsync();

                foreach (var x in masters)
                {

                    var detailCount = await Context.FlightGroupDetail.AsNoTracking().CountAsync(g => g.FlightGroupMasterId == x.Id);
                    var employeeCount = await Context.Employee.AsNoTracking().CountAsync(e => e.FlightGroupMasterId == x.Id && e.Active == 1);
                    var ClusterStatus = await GetClusterStatus(x.Id);
                    var newData = new GetAllFlightGroupMasterResponse
                    {
                        Id = x.Id,
                        Active = x.Active,
                        Code = x.Code,
                        DateCreated = x.DateCreated,
                        Description = x.Description,
                        DateUpdated = x.DateUpdated,
                        ClusterStatus = ClusterStatus,
                        EmployeeCount = employeeCount,
                        DetailCount = detailCount
                    };

                    returnData.Add(newData);
                }

                if (request.fullcluster.HasValue)
                {
                    return await Task.FromResult(returnData.Where(x => x.ClusterStatus != 0).ToList());
                }
                else
                {
                    return await Task.FromResult(returnData.ToList());
                }
            }
            else {
                var masters =await Context.FlightGroupMaster.AsNoTracking().ToListAsync();

                foreach (var x in masters)
                {

                    var ClusterStatus = await GetClusterStatus(x.Id);
                    var detailCount = await Context.FlightGroupDetail.AsNoTracking().CountAsync(g => g.FlightGroupMasterId == x.Id);
                    var employeeCount = await Context.Employee.AsNoTracking().CountAsync(e => e.FlightGroupMasterId == x.Id && e.Active == 1);
                    var newData = new GetAllFlightGroupMasterResponse
                    {
                        Id = x.Id,
                        Active = x.Active,
                        Code = x.Code,
                        DateCreated = x.DateCreated,
                        Description = x.Description,
                        DateUpdated = x.DateUpdated,
                        ClusterStatus = ClusterStatus,
                        DetailCount =detailCount,
                        EmployeeCount =employeeCount
                    };

                    returnData.Add(newData);
                }


                if (request.fullcluster.HasValue)
                {
                    return await Task.FromResult(returnData.Where(x=>x.ClusterStatus == 1).ToList());
                }
                else {
                    return await Task.FromResult(returnData.ToList());
                }

            }
        }

        private async Task<int> GetClusterStatus(int Id)
        {
            var detailCount =await Context.FlightGroupDetail.AsNoTracking().CountAsync(g => g.FlightGroupMasterId == Id);
            var clusteredDetailCount =await Context.FlightGroupDetail.AsNoTracking().CountAsync(g => g.FlightGroupMasterId == Id && g.ClusterId != null);
            if (detailCount > 0)
            {
                if (detailCount == clusteredDetailCount)
                {
                    return 1;
                }
                else {
                    if (clusteredDetailCount == 0)
                    {
                        return 0;
                    }
                    else {                 
                        return -1;
                    }
                }
            }
            else {
                return 0;
            }
        }

        public async Task<FlightGroupMaster> GetbyId(int id, CancellationToken cancellationToken)
        {
            return await Context.FlightGroupMaster.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<GetFlightGroupMasterResponse> GetProfile(GetFlightGroupMasterRequest request, CancellationToken cancellationToken) 
        {
           var headerData =await Context.FlightGroupMaster.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id);


            if (headerData == null)
            {
                return null;
            }

            var daysOfWeekOrder = new Dictionary<string, int>
            {
                { "Monday", 1 },
                { "Tuesday", 2 },
                { "Wednesday", 3 },
                { "Thursday", 4 },
                { "Friday", 5 },
                { "Saturday", 6 },
                { "Sunday", 7 }
            };


            var detailData = await (from GroupDetail in  Context.FlightGroupDetail.AsNoTracking().Where(x => x.FlightGroupMasterId == request.Id)
                                    join shift in Context.Shift.AsNoTracking() on GroupDetail.ShiftId equals shift.Id into  shiftData 
                                    from shift in shiftData.DefaultIfEmpty()
                                    select new { 
                                        Id = GroupDetail.Id,
                                        ShiftId =GroupDetail.ShiftId,
                                        DayNum = GroupDetail.DayNum,
                                        Direction = GroupDetail.Direction,
                                        ShiftCode = shift.Code,
                                        ClusterId = GroupDetail.ClusterId,
                                        SeqNumber = GroupDetail.SeqNumber,
                                        FlightGroupMasterId = GroupDetail.FlightGroupMasterId,
                                    })
                .OrderBy(x => x.DayNum)
                .ThenBy(x => x.Direction) // Use ThenBy for secondary sorting
                .ToListAsync(cancellationToken);


            var response = new GetFlightGroupMasterResponse
            {
                Id = headerData.Id,
                Code = headerData.Code,
                Description = headerData.Description,
                Active = headerData.Active,
                DateCreated = headerData.DateCreated,
                DateUpdated = headerData.DateUpdated,
                Detail = detailData.Select(d => new FlightGroupDetailResponseMaster
                {
                    Id = d.Id,
                    FlightGroupMasterId = d.FlightGroupMasterId,
                    ShiftId = d.ShiftId,
                    ShiftCode = d.ShiftCode,
                    ClusterId = d.ClusterId,
                    DayNum = d.DayNum,
                    Direction = d.Direction,
                    SeqNumber = d.SeqNumber
                })
                .OrderBy(x => daysOfWeekOrder[x.DayNum]) // Sort by DayNum based on the defined order
                .ThenBy(x => x.Direction) // Sort by Direction after sorting by DayNum
                .ToList()
            };

            return response;
        }


        public async Task CreateShiftConfig(int FlightGroupMasterId, int DayPattern)
        {

            if (DayPattern == 28)
            {
               List<string> activeShiftCodes = new List<string> { "DS", "NS" };
                var activeShifts = Context.Shift.AsNoTracking()
                   .Where(x => activeShiftCodes.Contains(x.Code))
                   .ToList();
                if (activeShifts.Count == 2)
                {
                    foreach (var item in activeShifts)
                    {
                        await GenerateFlightGroupDetail(FlightGroupMasterId, item.Id);
                    }
                }
                else
                {
                    throw new BadRequestException("Shift configuration information is incomplete. Complete the shift register");
                }

            }
            else if (DayPattern == 42)
            {
                List<string> activeShiftCodes = new List<string> { "DS", "NS", "N2" };
                var activeShifts = Context.Shift.AsNoTracking()
                   .Where(x => activeShiftCodes.Contains(x.Code))
                   .ToList();

                if (activeShifts.Count == 3)
                {
                    foreach (var item in activeShifts)
                    {
                        await GenerateFlightGroupDetail(FlightGroupMasterId, item.Id);
                    }
                }
                else {
                    throw new BadRequestException("Shift configuration information is incomplete. Complete the shift register");
                }

            }
            else {
                List<string> activeShiftCodes = new List<string> { "DS", "NS", "D2", "N2" };
                var activeShifts = Context.Shift.AsNoTracking()
                   .Where(x => activeShiftCodes.Contains(x.Code))
                   .ToList();

                if (activeShifts.Count == 4)
                {
                    foreach (var item in activeShifts)
                    {
                        await GenerateFlightGroupDetail(FlightGroupMasterId, item.Id);
                    }
                }
                else
                {
                    throw new BadRequestException("Shift configuration information is incomplete. Complete the shift register");
                }

            }


            //var activeShifts = Context.Shift
            //   .Where(x => x.TransportGroup == 1)
            //   .ToList();

        }



        private async Task GenerateFlightGroupDetail(int FlightGroupMasterId, int ShiftId)
        {
            string[] directions = new string[] { "IN", "OUT", };
            string[] dayNums = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

                int seq = 1;
                foreach (string dayName in dayNums)
                {
                    foreach (var direction in directions)
                    {

                    var currentItem = await Context.FlightGroupDetail.Where(x => x.FlightGroupMasterId == FlightGroupMasterId
                    && x.ShiftId == ShiftId
                    && x.DayNum == dayName
                    && x.Direction == direction).FirstOrDefaultAsync();
                       

                        if (currentItem == null)
                        {
                            var newData = new FlightGroupDetail
                            {
                                Active = 1,
                                Direction = direction,
                                ClusterId = null,
                                DateCreated = DateTime.Now,
                                DayNum = dayName,
                                FlightGroupMasterId = FlightGroupMasterId,
                                ShiftId = ShiftId,
                                SeqNumber = seq

                            };
                            Context.FlightGroupDetail.Add(newData);
                        }
                        seq++;

                    }
                }
                await Context.SaveChangesAsync();
           

            return;



        }


    }
}
