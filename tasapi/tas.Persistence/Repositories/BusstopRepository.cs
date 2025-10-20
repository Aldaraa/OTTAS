using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BusstopFeature.DeleteBusstop;
using tas.Application.Features.BusstopFeature.GetAllBusstop;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{


    public class BusstopRepository : BaseRepository<Busstop>, IBusstopRepository
    {
        public BusstopRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public async Task<List<GetAllBusstopResponse>> GetAllBusstop(GetAllBusstopRequest request, CancellationToken cancellationToken)
        {
            var Busstops = new List<Busstop>();
            var returnData = new List<GetAllBusstopResponse>();
            Busstops = await Context.Busstop.AsNoTracking().ToListAsync();


            foreach (var x in Busstops)
            {
                var newRecord = new GetAllBusstopResponse
                {
                    Id = x.Id,
                    Active = x.Active,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                };
                returnData.Add(newRecord);
            }

            return returnData.OrderByDescending(x => x.DateCreated).ToList();





        }


        #region DeleteBusstop
        public async Task DeletForce(DeleteBusstopRequest request, CancellationToken cancellationToken)
        {
            var currentData =await  Context.Busstop.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null) { 
                Context.Busstop.Remove(currentData);
            }
        }
        #endregion
    }
}
