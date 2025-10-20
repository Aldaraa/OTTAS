using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployerFeature.GetAllEmployer;
using tas.Application.Features.EmployerFeature.GetAllReportEmployer;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class EmployerRepository : BaseRepository<Employer>, IEmployerRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly BulkImportExcelService _bulkImportExcelService;
        public EmployerRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, BulkImportExcelService bulkImportExcelService) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _bulkImportExcelService = bulkImportExcelService;
        }



        public Task<Employer> GetbyId(int id, CancellationToken cancellationToken)
        {
            return Context.Employer.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<GetAllEmployerResponse>> GetAllData(GetAllEmployerRequest request, CancellationToken cancellationToken)
        {
            List<Employer> employers;
            if (request.status.HasValue)
            {
                employers = await Context.Employer
                                    .Where(e => e.Active == request.status.Value)
                                    .ToListAsync(cancellationToken);
            }
            else
            {
                employers = await Context.Employer.AsNoTracking().OrderBy(x=> x.Description)
                                    .ToListAsync(cancellationToken);
            }

            var returnData = new List<GetAllEmployerResponse>(); 

            foreach (var e in employers)
            {
                var empCount = await Context.Employee.AsNoTracking().CountAsync(x => x.EmployerId == e.Id && x.Active == 1);

                var newRecord = new GetAllEmployerResponse
                {
                    Id = e.Id,
                    Code = e.Code,
                    Description = e.Description,
                    Active = e.Active,
                    EmployeeCount = empCount,
                    DateCreated = e.DateCreated,
                    DateUpdated = e.DateUpdated
                };

                returnData.Add(newRecord);

            }

            var response = returnData.OrderBy(x=> x.Description)
                .ToList();
            return response;
        }


        public async Task<List<GetAllReportEmployerResponse>> GetAllReportData(GetAllReportEmployerRequest request, CancellationToken cancellationToken)
        {
            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;

            var reportEmployerIds = await Context.SysRoleEmployeeReportEmployer.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.EmployerId).ToListAsync();

            if (reportEmployerIds.Count == 0)
            {
                var employers = await Context.Employer.AsNoTracking().Where(x => x.Active == 1).Select(x => new GetAllReportEmployerResponse
                {
                    Code = x.Code,
                    Description = x.Description,
                    Id = x.Id,
                }).ToListAsync(cancellationToken);

                return employers;
            }
            else
            {
                var employers = await Context.Employer.AsNoTracking().Where(x => x.Active == 1 &&  reportEmployerIds.Contains(x.Id)).Select(x => new GetAllReportEmployerResponse
                {
                    Code = x.Code,
                    Description = x.Description,
                    Id = x.Id,
                }).ToListAsync(cancellationToken);

                return employers;
            }


        }

    }
}
