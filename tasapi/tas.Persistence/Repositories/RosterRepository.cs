using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RoomFeature.GetAllRoom;
using tas.Application.Features.RoomFeature.GetRoom;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;
using tas.Application.Features.RosterFeature.GetAllRoster;
using tas.Application.Features.RosterFeature.GetRoster;
using System.Linq.Expressions;

namespace tas.Persistence.Repositories
{

    public class RosterRepository : BaseRepository<Roster>, IRosterRepository
    {
        public RosterRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }




        public async Task<List<GetAllRosterResponse>> GetAllData(GetAllRosterRequest request, CancellationToken cancellationToken)
        {
            var rosters = new List<Roster>();

            if (request.status.HasValue)
            {
                rosters =await Context.Roster.Where(x => x.Active == request.status).ToListAsync();
            }
            else
            {   
                rosters =await Context.Roster.ToListAsync();
            }

            var returnData =new List<GetAllRosterResponse>();

            foreach (var x in rosters)
            {

                var currData = await Context.RosterGroup.AsNoTracking().FirstOrDefaultAsync(g => g.Id == x.RosterGroupId);
                var detailCount = await Context.RosterDetail.AsNoTracking().CountAsync(gc => gc.RosterId == x.Id);

                var employeeCount = await Context.Employee.AsNoTracking().Where(a=> a.Active == 1 && a.RosterId ==x.Id).CountAsync();   
                var newRecord = new GetAllRosterResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Active = x.Active,
                    RosterGroupId = x.RosterGroupId,
                    DateCreated = x.DateCreated,
                    Description = x.Description,
                    RosterGroupName = currData?.Description,
                    DetailCount =detailCount,
                    EmployeeCount = employeeCount
                };

                returnData.Add(newRecord);
            }

            return returnData.OrderByDescending(x => x.DateCreated).ToList();
        }
        

        //public async Task<List<GetAllRosterResponse>> GetAll(CancellationToken cancellationToken)
        //{



        //    var result = await Context.Room
        //        .Join(Context.Camp,
        //            room => room.CampId,
        //            camp => camp.Id,
        //            (room, Camp) => new { room, Camp }
        //            ).Join(
        //             Context.RoomType,
        //             room => room.room.RoomTypeId,
        //             roomtype => roomtype.Id,
        //             (room, roomtype) => new GetAllRoomResponse
        //             {
        //                 Id = room.room.Id,
        //                 Number = room.room.Number,
        //                 CampName = room.Camp.Description,
        //                 BedCount = room.room.BedCount,
        //                 Private = room.room.Private,
        //                 Active = room.room.Active,
        //                 RoomTypeName = roomtype.Description,
        //                 CampId = room.room.Id,
        //                 DateCreated = room.room.DateCreated,
        //                 DateUpdated = room.room.DateUpdated,
        //                 RoomTypeId = room.room.RoomTypeId
        //             }).ToListAsync(cancellationToken);
        //    return result;
        //}

        //public async Task<GetRoomResponse?> Get(int Id, CancellationToken cancellationToken)
        //{
        //    return await Context.Room.Where(x => x.Id == Id)
        //        .Join(Context.Camp,
        //            room => room.CampId,
        //            camp => camp.Id,
        //            (room, Camp) => new { room, Camp }
        //            ).Join(
        //             Context.RoomType,
        //             room => room.room.RoomTypeId,
        //             roomtype => roomtype.Id,
        //             (room, roomtype) => new GetRoomResponse
        //             {
        //                 Id = room.room.Id,
        //                 Number = room.room.Number,
        //                 CampName = room.Camp.Description,
        //                 BedCount = room.room.BedCount,
        //                 Private = room.room.Private,
        //                 Active = room.room.Active,
        //                 RoomTypeName = roomtype.Description,
        //                 CampId = room.room.Id,
        //                 DateCreated = room.room.DateCreated,
        //                 DateUpdated = room.room.DateUpdated,
        //                 RoomTypeId = room.room.RoomTypeId,
        //                 BedList = Context.Bed.Where(x => x.RoomId == room.room.Id).ToList(),
        //             }).FirstOrDefaultAsync(cancellationToken);

        //}

    }
}
