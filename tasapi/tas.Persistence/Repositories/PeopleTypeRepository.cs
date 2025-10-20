using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.PeopleTypeFeature.GetAllPeopleType;
using tas.Application.Features.PositionFeature.GetAllPosition;
using tas.Application.Features.RosterGroupFeature.GetAllRosterGroup;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
 

    public class PeopleTypeRepository : BaseRepository<PeopleType>, IPeopleTypeRepository
    {
        public PeopleTypeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public Task<PeopleType> GetbyId(int id, CancellationToken cancellationToken)
        {
            return Context.PeopleType.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


        public async Task<List<GetAllPeopleTypeResponse>> GetAllData(GetAllPeopleTypeRequest request, CancellationToken cancellationToken)
        {

            List<PeopleType> peopleTypes;
            if (request.status.HasValue)
            {
                peopleTypes = await Context.PeopleType.AsNoTracking()
                                    .Where(e => e.Active == request.status.Value)
                                    .ToListAsync(cancellationToken);
            }
            else
            {
                peopleTypes = await Context.PeopleType.AsNoTracking()
                                    .ToListAsync(cancellationToken);
            }


            var returnData = new List<GetAllPeopleTypeResponse>();


            foreach (var e in peopleTypes)
            {
                var count = await Context.Employee.AsNoTracking().CountAsync(x => x.PeopleTypeId == e.Id && x.Active == 1);
                var newRecord = new GetAllPeopleTypeResponse
                {
                    Id = e.Id,
                    Code = e.Code,
                    Description = e.Description,
                    Active = e.Active,
                    EmployeeCount = count, 
                    DateCreated = e.DateCreated,
                    DateUpdated = e.DateUpdated
                };

                returnData.Add(newRecord);

            }

            return returnData.OrderByDescending(x => x.DateCreated).ToList();
        }




    }
}
