using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.SafeModeTransportFeature.CreateTransport;
using tas.Application.Features.SafeModeTransportFeature.DeleteTransport;
using tas.Application.Features.SafeModeTransportFeature.UpdateTransport;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public partial class SafeModeTransportRepository : BaseRepository<Transport>, ISafeModeTransportRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly CacheService _memoryCache;
        public SafeModeTransportRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, ICheckDataRepository checkDataRepository, CacheService memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _checkDataRepository = checkDataRepository;
            _memoryCache = memoryCache;
        }


        #region CreateData


        public async Task CreateTransport(CreateTransportRequest request, CancellationToken cancellationToken)
        {
           var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null) 
            {
                var currentSchedule  = await Context.TransportSchedule.AsNoTracking().Where(x=> x.Id == request.ScheduleId).FirstOrDefaultAsync();
                if (currentSchedule != null)
                {
                    DateTime time = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                    int hours = time.Hour;
                    int minutes = time.Minute;

                    var transportcount = await Context.Transport.AsNoTracking().CountAsync(x => x.ScheduleId == request.ScheduleId);

                    var activeTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentSchedule.ActiveTransportId);

                    if (activeTransport != null)
                    {
                        var transportin = new Transport
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
                            Direction = activeTransport.Direction,
                            EmployerId = request.EmployerId,
                            ChangeRoute = "Add travel safemode",
                            Status = currentSchedule.Seats > transportcount ? "Confirmed" : "Over booked"


                        };

                        Context.Transport.Add(transportin);
                        await Context.SaveChangesAsync();

                    }
                    else {
                        throw new BadRequestException("ActiveTransport not found");
                    }
                }
                else {
                    throw new BadRequestException("Transport schedule not found");
                }

            }
            else{
                throw new BadRequestException("Employee not found");
            }

        }

        #endregion


        #region DeleteData

        public async Task<int?> DeleteTransport(DeleteTransportRequest request, CancellationToken cancellationToken)
        {
            var currentTransport = await Context.Transport.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (currentTransport == null)
            {
                throw new BadRequestException("Record not found");
            }

            int? scheduleId = currentTransport.ScheduleId;
            currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
            currentTransport.ChangeRoute = "remove travel safemode";
            Context.Transport.Remove(currentTransport);

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the transport record.", ex);
            }

            string cacheEntityName = $"Employee_{currentTransport.EmployeeId}";
            _memoryCache.Remove($"API::{cacheEntityName}");

            return scheduleId;
        }
        #endregion

        #region UpdateData

        public async Task<int?> UpdateTransport(UpdateTransportRequest request, CancellationToken cancellationToken)
        {
            var currentTransport = await Context.Transport.Where(x => x.Id == request.TransportId).FirstOrDefaultAsync();
            if (currentTransport != null)
            {
                int? oldScheduleId = currentTransport.ScheduleId;

                var currentSchedule = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == request.ScheduleId).FirstOrDefaultAsync();
                if (currentSchedule != null)
                {
                    var activeTransport = await Context.ActiveTransport.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentSchedule.ActiveTransportId);
                    if (activeTransport != null)
                    {

                        DateTime time = DateTime.ParseExact(currentSchedule.ETD, "HHmm", CultureInfo.InvariantCulture);
                        int hours = time.Hour;
                        int minutes = time.Minute;

                        currentTransport.EventDate = currentSchedule.EventDate.Date;
                        currentTransport.EventDateTime = currentSchedule.EventDate.Date.AddHours(hours).AddMinutes(minutes);
                        currentTransport.ScheduleId = request.ScheduleId;
                        currentTransport.ActiveTransportId = activeTransport.Id;
                        currentTransport.DateUpdated = DateTime.Now;
                        currentTransport.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        currentTransport.ChangeRoute = "Update travel safemode";
                        Context.Transport.Update(currentTransport);
                        await Context.SaveChangesAsync();

                        string cacheEntityName = $"Employee_{currentTransport.EmployeeId}";
                        _memoryCache.Remove($"API::{cacheEntityName}");

                        return oldScheduleId;
                    }
                    else
                    {
                        throw new BadRequestException("ActiveTransport not found");
                    }

                }
                else
                {
                    throw new BadRequestException("Transport schedule not found");
                }

            }
            else
            {
                throw new BadRequestException("Record not found");
            }
        }

        #endregion


    }
}
