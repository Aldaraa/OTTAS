using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using tas.Application.Common.Exceptions;
using tas.Application.Features.DepartmentFeature.AddDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.AddDepartmentManager;
using tas.Application.Features.DepartmentFeature.AddDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.CreateDepartment;
using tas.Application.Features.DepartmentFeature.CustomListDepartmment;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentManager;
using tas.Application.Features.DepartmentFeature.DeleteDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.GetAdminsDepartment;
using tas.Application.Features.DepartmentFeature.GetAllDepartment;
using tas.Application.Features.DepartmentFeature.GetAllDepartmentAdmins;
using tas.Application.Features.DepartmentFeature.GetAllDepartmentManagers;
using tas.Application.Features.DepartmentFeature.GetAllReportDepartment;
using tas.Application.Features.DepartmentFeature.GetDepartment;
using tas.Application.Features.DepartmentFeature.GetManagersDepartment;
using tas.Application.Features.DepartmentFeature.GetParentDepartments;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentAdmin;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentManager;
using tas.Application.Features.DepartmentFeature.SetMainDepartmentSupervisor;
using tas.Application.Features.DepartmentFeature.UpdateDepartment;
using tas.Application.Features.RoomFeature.FindRoomDateOccupancyAnalyze;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{


    public partial class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
       
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        private readonly BulkImportExcelService _bulkImportExcelService;
        public DepartmentRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, BulkImportExcelService bulkImportExcelService) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
            _bulkImportExcelService = bulkImportExcelService;
            
        }   


        public async Task<GetDepartmentResponse> GetbyId(GetDepartmentRequest request, CancellationToken cancellationToken)
        {
            var currentDepartment = await Context.Department.AsNoTracking().Where(x=> x.Id == request.Id).FirstOrDefaultAsync();
            if (currentDepartment == null)
                throw new NotFoundNoDataException("");
            var getDepartmentResponse = new GetDepartmentResponse
            {
                Id = currentDepartment.Id,
                Name = currentDepartment.Name,
                Active = currentDepartment.Active,
                DateCreated = currentDepartment.DateCreated,
                DateUpdated = currentDepartment.DateUpdated,
                EmployeeCount = Context.Employee.Count(y => y.DepartmentId == currentDepartment.Id && currentDepartment.Active == 1),
                ParentDepartmentId = currentDepartment.ParentDepartmentId,
                CostCodeId = currentDepartment.CostCodeId,
                Admins = new List<GetDepartmentResponseAdmin>(),
                Managers = new List<GetDepartmentResponseManager>(),
                Supervisors = new List<GetDepartmentResponseSupervisor>(),
                CostCodes = new List<GetDepartmentResponseCostCode>()
            };

            var departmentAdmins = await GetDepartmentAdmins(request.Id);
            var departmentManagers = await GetDepartmentManagers(request.Id);
            var departmentSupervisors = await GetDepartmentSupervisor(request.Id);
            var departmentCostCodes = await GetDepartmentCostCodes(request.Id);




            getDepartmentResponse.Admins = departmentAdmins;
            getDepartmentResponse.Managers = departmentManagers;
            getDepartmentResponse.Supervisors = departmentSupervisors;
            getDepartmentResponse.CostCodes = departmentCostCodes;
            return getDepartmentResponse;
        }

        private async Task<List<GetDepartmentResponseAdmin>> GetDepartmentAdmins(int DepartmentId)
        {

            var data = await (from departmentData in Context.DepartmentAdmin.AsNoTracking().Where(x => x.DepartmentId == DepartmentId)
                              join employee in Context.Employee.AsNoTracking() on departmentData.EmployeeId equals employee.Id into employeeData
                              from employee in employeeData.DefaultIfEmpty()
                              select new GetDepartmentResponseAdmin {
                                  Id = departmentData.Id,
                                  EmployeeId = employee.Id,
                                  FullName = $"{employee.Firstname} {employee.Lastname}",
                                  SAPID = employee.SAPID,
                                  ADAccount = employee.ADAccount,
                                  Main = departmentData.Main
                              }).ToListAsync();

            return data;

        }

        private async Task<List<GetDepartmentResponseManager>> GetDepartmentManagers(int DepartmentId)
        {


            var data = await (from departmentData in Context.DepartmentManager.AsNoTracking().Where(x => x.DepartmentId == DepartmentId)
                              join employee in Context.Employee.AsNoTracking() on departmentData.EmployeeId equals employee.Id into employeeData
                              from employee in employeeData.DefaultIfEmpty()
                              select new GetDepartmentResponseManager
                              {
                                  Id = departmentData.Id,
                                  EmployeeId = employee.Id,
                                  FullName = $"{employee.Firstname} {employee.Lastname}",
                                  SAPID = employee.SAPID,
                                  ADAccount = employee.ADAccount,
                                  Main = departmentData.Main

                              }).ToListAsync();

            return data;
        }

        private async Task<List<GetDepartmentResponseSupervisor>> GetDepartmentSupervisor(int DepartmentId)
        {
            var data = await (from departmentData in Context.DepartmentSupervisor.AsNoTracking().Where(x => x.DepartmentId == DepartmentId)
                              join employee in Context.Employee.AsNoTracking() on departmentData.EmployeeId equals employee.Id into employeeData
                              from employee in employeeData.DefaultIfEmpty()
                              select new GetDepartmentResponseSupervisor
                              {
                                  Id = departmentData.Id,
                                  EmployeeId = employee.Id,
                                  FullName = $"{employee.Firstname} {employee.Lastname}",
                                  SAPID = employee.SAPID,
                                  ADAccount = employee.ADAccount,
                                  Main = departmentData.Main    
                              }).ToListAsync();

            return data;

        }

        private async Task<List<GetDepartmentResponseCostCode>> GetDepartmentCostCodes(int DepartmentId)
        {
            var data = await (from departmentData in Context.DepartmentCostCode.AsNoTracking().Where(x => x.DepartmentId == DepartmentId)
                              join costcode in Context.CostCodes.AsNoTracking() on departmentData.CostCodeId equals costcode.Id into costcodeData
                              from costcode in costcodeData.DefaultIfEmpty()
                              select new GetDepartmentResponseCostCode
                              {
                                  Id = departmentData.Id,
                                  CostCode = $"{costcode.Number} {costcode.Description}"
                              }).ToListAsync();

            return data;

        }

        public async Task<List<CustomListDepartmentResponse>> GetMinimumList(CancellationToken cancellationToken) 
        {

            var result = await Context.Department
                .Join(Context.Department,
                    maindep => maindep.ParentDepartmentId,
                    childep => childep.Id,
                    (childep, maindep) => new CustomListDepartmentResponse
                    {
                        Id = childep.Id,
                        Name = childep.Name,
                        parentdepartmentname = maindep.Name,
                        EmployeeCount = Context.Employee.AsNoTracking().Count(x => x.DepartmentId == childep.Id && x.Active == 1),
                    }).ToListAsync(cancellationToken);

            return result;
        }



        public async Task<List<GetAllDepartmentResponse>> GetAllDepartmentsWithChildren(GetAllDepartmentRequest request, CancellationToken cancellationToken)
        {
            bool filtered = false;
            IQueryable<Department> allDepartments = Context.Department.AsNoTracking();
            //if (request.parentDepartmentId.HasValue)
            //{
            //    allDepartments = allDepartments.Where(x => x.Id == request.parentDepartmentId);
            //}

            if (!string.IsNullOrWhiteSpace(request.departmentName))
            {
                allDepartments = allDepartments.Where(x => x.Name.Contains(request.departmentName));
                filtered = true;
            }

            if (!string.IsNullOrWhiteSpace(request.keyword))
            {
                var managerIds = await Context.DepartmentManager.AsNoTracking().Select(x=> new { x.EmployeeId, x.DepartmentId }).ToListAsync();
                var adminIds = await Context.DepartmentAdmin.AsNoTracking().Select(x => new { x.EmployeeId, x.DepartmentId }).ToListAsync();
                var supervisorIds = await Context.DepartmentSupervisor.AsNoTracking().Select(x => new { x.EmployeeId, x.DepartmentId }).ToListAsync();


                var allData = new List<(int EmployeeId, int DepartmentId)>();

                foreach (var managerId in managerIds)
                {
                    allData.Add((managerId.EmployeeId, managerId.DepartmentId));
                }

                foreach (var adminId in adminIds)
                {
                    allData.Add((adminId.EmployeeId, adminId.DepartmentId));
                }

                foreach (var supervisorId in supervisorIds)
                {
                    allData.Add((supervisorId.EmployeeId, supervisorId.DepartmentId));
                }
                var allEmpIds = new List<int>();
                allEmpIds.AddRange(managerIds.Select(x=> x.EmployeeId));
                allEmpIds.AddRange(adminIds.Select(x => x.EmployeeId));
                allEmpIds.AddRange(supervisorIds.Select(x => x.EmployeeId));

                if (allEmpIds.Count > 0)
                {
                    var depEmployeeIds = await Context.Employee.AsNoTracking()
                        .Where(x => allEmpIds.Contains(x.Id) &&
                                    (EF.Functions.Like(x.Firstname, $"%{request.keyword}%") ||
                                     EF.Functions.Like(x.Lastname, $"%{request.keyword}%") ||
                                     (x.SAPID != null && EF.Functions.Like(x.SAPID.ToString(), $"%{request.keyword}%"))))
                        .Select(x => x.Id)
                        .ToListAsync();

                //    if (depEmployeeIds.Count > 0)
                  //  {
                      var filteredDeparments =   allData.Where(x => depEmployeeIds.Contains(x.EmployeeId)).Select(x=> x.DepartmentId).ToList();

                     allDepartments =   allDepartments.Where(x => filteredDeparments.Contains(x.Id));
                    //}

                }

                filtered = true;


            }




        //    var allDeparmentResult =Context.Department.Contains(allDepartments.ToListAsync());

            var getAllDepartmentResponses = new List<GetAllDepartmentResponse>();
            foreach (var item in await allDepartments.ToListAsync())
            {
                var employeeCount = await Context.Employee.AsNoTracking().CountAsync(y => y.DepartmentId == item.Id && y.Active == 1);
                int? depManagers = 0;
                int? depAdmins = 0;
                int supervisers = 0;


                depManagers = await Context.DepartmentManager.AsNoTracking().Where(x => x.DepartmentId == item.Id).CountAsync();
                depAdmins = await Context.DepartmentAdmin.AsNoTracking().Where(x => x.DepartmentId == item.Id).CountAsync();
                string? costCode = string.Empty;
                if (item.CostCodeId.HasValue)
                {
                    var currentCost = await Context.CostCodes.AsNoTracking().Where(x => x.Id == item.CostCodeId).FirstOrDefaultAsync();
                    if (currentCost != null)
                    {
                        costCode = $"{currentCost.Code} {currentCost.Number} {currentCost.Description}";
                    }
                }



                var newRecord = new GetAllDepartmentResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    Active = item.Active,
                    DateCreated = item.DateCreated,
                    DateUpdated = item.DateUpdated,
                    Admins = depAdmins,
                    Managers = depManagers,
                    CostCodeId = item.CostCodeId,
                    CostCodeDescr = costCode,
                    Supervisers = supervisers,
                    EmployeeCount = employeeCount,
                    ParentDepartmentId = item.ParentDepartmentId,
                    ChildDepartments = new List<GetAllDepartmentResponse>()
                };

                getAllDepartmentResponses.Add(newRecord);

            }

            if (!filtered)
            {
                var rootDepartments = getAllDepartmentResponses.Where(c => !c.ParentDepartmentId.HasValue ).ToList();
                PopulateChildren(rootDepartments, getAllDepartmentResponses);

                return rootDepartments;
            }
            else {
              //  var rootDepartments = getAllDepartmentResponses.Where(c => !c.ParentDepartmentId.HasValue).ToList();
             //   PopulateChildren(rootDepartments, getAllDepartmentResponses);

                return getAllDepartmentResponses;
            }



        }

        private void PopulateChildren(ICollection<GetAllDepartmentResponse> parentDepartments, List<GetAllDepartmentResponse> allDepartments)
        {
            foreach (var parentDepartment in parentDepartments)
            {
                parentDepartment.ChildDepartments = allDepartments.Where(c => c.ParentDepartmentId == parentDepartment.Id).ToList();
                PopulateChildren(parentDepartment.ChildDepartments, allDepartments);
            }
        }


        public async Task CreateDepartment(CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
           var olddata =await Context.Department.Where(x => x.Name.ToLower() == request.Name.ToLower()).FirstOrDefaultAsync();
            if (olddata == null)
            {
                var newData = new Department
                {
                    Active = 1,
                    ParentDepartmentId = request.ParentDepartmentId,
                    Name = request.Name,
                    DateCreated = DateTime.Now,
                    CostCodeId = request.CostCodeId,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                };

                Context.Department.Add(newData);


            }
            else {
                throw new BadRequestException("Duplicate department name. Enter another name!");
            }
           
                
        }

        public async Task UpdateDepartment(UpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            var olddata = await Context.Department.Where(x => x.Name.ToLower() == request.Name.ToLower() && x.Id != request.Id).FirstOrDefaultAsync();
            if (olddata == null)
            {
                var currentDepartment = await Context.Department.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if (request.Id == request.ParentDepartmentId)
                {
                    throw new BadRequestException("Master department is wrong\r\n");
                }
                if (currentDepartment != null)
                {
                    currentDepartment.Active = 1;
                    currentDepartment.ParentDepartmentId = request.ParentDepartmentId;
                    currentDepartment.Name = request.Name;
                    currentDepartment.DateUpdated = DateTime.Now;
                    currentDepartment.CostCodeId = request.CostCodeId;
                    currentDepartment.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                    Context.Department.Update(currentDepartment);
                    await  Task.CompletedTask;
                }

            }
            else
            {
                throw new BadRequestException("Duplicate department name. Enter another name!");
            }
        }


        #region DepartmentManager



        public async Task AddDepartmentManager(AddDepartmentManagerRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentManager.Where(x => x.DepartmentId == request.DepartmentId && x.EmployeeId == request.DepartmentManagerId).FirstOrDefaultAsync();
            if (currentData == null)
            {
                DepartmentManager newRecord = new DepartmentManager()
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    DepartmentId = request.DepartmentId,
                    EmployeeId = request.DepartmentManagerId
                };

                Context.DepartmentManager.Add(newRecord);

                var currentUser = await Context.Employee.AsNoTracking().Where(x => x.Id == request.DepartmentManagerId).Select(x => new { x.ADAccount,x.Firstname }).FirstOrDefaultAsync();

                var currentApprovalGroup =await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "linemanager").FirstOrDefaultAsync();
                if (currentApprovalGroup != null) {
                   var hasLinemanagergroup =await  Context.RequestGroupEmployee.AsNoTracking()
                        .Where(x => x.EmployeeId == request.DepartmentManagerId && x.RequestGroupId == currentApprovalGroup.Id)
                        .AnyAsync();
                    if (!hasLinemanagergroup) {
                        Context.RequestGroupEmployee.Add(new RequestGroupEmployee
                        {
                            Active = 1,
                            DateCreated = DateTime.Now,
                            EmployeeId = request.DepartmentManagerId,
                            RequestGroupId = currentApprovalGroup.Id,
                            DisplayName = currentUser?.Firstname,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,

                        }) ;
                    }
                }



                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _hTTPUserRepository.ClearRoleCache(currentUser.ADAccount);
                }


            }
            else {
                throw new BadRequestException("Employee is already registered in this department.");
            }

        }

        public async Task DeleteDepartmentManager(DeleteDepartmentManagerRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentManager
                .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {
                Context.DepartmentManager.Remove(currentData);

                var currentUser = await Context.Employee.Where(x => x.Id == currentData.EmployeeId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                var hasOtherDepartmentData =await Context.DepartmentManager.AsNoTracking().Where(x => x.EmployeeId == currentData.EmployeeId && x.DepartmentId != currentData.DepartmentId).AnyAsync();
                if (!hasOtherDepartmentData) 
                {
                    var currentApprovalGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "linemanager").FirstOrDefaultAsync();
                    if (currentApprovalGroup != null) {
                        var deletegroupdata = await Context.RequestGroupEmployee.Where(x => x.EmployeeId == currentData.EmployeeId && x.RequestGroupId == currentApprovalGroup.Id).FirstOrDefaultAsync();
                        if (deletegroupdata != null) {
                            Context.RequestGroupEmployee.Remove(deletegroupdata);
                        }

                    }
                }
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _hTTPUserRepository.ClearRoleCache(currentUser.ADAccount);
                }
            }
            else
            {
                throw new BadRequestException("Record not found");
            }


        }

        #endregion

        #region DepartmentAdmin



        public async Task AddDepartmentAdmin(AddDepartmentAdminRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentAdmin.Where(x => x.DepartmentId == request.DepartmentId && x.EmployeeId == request.DepartmentAdminId).FirstOrDefaultAsync(cancellationToken);
            if (currentData == null)
            {
                DepartmentAdmin newRecord = new DepartmentAdmin()
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    DepartmentId = request.DepartmentId,
                    EmployeeId = request.DepartmentAdminId
                };

                Context.DepartmentAdmin.Add(newRecord);

                //administrator
                var currentUser = await Context.Employee.Where(x => x.Id == request.DepartmentAdminId).Select(x => new { x.ADAccount,x.Firstname }).FirstOrDefaultAsync();

                var currentApprovalGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "administrator").FirstOrDefaultAsync();
                if (currentApprovalGroup != null)
                {
                    var hasLinemanagergroup = await Context.RequestGroupEmployee.AsNoTracking()
                         .Where(x => x.EmployeeId == request.DepartmentAdminId && x.RequestGroupId == currentApprovalGroup.Id)
                         .AnyAsync();
                    if (!hasLinemanagergroup)
                    {
                        Context.RequestGroupEmployee.Add(new RequestGroupEmployee
                        {
                            Active = 1,
                            DateCreated = DateTime.Now,
                            EmployeeId = request.DepartmentAdminId,
                            RequestGroupId = currentApprovalGroup.Id,
                            DisplayName = currentUser?.Firstname,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,

                        });
                    }
                }


                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _hTTPUserRepository.ClearRoleCache(currentUser.ADAccount);
                }
            }
            else
            {
                throw new BadRequestException("Employee is already registered in this department.");
            }

        }



        public async Task DeleteDepartmentAdmin(DeleteDepartmentAdminRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentAdmin
                .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {
                Context.DepartmentAdmin.Remove(currentData);

                var hasOtherDepartmentData = await Context.DepartmentAdmin.AsNoTracking().Where(x => x.EmployeeId == currentData.EmployeeId && x.DepartmentId != currentData.DepartmentId).AnyAsync();
                if (!hasOtherDepartmentData)
                {
                    var currentApprovalGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "administrator").FirstOrDefaultAsync();
                    if (currentApprovalGroup != null)
                    {
                        var deletegroupdata = await Context.RequestGroupEmployee.Where(x => x.EmployeeId == currentData.EmployeeId && x.RequestGroupId == currentApprovalGroup.Id).FirstOrDefaultAsync();
                        if (deletegroupdata != null)
                        {
                            Context.RequestGroupEmployee.Remove(deletegroupdata);
                        }

                    }
                }

                var currentUser = await Context.Employee.Where(x => x.Id == currentData.EmployeeId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _hTTPUserRepository.ClearRoleCache(currentUser.ADAccount);
                }

            }
            else {
                throw new BadRequestException("Record not found");
            }

        }

        #endregion

        #region Supervisor




        public async Task DeleteDepartmentSupervisor(DeleteDepartmentSupervisorRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentSupervisor
                .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {
                Context.DepartmentSupervisor.Remove(currentData);


                var currentUser = await Context.Employee.Where(x => x.Id == currentData.EmployeeId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _hTTPUserRepository.ClearRoleCache(currentUser.ADAccount);
                }
            }
            else
            {
                throw new BadRequestException("Record not found");
            }
            
        }

        public async Task AddDepartmentSupervisor(AddDepartmentSupervisorRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.DepartmentSupervisor.Where(x => x.DepartmentId == request.DepartmentId && x.EmployeeId == request.DepartmentSupervisorId).FirstOrDefaultAsync();
            if (currentData == null)
            {
                DepartmentSupervisor newRecord = new DepartmentSupervisor()
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    DepartmentId = request.DepartmentId,
                    EmployeeId = request.DepartmentSupervisorId
                };

                Context.DepartmentSupervisor.Add(newRecord);


                var currentUser = await Context.Employee.AsNoTracking().Where(x => x.Id == request.DepartmentSupervisorId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _hTTPUserRepository.ClearRoleCache(currentUser.ADAccount);
                }
            }
            else
            {
                throw new BadRequestException("Employee is already registered in this department.");
            }

        }


        #endregion

        #region Managers And Admins


        public async Task<List<GetAdminsDepartmentResponse>> GetAdminsDepartment(GetAdminsDepartmentRequest request, CancellationToken cancellationToken)
        {
            return  await (from depadmin in Context.DepartmentAdmin.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                                   join department in Context.Department.AsNoTracking() on depadmin.DepartmentId equals department.Id into depData
                                    from department in depData.DefaultIfEmpty()
                                    select new GetAdminsDepartmentResponse {
                                        Id =depadmin.Id,
                                        DepartmentId = department.Id,
                                        Name = department.Name,
                                        DateCreated = depadmin.DateCreated
                                    }).ToListAsync();
        }


        public async Task<List<GetManagersDepartmentResponse>> GetManagersDepartment(GetManagersDepartmentRequest request, CancellationToken cancellationToken)
        {
            return await (from depadmin in Context.DepartmentManager.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                          join department in Context.Department.AsNoTracking() on depadmin.DepartmentId equals department.Id into depData
                          from department in depData.DefaultIfEmpty()
                          select new GetManagersDepartmentResponse
                          {
                              Id = depadmin.Id,
                              DepartmentId = department.Id,
                              Name = department.Name,
                              DateCreated = depadmin.DateCreated
                          }).ToListAsync();
        }


        public async Task<List<GetAllDepartmentAdminsResponse>> GetAllDepartmentAdmins(GetAllDepartmentAdminsRequest request, CancellationToken cancellationToken)
        {

            var currentRole = await Context.SysRole.AsNoTracking().Where(x => x.RoleTag == "DepartmentAdmin").Select(x => new { x.Id }).FirstOrDefaultAsync();
            if (currentRole != null)
            {
                return await (from admins in Context.SysRoleEmployees.AsNoTracking().Where(x => x.RoleId == currentRole.Id)
                              join employee in Context.Employee.AsNoTracking() on admins.EmployeeId equals employee.Id into employeeData
                              from employee in employeeData.DefaultIfEmpty()
                              join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                              from department in departmentData.DefaultIfEmpty()

                              select new GetAllDepartmentAdminsResponse
                              {
                                  Id = admins.Id,
                                  EmployeeId = admins.EmployeeId,
                                  FullName = $"{employee.Firstname} {employee.Lastname}",
                                  SAPID = employee.SAPID,
                                  DepartmentId = employee.DepartmentId,
                                  DepartmentName = department.Name

                              }).ToListAsync(cancellationToken);
            }
            else
            {
                return new List<GetAllDepartmentAdminsResponse>();
            }
        }

        public async Task<List<GetAllDepartmentManagersResponse>> GetAllDepartmentManagers(GetAllDepartmentManagersRequest request, CancellationToken cancellationToken)
        {

            var currentRole =await Context.SysRole.Where(x => x.RoleTag == "DepartmentManager").Select(x=> new { x.Id }).FirstOrDefaultAsync();
            if (currentRole != null)
            {
                return await (from admins in Context.SysRoleEmployees.AsNoTracking().Where(x=> x.RoleId == currentRole.Id)
                              join employee in Context.Employee.AsNoTracking() on admins.EmployeeId equals employee.Id into employeeData
                              from employee in employeeData.DefaultIfEmpty()
                              join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                              from department in departmentData.DefaultIfEmpty()

                              select new GetAllDepartmentManagersResponse
                              {
                                  Id = admins.Id,
                                  EmployeeId = admins.EmployeeId,
                                  FullName = $"{employee.Firstname} {employee.Lastname}",
                                  DepartmentId = employee.DepartmentId,
                                  SAPID = employee.SAPID,
                                  DepartmentName = department.Name,

                              }).ToListAsync(cancellationToken);
            }
            else {
                return new List<GetAllDepartmentManagersResponse>();
            }
           

        }


        #endregion


        #region GetParentDepartments

        public async Task<List<GetParentDepartmentsResponse>> GetParentDepartments(GetParentDepartmentsRequest request, CancellationToken cancellationToken)
        {
            var result = new List<GetParentDepartmentsResponse>();
            await GetParentDepartments(request.DepartmentId, result);

            return result; 
            
        }


        private async Task GetParentDepartments(int departmentId, List<GetParentDepartmentsResponse> result)
        {
            var department =await Context.Department.AsNoTracking().FirstOrDefaultAsync(d => d.Id == departmentId);
            if (department != null)
            {
                if (department.Name != "ALL") {


                    var departmentAdminData = await (from departmentAdmin in Context.DepartmentAdmin.AsNoTracking().Where(x => x.DepartmentId == department.Id && x.Main == 1)
                                                  join admin in Context.Employee.AsNoTracking() on departmentAdmin.EmployeeId equals admin.Id into adminData
                                                  from admin in adminData.DefaultIfEmpty()
                                                  select new
                                                  {
                                                      DepartmentId = departmentAdmin.DepartmentId,
                                                      EmployeeId = admin.Id,
                                                      Fullname = $"{admin.Firstname} {admin.Lastname}"
                                                  }).FirstOrDefaultAsync();


                    var departmentManagerData = await (from departmentManager in Context.DepartmentManager.AsNoTracking().Where(x => x.DepartmentId == department.Id && x.Main == 1)
                                                     join manager in Context.Employee.AsNoTracking() on departmentManager.EmployeeId equals manager.Id into managerData
                                                     from manager in managerData.DefaultIfEmpty()
                                                     select new
                                                     {
                                                         DepartmentId = departmentManager.DepartmentId,
                                                         EmployeeId = manager.Id,
                                                         Fullname = $"{manager.Firstname} {manager.Lastname}"
                                                     }).FirstOrDefaultAsync();

                    var departmentSupervisorData = await (from departmentSupervisor in Context.DepartmentSupervisor.AsNoTracking().Where(x => x.DepartmentId == department.Id && x.Main == 1)
                                                       join supervisor in Context.Employee.AsNoTracking() on departmentSupervisor.EmployeeId equals supervisor.Id into supervisorData
                                                       from supervisor in supervisorData.DefaultIfEmpty()
                                                       select new
                                                       {
                                                           DepartmentId = departmentSupervisor.DepartmentId,
                                                           EmployeeId = supervisor.Id,
                                                           Fullname = $"{supervisor.Firstname} {supervisor.Lastname}"
                                                       }).FirstOrDefaultAsync();


                    result.Add(new GetParentDepartmentsResponse { Id = department.Id,
                        Name = department.Name, 
                        AdminId = departmentAdminData?.EmployeeId,
                        AdminName = departmentAdminData?.Fullname,
                        ManagerId = departmentManagerData?.EmployeeId,
                        ManagerName = departmentManagerData?.Fullname,
                        SupervisorId = departmentSupervisorData?.EmployeeId,
                        SupervisorName = departmentSupervisorData?.Fullname,
                        DepartmentLevel = result.Count
                    });
                    if (department.ParentDepartmentId.HasValue)
                    {
                        await GetParentDepartments(department.ParentDepartmentId.Value, result);
                    }
                }

            }
        }
        #endregion



        #region SetMainDepartmentManager

        public async Task SetMainDepartmentManager(SetMainDepartmentManagerRequest request, CancellationToken cancellationToken)
        {
            var currentManager = await Context.DepartmentManager.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (currentManager != null)
            {
                var managersInDepartment = await Context.DepartmentManager.AsNoTracking()
                    .Where(x => x.DepartmentId == currentManager.DepartmentId)
                    .ToListAsync(cancellationToken);

                foreach (var manager in managersInDepartment)
                {
                    manager.Main = manager.Id == request.Id ? 1 : 0;
                    Context.Entry(manager).State = EntityState.Modified;
                }
            }

        }


        #endregion


        #region SetMainDepartmentAdmin

        public async Task SetMainDepartmentAdmin(SetMainDepartmentAdminRequest request, CancellationToken cancellationToken)
        {
            var currentAdmin = await Context.DepartmentAdmin.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (currentAdmin != null)
            {
                var adminsInDepartment = await Context.DepartmentAdmin
                    .Where(x => x.DepartmentId == currentAdmin.DepartmentId)
                    .ToListAsync(cancellationToken);

                foreach (var admin in adminsInDepartment)
                {
                    admin.Main = admin.Id == request.Id ? 1 : 0;
                    Context.Entry(admin).State = EntityState.Modified;
                }
            }

        }


        #endregion



        #region SetMainDepartmentSupervisor

        public async Task SetMainDepartmentSupervisor(SetMainDepartmentSupervisorRequest request, CancellationToken cancellationToken)
        {
            var currentSupervisor = await Context.DepartmentSupervisor
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (currentSupervisor != null)
            {
                var supervisorInDepartment = await Context.DepartmentSupervisor
                    .Where(x => x.DepartmentId == currentSupervisor.DepartmentId)
                    .ToListAsync(cancellationToken);
                foreach (var supervirsor in supervisorInDepartment)

                {
                    supervirsor.Main = supervirsor.Id == request.Id ? 1 : 0;
                    Context.Entry(supervirsor).State = EntityState.Modified;
                }
            }

        }


        #endregion


        #region GetReportDepartmentData


        public async Task<List<GetAllReportDepartmentResponse>> GetAllReportDepartmentsWithChildren(GetAllReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            var userId = _hTTPUserRepository.LogCurrentUser()?.Id;

            var userDepartmentDta = await Context.SysRoleEmployeeReportDepartment.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x=> x.DepartmentId).ToListAsync();


            if (userDepartmentDta.Count == 0)
            {
                IQueryable<Department> allDepartments = Context.Department.AsNoTracking();


                var getAllDepartmentResponses = new List<GetAllReportDepartmentResponse>();
                foreach (var item in await allDepartments.ToListAsync())
                {
                    var newRecord = new GetAllReportDepartmentResponse
                    {
                        Id = item.Id,
                        Name = item.Name,
                        ParentDepartmentId = item.ParentDepartmentId,
                        ChildDepartments = new List<GetAllReportDepartmentResponse>()
                    };

                    getAllDepartmentResponses.Add(newRecord);

                }
                var rootDepartments = getAllDepartmentResponses.Where(c => !c.ParentDepartmentId.HasValue).ToList();
                PopulateReportChildren(rootDepartments, getAllDepartmentResponses);

                return rootDepartments;


            }
            else {
                IQueryable<Department> allDepartments = Context.Department.AsNoTracking();


                var getAllDepartmentResponses = new List<GetAllReportDepartmentResponse>();
                foreach (var item in await allDepartments.Where(c=> userDepartmentDta.Contains(c.Id) || c.ParentDepartmentId == null || userDepartmentDta.Contains(c.ParentDepartmentId.Value)).ToListAsync())
                {
                    var newRecord = new GetAllReportDepartmentResponse
                    {
                        Id = item.Id,
                        Name = item.Name,
                        ParentDepartmentId = item.ParentDepartmentId,
                        ChildDepartments = new List<GetAllReportDepartmentResponse>()
                    };

                    getAllDepartmentResponses.Add(newRecord);

                }
                var rootDepartments = getAllDepartmentResponses.Where(c => !c.ParentDepartmentId.HasValue).ToList();
                PopulateReportChildren(rootDepartments, getAllDepartmentResponses);

                return rootDepartments;
            }





        }

        private void PopulateReportChildren(ICollection<GetAllReportDepartmentResponse> parentDepartments, List<GetAllReportDepartmentResponse> allDepartments)
        {
            foreach (var parentDepartment in parentDepartments)
            {
                parentDepartment.ChildDepartments = allDepartments.Where(c => c.ParentDepartmentId == parentDepartment.Id).ToList();
                PopulateReportChildren(parentDepartment.ChildDepartments, allDepartments);
            }
        }



        #endregion


    }
}
