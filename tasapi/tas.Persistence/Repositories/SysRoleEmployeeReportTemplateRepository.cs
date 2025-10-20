using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.GetAllSysRoleEmployeeReportTemplate;
using tas.Application.Features.SysRoleEmployeeReportTemplateFeature.UpdateSysRoleEmployeeReportTemplate;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{


    public class SysRoleEmployeeReportTemplateRepository : BaseRepository<SysRoleEmployeeReportTemplate>, ISysRoleEmployeeReportTemplateRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        public SysRoleEmployeeReportTemplateRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
        }

        #region RoleRepportTemplate Data

        public async Task<List<GetAllSysRoleEmployeeReportTemplateResponse>> GetEmployeeReportTemplate(GetAllSysRoleEmployeeReportTemplateRequest request, CancellationToken cancellationToken)
        {


            var result = new List<GetAllSysRoleEmployeeReportTemplateResponse>();

            var allReportTemplates = await Context.ReportTemplate.ToListAsync(cancellationToken);
            var EmployeeTemplateIds = await Context.SysRoleEmployeeReportTemplate.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId).Select(x => new { x.ReportTemplateId }).ToListAsync(cancellationToken);

            if (EmployeeTemplateIds.Count > 0)
            {
                foreach (var item in allReportTemplates)
                {
                    var itemReportTemplateRole = EmployeeTemplateIds.FirstOrDefault(x => x.ReportTemplateId == item.Id); ;
                    var newData = new GetAllSysRoleEmployeeReportTemplateResponse
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Code = item.Code,
                        Permission = itemReportTemplateRole != null ? 1 : 0
                    };

                    result.Add(newData);


                }
                return result;
            }
            else {
                var currentRoleData =await Context.SysRoleEmployees.AsNoTracking().Where(x => x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
                if (currentRoleData != null)
                {
                    var RoleTemplateIds = await Context.SysRoleReportTemplate.AsNoTracking().Where(x => x.RoleId == currentRoleData.RoleId).Select(x => new { x.ReportTemplateId }).ToListAsync(cancellationToken);

                    foreach (var item in allReportTemplates)
                    {
                        var itemReportTemplateRole = RoleTemplateIds.FirstOrDefault(x => x.ReportTemplateId == item.Id); ;
                        var newData = new GetAllSysRoleEmployeeReportTemplateResponse
                        {
                            Id = item.Id,
                            Description = item.Description,
                            Code = item.Code,
                            Permission = itemReportTemplateRole != null ? 1 : 0
                        };

                        result.Add(newData);


                    }
                    return result;
                }
                else {
                    return new List<GetAllSysRoleEmployeeReportTemplateResponse>();
                }

            }

          
        }

        #endregion

        public async Task UpdateReportTemplateRole(UpdateSysRoleEmployeeReportTemplateRequest request, CancellationToken cancellationToken)
        {

            var templateIds = request.ReportTemplatePermissions.Select(item => item.ReportTemplateId).ToList();

            var roleDeleteMenuIds = await Context.SysRoleEmployeeReportTemplate
                .Where(x => x.EmployeeId == request.EmployeeId && !templateIds.Contains(x.ReportTemplateId))
                .ToListAsync(cancellationToken);

            foreach (var item in roleDeleteMenuIds)
            {
                Context.SysRoleEmployeeReportTemplate.Remove(item);
            }

            foreach (var item in templateIds)
            {
                var existingTemplate = await Context.SysRoleEmployeeReportTemplate
                    .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId && x.ReportTemplateId == item);

                if (existingTemplate == null)
                {
                    Context.SysRoleEmployeeReportTemplate.Add(new SysRoleEmployeeReportTemplate
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        ReportTemplateId = item,
                        EmployeeId = request.EmployeeId,
                        UserIdCreated = _userRepository.LogCurrentUser()?.Id
                    });
                }
            }

        }


    }
}
