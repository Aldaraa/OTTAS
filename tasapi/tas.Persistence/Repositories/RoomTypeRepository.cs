using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tas.Application.Features.PeopleTypeFeature.GetAllPeopleType;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Application.Features.RoomTypeFeature.GetAllRoomType;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RoomTypeRepository : BaseRepository<RoomType>, IRoomTypeRepository
    {
            private readonly IConfiguration _configuration;
        public RoomTypeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
        }
        public async Task<List<GetAllRoomTypeResponse>> GetAllData(GetAllRoomTypeRequest request, CancellationToken cancellationToken)
        {

            var RoomTypeQuery = Context.RoomType.AsNoTracking().AsQueryable();

            // Apply filters
            if (request.campId.HasValue)
            {
                var roomTypeIds = await Context.Room.AsNoTracking()
                    .Where(x => x.CampId == request.campId.Value)
                    .Select(x => x.RoomTypeId)
                    .Distinct()
                    .ToListAsync();

                RoomTypeQuery = RoomTypeQuery.Where(x => roomTypeIds.Contains(x.Id));
            }

            if (request.status.HasValue)
            {
                RoomTypeQuery = RoomTypeQuery.Where(x => x.Active == request.status);
            }

            var query = from rt in RoomTypeQuery
                        join rr in (
                            from e in Context.Employee.AsNoTracking()
                            join r in Context.Room.AsNoTracking() on e.RoomId equals r.Id into roomGroup
                            from rg in roomGroup.DefaultIfEmpty()
                            where e.RoomId != null
                            group rg by rg.RoomTypeId into grouped
                            select new
                            {
                                RoomTypeId = grouped.Key,
                                Count = grouped.Count()
                            }
                        ) on rt.Id equals rr.RoomTypeId into rrGroup
                        from rr in rrGroup.DefaultIfEmpty()
                        join rrtt in (
                            from r in Context.Room.AsNoTracking()
                            group r by r.RoomTypeId into grouped
                            select new
                            {
                                RoomTypeId = grouped.Key,
                                Count = grouped.Count()
                            }
                        ) on rt.Id equals rrtt.RoomTypeId into rrttGroup
                        from rrtt in rrttGroup.DefaultIfEmpty()
                        select new GetAllRoomTypeResponse
                        {
                            Id = rt.Id,
                            Description = rt.Description,
                            Active = rt.Active,
                            DateCreated = rt.DateCreated,
                            DateUpdated = rt.DateUpdated,
                            EmployeeCount = rr.Count,
                            RoomCount = rrtt.Count
                        };

            var result = await query.OrderByDescending(x => x.DateUpdated).ToListAsync(cancellationToken);

            return result;

        }
    }
}
