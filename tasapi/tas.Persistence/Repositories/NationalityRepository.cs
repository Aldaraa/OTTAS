using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Application.Features.PositionFeature.AllPosition;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class NationalityRepository : BaseRepository<Nationality>, INationalityRepository
    {
        public NationalityRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public Task<Nationality> GetbyId(int id, CancellationToken cancellationToken)
        {
            return Context.Nationality.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


        public async Task<List<GetAllNationalityResponse>> GetAllData(GetAllNationalityRequest request, CancellationToken cancellationToken)
        {
            var nationalityQuery = Context.Nationality.AsQueryable();
            if (request.status.HasValue)
            {
                nationalityQuery = nationalityQuery.Where(x => x.Active == request.status);
            }


            var query = from p in nationalityQuery
                        join ee in (from e in Context.Employee
                                    group e by e.NationalityId into grouped
                                    select new
                                    {
                                        NationalityId = grouped.Key,
                                        Count = grouped.Count()
                                    })
                        on p.Id equals ee.NationalityId into ppGrouped
                        from ee in ppGrouped.DefaultIfEmpty()
                        select new GetAllNationalityResponse
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Active = p.Active,
                            DateCreated = p.DateCreated,
                            DateUpdated = p.DateUpdated,
                            Description = p.Description,
                            EmployeeCount = ee.Count
                        };

            var result = await query.OrderByDescending(x=> x.DateUpdated).ToListAsync(cancellationToken);

            return result;




        }
    }
}
