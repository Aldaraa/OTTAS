using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.CampFeature.GetAllCamp;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class CampRepository : BaseRepository<Camp>, ICampRepository
    {
        public CampRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public async Task<Camp> GetbyId(int id, CancellationToken cancellationToken)
        {
           var returnData =  await Context.Camp.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
            return returnData;

        }

        public async Task<List<GetAllCampResponse>> GetAllData(GetAllCampRequest request, CancellationToken cancellationToken)
        {
            var camps = new List<Camp>();
            var returnData = new List<GetAllCampResponse>();
            if (request.status.HasValue)
            {
                camps =await Context.Camp.AsNoTracking().Where(x => x.Active == request.status.Value).ToListAsync();
            }
            else {
                camps =await Context.Camp.AsNoTracking().ToListAsync();
            }

            foreach (var x in camps)
            {
                var roomCount = await Context.Room.AsNoTracking().CountAsync(c => c.CampId == x.Id);
                var newRecord = new GetAllCampResponse
                {
                    Id = x.Id,
                    Active = x.Active,
                    Code = x.Code,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Description = x.Description,
                    RoomCount =roomCount
                };
                returnData.Add(newRecord);
            }

            return returnData.OrderByDescending(x=>x.DateCreated).ToList();


        
        

        }
    }
}
