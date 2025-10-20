using AutoMapper.Execution;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using tas.Application.Features.RequestGroupConfigFeature.GetRequestDocumentType;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroup;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupAdd;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupById;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupByType;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupEmpLines;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupOrder;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupRemove;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentGroupUpdate;
using tas.Application.Features.RequestGroupConfigFeature.RequestDocumentRoute;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestGroupConfigRepository : BaseRepository<RequestGroupConfig>, IRequestGroupConfigRepository
    {
        public readonly HTTPUserRepository _hTTPUserRepository;
        public RequestGroupConfigRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _hTTPUserRepository = hTTPUserRepository;
        }

        public async Task<List<GetRequestDocumentTypeResponse>> GetAllDocuments(GetRequestDocumentTypeRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<GetRequestDocumentTypeResponse>();
            returnData.Add(new GetRequestDocumentTypeResponse { Name = RequestDocumentType.NonSiteTravel });
            returnData.Add(new GetRequestDocumentTypeResponse { Name = RequestDocumentType.ProfileChanges });
            returnData.Add(new GetRequestDocumentTypeResponse { Name = RequestDocumentType.SiteTravel });
            returnData.Add(new GetRequestDocumentTypeResponse { Name = RequestDocumentType.DeMobilisation });
            returnData.Add(new GetRequestDocumentTypeResponse { Name = RequestDocumentType.ExternalTravel });


            return returnData;
        }


        public async Task<List<RequestDocumentGroupResponse>> GetApproval(RequestDocumentGroupRequest request, CancellationToken cancellationToken)
        { 
            var returnData = new List<RequestDocumentGroupResponse>();
            var dbData = await Context.RequestGroupConfig
                .Where(x => x.Document == request.document)
                .OrderBy(x => x.OrderIndex).ToArrayAsync();

            returnData.Add(new RequestDocumentGroupResponse { Id = 0, GroupId = 0, GroupName = "Requester", OrderIndex = 0, RuleAction = null });

            foreach (var item in dbData)
            {
                var currentGroup =await Context.RequestGroup.Where(x => x.Id == item.GroupId).FirstOrDefaultAsync();
                if (currentGroup != null) 
                {
                    var newRecord = new RequestDocumentGroupResponse
                    {
                        Id = item.Id,
                        GroupName = currentGroup.Description,
                        OrderIndex = item.OrderIndex,
                        RuleAction = item.RuleAction,
                        GroupId = item.Id
                    };

                    returnData.Add(newRecord);
                }
            }

            returnData.Add(new RequestDocumentGroupResponse { Id = 99999, GroupId = 0, GroupName = "Completed", OrderIndex = dbData.Count() + 1, RuleAction = null });

            return returnData;
        }




        public async Task AddApproval(RequestDocumentGroupAddRequest request, CancellationToken cancellationToken)
        {

            if (request.document == RequestDocumentType.NonSiteTravel)
            {
                var newData = new RequestGroupConfig
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    GroupId = request.groupId,
                    Document = request.document,
                    RuleAction = request.RuleAction,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id

                };
                Context.RequestGroupConfig.Add(newData);
            }
            else {
                var newData = new RequestGroupConfig
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    GroupId = request.groupId,
                    Document = request.document,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id

                };
                Context.RequestGroupConfig.Add(newData);
            }

            await Task.CompletedTask;

        
        }

        public async Task UpdateApproval(RequestDocumentGroupUpdateRequest request, CancellationToken cancellationToken)
        {

            var currentApproval =await Context.RequestGroupConfig.Where(x => x.Id == 1).FirstOrDefaultAsync();
            if (currentApproval != null)
            {
                if (currentApproval.Document == RequestDocumentType.NonSiteTravel)
                { 
                    currentApproval.RuleAction = request.RuleAction;
                    currentApproval.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    currentApproval.DateUpdated = DateTime.Now;
                    Context.RequestGroupConfig.Update(currentApproval);
                }
            }
            await Task.CompletedTask;


        }

        public async Task OrderApproval(RequestDocumentGroupOrderRequest request, CancellationToken cancellationToken)
        {

            var configDetails = await Context.RequestGroupConfig
            .Where(rd => rd.Document == request.document)
            .OrderBy(rd => rd.OrderIndex)
            .ToListAsync(cancellationToken);

            int RowNum = 1;
            foreach (int Id in request.Ids)
            {
                var currentData = configDetails.FirstOrDefault(x => x.Id == Id);
                if (currentData != null)
                {
                    currentData.OrderIndex = RowNum;
                    Context.RequestGroupConfig.Update(currentData);
                }
                RowNum++;
            }
            await Task.CompletedTask;
        }

        public async Task RemoveApproval(RequestDocumentGroupRemoveRequest request, CancellationToken cancellationToken)
        {
            var removeData =await   Context.RequestGroupConfig.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (removeData != null)
            { 
                Context.RequestGroupConfig.Remove(removeData);
            }
        }


        public async Task<RequestDocumentGroupByTypeResponse> GetGroupsAndMembersByType(RequestDocumentGroupByTypeRequest request, CancellationToken cancellationToken)
        {
            var returnDataGroups = new List<RequestDocumentGroupByTypeApprovalGroups>();

            var returnData = new RequestDocumentGroupByTypeResponse();
            returnData.PendingRequests = 0;

            if (request.empId.HasValue)
            {
                returnData.PendingRequests = await Context.RequestDocument.AsNoTracking()
                    .Where(x => x.EmployeeId == request.empId && x.DocumentType == request.documentType
                    && (x.CurrentAction == RequestDocumentAction.Submitted || x.CurrentAction == RequestDocumentAction.Approved
                    || x.CurrentAction == RequestDocumentAction.WaitingAgent)).CountAsync();
            }
            var groups = new List<RequestGroupConfig>();

                    if (request.documentType == RequestDocumentType.ExternalTravel)
                    {
                        groups = await Context.RequestGroupConfig
                            .Where(x => x.Document == RequestDocumentType.SiteTravel).OrderBy(x => x.OrderIndex).ToListAsync();
                    }
                    else
                    {
                        groups = await Context.RequestGroupConfig
                            .Where(x => x.Document == request.documentType).OrderBy(x => x.OrderIndex).ToListAsync();
                    }


            foreach (var item in groups)
                {

                    var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == item.GroupId).FirstOrDefaultAsync();
                    var itemgroupEmployees = await Context.RequestGroupEmployee.AsNoTracking().Where(x => x.RequestGroupId == item.GroupId).ToListAsync();
                    if (currentGroup.Description == "Line Manager" || currentGroup.GroupTag == "linemanager")
                    {

                    if (request.empId.HasValue)
                    {
                        var lineManagerIds = await GetLineManagerIds(request.empId.Value);
                        if (lineManagerIds.Count > 0)
                        {
                            var members = new List<RequestDocumentGroupByTypeEmployees>();
                            var empIds = itemgroupEmployees.Where(x => lineManagerIds.Contains(x.EmployeeId.Value)).Select(x => x.EmployeeId).ToList();
                            if (empIds.Count > 0)
                            {
                                foreach (var employee in itemgroupEmployees)
                                {
                                    if (empIds.Contains(employee.EmployeeId))
                                    {
                                        var currentMemberEmployees = await Context.Employee.AsNoTracking().Where(x => x.Id == employee.EmployeeId).Select(x => new { x.Lastname, x.Firstname }).FirstOrDefaultAsync();
                                        members.Add(new RequestDocumentGroupByTypeEmployees
                                        {
                                            id = employee.Id,
                                            displayName = employee.DisplayName,
                                            employeeId = employee.EmployeeId,
                                            fullName = $"{currentMemberEmployees?.Firstname} {currentMemberEmployees?.Lastname}"
                                        });
                                    }

                                }

                            }

                            var newRecord = new RequestDocumentGroupByTypeApprovalGroups
                            {
                                id = item.Id,
                                GroupName = currentGroup?.Description,
                                OrderIndex = item.OrderIndex,
                                groupMembers = members,
                                GroupId = currentGroup?.Id,
                                GroupTag = currentGroup?.GroupTag


                            };

                            returnDataGroups.Add(newRecord);
                        }

                    }
                    else {
                        var newRecord = new RequestDocumentGroupByTypeApprovalGroups
                        {
                            id = item.Id,
                            GroupName = currentGroup?.Description,
                            OrderIndex = item.OrderIndex,
                            GroupId = currentGroup?.Id,
                            GroupTag = currentGroup?.GroupTag


                        };

                        returnDataGroups.Add(newRecord);
                    }
                }
                    else
                    {
                        var members = new List<RequestDocumentGroupByTypeEmployees>();
                        foreach (var employee in itemgroupEmployees)
                        {
                            var currentMemberEmployees = await Context.Employee.AsNoTracking().Where(x => x.Id == employee.EmployeeId).Select(x => new { x.Lastname, x.Firstname }).FirstOrDefaultAsync();
                            members.Add(new RequestDocumentGroupByTypeEmployees { id = employee.Id, displayName = employee.DisplayName, employeeId = employee.EmployeeId, fullName = $"{currentMemberEmployees?.Lastname} {currentMemberEmployees?.Firstname}" });
                        }


                        var newRecord = new RequestDocumentGroupByTypeApprovalGroups
                        {
                            id = item.Id,
                            GroupName = currentGroup?.Description,
                            OrderIndex = item.OrderIndex,
                            groupMembers = members,
                            GroupTag = currentGroup?.GroupTag,
                            GroupId = currentGroup?.Id,


                        };
                        returnDataGroups.Add(newRecord);
                    }

                }



            returnData.groups = returnDataGroups;



            return returnData;

        }


        public async Task<List<RequestDocumentGroupByIdResponse>> GetGroupsAndMembersById(RequestDocumentGroupByIdRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<RequestDocumentGroupByIdResponse>();
            var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == request.documentId).FirstOrDefaultAsync();

            if (currentDocument != null) {

                var groups = new List<RequestGroupConfig>();

                if (currentDocument.DocumentType == RequestDocumentType.ExternalTravel)
                {
                    groups = await Context.RequestGroupConfig
                        .Where(x => x.Document == RequestDocumentType.SiteTravel).OrderBy(x => x.OrderIndex).ToListAsync();
                }
                else {
                    groups = await Context.RequestGroupConfig
                        .Where(x => x.Document == currentDocument.DocumentType).OrderBy(x => x.OrderIndex).ToListAsync();
                }



                foreach (var item in groups)
                {


                    var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == item.GroupId).FirstOrDefaultAsync();
                    var itemgroupEmployees = await Context.RequestGroupEmployee.AsNoTracking().Where(x => x.RequestGroupId == item.GroupId).ToListAsync();
                    if (currentGroup.Description == "Line Manager" || currentGroup.GroupTag == "linemanager")
                    {

                        var lineManagerIds = await GetLineManagerIds(currentDocument.EmployeeId.Value);
                        if (lineManagerIds.Count > 0)
                        {
                            var members = new List<RequestDocumentGroupByIdEmployees>();
                            var empIds =  itemgroupEmployees.Where(x => lineManagerIds.Contains(x.EmployeeId.Value)).Select(x=> x.EmployeeId).ToList();
                            if (empIds.Count > 0)
                            {
                                foreach (var employee in itemgroupEmployees)
                                {
                                    if (empIds.Contains(employee.EmployeeId))
                                    {
                                        var currentMemberEmployees = await Context.Employee.AsNoTracking().Where(x => x.Id == employee.EmployeeId).Select(x => new { x.Lastname, x.Firstname }).FirstOrDefaultAsync();
                                        members.Add(new RequestDocumentGroupByIdEmployees
                                        {
                                            id = employee.Id,
                                            displayName = employee.DisplayName,
                                            employeeId = employee.EmployeeId,
                                            fullName = $"{currentMemberEmployees?.Firstname} {currentMemberEmployees?.Lastname}"
                                        });
                                    }

                                }

                            }

                            var newRecord = new RequestDocumentGroupByIdResponse
                            {
                                id = item.Id,
                                GroupName = currentGroup?.Description,
                                OrderIndex = item.OrderIndex,
                                groupMembers = members,
                                GroupId = currentGroup?.Id,
                                GroupTag = currentGroup?.GroupTag


                            };

                            returnData.Add(newRecord);
                        }



                    }
                    else {
                        var members = new List<RequestDocumentGroupByIdEmployees>();
                        foreach (var employee in itemgroupEmployees)
                        {
                            var currentMemberEmployees = await Context.Employee.AsNoTracking().Where(x => x.Id == employee.EmployeeId).Select(x => new { x.Lastname, x.Firstname }).FirstOrDefaultAsync();
                            members.Add(new RequestDocumentGroupByIdEmployees { id = employee.Id, displayName = employee.DisplayName, employeeId = employee.EmployeeId, fullName = $"{currentMemberEmployees?.Lastname} {currentMemberEmployees?.Firstname}" });
                        }


                        var newRecord = new RequestDocumentGroupByIdResponse
                        {
                            id = item.Id,
                            GroupName = currentGroup?.Description,
                            OrderIndex = item.OrderIndex,
                            groupMembers = members,
                            GroupTag = currentGroup?.GroupTag,
                            GroupId = currentGroup?.Id,


                        };
                        returnData.Add(newRecord);
                    }

                }
            }

           

            return returnData;

        }

        #region GET LINEMANAGERS

        public async Task<List<RequestDocumentGroupEmpLinesResponse>> GetEmployeeLineManagers(RequestDocumentGroupEmpLinesRequest request, CancellationToken cancellationToken)
        {

            var lineManagerIds = await GetLineManagerIds(request.empId) ?? new List<int>();
            if (!lineManagerIds.Any())
                return new List<RequestDocumentGroupEmpLinesResponse>();

            return await Context.Employee
                .AsNoTracking()
                .Where(x => lineManagerIds.Contains(x.Id))
                .Select(x => new RequestDocumentGroupEmpLinesResponse
                {
                    id = x.Id,
                    fullName = $"{x.Firstname} {x.Lastname}"
                })
                .ToListAsync(cancellationToken);

            //var lineManagerIds = await GetLineManagerIds(request.empId);
            //var returnData = await Context.Employee.AsNoTracking().Where(x => lineManagerIds.Contains(x.Id)).Select(x => new RequestDocumentGroupEmpLinesResponse
            //{
            //    id = x.Id,
            //    fullName = $"{x.Firstname} {x.Lastname}"

            //}).ToListAsync();

            //return returnData;


        }


        private async Task<List<int>> GetLineManagerIds(int employeeId)
        {
            var returnData = new List<int>();

            var hiericalData = await Context.RequestLineManagerEmployee.AsNoTracking().Where(x => x.EmployeeId == employeeId).Select(x=> x.LineManagerId).ToListAsync(); ;
            if (hiericalData.Count > 0)
            {
                returnData.AddRange(hiericalData);
                return returnData;
            }

            var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == employeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                if (currentEmployee.DepartmentId != null)
                {
                    var currentDepartment = await Context.Department.AsNoTracking().Where(x => x.Id == currentEmployee.DepartmentId).FirstOrDefaultAsync();
                    if (currentDepartment != null)
                    {
                        var allDepartments = await Context.Department.AsNoTracking().ToListAsync();


                        var allParentDepartmentIds = GetAllParentDepartments(currentDepartment, allDepartments);
                        var departmentIds = allParentDepartmentIds.Select(x => x.Id).ToList();
                        departmentIds.Add(currentDepartment.Id);
                        if (departmentIds.Count > 0)
                        {
    
                            var departManangerIds = await Context.DepartmentManager.Where(x => departmentIds.Contains(x.DepartmentId))
                                .Select(x => x.EmployeeId).ToListAsync();
                            returnData.AddRange(departManangerIds);
                        }
    


                    }

                }
            
            }

            if (returnData.Count == 0)
            {
                var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "linemanager").FirstOrDefaultAsync();
                if (currentGroup != null)
                {
                    returnData = await Context.RequestGroupEmployee.AsNoTracking().Where(x => x.RequestGroupId == currentGroup.Id)
                        .Select(x => x.EmployeeId.Value ).ToListAsync();
                }
            }
            return returnData;
        }

        private List<Department> GetAllChildDepartments(Department department, List<Department> allDepartments)
        {
            List<Department> children = new List<Department>();
            GetAllChildDepartmentsRecursive(department, allDepartments, children);
            return children;
        }

        private void GetAllChildDepartmentsRecursive(Department department, List<Department> allDepartments, List<Department> children)
        {
            var childDepartments = allDepartments.Where(d => d.ParentDepartmentId == department.Id);
            foreach (var child in childDepartments)
            {
                children.Add(child);
                GetAllChildDepartmentsRecursive(child, allDepartments, children);
            }
        }


        private List<Department> GetAllParentDepartments(Department department, List<Department> allDepartments)
        {
            List<Department> parents = new List<Department>();
            GetAllParentDepartmentsRecursive(department, allDepartments, parents);
            return parents;
        }

        private void GetAllParentDepartmentsRecursive(Department department, List<Department> allDepartments, List<Department> parents)
        {
            if (department.ParentDepartmentId.HasValue)
            {
                Department parent = allDepartments.FirstOrDefault(d => d.Id == department.ParentDepartmentId);
                if (parent != null)
                {
                    parents.Add(parent);
                    GetAllParentDepartmentsRecursive(parent, allDepartments, parents);
                }
            }
        }


        #endregion
        public async Task<List<RequestDocumentRouteResponse>> GetRequestDocumentRoute(RequestDocumentRouteRequest request, CancellationToken cancellationToken)
        { 
            var returnData = new List<RequestDocumentRouteResponse>();
            var currentPosition = await Context.RequestDocument
              .Where(x => x.Id == request.documentId)
            .FirstOrDefaultAsync(cancellationToken);
            if (currentPosition != null)
            {

                RequestGroupConfig[] dbData;
                if (currentPosition.DocumentType == RequestDocumentType.ExternalTravel)
                {
                    dbData = await Context.RequestGroupConfig.AsNoTracking()
                      .Where(x => x.Document == RequestDocumentType.SiteTravel)
                      .OrderBy(x => x.OrderIndex).ToArrayAsync(cancellationToken);
                }
                else {
                    dbData = await Context.RequestGroupConfig.AsNoTracking()
                      .Where(x => x.Document == currentPosition.DocumentType)
                      .OrderBy(x => x.OrderIndex).ToArrayAsync(cancellationToken);
                }



                if (currentPosition.CurrentAction == RequestDocumentAction.Declined)
                {
                    returnData.Add(new RequestDocumentRouteResponse { Id = 0, GroupId = 0, GroupName = "Requester", OrderIndex = 0, CurrentPosition = 1, GroupTag = "Requester" });
                    foreach (var item in dbData)
                    {
                        var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == item.GroupId).FirstOrDefaultAsync(cancellationToken);
                        if (currentGroup != null)
                        {
                            var newRecord = new RequestDocumentRouteResponse
                            {
                                Id = item.Id,
                                GroupName = currentGroup.Description,
                                OrderIndex = item.OrderIndex,
                                GroupId = item.Id,
                                CurrentPosition = 0,
                                GroupTag = currentGroup.GroupTag
                            };

                            returnData.Add(newRecord);
                        }
                    }

                    returnData.Add(new RequestDocumentRouteResponse { Id = 9999999, GroupId = 9999999, GroupName = "Completed", OrderIndex = dbData.Count() + 1, CurrentPosition = currentPosition.CurrentAction == RequestDocumentAction.Completed ? 1 : 0, GroupTag = "Completed" });
                    return returnData;
                }

                else
                {
                    if (currentPosition.AssignedRouteConfigId != null) 
                    {
                        returnData.Add(new RequestDocumentRouteResponse { Id = 0, GroupId = 0, GroupName = "Requester", OrderIndex = 0, CurrentPosition = 0, GroupTag = "Requester" });
                        foreach (var item in dbData)
                        {
                            var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == item.GroupId).FirstOrDefaultAsync(cancellationToken);
                            if (currentGroup != null)
                            {
                                if (currentPosition.AssignedRouteConfigId != null)
                                {
                                    var newRecord = new RequestDocumentRouteResponse
                                    {
                                        Id = item.Id,
                                        GroupName = currentGroup.Description,
                                        OrderIndex = item.OrderIndex,
                                        GroupId = item.Id,
                                        CurrentPosition = currentPosition.CurrentAction == RequestDocumentAction.Completed ? 0 : item.Id != currentPosition.AssignedRouteConfigId.Value ? 0 : 1,
                                        GroupTag = currentGroup.GroupTag
                                    };

                                    returnData.Add(newRecord);
                                }
                                else
                                {
                                    var newRecord = new RequestDocumentRouteResponse
                                    {
                                        Id = item.Id,
                                        GroupName = currentGroup.Description,
                                        OrderIndex = item.OrderIndex,
                                        GroupId = item.Id,
                                        CurrentPosition = currentPosition.CurrentAction == RequestDocumentAction.Completed ? 0 : 0,
                                        GroupTag = currentGroup.GroupTag
                                    };

                                    returnData.Add(newRecord);
                                }
                                returnData.Add(new RequestDocumentRouteResponse { Id = 9999999, GroupId = 9999999, GroupName = "Completed", OrderIndex = dbData.Count() + 1, CurrentPosition = currentPosition.CurrentAction == RequestDocumentAction.Completed ? 1 : 0, GroupTag = "Completed" });
                            }
                        }



                    }
                    if (currentPosition.AssignedEmployeeId != null )
                    {
                        returnData.Add(new RequestDocumentRouteResponse { Id = 0, GroupId = 0, GroupName = "Requester", OrderIndex = 0, CurrentPosition = 1, GroupTag = "Requester" });
                        foreach (var item in dbData)
                        {
                            var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == item.GroupId).FirstOrDefaultAsync(cancellationToken);
                            if (currentGroup != null)
                            {
                                var newRecord = new RequestDocumentRouteResponse
                                {
                                    Id = item.Id,
                                    GroupName = currentGroup.Description,
                                    OrderIndex = item.OrderIndex,
                                    GroupId = item.Id,
                                    CurrentPosition = currentPosition.CurrentAction == RequestDocumentAction.Completed ? 0 : 0,
                                    GroupTag = currentGroup.GroupTag
                                };

                                returnData.Add(newRecord);

                                returnData.Add(new RequestDocumentRouteResponse { Id = 9999999, GroupId = 9999999, GroupName = "Completed", OrderIndex = dbData.Count() + 1, CurrentPosition = currentPosition.CurrentAction == RequestDocumentAction.Completed ? 1 : 0, GroupTag = "Completed" });
                            }
                        }



                    }



                }

                return returnData;
            }

            return returnData;
        
        }








    }
}
