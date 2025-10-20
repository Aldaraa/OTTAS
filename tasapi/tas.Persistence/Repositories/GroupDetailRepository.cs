using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.GetAllGroupDetail;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class GroupDetailRepository : BaseRepository<GroupDetail>, IGroupDetailRepository
    {
        public GroupDetailRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public async Task<GetAllGroupDetailResponse> GetAllByGroupMasterId(int? active, int groupMasterId, CancellationToken cancellationToken)
        {
            var data = new List<GroupDetail>();
            if (active.HasValue && active.Value > 0)
            {
                data =await Context.GroupDetail.Where(x => x.GroupMasterId == groupMasterId && x.Active == active).OrderByDescending(x => x.isDefault).ToListAsync(cancellationToken);
            }
            else {
                data = await Context.GroupDetail.Where(x => x.GroupMasterId == groupMasterId).OrderByDescending(x=> x.isDefault).ToListAsync(cancellationToken);
            }




            var masterData =await Context.GroupMaster.Where(x => x.Id == groupMasterId).Select(x => new { x.Id, x.Description }).FirstOrDefaultAsync();
            if (masterData != null)
            {
                var returnDataDetail = new List<GetAllGroupDetail>();



                foreach (var item in data)
                {
                    var newData = new GetAllGroupDetail
                    {
                        Id = item.Id,
                        Active = item.Active,
                        Code = item.Code,
                        isDefault = item.isDefault,
                        Description = item.Description,
                        DateCreated = item.DateCreated,
                        EmployeeCount = Context.GroupMembers.Count(x => x.GroupDetailId == item.Id),
                        DateUpdated = item.DateUpdated,
                        GroupMasterId = item.GroupMasterId
                    };

                    returnDataDetail.Add(newData);
                }

                var returnData = new GetAllGroupDetailResponse
                {
                    Id = masterData.Id,
                    Name = masterData.Description,
                    details = returnDataDetail.OrderByDescending(x=> x.DateUpdated).ToList()
                };

                return returnData;

            }
            else
            {
                return new GetAllGroupDetailResponse();
            }


        }
    }
}
