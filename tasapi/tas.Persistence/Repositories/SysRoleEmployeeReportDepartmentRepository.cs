using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.AddSysRoleEmployeeReportDepartment;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.DeleteSysRoleEmployeeReportDepartment;
using tas.Application.Features.SysRoleEmployeeReportDepartmentFeature.GetSysRoleEmployeeReportDepartment;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class SysRoleEmployeeReportDepartmentRepository : BaseRepository<SysRoleEmployeeReportDepartment>, ISysRoleEmployeeReportDepartmentRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        public SysRoleEmployeeReportDepartmentRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
        }

        public async Task AddSysRoleEmployeeReportDepartment(AddSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.SysRoleEmployeeReportDepartment.Where(x => x.DepartmentId == request.DepartmentId && x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync(cancellationToken);
            if (currentData == null)
            {
                SysRoleEmployeeReportDepartment newRecord = new SysRoleEmployeeReportDepartment()
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _userRepository.LogCurrentUser()?.Id,
                    DepartmentId = request.DepartmentId,
                    EmployeeId = request.EmployeeId
                };

                Context.SysRoleEmployeeReportDepartment.Add(newRecord);


                var currentUser = await Context.Employee.Where(x => x.Id == request.DepartmentId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _userRepository.ClearRoleCache(currentUser.ADAccount);
                }
            }
            else
            {
                throw new BadRequestException("Employee is already registered in this department.");
            }
        }

        public async Task DeleteSysRoleEmployeeReportDepartment(DeleteSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.SysRoleEmployeeReportDepartment
              .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {
                Context.SysRoleEmployeeReportDepartment.Remove(currentData);
                var currentUser = await Context.Employee.AsNoTracking().Where(x => x.Id == currentData.EmployeeId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _userRepository.ClearRoleCache(currentUser.ADAccount);
                }

            }
            else
            {
                throw new BadRequestException("Record not found");
            }

        }


        public async Task<List<GetSysRoleEmployeeReportDepartmentResponse>> GetData(GetSysRoleEmployeeReportDepartmentRequest request, CancellationToken cancellationToken)
        {

            return await (from sysRoleData in Context.SysRoleEmployeeReportDepartment.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                              join department in Context.Department.AsNoTracking() on sysRoleData.DepartmentId equals department.Id into depData
                              from department in depData.DefaultIfEmpty()
                              select new GetSysRoleEmployeeReportDepartmentResponse
                              {
                                  Id = sysRoleData.Id,
                                  DepartmentId = department.Id, 
                                  Name = department.Name,
                                  DateCreated = sysRoleData.DateCreated

                              }).ToListAsync();

        }


    }

}
