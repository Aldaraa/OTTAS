using Azure.Core;
using MediatR;
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

    public class TransportScheduleCalculateRepository : BaseRepository<TransportSchedule>, ITransportScheduleCalculateRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public TransportScheduleCalculateRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
        }


        public async Task CalculateByScheduleId(int scheduleId, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch transport data only if the schedule and seats are available
                var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId)
                                             join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id into activetransportData
                                             from activetransport in activetransportData.DefaultIfEmpty()
                                             join transportmode in Context.TransportMode.AsNoTracking() on activetransport.TransportModeId equals transportmode.Id into modeData
                                             from transportmode in modeData.DefaultIfEmpty()
                                             select new
                                             {
                                                 Id = schedule.Id,
                                                 Seats = schedule.Seats,
                                                 Mode = transportmode.Code
                                             }).FirstOrDefaultAsync();

                if (currentSchedule == null || currentSchedule.Mode == "DRIVE" || currentSchedule.Mode == "drive")
                    return;

                // Fetch only the necessary fields in employeeTransports
                var employeeTransports = await Context.Transport
                    .Where(x => x.ScheduleId == scheduleId)
                    .OrderBy(x => x.DateCreated)
                    .Select(x => new { x.Id, x.Status })
                    .ToListAsync();

                if (employeeTransports.Count == 0)
                    return;

                int currentSeatIndex = 0;
                foreach (var item in employeeTransports)
                {
                    currentSeatIndex++;
                    string status = currentSchedule.Seats >= currentSeatIndex ? "Confirmed" : "Over Booked";

                    // Update the transport status using bulk update approach
                    await Context.Transport
                        .Where(x => x.Id == item.Id)
                        .ExecuteUpdateAsync(t => t.SetProperty(t => t.Status, status));
                }
            }
            catch (Exception ex)
            {

                // Log or handle the exception as appropriate
            }
        }



    //    public async Task CalculateByScheduleId(int ScheduleId, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            var employeeTransports = await Context.Transport
    //         .Where(x => x.ScheduleId == ScheduleId).OrderBy(x => x.DateCreated).ToListAsync();

    //            if (employeeTransports.Count == 0)
    //                return;

    //            var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == ScheduleId)
    //                                         join activetransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activetransport.Id into activetransportData
    //                                         from activetransport in activetransportData.DefaultIfEmpty()
    //                                         join transportmode in Context.TransportMode on activetransport.TransportModeId equals transportmode.Id into modeData
    //                                         from transportmode in modeData.DefaultIfEmpty()
    //                                         select new
    //                                         {
    //                                             Id = schedule.Id,
    //                                             Seats = schedule.Seats,
    //                                             Mode = transportmode.Code

    //                                         }).FirstOrDefaultAsync();



    //            int currentSeatIndex = 0;
    //            if (currentSchedule != null)
    //            {
    //                if (currentSchedule.Mode != "DRIVE")
    //                {
    //                    foreach (var item in employeeTransports)
    //                    {
    //                        currentSeatIndex++;
    //                        if (currentSchedule.Seats >= currentSeatIndex)
    //                        {
    //                            item.Status = "Confirmed";
    //                        }
    //                        else
    //                        {
    //                            item.Status = "Over Booked";
    //                        }


    //                        Context.Transport.Update(item);
    //                    }

    //                    await Context.SaveChangesAsync();
    //                }
    //            }
    //        }
    //        catch (Exception)
    //        {

    //        }

         

          
    //    }



    }

}
