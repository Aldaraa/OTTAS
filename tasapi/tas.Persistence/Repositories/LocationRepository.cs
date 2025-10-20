using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{ 
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        public LocationRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public async Task<Location> GetbyId(int id, CancellationToken cancellationToken)
        {
            return await Context.Location.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
