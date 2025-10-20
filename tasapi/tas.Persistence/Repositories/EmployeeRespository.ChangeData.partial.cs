using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeFeature.ActiveEmployeeDirectRequest;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeData;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeDataGroup;
using tas.Application.Features.EmployeeFeature.ChangeEmployeeLocation;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class EmployeeRepository
    {

        public async Task ChangeEmployeeData(ChangeEmployeeDataRequest request, CancellationToken cancellationToken)
        {
            if (request.changeDataType == ChangeEmployeeDataType.CostCode)
            {
                await ChangeEmployeeCostCodeData(request, cancellationToken);
            }
            if (request.changeDataType == ChangeEmployeeDataType.Department)
            {
                await ChangeEmployeeDepartmentData(request, cancellationToken);
            }
            if (request.changeDataType == ChangeEmployeeDataType.Employer)
            {
                await ChangeEmployeeEmployerData(request, cancellationToken);
            }
            if (request.changeDataType == ChangeEmployeeDataType.Position)
            {
                await ChangeEmployeePositionData(request, cancellationToken);
            }
            else {
                return;
            }
        }


        #region ChangeCostCode

        private async Task ChangeEmployeeCostCodeData(ChangeEmployeeDataRequest requestData, CancellationToken cancellationToken)
        {

            foreach (var request in requestData.data)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == request.employeeId).FirstOrDefaultAsync(cancellationToken);
                if (currentEmployee != null)
                {
                    var currentCostCode = await Context.CostCodes.Where(x => x.Id == request.DataId).FirstOrDefaultAsync(cancellationToken);
                    if (currentCostCode != null)
                    {

                        if (request.profileUpdate)
                        {
                            var EmployeeTransport = await Context.Transport
                            .Where(x => x.EmployeeId == request.employeeId
                            && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);
                            EmployeeTransport.ForEach(x => x.CostCodeId = request.DataId);

                            var EmployeeStatus = await Context.EmployeeStatus
                                .Where(x => x.EmployeeId == request.employeeId
                                && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);



                            currentEmployee.CostCodeId = request.DataId;

                            foreach (var item in EmployeeStatus)
                            {
                                item.CostCodeId = request.DataId;
                                item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                item.DateUpdated = DateTime.Now;
                                item.ChangeRoute = "Changes on Costcode";

                                Context.EmployeeStatus.Update(item);
                            }

                            Context.Employee.Update(currentEmployee);

                            string cacheEntityName = $"Employee_{currentEmployee.Id}";
                            _memoryCache.Remove($"API::{cacheEntityName}");
                        }
                        else
                        {

                            if (request.endDate.HasValue)
                            {
                                var EmployeeTransport = await Context.Transport
                                .Where(x => x.EmployeeId == request.employeeId
                                && x.EventDate.Value.Date >= request.startDate
                                && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);
                                EmployeeTransport.ForEach(x => x.CostCodeId = request.DataId);

                                var EmployeeStatus = await Context.EmployeeStatus
                                    .Where(x => x.EmployeeId == request.employeeId
                                    && x.EventDate.Value.Date >= request.startDate
                                    && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);

                                foreach (var item in EmployeeStatus)
                                {
                                    item.CostCodeId = request.DataId;
                                    item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    item.DateUpdated = DateTime.Now;
                                    item.ChangeRoute = "Changes on Costcode";

                                    Context.EmployeeStatus.Update(item);
                                }
                            }
                            else
                            {
                                throw new BadRequestException("If Permanent unchecked, select the enddate required");
                            }

                        }
                    }
                }
            }

         

            await Task.CompletedTask;
        }


        #endregion


        #region ChangeDepartment

        private async Task ChangeEmployeeDepartmentData(ChangeEmployeeDataRequest requestData, CancellationToken cancellationToken)
        {

            foreach (var request in requestData.data)
            {

                var currentEmployee = await Context.Employee.Where(x => x.Id == request.employeeId).FirstOrDefaultAsync(cancellationToken);
                if (currentEmployee != null)
                {
                    var currentDepartment = await Context.Department.Where(x => x.Id == request.DataId).FirstOrDefaultAsync(cancellationToken);
                    if (currentDepartment != null)
                    {

                        if (request.profileUpdate)
                        {
                            var EmployeeTransport = await Context.Transport
                              .Where(x => x.EmployeeId == request.employeeId
                              && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);
                            EmployeeTransport.ForEach(x => x.DepId = request.DataId);

                            var EmployeeStatus = await Context.EmployeeStatus
                                .Where(x => x.EmployeeId == request.employeeId
                                && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);
                            foreach (var item in EmployeeStatus)
                            {
                                item.DepId = request.DataId;
                                item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                item.DateUpdated = DateTime.Now;
                                item.ChangeRoute = "Changes on Department";

                                Context.EmployeeStatus.Update(item);
                            }

                            currentEmployee.DepartmentId = request.DataId;
                            Context.Employee.Update(currentEmployee);
                            string cacheEntityName = $"Employee_{currentEmployee.Id}";
                            _memoryCache.Remove($"API::{cacheEntityName}");
                        }
                        else
                        {
                            if (request.endDate.HasValue)
                            {
                                var EmployeeTransport = await Context.Transport
                                  .Where(x => x.EmployeeId == request.employeeId
                                  && x.EventDate.Value.Date >= request.startDate
                                  && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);
                                EmployeeTransport.ForEach(x => x.DepId = request.DataId);

                                var EmployeeStatus = await Context.EmployeeStatus
                                    .Where(x => x.EmployeeId == request.employeeId
                                    && x.EventDate.Value.Date >= request.startDate
                                    && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);

                                foreach (var item in EmployeeStatus)
                                {
                                    item.DepId = request.DataId;
                                    item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    item.DateUpdated = DateTime.Now;
                                    item.ChangeRoute = "Changes on Department";

                                    Context.EmployeeStatus.Update(item);
                                }
                            }
                            else
                            {
                                throw new BadRequestException("If Permanent unchecked, select the enddate required");
                            }

                        }
                    }
                }

            }

            await Task.CompletedTask;
        }


        #endregion


        #region ChangeGroupData

        public async Task ChangeEmployeeDataGroup(ChangeEmployeeDataGroupRequest request, CancellationToken cancellationToken)
        {
            foreach (var empId in request.EmpIds)
            {

              var CurrentData = await  Context.Employee.Where(x => x.Id == empId).FirstOrDefaultAsync();
                if (CurrentData != null)
                {
                    var currentGroupData = await Context.GroupMembers.Where(x => x.EmployeeId == empId && x.GroupMasterId == request.GroupMasterId).FirstOrDefaultAsync();
                    if (currentGroupData != null)
                    {
                        currentGroupData.GroupDetailId = request.GroupDetailId;
                        currentGroupData.DateUpdated = DateTime.Now;
                        currentGroupData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        Context.GroupMembers.Update(currentGroupData);

                        string cacheEntityName = $"Employee_{empId}";
                        _memoryCache.Remove($"API::{cacheEntityName}");
                    }
                    else {
                        var newRecord = new GroupMembers
                        {
                            DateCreated = DateTime.Now,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            GroupDetailId = request.GroupDetailId,
                            GroupMasterId = request.GroupMasterId,
                            EmployeeId = empId
                            
                        };

                        Context.GroupMembers.Add(newRecord);
                        string cacheEntityName = $"Employee_{empId}";
                        _memoryCache.Remove($"API::{cacheEntityName}");
                    }
                }

            }
        }


        #endregion


        #region ChangeEmployer

        private async Task ChangeEmployeeEmployerData(ChangeEmployeeDataRequest requestData, CancellationToken cancellationToken)
        {
            foreach (var request in requestData.data)
            {

                var currentEmployee = await Context.Employee.Where(x => x.Id == request.employeeId).FirstOrDefaultAsync(cancellationToken);
                if (currentEmployee != null)
                {
                    var currentEmployer = await Context.Employer.Where(x => x.Id == request.DataId).FirstOrDefaultAsync(cancellationToken);
                    if (currentEmployer != null)
                    {
                        if (request.profileUpdate)
                        {


                            var EmployeeTransport = await Context.Transport
                            .Where(x => x.EmployeeId == request.employeeId
                            && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);
                            EmployeeTransport.ForEach(x => x.EmployerId = request.DataId);

                            var EmployeeStatus = await Context.EmployeeStatus
                                .Where(x => x.EmployeeId == request.employeeId
                                && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);


                            foreach (var item in EmployeeStatus)
                            {
                                item.EmployerId = request.DataId;
                                item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                item.DateUpdated = DateTime.Now;
                                item.ChangeRoute = "Changes on Employer";

                                Context.EmployeeStatus.Update(item);
                            }
                            currentEmployee.EmployerId = request.DataId;
                            Context.Employee.Update(currentEmployee);
                            string cacheEntityName = $"Employee_{currentEmployee.Id}";
                            _memoryCache.Remove($"API::{cacheEntityName}");
                        }
                        else
                        {

                            if (request.endDate.HasValue)
                            {
                                var EmployeeTransport = await Context.Transport
                            .Where(x => x.EmployeeId == request.employeeId
                            && x.EventDate.Value.Date >= request.startDate
                            && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);
                                EmployeeTransport.ForEach(x => x.EmployerId = request.DataId);

                                var EmployeeStatus = await Context.EmployeeStatus
                                    .Where(x => x.EmployeeId == request.employeeId
                                    && x.EventDate.Value.Date >= request.startDate
                                    && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);
                                foreach (var item in EmployeeStatus)
                                {
                                    item.EmployerId = request.DataId;
                                    item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    item.DateUpdated = DateTime.Now;
                                    item.ChangeRoute = "Changes on Employer";

                                    Context.EmployeeStatus.Update(item);
                                }
                            }
                            else
                            {
                                throw new BadRequestException("If Permanent unchecked, select the enddate required");
                            }
                        }
                    }
                }

            }

            await Task.CompletedTask;
        }


        #endregion


        #region ChangePosition

        private async Task ChangeEmployeePositionData(ChangeEmployeeDataRequest requestData, CancellationToken cancellationToken)
        {

            foreach (var request in requestData.data)
            {

                var currentEmployee = await Context.Employee.Where(x => x.Id == request.employeeId).FirstOrDefaultAsync(cancellationToken);
                if (currentEmployee != null)
                {
                    var currentPosition = await Context.Position.Where(x => x.Id == request.DataId).FirstOrDefaultAsync(cancellationToken);
                    if (currentPosition != null)
                    {

                        if (request.profileUpdate)
                        {


                            var EmployeeTransport = await Context.Transport
                            .Where(x => x.EmployeeId == request.employeeId
                            && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);
                            EmployeeTransport.ForEach(x => x.PositionId = request.DataId);

                            var EmployeeStatus = await Context.EmployeeStatus
                                .Where(x => x.EmployeeId == request.employeeId
                                && x.EventDate.Value.Date >= request.startDate).ToListAsync(cancellationToken);
                            foreach (var item in EmployeeStatus)
                            {
                                item.PositionId = request.DataId;
                                item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                item.DateUpdated = DateTime.Now;
                                item.ChangeRoute = "Changes on Employer";

                                Context.EmployeeStatus.Update(item);
                            }


                            currentEmployee.PositionId = request.DataId;
                            Context.Employee.Update(currentEmployee);
                        }
                        else
                        {

                            if (request.endDate.HasValue)
                            {
                                var EmployeeTransport = await Context.Transport
                                    .Where(x => x.EmployeeId == request.employeeId
                                    && x.EventDate.Value.Date >= request.startDate
                                    && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);
                                EmployeeTransport.ForEach(x => x.PositionId = request.DataId);

                                var EmployeeStatus = await Context.EmployeeStatus
                                    .Where(x => x.EmployeeId == request.employeeId
                                    && x.EventDate.Value.Date >= request.startDate
                                    && x.EventDate.Value.Date <= request.endDate).ToListAsync(cancellationToken);
                                foreach (var item in EmployeeStatus)
                                {
                                    item.PositionId = request.DataId;
                                    item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    item.DateUpdated = DateTime.Now;
                                    item.ChangeRoute = "Changes on Position";

                                    Context.EmployeeStatus.Update(item);
                                }
                            }
                            else
                            {
                                throw new BadRequestException("If Permanent unchecked, select the enddate required");
                            }

                        }

                    }
                }
            }

            await Task.CompletedTask;
        }


        #endregion


        public async Task UpdateProfileChangeData(Employee employee)
        {
            var currentEmployee = await Context.Employee
                .Where(x => x.Id == employee.Id).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {

                int? costCodeId = currentEmployee.CostCodeId;
                int? departmentId = currentEmployee.DepartmentId;
                int? employerId = currentEmployee.EmployerId;
                int? positionId = currentEmployee.PositionId;
                await ChangeVisitEmployee(employee.Id);
                var employeeStatusFuturePlan = await Context.EmployeeStatus
                    .Where(x => x.EmployeeId == employee.Id && x.EventDate.Value.Date >= DateTime.Today)
                    .ToListAsync();

                var employeeTransportFuturePlan = await
                    Context.Transport.Where(x => x.EmployeeId == employee.Id && x.EventDate.Value.Date >= DateTime.Today)
                    .ToListAsync();
                foreach (var item in employeeStatusFuturePlan)
                {
                        item.CostCodeId = employee.CostCodeId;
                        item.DepId = employee.DepartmentId;
                        item.EmployerId = employee.EmployerId;
                        item.PositionId = employee.PositionId;
                        Context.EmployeeStatus.Update(item);
                }


                foreach (var item in employeeTransportFuturePlan)
                {
                    item.CostCodeId = employee.CostCodeId;
                    item.DepId = employee.DepartmentId;
                    item.EmployerId = employee.EmployerId;
                    item.PositionId = employee.PositionId;
                    Context.Transport.Update(item);
                }
            }



          //  await Context.SaveChangesAsync();
            await Task.CompletedTask;
        }

        private async Task ChangeVisitEmployee(int empId)
        {

            var currentEmployee =await Context.Employee.Where(x => x.Id == empId && x.Active == 2).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                currentEmployee.Active = 1;
                currentEmployee.CompletionDate = DateTime.Now;    
                Context.Employee.Update(currentEmployee);
            }

           
        }


        #region ActiveRequestDirect
        public async Task ActiveEmployeeRequestDirect(ActiveEmployeeDirectRequest request, CancellationToken cancellationToken) 
        {
            var currentEmployee = await Context.Employee.Where(c => c.Id == request.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                if (currentEmployee.Active == 0)
                {
                    currentEmployee.Active = 1;
                    currentEmployee.CommenceDate = DateTime.Today;
                    currentEmployee.DateUpdated = DateTime.Now;
                    currentEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    Context.Employee.Update(currentEmployee);

                }
                else {
                    throw new BadRequestException("This employee cannot be activated");
                }

            }
            else {
                throw new BadRequestException("Employee not found");
            
            }
        }

        #endregion



        #region ChangeEmployeeLocation

        public async Task ChangeEmployeeLocation(ChangeEmployeeLocationRequest request, CancellationToken cancellationToken)
        {
            foreach (var item in request.data)
            {
                var itemEmployee = await Context.Employee.Where(x => x.Id == item.employeeId).FirstOrDefaultAsync();
                if (itemEmployee != null)
                {
                    itemEmployee.LocationId = item.LocationId;
                    itemEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    itemEmployee.DateUpdated = DateTime.Now;
                    Context.Employee.Update(itemEmployee);
                }

            }
        }


        #endregion


        #region DeActive on Delete more data


        public async Task DeActiveEmployeeDelete(int EmployeeId, CancellationToken cancellationToken)
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

            var groupemployees = await Context.RequestGroupEmployee.Where(x => x.EmployeeId == EmployeeId).ToListAsync();
            foreach (var groupemployee in groupemployees)
            {
                Context.RequestGroupEmployee.Remove(groupemployee);
            }
            var departmentsupervisors = await Context.DepartmentSupervisor.Where(x => x.EmployeeId == EmployeeId).ToListAsync();
            foreach (var supervisor in departmentsupervisors)
            {
                Context.DepartmentSupervisor.Remove(supervisor);
            }

            var futureRoomBooking = await Context.EmployeeStatus
                .Where(x => x.EmployeeId == EmployeeId && x.EventDate.Value.Date >= DateTime.Today)
                .ToListAsync();

            foreach (var booking in futureRoomBooking)
            {
                booking.ChangeRoute = "DeActive Employee";
                Context.EmployeeStatus.Remove(booking);
            }


            var futureTransport = await Context.Transport
                .Where(x => x.EmployeeId == EmployeeId
                && x.EventDate.Value.Date > DateTime.Today)
                .ToListAsync();
            foreach (var transport in futureTransport)
            {
                transport.ChangeRoute = "DeActive Employee";
                Context.Transport.Remove(transport);
            }


            var requestLineManagers = await Context.RequestLineManagerEmployee
                .Where(x => x.LineManagerId == EmployeeId)
                .ToListAsync();
            foreach (var data in requestLineManagers)
            {
                Context.RequestLineManagerEmployee.Remove(data);
            }


            var requestLineManagerEmployees = await Context.RequestLineManagerEmployee
            .Where(x => x.EmployeeId == EmployeeId)
            .ToListAsync();
            foreach (var data in requestLineManagerEmployees)
            {
                Context.RequestLineManagerEmployee.Remove(data);
            }


            var requestDelegatesFrom = await Context.RequestDelegates
            .Where(x => x.FromEmployeeId == EmployeeId)
            .ToListAsync();
            foreach (var data in requestDelegatesFrom)
            {
                Context.RequestDelegates.Remove(data);
            }


            var employeeMenu = await Context.SysRoleEmployeeMenu.Where(x => x.EmployeeId == EmployeeId).ToListAsync();

            if (employeeMenu.Count > 0)
            {
                Context.SysRoleEmployeeMenu.RemoveRange(employeeMenu);
            }



        }



        #endregion
    }
}
