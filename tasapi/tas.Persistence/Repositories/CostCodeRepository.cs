using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.EmployerFeature.GetAllEmployer;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    
    public partial class CostCodeRepository : BaseRepository<CostCode>, ICostCodeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly BulkImportExcelService _bulkImportExcelService;

        public CostCodeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, BulkImportExcelService bulkImportExcelService) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _bulkImportExcelService = bulkImportExcelService;
        }

        public Task<CostCode> GetbyId(int id, CancellationToken cancellationToken)
        {
            return Context.CostCodes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


        

        public async Task<List<GetAllCostCodeResponse>> GetAllData(GetAllCostCodeRequest request, CancellationToken cancellationToken)
        {

            var costcodeQuery = Context.CostCodes.AsQueryable();
            if (request.status.HasValue)
            {
                costcodeQuery = costcodeQuery.Where(x => x.Active == request.status);
            }


            var query = from p in costcodeQuery
                        join ee in (from e in Context.Employee
                                    group e by e.CostCodeId into grouped
                                    select new
                                    {
                                        CostCodeId = grouped.Key,
                                        Count = grouped.Count()
                                    })
                        on p.Id equals ee.CostCodeId into ppGrouped
                        from ee in ppGrouped.DefaultIfEmpty()
                        select new GetAllCostCodeResponse
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Active = p.Active,
                            DateCreated = p.DateCreated,
                            DateUpdated = p.DateUpdated,
                            Description = p.Description,
                            EmployeeCount = ee.Count,
                            Number = p.Number
                        };

            var result = await query.OrderByDescending(x=> x.DateUpdated).ToListAsync(cancellationToken);

            return result;
        }



    }
}
