using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DepartmentGroupConfigFeature.CreateDepartmentGroupConfig;
using tas.Application.Features.DepartmentGroupConfigFeature.DeleteDepartmentGroupConfig;
using tas.Application.Features.DepartmentGroupConfigFeature.GetDepartmentGroupConfig;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class DepartmentGroupConfigRepository : BaseRepository<DepartmentGroupConfig>, IDepartmentGroupConfigRepository
    {

        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public DepartmentGroupConfigRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;

        }


        public async Task CreateDepartmentGroupConfig(CreateDepartmentGroupConfigRequest request, CancellationToken cancellationToken)
        {
            if (request.EmployerIds.Count > 0)
            {
                foreach (var item in request.EmployerIds)
                {
                    var currentData = await Context.DepartmentGroupConfig
                                         .AsNoTracking().Where(x => 
                                                    x.GroupMasterId == request.GroupMasterId 
                                                    && x.GroupDetailId == request.GroupDetailId 
                                                    && x.DepartmentId == request.DepartmentId
                                                    && x.EmployerId == item)
                                         .FirstOrDefaultAsync();
                    if (currentData == null)
                    {
                        var newRecord = new DepartmentGroupConfig
                        {
                            DepartmentId = request.DepartmentId,
                            GroupMasterId = request.GroupMasterId,
                            GroupDetailId = request.GroupDetailId,
                            EmployerId = item,
                            Active = 1,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                        };

                        Context.DepartmentGroupConfig.Add(newRecord);

                    }
                }
            }
            else {
               var currentData =await Context.DepartmentGroupConfig
                    .AsNoTracking().Where(x => x.GroupMasterId == request.GroupMasterId && x.GroupDetailId == request.GroupDetailId && x.DepartmentId == request.DepartmentId)
                    .FirstOrDefaultAsync();
                if (currentData == null)
                {
                    var newRecord = new DepartmentGroupConfig
                    {
                        DepartmentId = request.DepartmentId,
                        GroupMasterId = request.GroupMasterId,
                        GroupDetailId = request.GroupDetailId,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                    };

                    Context.DepartmentGroupConfig.Add(newRecord);

                }
            }
        }



        public async Task DeleteDepartmentGroupConfig(DeleteDepartmentGroupConfigRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentGroupConfig.Where(x => request.Ids.Contains(x.Id)).ToListAsync();
            if (currentData != null)
            { 
                Context.DepartmentGroupConfig.RemoveRange(currentData);
            }

        }



        public async Task<List<GetDepartmentGroupConfigResponse>> GetDepartmentGroupConfig(GetDepartmentGroupConfigRequest request, CancellationToken cancellationToken)
        {
            var data = await (from departmentGroupData in Context.DepartmentGroupConfig.AsNoTracking().Where(x => x.DepartmentId == request.DepartentId)
                              join groupmaster in Context.GroupMaster.AsNoTracking() on departmentGroupData.GroupMasterId equals groupmaster.Id into groupMasterData
                              from groupmaster in groupMasterData.DefaultIfEmpty()
                              join groupdetail in Context.GroupDetail.AsNoTracking() on departmentGroupData.GroupDetailId equals groupdetail.Id into groupDetailData
                              from groupdetail in groupDetailData.DefaultIfEmpty()
                              join employer in Context.Employer.DefaultIfEmpty() on departmentGroupData.EmployerId equals employer.Id into employerData
                              from employer in employerData.DefaultIfEmpty()
                              select new GetDepartmentGroupConfigResponse
                              {
                                  Id =departmentGroupData.Id,
                                  GroupMasterName = groupmaster.Description,
                                  GroupMasterId = groupmaster.Id,
                                  GroupDetailName = groupdetail.Description,
                                  GroupDetailId = groupdetail.Id,
                                  EmployerName = employer.Description,
                                  EmployerId = employer.Id,

                              }).ToListAsync(cancellationToken);

            return data;
        }




    }

}
