using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using tas.Application.Common.Exceptions;
using tas.Application.Features.SysTeamFeature.DeleteUserSysTeam;
using tas.Application.Features.SysTeamFeature.GetAllSysTeam;
using tas.Application.Features.SysTeamFeature.GetSysTeam;
using tas.Application.Features.SysTeamFeature.SetMenuSysTeam;
using tas.Application.Features.SysTeamFeature.SetUserSysTeam;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class SysTeamRepository : BaseRepository<SysTeam>, ISysTeamRepository
    {
        public SysTeamRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }

        public async Task<List<GetAllSysTeamResponse>> GetAllSysTeam(CancellationToken cancellationToken)
        {

            var result = await (from team in Context.SysTeam
                          select new GetAllSysTeamResponse
                          {
                              Id = team.Id,
                              Name = team.Name,
                          }).ToListAsync();
            return result;

        }


        public async Task<GetSysTeamResponse> GetSysTeamProfile(int teamId, CancellationToken cancellationToken)
        {

            return null;
            //var menuQuery = from sm in Context.SysMenu
            //            join sa in Context.SysApplication on sm.ApplicationId equals sa.Id
            //            from tt in Context.SysTeamMenu
            //                         .Where(stm => stm.TeamId == teamId && stm.MenuId == sm.Id)
            //                         .DefaultIfEmpty()
            //            orderby sa.OrderIndex, sm.OrderIndex
            //            select new TeamMenu
            //            {
            //                Id = sm.Id,
            //                Name = sm.Name,
            //                Route = sm.Route,
            //                ApplicationName = sa.Name,
            //                Permission = tt != null ? tt.Active : 0
            //            };

            //var menuResult = await menuQuery.ToListAsync(cancellationToken);

            //var employeeQuery = from stu in Context.SysTeamUser
            //            join e in Context.Employee on stu.EmployeeId equals e.Id
            //            join d in Context.Department on e.DepartmentId equals d.Id
            //            where stu.TeamId == teamId
            //            orderby e.Firstname
            //            select new TeamUser
            //            {
            //                Id = e.Id,
            //                lastname = e.Lastname,
            //                Firstname =  e.Firstname,
            //                NRN = e.NRN,
            //                DepartmentName = d.Name
            //            };
            //var employeeResult = await employeeQuery.ToListAsync(cancellationToken);


            //var returnData = await (from sysTeam in Context.SysTeam
            //                        where sysTeam.Id == teamId
            //                        select new GetSysTeamResponse
            //                        {
            //                            Id = sysTeam.Id,
            //                            Name = sysTeam.Name,
            //                            TeamMenus = menuResult,
            //                            TeamUsers = employeeResult
            //                        }).FirstOrDefaultAsync(cancellationToken);
            //return returnData;
        }
        public async Task SetMenuSysTeam(SetMenuSysTeamBulkRequest requests, CancellationToken cancellationToken) 
        {
            foreach (var request in requests.Requests)
            {
                //int TeamId, int MenuId, bool Permission
                // Find the existing record in the SysTeamMenu table
                var existingRecord = await Context.SysTeamMenu
                    .FirstOrDefaultAsync(x => x.TeamId == request.TeamId && x.MenuId == request.MenuId, cancellationToken);

                if (existingRecord != null)
                {
                    existingRecord.Active = Convert.ToInt32(request.Permission);
                }
                else
                {
                    // Create a new record if it doesn't exist
                    var newRecord = new SysTeamMenu
                    {
                        TeamId = request.TeamId,
                        MenuId = request.MenuId,
                        Active =Convert.ToInt32(request.Permission)
                    };
                    Context.SysTeamMenu.Add(newRecord);
                }
            }
          //  await Context.SaveChangesAsync(cancellationToken);
        }


        public async  Task SetUserSysTeam(SetUserSysTeamRequest request, CancellationToken cancellationToken)
        {
            var existingRecord = await Context.SysTeamUser
                .FirstOrDefaultAsync(x => x.TeamId == request.TeamId && x.EmployeeId == request.UserId, cancellationToken);

            if (existingRecord == null)
            {
                var newRecord = new SysTeamUser
                {
                    TeamId = request.TeamId,
                    EmployeeId = request.UserId,
                    Active = 1
                };
                Context.SysTeamUser.Add(newRecord);
            }
            else {
                throw new BadRequestException("Employee already added!");
            }

            
        }

        public async Task DeleteUserSysTeam(DeleteUserSysTeamRequest request, CancellationToken cancellationToken)
        {
            var deleteRecord = await Context.SysTeamUser
                .FirstOrDefaultAsync(x => x.TeamId == request.TeamId && x.EmployeeId == request.UserId, cancellationToken);

            if (deleteRecord != null)
            {
                Context.Remove(deleteRecord);
            }
            else
            {
                throw new BadRequestException("Record not found!");
            }
        }
    }
}
