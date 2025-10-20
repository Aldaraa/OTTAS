using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupMasterFeature.GetAllGroupMaster;
using tas.Application.Features.GroupMasterFeature.GetProfileGroupMaster;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class GroupMasterRepository : BaseRepository<GroupMaster>, IGroupMasterRepository
    {
        public GroupMasterRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }

        public async Task ChangeOrderBy(List<int> GroupMasterIds, CancellationToken cancellationToken)
        {
            var rowNum = 1;
            foreach (int Id in GroupMasterIds)
            {
              var currentData =  await Context.GroupMaster.FirstOrDefaultAsync(x=> x.Id == Id);
                if (currentData != null)
                {
                    currentData.OrderBy = rowNum;
                    Context.GroupMaster.Update(currentData);
                }


               rowNum++;
            }

             await Task.CompletedTask;
        }



        public async Task<GroupMaster> GetbyId(int id, CancellationToken cancellationToken)
        {
            return await Context.GroupMaster.Where(x=> x.Id == id).FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<List<GetAllGroupMasterResponse>> GetAllData(GetAllGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var masters = new List<GroupMaster>();
            if (request.status.HasValue)
            {
                masters =await Context.GroupMaster.Where(x=> x.Active == request.status.Value).ToListAsync();
            }
            else {
                masters =await Context.GroupMaster.ToListAsync();
            }

         var returnData = masters.Select(x => new GetAllGroupMasterResponse
            {
                Id = x.Id,
                Description = x.Description,
                Active = x.Active,
                ShowOnProfile = x.ShowOnProfile,
                CreateLog = x.CreateLog,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                OrderBy = x.OrderBy,
                Required = x.Required,
                DetailCount = Context.GroupDetail.Count(d => d.GroupMasterId == x.Id)
            }).OrderBy(x=> x.OrderBy).OrderByDescending(x => x.DateUpdated).ToList();

            return await Task.FromResult(returnData);
        }



        public async Task<List<GetProfileGroupMasterResponse>> ProfileData(GetProfileGroupMasterRequest request, CancellationToken cancellationToken)
        {
            var masterData =await Context.GroupMaster.Where(x => x.ShowOnProfile == 1 && x.Active == 1).OrderBy(x=> x.OrderBy).ToListAsync(cancellationToken);
            var returnData = new List<GetProfileGroupMasterResponse>();
            foreach (var item in masterData)
            {
              var itemData =  new GetProfileGroupMasterResponse
                {
                    Id = item.Id,
                    Description = item.Description,
                    details = await GetMasterDetail(item.Id, cancellationToken),
                    CreateLog = item.CreateLog,
                    Required = item.Required
                };

                returnData.Add(itemData);
            }

            return returnData;
        }

        private async Task<List<GetProfileGroupMasterDetail>> GetMasterDetail(int masterId, CancellationToken cancellationToken)
        {
            List<GetProfileGroupMasterDetail> result = await Context.GroupDetail
                .Where(x => x.GroupMasterId == masterId)
                .Select(x => new GetProfileGroupMasterDetail
                {
                    Id = x.Id,
                    Description = x.Description,
                    Code = x.Code
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
