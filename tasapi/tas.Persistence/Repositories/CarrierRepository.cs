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


    public class CarrierRepository : BaseRepository<Carrier>, ICarrierRepository
    {
        public CarrierRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public Task<Carrier> GetbyId(int id, CancellationToken cancellationToken)
        {
            return Context.Carrier.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
