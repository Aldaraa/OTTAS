using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class ShiftRepository : BaseRepository<Shift>, IShiftRepository
    {
        public ShiftRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
        }

        public Task<Shift> GetbyId(int id, CancellationToken cancellationToken)
        {
            return Context.Shift.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
  
       

        public async Task<List<GetAllShiftResponse>> GetAllShift(GetAllShiftRequest request,  CancellationToken cancellationToken)
        {

            var shitData = Context.Shift.AsQueryable();
            if (request.status.HasValue)
            {
                shitData = shitData.Where(x => x.Active == request.status);
            }

            var response = await (from shiftQueryData in shitData
                                  join color in Context.Color on shiftQueryData.ColorId equals color.Id into colorData
                                  from color in colorData.DefaultIfEmpty()
                                  select new GetAllShiftResponse
                                  {
                                      Id = shiftQueryData.Id,
                                      Active = shiftQueryData.Active,
                                      Code = shiftQueryData.Code,
                                      ColorCode = color.Code,
                                      ColorId = shiftQueryData.ColorId,
                                      DateCreated = shiftQueryData.DateCreated,
                                      DateUpdated = shiftQueryData.DateUpdated,
                                      OnSite = shiftQueryData.OnSite,
                                      isDefault = shiftQueryData.isDefault,
                                      Description = shiftQueryData.Description,
                                      TransportGroup = shiftQueryData.TransportGroup
                                     

                                  }).OrderByDescending(x=> x.DateUpdated).ToListAsync();
            return response;    

        }


    }
}
