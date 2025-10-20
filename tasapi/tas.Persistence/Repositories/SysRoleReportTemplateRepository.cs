using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.SysReportTemplateFeature.GetAllSysReportTemplate;
using tas.Application.Features.SysRoleMenuFeature.GetAllSysRoleMenu;
using tas.Application.Features.SysRoleMenuFeature.UpdateSysRoleMenu;
using tas.Application.Features.SysRoleReportTemplateFeature.GetAllSysRoleReportTemplate;
using tas.Application.Features.SysRoleReportTemplateFeature.UpdateSysRoleReportTemplate;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace tas.Persistence.Repositories
{

    public class SysRoleReportTemplateRepository : BaseRepository<SysRoleReportTemplate>, ISysRoleReportTemplateRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly HTTPUserRepository _userRepository;
        public SysRoleReportTemplateRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _Configuration = configuration;
            _userRepository = hTTPUserRepository;
        }

        #region RoleRepportTemplate Data

        public async Task<List<GetAllSysRoleReportTemplateResponse>> GetRoleReportTemplate(GetAllSysRoleReportTemplateRequest request, CancellationToken cancellationToken)
        {


            var result = new List<GetAllSysRoleReportTemplateResponse>();

            var allReportTemplates = await Context.ReportTemplate.ToListAsync(cancellationToken);
            var RoleTemplateIds = await Context.SysRoleReportTemplate.Where(x => x.RoleId == request.RoleId).Select(x => new { x.ReportTemplateId }).ToListAsync(cancellationToken);

            foreach (var item in allReportTemplates)
            {
                var itemReportTemplateRole = RoleTemplateIds.FirstOrDefault(x => x.ReportTemplateId == item.Id); ;
                var newData = new GetAllSysRoleReportTemplateResponse
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

        #endregion

        public async Task UpdateReportTemplateRole(UpdateSysRoleReportTemplateRequest request, CancellationToken cancellationToken)
        {

            var templateIds = request.ReportTemplatePermissions.Select(item => item.ReportTemplateId).ToList();

            var roleDeleteMenuIds = await Context.SysRoleReportTemplate
                .Where(x => x.RoleId == request.RoleId && !templateIds.Contains(x.ReportTemplateId))
                .ToListAsync(cancellationToken);

            foreach (var item in roleDeleteMenuIds)
            {
                Context.SysRoleReportTemplate.Remove(item);
            }

            foreach (var item in templateIds)
            {
                var existingMenu = await Context.SysRoleReportTemplate
                    .FirstOrDefaultAsync(x => x.RoleId == request.RoleId && x.ReportTemplateId == item);

                if (existingMenu == null)
                {
                    Context.SysRoleReportTemplate.Add(new SysRoleReportTemplate
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        ReportTemplateId = item,
                        RoleId = request.RoleId,
                        UserIdCreated = _userRepository.LogCurrentUser()?.Id
                    });
                }
            }

        }


    }
}
