using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.AddSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.DeleteSysRoleEmployeeReportEmployer;
using tas.Application.Features.SysRoleEmployeeReportEmployerFeature.GetSysRoleEmployeeReportEmployer;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class SysRoleEmployeeReportEmployerRepository : BaseRepository<SysRoleEmployeeReportEmployer>, ISysRoleEmployeeReportEmployerRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        public SysRoleEmployeeReportEmployerRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
        }

        public async Task AddSysRoleEmployeeReportEmployer(AddSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.SysRoleEmployeeReportEmployer.Where(x => x.EmployerId == request.EmployerId && x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync(cancellationToken);
            if (currentData == null)
            {
                SysRoleEmployeeReportEmployer newRecord = new SysRoleEmployeeReportEmployer()
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _userRepository.LogCurrentUser()?.Id,
                    EmployerId = request.EmployerId,
                    EmployeeId = request.EmployeeId
                };

                Context.SysRoleEmployeeReportEmployer.Add(newRecord);


                var currentUser = await Context.Employee.Where(x => x.Id == request.EmployerId).Select(x => new { x.ADAccount }).FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(currentUser?.ADAccount))
                {
                    _userRepository.ClearRoleCache(currentUser.ADAccount);
                }
            }
            else
            {
                throw new BadRequestException("Employee is already registered in this Employer.");
            }
        }

        public async Task DeleteSysRoleEmployeeReportEmployer(DeleteSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.SysRoleEmployeeReportEmployer
              .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {
                Context.SysRoleEmployeeReportEmployer.Remove(currentData);
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


        public async Task<List<GetSysRoleEmployeeReportEmployerResponse>> GetData(GetSysRoleEmployeeReportEmployerRequest request, CancellationToken cancellationToken)
        {

            return await (from sysRoleData in Context.SysRoleEmployeeReportEmployer.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId)
                          join Employer in Context.Employer.AsNoTracking() on sysRoleData.EmployerId equals Employer.Id into depData
                          from Employer in depData.DefaultIfEmpty()
                          select new GetSysRoleEmployeeReportEmployerResponse
                          {
                              Id = sysRoleData.Id,
                              EmployerId = Employer.Id,
                              Name = Employer.Description,
                              DateCreated = sysRoleData.DateCreated

                          }).ToListAsync();

        }


    }
}
