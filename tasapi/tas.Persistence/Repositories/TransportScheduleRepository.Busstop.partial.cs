using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.BusstopFeature.UpdateBusstop;
using tas.Application.Features.TransportScheduleFeature.GetScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.RemoveScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.UpdateScheduleBusstop;
using tas.Application.Repositories;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class TransportScheduleRepository
    {
        public async Task UpdateScheduleScheduleBusstop(UpdateScheduleBusstopRequest request, CancellationToken cancellationToken)
        {

            var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(c => c.Id == request.Id).FirstOrDefaultAsync();
            if (currentSchedule != null)
            {
                await RemoveBeforeBusstop(request.Id);
                var newData = new List<TransportScheduleBusstop>();

                foreach (var item in request.Busstops)
                {
                    var newRecord = new TransportScheduleBusstop()
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        Description = item.Description,
                        ETD = item.ETD,
                        ScheduleId = request.Id,
                    };

                    newData.Add(newRecord);

                }

                if (newData.Count > 0)
                {
                    Context.TransportScheduleBusstop.AddRange(newData);
                }
            }else
            {
                throw new BadRequestException("Transport schedule not found");

            }



        }


        private async Task RemoveBeforeBusstop(int scheduleId)
        {

            var oldata =await Context.TransportScheduleBusstop.Where(x=> x.ScheduleId == scheduleId).ToListAsync();
            if (oldata.Count > 0)
            {
                Context.TransportScheduleBusstop.RemoveRange(oldata);
            }
            
        }



        public async Task<List<GetScheduleBusstopResponse>> GetScheduleBusstop(GetScheduleBusstopRequest request, CancellationToken cancellationToken)
        { 
            return await   Context.TransportScheduleBusstop.Where(x=> x.ScheduleId == request.ScheduleId).Select (x => new GetScheduleBusstopResponse
            {
                Description = x.Description,
                ETD = x.ETD,
                Id = x.Id

            }).ToListAsync();

        }



        public async Task RemoveScheduleScheduleBusstop(RemoveScheduleBusstopRequest request, CancellationToken cancellationToken) 
        {
            var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(c => c.Id == request.Id).FirstOrDefaultAsync();
            if (currentSchedule != null)
            {
                await RemoveBeforeBusstop(request.Id);
                
            }
            else
            {
                throw new BadRequestException("Transport schedule not found");

            }

        }


    }
}
