using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CostCodeFeature.GetAllCostCode;
using tas.Application.Features.PositionFeature.AllPosition;
using tas.Application.Features.StateFeature.GetAllState;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class StateRepository : BaseRepository<State>, IStateRepository
    {
        public StateRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public async Task<State> GetbyId(int id, CancellationToken cancellationToken)
        {
           // Context.State.FirstOrDefaultAsync(x=> x.Id == id).ConfigureAwait(false);
            return await Context.State.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<GetAllStateResponse>> GetAllData(GetAllStateRequest request, CancellationToken cancellationToken)
        {
            var stateQuery = Context.State.AsQueryable();
            if (request.status.HasValue)
            {
                stateQuery = stateQuery.Where(x => x.Active == request.status);
            }


            var query = from p in stateQuery
                        join ee in (from e in Context.Employee
                                    group e by e.StateId into grouped
                                    select new
                                    {
                                        stateId = grouped.Key,
                                        Count = grouped.Count()
                                    }
                                      )

                                      on p.Id equals ee.stateId into ppGrouped
                        from ee in ppGrouped.DefaultIfEmpty()
                        select new GetAllStateResponse
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Active = p.Active,
                            DateCreated = p.DateCreated,
                            DateUpdated = p.DateUpdated,
                            Description = p.Description,
                            EmployeeCount = ee.Count == null ? 0 : ee.Count,
                           
                        };

            var result = await query.OrderByDescending(x => x.DateUpdated).ToListAsync(cancellationToken);

            return result;

        }

    }
}
