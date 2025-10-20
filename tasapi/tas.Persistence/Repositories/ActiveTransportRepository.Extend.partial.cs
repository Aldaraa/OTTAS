using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.ExtendActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetExtendActiveTransport;
using tas.Application.Features.RequestNonSiteTicketConfigFeature.ExtractOptioRequestNonSiteTicket;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;

namespace tas.Persistence.Repositories
{
    public partial class ActiveTransportRepository
    {


        




        public async Task Extend(ExtendActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.ActiveTransport.AsNoTracking().Where(x => x.Id == request.ActiveTransportId).FirstOrDefaultAsync();
            if (currentData != null)
            {
                if (currentData.Active == 1)
                {
                    var currfromLocationCode = await Context.Location.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentData.fromLocationId);
                    var currtoLocationCode = await Context.Location.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentData.toLocationId);

                    string fromLocationCode = currfromLocationCode?.Code ?? string.Empty;
                    string toLocationCode = currtoLocationCode?.Code ?? string.Empty;

                    var localdays = new List<string>();
                    localdays.Add(currentData.DayNum);

                    await CreateScheduleMain(request.ETD, request.ETA,
                           currentData.Code, currentData.Id, request.StartDate,
                           request.EndDate, request.Seats.Value, currentData.FrequencyWeeks,
                           currentData.DayNum, fromLocationCode, toLocationCode);

                    return;

                }
                else {
                    throw new BadRequestException("Only active actvetransport can be extended");
                }
            }
            else {
                throw new BadRequestException("Activetransport not found");
            }


            
        }


        private async Task CreateScheduleMain(
         string etd, string eta, string code,
        int activeTransportId, DateTime startDate, DateTime endDate, int seats, int frequencyWeeks,
        string dayNum, string fromLocationCode, string toLocationCode)
        {
            List<DayOfWeekNumber> daysOfWeek = new List<DayOfWeekNumber>();
                if (Enum.TryParse(dayNum, out DayOfWeekNumber dayOfWeek))
                {
                    daysOfWeek.Add(dayOfWeek);
                }

            DateTime currentDate = startDate;
            int aa = 0;

            while (currentDate <= endDate)
            {
                if (daysOfWeek.Contains((DayOfWeekNumber)currentDate.DayOfWeek))
                {

                    TransportSchedule schedule = new TransportSchedule
                    {
                        Code = code,
                        Description = string.Format("{0} {1} {2} {3}", etd, fromLocationCode, eta, toLocationCode),
                        EventDate = currentDate,
                        Seats = seats,
                        ETD = etd,
                        ETA = eta,
                        ActiveTransportId = activeTransportId,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id


                    };
                    Context.TransportSchedule.Add(schedule);
                    aa++;
                    //  context.SaveChangesAsync();
                }

                currentDate = currentDate.AddDays(1);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    currentDate = currentDate.AddDays(7 * (frequencyWeeks - 1));
                }
            }
            await Context.SaveChangesAsync();

        }




        #region GetExtendInfo

        public async Task<GetExtendActiveTransportResponse> GetExtendActiveTransport(GetExtendActiveTransportRequest request, CancellationToken cancellationToken)
        {
            var returData = await (from transport in Context.ActiveTransport.AsNoTracking().Where(x => x.Id == request.ActiveTransportId)
                                   join carrier in Context.Carrier.AsNoTracking() on transport.CarrierId equals carrier.Id into carrierData
                                   from carrier in carrierData.DefaultIfEmpty()
                                   join fromlocation in Context.Location.AsNoTracking() on transport.fromLocationId equals fromlocation.Id into fromlocationData
                                   from fromlocation in fromlocationData.DefaultIfEmpty()
                                   join tolocation in Context.Location.AsNoTracking() on transport.toLocationId equals tolocation.Id into tolocationData
                                   from tolocation in tolocationData.DefaultIfEmpty()
                                   join transportmode in Context.TransportMode.AsNoTracking() on transport.TransportModeId equals transportmode.Id into transportmodeData
                                   from transportmode in transportmodeData.DefaultIfEmpty()
                                   select new GetExtendActiveTransportResponse
                                   {
                                       Id = transport.Id,
                                       Code = transport.Code,
                                       Carrier = carrier.Description,
                                       DayNum = transport.DayNum,
                                       ETA = transport.ETA,
                                       ETD = transport.ETD,
                                       FromLocation = fromlocation.Description,
                                       ToLocation = tolocation.Description,
                                       Seat = transport.Seats,
                                       TransportMode = transportmode.Code   ,
                                       FrequencyWeeks = transport.FrequencyWeeks
                                       
                                   }).FirstOrDefaultAsync();

            if (returData != null)
            {
                var result =await Context.TransportSchedule
                            .Where(ts => ts.ActiveTransportId == request.ActiveTransportId)
                            .OrderByDescending(ts => ts.EventDate)
                            .FirstOrDefaultAsync();
                if (result != null)
                {
                    if (result.EventDate.Date.AddDays(7) > DateTime.Today)
                    {

                        returData.ScheduleStartDate = result.EventDate.Date.AddDays(7 * (returData.FrequencyWeeks ?? 1)).Date;
                    }
                    else {
                        returData.ScheduleStartDate = NextDayNumDate(returData.DayNum);
                    }
                    
                }
                else {

                    returData.ScheduleStartDate = NextDayNumDate(returData.DayNum);
                    
                }


                return returData;
            }
            else {
                throw new BadRequestException("Active transport not found");
            }


        }

        private DateTime NextDayNumDate(string? Daynum)
        {
            if (!string.IsNullOrWhiteSpace(Daynum))
            {
                if (Enum.TryParse(Daynum, out DayOfWeek dayOfWeek))
                {
                    DateTime today = DateTime.Today;
                    DateTime nextday = today.AddDays(((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7);

                    // If today is the same as the desired day, move to the next week
                    if (nextday == today)
                    {
                        nextday = nextday.AddDays(7);
                    }

                    return nextday;
                }
                else
                {
                    DateTime today = DateTime.Today;
                    DateTime nextMonday = today.AddDays(((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7);

                    if (nextMonday == today)
                    {
                        nextMonday = nextMonday.AddDays(7);
                    }

                    return nextMonday;
                }
            }
            else {
                DateTime today = DateTime.Today;
                DateTime nextMonday = today.AddDays(((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7);

                if (nextMonday == today)
                {
                    nextMonday = nextMonday.AddDays(7);
                }

                return nextMonday;
            }
          
        }




        #endregion


    }
}
