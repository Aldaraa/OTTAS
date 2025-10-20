using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupMembersFeature.CreateGroupMembers;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Common;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class GroupMembersRepository : BaseRepository<GroupMembers>, IGroupMembersRepository
    {
        private readonly IConfiguration _configuration; 
        private readonly CacheService _cacheService;

        public GroupMembersRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, CacheService cacheService) : base(context, configuration, hTTPUserRepository)
        {
            _cacheService = cacheService;
        }
        public async Task SaveData(CreateGroupMembersRequest request, CancellationToken cancellationToken)
        {
            foreach (var item in request.GroupData)
            {
                var currentData =await Context.GroupMembers.FirstOrDefaultAsync(x => x.GroupMasterId == item.GroupMasterId && x.EmployeeId == request.EmployeeId);
                if (item.GroupDetailId == null)
                {
                    if (currentData != null) {
                        Context.GroupMembers.Remove(currentData);
                    }
                    
                }
                else {

                    if (currentData != null)
                    {
                        currentData.GroupMasterId = item.GroupMasterId;
                        currentData.GroupDetailId = item.GroupDetailId;
                        currentData.DateUpdated = DateTime.Now;
                        currentData.UserIdUpdated = 1;
                        Context.GroupMembers.Update(currentData);
                    }
                    else {
                        Context.GroupMembers.Add(
                            new GroupMembers
                            {
                                GroupDetailId = item.GroupDetailId,
                                GroupMasterId = item.GroupMasterId,
                                EmployeeId = request.EmployeeId,
                                DateCreated = DateTime.Now,
                                UserIdCreated = 1
                            });
                    }
                }
            }

           await GroupMasterCacheEnable();
        }



        private async Task GroupMasterCacheEnable()
        {
            string cacheEntityName = "GroupMasterAudit";
            var cacheKey = $"API::{cacheEntityName}";
            List<GroupMaster> outData;

            if (!_cacheService.TryGetValue(cacheKey, out outData))
            {
                var data = await Context.GroupMaster.AsNoTracking()
                    .Where(x => x.Active == 1)
                .ToListAsync();
                _cacheService.Set(cacheKey, data, TimeSpan.FromMinutes(GlobalConstants.ENDPOINT_MASTER_CACHE_MINUTE));
            }
        }


    }
}
