using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.TransportFeature.AddExternalTravel;
using tas.Application.Features.TransportFeature.RemoveExternalTransport;
using tas.Application.Features.TransportFeature.ReScheduleExternalTransport;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class TransportRepository
    {
        #region RemoveExternalTransport

        public async Task RemoveExternalTransport(RemoveExternalTransportRequest request, CancellationToken cancellationToken)
        {
            var currentTransport = await Context.Transport.Where(x => x.Id == request.TransportId).FirstOrDefaultAsync();

            if (currentTransport != null)
            {
                currentTransport.ChangeRoute = "Remove External transport";
                currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser().Id;
                currentTransport.DateUpdated = DateTime.Now;
                Context.Transport.Remove(currentTransport);
            }
            else
            {
                throw new BadRequestException("Transport data not found");
            }
        }




        #endregion


        #region ReScheduleExternalTransport

        public async Task ReScheduleExternalTransport(ReScheduleExternalTransportRequest request, CancellationToken cancellationToken)
        {
            var oldData = await Context.Transport.Where(x => x.Id == request.oldTransportId).FirstOrDefaultAsync();
            if (oldData != null)
            {
                var newScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.ScheduleId).FirstOrDefaultAsync();
                if (newScheduleData != null)
                {

                    var currentLastActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == newScheduleData.ActiveTransportId);

                    if (currentLastActiveTransport != null)
                    {
                        DateTime time = DateTime.ParseExact(newScheduleData.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int hours = time.Hour;
                        int minutes = time.Minute;

                        var transportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == newScheduleData.Id);

                        oldData.ScheduleId = newScheduleData.Id;
                        oldData.ActiveTransportId = newScheduleData.ActiveTransportId;
                        oldData.EventDate = newScheduleData.EventDate.Date;
                        oldData.EventDateTime = newScheduleData.EventDate.Date.AddHours(hours).AddMinutes(minutes);
                        oldData.Status = newScheduleData.Seats > transportcount ? "Confirmed" : "Over booked";
                        oldData.DateUpdated = DateTime.Now;
                        oldData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;

                        Context.Transport.Update(oldData);
                    }
                }
            }

        }


        #endregion


        #region AddExternalTravel
        public async Task AddExternalTravel(AddExternalTravelRequest request, CancellationToken cancellationToken)
        {

            var currentSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.FirstSheduleId);
            if (currentSchedule != null)
            {
                var currentActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentSchedule.ActiveTransportId);

                if (currentActiveTransport != null)
                {
                    DateTime time = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                    int hours = time.Hour;
                    int minutes = time.Minute;

                    var transportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == currentSchedule.Id);

                    var transport = new Transport
                    {
                        EmployeeId = request.EmployeeId,
                        PositionId = request.PositionId,
                        DepId = request.DepartmentId,
                        CostCodeId = request.CostCodeId,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                        ActiveTransportId = currentSchedule.ActiveTransportId,
                        EventDate = currentSchedule.EventDate.Date,
                        EventDateTime = currentSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                        ScheduleId = currentSchedule.Id,
                        Active = 1,
                        Direction = currentActiveTransport.Direction,
                        EmployerId = request.EmployerId,
                        ChangeRoute = "Add travel profile",
                        Status = currentSchedule.Seats > transportcount ? "Confirmed" : "Over booked"


                    };

                    Context.Transport.Add(transport);

                    if (request.LastSheduleId.HasValue)
                    {
                        var currentLastSchedule = await Context.TransportSchedule.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.LastSheduleId);
                        if (currentLastSchedule != null)
                        {
                            var currentLastActiveTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentLastSchedule.ActiveTransportId);

                            if (currentLastActiveTransport != null)
                            {
                                DateTime lasttime = DateTime.ParseExact(currentLastSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                                int lasthours = time.Hour;
                                int lastminutes = time.Minute;

                                var lasttransportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == currentLastSchedule.Id);

                                var Lasttransport = new Transport
                                {
                                    EmployeeId = request.EmployeeId,
                                    PositionId = request.PositionId,
                                    DepId = request.DepartmentId,
                                    CostCodeId = request.CostCodeId,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    ActiveTransportId = currentLastSchedule.ActiveTransportId,
                                    EventDate = currentLastSchedule.EventDate.Date,
                                    EventDateTime = currentLastSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes),
                                    ScheduleId = currentLastSchedule.Id,
                                    Active = 1,
                                    Direction = currentLastActiveTransport.Direction,
                                    EmployerId = request.EmployerId,
                                    ChangeRoute = "Add travel profile",
                                    Status = currentSchedule.Seats > transportcount ? "Confirmed" : "Over booked"
                                };


                                Context.Transport.Add(Lasttransport);

                            }

                        }
                    }
                }


            }


        }


        #endregion


    }
}
