using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{


    public class JoblogRepository : BaseRepository<Joblog>, IJoblogRepository
    {
        public JoblogRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        #region DeleteEmployee 



        public async Task ExeceuteEmployeeStatusJob(CancellationToken cancellationToken)
        {
            await DeActiveExecute();
            await ReActiveExecute();
            var newLog = new Joblog
            {
                Active = 1,
                DateCreated = DateTime.Now,
                Description = "TAS SYSTEM JOB EXECUTED --EMPLOYEE DEACTIVE AND REACTIVE",

            };
            Context.Joblog.Add(newLog);
            await Context.SaveChangesAsync();
            await Task.CompletedTask;
        }


 

        private async Task ReActiveExecute()
        {
            var data = await Context.StatusChangesEmployeeRequest
                .Where(x => x.EventDate.Value.Date <= DateTime.Today && x.StatusType == "REACTIVE").ToListAsync();
            foreach (var item in data)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId && x.Active != 1).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    currentEmployee.Active = 1;
                    currentEmployee.CommenceDate = item?.EventDate;
                    var empHisDat = new EmployeeHistory
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        EventDate = DateTime.Now,
                        UserIdCreated = item.UserIdCreated,
                        EmployeeId = item.EmployeeId,
                        Action = "Join"
                    };
                    Context.EmployeeHistory.Add(empHisDat);
                    Context.Employee.Update(currentEmployee);
                    Context.StatusChangesEmployeeRequest.Remove(item);
                }
            }
            await Task.CompletedTask;
        }


        private async Task DeActiveExecute()
        {
            var data = await Context.StatusChangesEmployeeRequest
                .Where(x => x.EventDate.Value.Date <= DateTime.Today && x.StatusType == "DEACTIVE").ToListAsync();
            foreach (var item in data)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId && x.Active == 1).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    var complenceDataCheck = await Context.EmployeeStatus
                        .Where(x => x.EmployeeId == item.EmployeeId && x.EventDate.Value.Date == DateTime.Today && x.RoomId != null)
                        .FirstOrDefaultAsync();
                    if (complenceDataCheck == null)
                    {

                        await DeleteMoreData(item.EmployeeId);
                        currentEmployee.Active = 0;
                        currentEmployee.RoomId = null;
                        var empHisDat = new EmployeeHistory
                        {
                            Comment = item.Comment,
                            Active = 1,
                            DateCreated = DateTime.Now,
                            EventDate = DateTime.Today,
                            TerminationTypeId = item.TerminationTypeId,
                            UserIdCreated = item.UserIdCreated,
                            EmployeeId = item.EmployeeId,
                            Action = "Termination"
                        };

                        Context.EmployeeHistory.Add(empHisDat);

                        Context.Employee.Update(currentEmployee);
                        Context.StatusChangesEmployeeRequest.Remove(item);
                    }

                }
            }
            await Task.CompletedTask;
        }


        private async Task DeleteMoreData(int EmployeeId)
        {

            var empRoles = await Context.SysRoleEmployees
          .Where(x => x.EmployeeId == EmployeeId).ToListAsync();
            foreach (var role in empRoles)
            {
                var depAdmins = await Context.DepartmentAdmin
                    .Where(x => x.EmployeeId == role.EmployeeId).ToListAsync();
                foreach (var admin in depAdmins)
                {
                    Context.DepartmentAdmin.Remove(admin);
                }

                var depManagers = await Context.DepartmentManager
                    .Where(x => x.EmployeeId == role.EmployeeId).ToListAsync();
                foreach (var manager in depManagers)
                {
                    Context.DepartmentManager.Remove(manager);
                }
                Context.SysRoleEmployees.Remove(role);
            }
            var empTeams = await Context.SysTeamUser
                .Where(x => x.EmployeeId == EmployeeId).ToListAsync();
            foreach (var team in empTeams)
            {
                Context.SysTeamUser.Remove(team);
            }

            var futureRoomBooking = await Context.EmployeeStatus
                .Where(x => x.EmployeeId == EmployeeId && x.EventDate.Value.Date >= DateTime.Today)
                .ToListAsync();

            foreach (var booking in futureRoomBooking)
            {
                booking.ChangeRoute = "DeActive delete data";
                Context.EmployeeStatus.Remove(booking);
            }


            var futureTransport = await Context.Transport
                .Where(x => x.EmployeeId == EmployeeId
                && x.EventDate.Value.Date >= DateTime.Today)
                .ToListAsync();
            foreach (var transport in futureTransport)
            {
                transport.ChangeRoute = "DeActive delete data";
                Context.Transport.Remove(transport);
            }


            await Task.CompletedTask;
        }


        #endregion


        #region RequestDelegationDelete

        public async Task ExecuteRequestDelegationJob(CancellationToken cancellationToken)
        {

            var deleteDelegates = await Context.RequestDelegates
                    .Where(x => x.EndDate.Date <= DateTime.Today).ToListAsync();
            foreach (var item in deleteDelegates)
            {
               Context.RequestDelegates.Remove(item);
            }

            var newLog = new Joblog
            {
                Active = 1,
                DateCreated = DateTime.Now,
                Description = "TAS SYSTEM JOB EXECUTED --REQUEST DELEGATION PROCESS",

            };
            Context.Joblog.Add(newLog);

            await Context.SaveChangesAsync();
            await Task.CompletedTask;
        }


        #endregion




    }
}
