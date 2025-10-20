using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PositionFeature.GetAllRoomType;
using tas.Application.Features.RosterGroupFeature.GetAllRosterGroup;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class RosterGroupRepository : BaseRepository<RosterGroup>, IRosterGroupRepository
    {
        public RosterGroupRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }

        public async Task<List<GetAllRosterGroupResponse>> GetAllData(GetAllRosterGroupRequest request, CancellationToken cancellationToken)
        {
            var rosterGroupQuery = Context.RosterGroup.AsQueryable();
            if (request.status.HasValue)
            {
                rosterGroupQuery = rosterGroupQuery.Where(x => x.Active == request.status);
            }

            var query = await (from rg in rosterGroupQuery
                               join r in Context.Roster
                               on rg.Id equals r.RosterGroupId into rGroup
                               from r in rGroup.DefaultIfEmpty()
                               group r by new
                               {
                                   rg.Id,
                                   rg.Description,
                                   rg.Active,
                                   rg.DateCreated,
                                   rg.DateUpdated
                               } into grouped
                               select new GetAllRosterGroupResponse
                               {
                                   Id = grouped.Key.Id,
                                   Description = grouped.Key.Description,
                                   Active = grouped.Key.Active,
                                   DateCreated = grouped.Key.DateCreated,
                                   DateUpdated = grouped.Key.DateUpdated,
                                   DetailCount = grouped.Count() > 0 ? grouped.Count() : 0,


                               }).OrderByDescending(x => x.DateUpdated).ToListAsync();
            foreach (var item in query) {
                item.EmployeeCount = await GetRosterGroupEmployeeCount(item.Id);
            }

            return query;


            //return query.OrderByDescending(x=> x.DateUpdated).ToListAsync();
        }


        private async Task<int?> GetRosterGroupEmployeeCount(int RostergroupId) { 

            var rosterIds =await   Context.Roster.AsNoTracking().Where(x=> x.RosterGroupId == RostergroupId && x.Active == 1).Select(x=> x.Id).ToListAsync();
           return await Context.Employee.AsNoTracking().Where(x=> rosterIds.Contains(x.RosterId.Value)).CountAsync();   
            
        }

    }

}
