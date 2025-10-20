using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestGroupEmployeeFeature.AddRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetAllRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestGroupInpersonateUsers;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerAdminEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.GetRequestLineManagerEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.OrderRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.RemoveRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.RequestGroupActiveEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.SetPrimaryRequestGroupEmployees;
using tas.Application.Features.RequestGroupEmployeeFeature.UpdateRequestGroupEmployees;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestGroupEmployeeRepository : BaseRepository<RequestGroupEmployee>, IRequestGroupEmployeeRepository
    {
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly IRequestDocumentRepository _requestDocumentRepository;
        public RequestGroupEmployeeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, IRequestDocumentRepository requestDocumentRepository) : base(context, configuration, hTTPUserRepository)
        {
            _HTTPUserRepository = hTTPUserRepository;
            _requestDocumentRepository = requestDocumentRepository;
        }



        public async Task<List<GetAllRequestGroupEmployeesResponse>> GetAllGroupEmployees(GetAllRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var groupEmpoyees = await Context.SysRoleEmployees.AsNoTracking().ToListAsync();



            var returnData = new List<GetAllRequestGroupEmployeesResponse>();
            foreach (var item in groupEmpoyees)
            {
                if (item.EmployeeId.HasValue)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.EmployeeId.Value)
                        .Select(x => new { x.Id, x.Firstname, x.Lastname, x.Email }).FirstOrDefaultAsync();


                    //var taskCount =await Context.RequestDocument.AsNoTracking().Where(x => x.AssignedEmployeeId == item.EmployeeId
                    //&& (x.CurrentAction != RequestDocumentAction.Cancelled && x.CurrentAction != RequestDocumentAction.Completed)).CountAsync();


                    var newRecord = new GetAllRequestGroupEmployeesResponse
                    {
                        EmployeeId = currentEmployee.Id,
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                        Email = currentEmployee.Email,
                        TaskCount = null,
                        
                    };
                    returnData.Add(newRecord);
                }
            }

            return returnData.OrderByDescending(x => x.TaskCount).ToList();

        }




        public async Task<List<RequestGroupActiveEmployeesResponse>> GetActiveEmployees(RequestGroupActiveEmployeesRequest request, CancellationToken cancellationToken)
        {
            // TODO: JGa, 2023-11-27 Soyoloo, Ganbaa 
            //var groupEmpoyees =await Context.RequestGroupEmployee.Select(x =>  x.EmployeeId).ToListAsync();
            var activeEmployees =await Context.SysRoleEmployees.AsNoTracking()
                //.Where(x=> !groupEmpoyees.Contains(x.EmployeeId))
                .Select(x=> new { x.EmployeeId, x.RoleId }).ToListAsync();

            var systemRoles =await Context.SysRole.AsNoTracking().ToListAsync();

            var returnData = new List<RequestGroupActiveEmployeesResponse>();
            foreach (var item in activeEmployees)
            {
                if (item.EmployeeId.HasValue)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.EmployeeId.Value)
                        .Select(x => new { x.Id, x.Firstname, x.Lastname, x.Email }).FirstOrDefaultAsync();

                    
                    var newRecord = new RequestGroupActiveEmployeesResponse
                    {
                        EmployeeId = currentEmployee.Id,
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                        Email = currentEmployee.Email,
                        
                        RoleName = systemRoles.Where(x=> x.Id == item.RoleId).Select(x=> x.Name).FirstOrDefault()
                    };

                    returnData.Add(newRecord);
                }
            }

            return returnData;

        }


        public async Task<GetRequestGroupEmployeesResponse> GetGroupEmployees(GetRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var groupEmployees =await Context.RequestGroupEmployee.AsNoTracking()
                .Where(x => x.RequestGroupId == request.GroupId).ToListAsync();
            var returnData = new GetRequestGroupEmployeesResponse();
            var returnEmployeeData = new List<GetRequestGroupEmployees>();

            var currentGroup =await Context.RequestGroup.AsNoTracking().Where(x => x.Id == request.GroupId).FirstOrDefaultAsync();

            if (currentGroup != null)
            {
                returnData.Id = request.GroupId;
                returnData.Name = currentGroup.Description;
                returnData.GroupTag = currentGroup.GroupTag;
                foreach (var item in groupEmployees)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.EmployeeId.Value)
                        .Select(x => new { x.Id, x.Firstname, x.Lastname, x.ADAccount, x.SAPID, x.Email }).FirstOrDefaultAsync();
                    var newData = new GetRequestGroupEmployees
                    {
                        DisplayName = item.DisplayName,
                        Id = item.Id,
                        EmployeeId = item.EmployeeId.Value,
                        FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                        OrderIndex = item.OrderIndex,
                        PrimaryContact = item.PrimaryContact,
                        ADAccount = currentEmployee.ADAccount,
                        SAPID = currentEmployee.SAPID


                    };

                    returnEmployeeData.Add(newData);

                }

                returnData.Employees = returnEmployeeData;

            }

            return returnData;

        }


        public async Task<GetRequestLineManagerEmployeesResponse> GetLineManagerEmployees(GetRequestLineManagerEmployeesRequest request, CancellationToken cancellationToken)
        {

            var returnData = new GetRequestLineManagerEmployeesResponse();
            var returnEmployeeData = new List<GetRequestLineManagerEmployees>();



            var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "linemanager").FirstOrDefaultAsync();

            if (currentGroup != null)
            {
                var groupEmployees = await Context.RequestGroupEmployee.AsNoTracking()
                        .Where(x => x.RequestGroupId == currentGroup.Id).ToListAsync();

                if (currentGroup != null)
                {
                    returnData.Id = currentGroup.Id;
                    returnData.Name = currentGroup.Description;
                    foreach (var item in groupEmployees)
                    {
                        var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.EmployeeId.Value)
                            .Select(x => new { x.Id, x.Firstname, x.Lastname, x.SAPID, x.ADAccount, x.Email }).FirstOrDefaultAsync();
                        var newData = new GetRequestLineManagerEmployees
                        {
                            DisplayName = item.DisplayName,
                            Id = item.Id,
                            EmployeeId = item.EmployeeId.Value,
                            FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                            OrderIndex = item.OrderIndex,
                            SAPID = currentEmployee.SAPID,
                            ADAccount = currentEmployee.ADAccount,
                            PrimaryContact = item.PrimaryContact

                        };

                        returnEmployeeData.Add(newData);

                    }

                    returnData.Employees = returnEmployeeData;

                }


            }
            return returnData;

        }


        public async Task<List<GetRequestLineManagerAdminEmployeesResponse>> GetLineManagerAdminEmployees(GetRequestLineManagerAdminEmployeesRequest request, CancellationToken cancellationToken)
        {

            var returnData = new List<GetRequestLineManagerAdminEmployeesResponse>();

            var role = _HTTPUserRepository.LogCurrentUser()?.Role;
            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;
            if (role == "DepartmentAdmin" || role == "DepartmentManager")
            {
                 var empIds = await   _requestDocumentRepository.GetRoleEmployeeIds();
                var resultGroupIds = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "linemanager" || x.GroupTag == "administrator").Select(x => x.Id).ToListAsync();
                if (resultGroupIds.Count > 0)
                {
                    var groupEmployees = await Context.RequestGroupEmployee.AsNoTracking()
                            .Where(x => resultGroupIds.Contains(x.RequestGroupId) && empIds.Contains(x.EmployeeId.Value) || x.EmployeeId == userId ).ToListAsync();

                    if (resultGroupIds != null)
                    {
                        foreach (var item in groupEmployees)
                        {
                            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.EmployeeId.Value)
                                .Select(x => new { x.Id, x.Firstname, x.Lastname, x.SAPID, x.ADAccount, x.Email }).FirstOrDefaultAsync();
                            var newData = new GetRequestLineManagerAdminEmployeesResponse
                            {
                                DisplayName = item.DisplayName,
                                Id = item.Id,
                                EmployeeId = item.EmployeeId.Value,
                                FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                                OrderIndex = item.OrderIndex,
                                SAPID = currentEmployee.SAPID,
                                ADAccount = currentEmployee.ADAccount,
                                PrimaryContact = item.PrimaryContact

                            };



                            if (newData != null)
                            {

                                var oldata = returnData.Where(c => c.EmployeeId == newData.EmployeeId).FirstOrDefault();
                                if (oldata == null)
                                {
                                    returnData.Add(newData);
                                }

                            }


                        }

                    }


                }
                return returnData;
            }
            else {
                var resultGroupIds = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "linemanager" || x.GroupTag == "administrator").Select(x => x.Id).ToListAsync();

                if (resultGroupIds.Count > 0)
                {
                    var groupEmployees = await Context.RequestGroupEmployee.AsNoTracking()
                            .Where(x => resultGroupIds.Contains(x.RequestGroupId)).ToListAsync();

                    if (resultGroupIds != null)
                    {
                        foreach (var item in groupEmployees)
                        {
                            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.EmployeeId.Value)
                                .Select(x => new { x.Id, x.Firstname, x.Lastname, x.SAPID, x.ADAccount, x.Email }).FirstOrDefaultAsync();
                            var newData = new GetRequestLineManagerAdminEmployeesResponse
                            {
                                DisplayName = item.DisplayName,
                                Id = item.Id,
                                EmployeeId = item.EmployeeId.Value,
                                FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}",
                                OrderIndex = item.OrderIndex,
                                SAPID = currentEmployee.SAPID,
                                ADAccount = currentEmployee.ADAccount,
                                PrimaryContact = item.PrimaryContact

                            };



                            if (newData != null)
                            {

                                var oldata = returnData.Where(c => c.EmployeeId == newData.EmployeeId).FirstOrDefault();
                                if (oldata == null)
                                {
                                    returnData.Add(newData);
                                }

                            }


                        }

                    }


                }
                return returnData;


            }

        }




        public async Task AddGroupEmployees(AddRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == request.GroupId).FirstOrDefaultAsync();

            if (currentGroup != null)
            {
                if (currentGroup.ReadOnly != 1)
                {
                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
                    if (currentEmployee != null)
                    {
                        var currentGroupEmployeeData = await Context.RequestGroupEmployee.AsNoTracking()
                            .Where(x => x.RequestGroupId == request.GroupId && x.EmployeeId == request.EmployeeId)
                            .FirstOrDefaultAsync();

                        var currentGroupEmployeeCount = await Context.RequestGroupEmployee.AsNoTracking().CountAsync(x => x.RequestGroupId == request.GroupId);
                        if (currentGroupEmployeeData == null)
                        {
                            var newRecord = new RequestGroupEmployee
                            {


                                RequestGroupId = request.GroupId,
                                EmployeeId = request.EmployeeId,
                                DateCreated = DateTime.Now,
                                PrimaryContact = 0,
                                OrderIndex = currentGroupEmployeeCount + 1,
                                Active = 1,
                                DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? $"{currentEmployee.Firstname}" : request.DisplayName,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,

                            };

                            Context.RequestGroupEmployee.Add(newRecord);
                        }
                        else
                        {
                            throw new BadRequestException("Emloyee has already been registered.");
                        }

                    }
                }
            }
          
            
        }


        public async Task RemoveGroupEmployees(RemoveRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var deleteData =await Context.RequestGroupEmployee.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (deleteData != null)
            {
                Context.RequestGroupEmployee.Remove(deleteData);
            }

            await Task.CompletedTask;

                
        }


        public async Task UpdateGroupEmployees(UpdateRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var updateData = await Context.RequestGroupEmployee.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (updateData != null)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == updateData.EmployeeId)
                      .Select(x => new { x.Id, x.Firstname, x.Lastname, x.Email }).FirstOrDefaultAsync();

                if (currentEmployee != null)
                {
                    updateData.DisplayName = string.IsNullOrEmpty(request.DisplayName) ? $"{currentEmployee.Firstname}" : request.DisplayName;
                    Context.RequestGroupEmployee.Update(updateData);
                }


            }
            await Task.CompletedTask;


        }


        public async Task SetPrimaryGroupEmployees(SetPrimaryRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestGroupEmployee.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                var currentOtherData =await Context.RequestGroupEmployee
                    .Where(x => x.RequestGroupId == currentData.RequestGroupId).ToListAsync();

                foreach (var item in currentOtherData)
                {
                    if (request.Id == item.Id)
                    {
                        item.PrimaryContact = 1;
                    }
                    else {
                        item.PrimaryContact = 0;
                    }

                    Context.RequestGroupEmployee.Update(item);
                }

                await Task.CompletedTask;

                

            }
        }

        public async Task OrderGroupEmployees(OrderRequestGroupEmployeesRequest request, CancellationToken cancellationToken)
        {
            var groupEmployees = await Context.RequestGroupEmployee
           .Where(rd => rd.RequestGroupId == request.GroupId)
           .OrderBy(rd => rd.OrderIndex)
           .ToListAsync(cancellationToken);

            int RowNum = 1;
            foreach (int Id in request.Ids)
            {
                var currentData = groupEmployees.FirstOrDefault(x => x.Id == Id);
                if (currentData != null)
                {
                    currentData.OrderIndex = RowNum;
                    Context.RequestGroupEmployee.Update(currentData);
                }
                RowNum++;
            }
            await Task.CompletedTask;

        }


        #region Inpersonate


        public async Task<List<GetRequestGroupInpersonateUsersResponse>> GetRequestGroupInpersonateUsers(GetRequestGroupInpersonateUsersRequest request, CancellationToken cancellationToken)
        { 
            var returnData = new List<GetRequestGroupInpersonateUsersResponse>();



            var groupemployees =await (from gemployees in Context.RequestGroupEmployee
                                  join employee in Context.Employee on gemployees.EmployeeId equals employee.Id
                                  join rgroup in Context.RequestGroup on gemployees.RequestGroupId equals rgroup.Id
                                  select new 
                                  {
                                    id = gemployees.Id,
                                    displayname = gemployees.DisplayName,
                                    employeeId = gemployees.EmployeeId,
                                    lastname = employee.Lastname,
                                    firstname = employee.Firstname,
                                    email = employee.Email,
                                    groupname = rgroup.Description,
                                    SAPID = employee.SAPID,
                                    ADAccount = employee.ADAccount
                                  }).ToListAsync();

            foreach (var item in groupemployees)
            {
                var newRecord = new GetRequestGroupInpersonateUsersResponse
                {
                    Id = item.id,
                    displayName = item.displayname,
                    fullName = $"{item.firstname} {item.lastname} ",
                    employeeId = item.employeeId,
                    email = item.email,
                    groupName = item.groupname,
                    SAPID = item.SAPID,
                    ADAccount = item.ADAccount

                };

                returnData.Add(newRecord);
            }



            return returnData;
        }


        #endregion

    }
}
