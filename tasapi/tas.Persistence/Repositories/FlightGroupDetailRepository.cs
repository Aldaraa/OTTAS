using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.FlightGroupDetailFeature.SetClusterFlightGroupDetail;
using tas.Application.Features.FlightGroupDetailFeature.UpdateShiftFlightGroupDetail;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class FlightGroupDetailRepository : BaseRepository<FlightGroupDetail>, IFlightGroupDetailRepository
    {
        public FlightGroupDetailRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {

        }


        public async Task SetCluster(SetClusterFlightGroupDetailRequest request, CancellationToken cancellationToken)
        {
            foreach (var item in request.request)
            {
                var itemData =await Context.FlightGroupDetail.FirstOrDefaultAsync(x => x.Id == item.FlightGroupDetailId);
                if (itemData != null)
                {
                    itemData.ClusterId = item.ClusterId;
                    Context.FlightGroupDetail.Update(itemData);
                }
            }
            Task.FromResult(0);
        }


        public async Task UpdateShift(UpdateShiftFlightGroupDetailRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.FlightGroupDetail.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            { 
                var currentShift = await Context.Shift.AsNoTracking().Where(c=> c.Id == request.shiftid).FirstOrDefaultAsync();
                if (currentShift != null)
                { 
                    currentData.ShiftId = currentShift.Id;
                    currentData.DateUpdated = DateTime.Now;
                    Context.FlightGroupDetail.Update(currentData);
                }
            }

        }


    }
}
