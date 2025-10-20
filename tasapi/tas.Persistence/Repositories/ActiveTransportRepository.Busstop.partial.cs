using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.UpdateBusstopActiveTransport;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class ActiveTransportRepository
    {
       public async Task UpdateBusstopActiveTransport(UpdateBusstopActiveTransportRequest request, CancellationToken cancellationToken) 
       {
          var currentSchedules =
                await Context.TransportSchedule.AsNoTracking().Where(x=> x.ActiveTransportId == request.Id && x.EventDate >= request.startDate && x.EventDate <= request.endDate).ToListAsync();
            foreach (var item in currentSchedules)
            {
                await SaveScheduleBusstop(item.Id, request);
            }

       }



        private async Task SaveScheduleBusstop(int ScheduleId, UpdateBusstopActiveTransportRequest request)
        {
            var newData = new List<TransportScheduleBusstop>();

            foreach (var item in request.Busstops)
            {
                var newRecord = new TransportScheduleBusstop()
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    Description = item.Description,
                    ETD = item.ETD,
                    ScheduleId = ScheduleId,
                };

                newData.Add(newRecord);
                    
            }

            if (newData.Count > 0) 
            {
                Context.TransportScheduleBusstop.AddRange(newData);
            }
        }  
       


        
    }
}
