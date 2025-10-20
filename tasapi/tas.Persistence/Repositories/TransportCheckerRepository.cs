using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class TransportCheckerRepository : BaseRepository<Transport>, ITransportCheckerRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly IMemoryCache _memoryCache;
        public TransportCheckerRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, ICheckDataRepository checkDataRepository, IMemoryCache memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _checkDataRepository = checkDataRepository;
            _memoryCache = memoryCache;
        }

        public async Task TransportAddValidDirectionSequenceCheck(int employeeId, int INScheduleId, int outScheduleData)
        {
            var inschedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == INScheduleId)
                                    join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
                                    select new
                                    {
                                        Id = schedule.Id,
                                        ActiveTransportId = activeTransport.Id,
                                        ETD = schedule.ETD,
                                        ETA = schedule.ETA,
                                        EventDate = schedule.EventDate,
                                        Direction = activeTransport.Direction,
                                    }).FirstOrDefaultAsync();

            var outchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == outScheduleData)
                                    join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
                                    select new
                                    {
                                        Id = schedule.Id,
                                        ActiveTransportId = activeTransport.Id,
                                        ETD = schedule.ETD,
                                        ETA = schedule.ETA,
                                        EventDate = schedule.EventDate,
                                        Direction = activeTransport.Direction,
                                    }).FirstOrDefaultAsync();

            if (inschedule != null && outchedule != null)
            {





                DateTime inScheduleDateTime = inschedule.EventDate.Date.Add(DateTime.ParseExact(inschedule.ETD, "HHmm", null).TimeOfDay);
                DateTime inScheduleArrivalDateTime = inschedule.EventDate.Date.Add(DateTime.ParseExact(inschedule.ETA, "HHmm", null).TimeOfDay);

                DateTime outScheduleDateTime = outchedule.EventDate.Date.Add(DateTime.ParseExact(outchedule.ETD, "HHmm", null).TimeOfDay);
                DateTime outScheduleArrivalDateTime = outchedule.EventDate.Date.Add(DateTime.ParseExact(outchedule.ETA, "HHmm", null).TimeOfDay);


                var beforeTransport = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.EventDateTime < inScheduleDateTime).OrderByDescending(x=> x.EventDateTime).FirstOrDefaultAsync();
                var nextTransport = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.EventDateTime > inScheduleDateTime).OrderBy(x=> x.EventDateTime).FirstOrDefaultAsync();

                if (beforeTransport == null)
                {
                    if (inScheduleDateTime >= outScheduleDateTime || inScheduleArrivalDateTime >= outScheduleArrivalDateTime || inScheduleArrivalDateTime >= outScheduleDateTime)
                    {
                        throw new BadRequestException("The 'IN' schedule's departure or arrival time cannot be after the 'OUT' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                    }
                }
                else {

                    if (beforeTransport.Direction == "IN")
                    {
                        if (inScheduleDateTime <= outScheduleDateTime || inScheduleArrivalDateTime <= outScheduleArrivalDateTime || inScheduleArrivalDateTime <= outScheduleDateTime)
                        {
                            throw new BadRequestException("The 'IN' schedule's departure or arrival time cannot be after the 'OUT' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                        }
                    }
                    else if (beforeTransport.Direction == "OUT")
                    {
                        if (inScheduleDateTime >= outScheduleDateTime || inScheduleArrivalDateTime >= outScheduleArrivalDateTime || inScheduleArrivalDateTime >= outScheduleDateTime)
                        {
                            throw new BadRequestException("The 'IN' schedule's departure or arrival time cannot be after the 'OUT' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                        }
                    }

                    else
                    {
                        if (inScheduleDateTime >= outScheduleDateTime || inScheduleArrivalDateTime >= outScheduleArrivalDateTime || inScheduleArrivalDateTime >= outScheduleDateTime)
                        {
                            throw new BadRequestException("The 'IN' schedule's departure or arrival time cannot be before the 'OUT' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                        }
                    }

                }




                //if (inschedule.EventDate.Date == outchedule.EventDate.Date)
                //{
                //    if (beforeTransport == null || beforeTransport.Direction == "IN")
                //    {
                //        if (inScheduleDateTime > outScheduleDateTime || inScheduleArrivalDateTime > outScheduleArrivalDateTime || inScheduleArrivalDateTime > outScheduleDateTime)
                //        {
                //            throw new BadRequestException("The 'IN' schedule's departure or arrival time cannot be after the 'OUT' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                //        }
                //    }
                //    else {


                //        if (beforeTransport.Direction == "OUT")
                //        {
                //            if (inScheduleDateTime > outScheduleDateTime || inScheduleArrivalDateTime > outScheduleArrivalDateTime || inScheduleArrivalDateTime > outScheduleDateTime)
                //            {
                //                throw new BadRequestException("The 'IN' schedule's departure or arrival time cannot be after the 'OUT' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                //            }
                //        }
                //        else {
                //            if (inScheduleDateTime < outScheduleDateTime || inScheduleArrivalDateTime < outScheduleArrivalDateTime || inScheduleArrivalDateTime < outScheduleDateTime)
                //            {
                //                throw new BadRequestException("The 'IN' schedule's departure or arrival time cannot be after the 'OUT' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                //            }
                //        }
                //    }



                //}




                if (inScheduleDateTime < outScheduleDateTime)
                {
                    var existingRecord = await Context.Transport.AsNoTracking()
                        .Where(t => t.EmployeeId == employeeId && t.EventDateTime <= inScheduleDateTime)
                        .OrderByDescending(t => t.EventDateTime)
                        .FirstOrDefaultAsync();




                    if (existingRecord != null)
                    {
                        if ((existingRecord.Direction == "IN" && inschedule.Direction == "IN") ||
                            (existingRecord.Direction == "OUT" && inschedule.Direction == "OUT"))
                        {
                            throw new BadRequestException("Invalid direction sequence. The direction 'IN' must be followed by 'OUT', and vice versa.");
                        }
                    }



                    var nextRecord = await Context.Transport.AsNoTracking()
                             .Where(t => t.EmployeeId == employeeId && t.EventDateTime >= inScheduleDateTime && t.EventDateTime <= outScheduleDateTime)
                             .OrderBy(t => t.EventDateTime)
                             .FirstOrDefaultAsync();

                    if (nextRecord != null)
                    {
                        if (nextRecord.Direction == "IN" && inschedule.Direction == "IN")
                        {
                            throw new BadRequestException("Invalid direction sequence. The direction 'IN' must be followed by 'OUT', and vice versa.");
                        }
                    }



                }
                else
                {
                    var existingRecord = await Context.Transport.AsNoTracking()
                        .Where(t => t.EmployeeId == employeeId && t.EventDateTime <= outScheduleDateTime)
                        .OrderByDescending(t => t.EventDateTime)
                        .FirstOrDefaultAsync();


                    var nextRecord = await Context.Transport.AsNoTracking()
                        .Where(t => t.EmployeeId == employeeId && t.EventDateTime >= outScheduleDateTime && t.EventDateTime <= inScheduleDateTime)
                        .OrderBy(t => t.EventDateTime)
                        .FirstOrDefaultAsync();


                    if (existingRecord != null)
                    {
                        if ((existingRecord.Direction == "IN" && outchedule.Direction == "IN") ||
                            (existingRecord.Direction == "OUT" && outchedule.Direction == "OUT"))
                        {
                            throw new BadRequestException("Invalid direction sequence. The direction 'IN' must be followed by 'OUT', and vice versa.");
                        }
                    }

                    if (nextRecord != null)
                    {
                        if ((nextRecord.Direction == "IN" && outchedule.Direction == "IN") ||
                            (nextRecord.Direction == "OUT" && outchedule.Direction == "OUT"))
                        {
                            throw new BadRequestException("Invalid direction sequence. The direction 'IN' must be followed by 'OUT', and vice versa.");
                        }
                    }
                }
            }
            else
            {
                throw new BadRequestException("Schedule not found");
            }
        }



        public async Task   TransportRescheduleValidDirectionSequenceCheck(int oldtransportId, int scheduleId)
        {

             var currenttransport = await Context.Transport.AsNoTracking().Where(x => x.Id == oldtransportId).FirstOrDefaultAsync();
            if (currenttransport != null)
            {
                var dailyTripData =await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == currenttransport.EmployeeId && x.EventDate.Value.Date == currenttransport.EventDate.Value.Date && x.Id != currenttransport.Id).FirstOrDefaultAsync();
                if (dailyTripData != null)
                {
                    throw new BadRequestException($"Employee has a scheduled trip on {dailyTripData.EventDateTime}. Unable to reschedule transport. Please delete the existing day trip first.");
                }

            }
            else {
                throw new BadRequestException("Existing transport not found");
            }
            var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId)
                                         join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
                                         select new
                                         {
                                             Id = schedule.Id,
                                             ActiveTransportId = activeTransport.Id,
                                             EventDate = schedule.EventDate,
                                             ETD = schedule.ETD,
                                             ETA = schedule.ETA,
                                             Direction = activeTransport.Direction,
                                         }).FirstOrDefaultAsync();

            var currentTransport = await Context.Transport.AsNoTracking().Where(x => x.Id == oldtransportId).FirstOrDefaultAsync();
            if (currentTransport != null)
            {
                if (currentSchedule != null)
                {
                    DateTime currentScheduleDepartureDateTime = currentSchedule.EventDate.Date.Add(DateTime.ParseExact(currentSchedule.ETD, "HHmm", null).TimeOfDay);
                    DateTime currentScheduleArrivalDateTime = currentSchedule.EventDate.Date.Add(DateTime.ParseExact(currentSchedule.ETA, "HHmm", null).TimeOfDay);

                    // Check for records after the updated schedule's departure time
                    var recordsAfterUpdate = await Context.Transport.AsNoTracking()
                        .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime >= currentScheduleDepartureDateTime && t.ScheduleId != currentTransport.ScheduleId)
                        .OrderBy(t => t.EventDateTime)
                        .FirstOrDefaultAsync();

                    if (recordsAfterUpdate != null)
                    {
                        if ((currentSchedule.Direction == "IN" && recordsAfterUpdate.Direction == "IN") ||
                            (currentSchedule.Direction == "OUT" && recordsAfterUpdate.Direction == "OUT"))
                        {
                            throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                        }
                    }

                    // Check for records before the updated schedule's departure time
                    var recordsBeforeUpdate = await Context.Transport.AsNoTracking()
                        .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime <= currentScheduleArrivalDateTime && t.ScheduleId != currentTransport.ScheduleId)
                        .OrderByDescending(t => t.EventDateTime)
                        .FirstOrDefaultAsync();

                    var externalData =await Context.Transport.AsNoTracking().Where(x => x.Direction == "EXTERNAL" && x.EventDate.Value.Date == currentSchedule.EventDate.Date && x.EmployeeId == currentTransport.EmployeeId).ToListAsync();
                    if (externalData.Count > 0)
                    {
                        foreach (var item in externalData)
                        {
                            var itemScheduleData =await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == item.ScheduleId).FirstOrDefaultAsync();
                            if (itemScheduleData != null)
                            {
                                DateTime itemScheduleDepartureDateTime = itemScheduleData.EventDate.Date.Add(DateTime.ParseExact(itemScheduleData.ETD, "HHmm", null).TimeOfDay);
                                DateTime itemScheduleArrivalDateTime = itemScheduleData.EventDate.Date.Add(DateTime.ParseExact(itemScheduleData.ETA, "HHmm", null).TimeOfDay);

                                if (itemScheduleDepartureDateTime > currentScheduleDepartureDateTime || itemScheduleDepartureDateTime > currentScheduleArrivalDateTime || itemScheduleArrivalDateTime > currentScheduleDepartureDateTime || itemScheduleArrivalDateTime > currentScheduleArrivalDateTime )
                                {
                                    throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                                }
                            }
                        }
                    }


                    if (recordsBeforeUpdate != null)
                    {
                        if ((recordsBeforeUpdate.Direction == "IN" && currentSchedule.Direction == "IN") ||
                            (recordsBeforeUpdate.Direction == "OUT" && currentSchedule.Direction == "OUT"))
                        {
                            throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                        }
                    }
                }
                else
                {
                    throw new BadRequestException("Schedule not found");
                }
            }
            else
            {
                throw new BadRequestException("Old Transport not found");
            }
        }


        /// <summary>
        /// TransportRescheduleValidDirectionSequenceCheck function like return value diff
        /// </summary>
        /// <param name="oldtransportId"></param>
        /// <param name="scheduleId"></param>
        /// <returns></returns>

        public async Task<bool> TransportRescheduleValidDirectionSequenceCheckStatus(int oldtransportId, int scheduleId)
        {

            var currenttransport = await Context.Transport.AsNoTracking().Where(x => x.Id == oldtransportId).FirstOrDefaultAsync();
            if (currenttransport != null)
            {
                var dailyTripData = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == currenttransport.EmployeeId && x.EventDate.Value.Date == currenttransport.EventDate.Value.Date && x.Id != currenttransport.Id).FirstOrDefaultAsync();
                if (dailyTripData != null)
                {
                    // throw new BadRequestException($"Employee has a scheduled trip on {dailyTripData.EventDateTime}. Unable to reschedule transport. Please delete the existing daily trip first.");
                    return false;
                }

            }
            else
            {
                // throw new BadRequestException("Existing transport not found");
                return false;
            }
            var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId)
                                         join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
                                         select new
                                         {
                                             Id = schedule.Id,
                                             ActiveTransportId = activeTransport.Id,
                                             EventDate = schedule.EventDate,
                                             ETD = schedule.ETD,
                                             ETA = schedule.ETA,
                                             Direction = activeTransport.Direction,
                                         }).FirstOrDefaultAsync();

            var currentTransport = await Context.Transport.AsNoTracking().Where(x => x.Id == oldtransportId).FirstOrDefaultAsync();
            if (currentTransport == null)
            {
                return false;
            }

            DateTime currentScheduleDepartureDateTime = currentSchedule.EventDate.Date.Add(DateTime.ParseExact(currentSchedule.ETD, "HHmm", null).TimeOfDay);
            DateTime currentScheduleArrivalDateTime = currentSchedule.EventDate.Date.Add(DateTime.ParseExact(currentSchedule.ETA, "HHmm", null).TimeOfDay);

            // Check for records after the updated schedule's departure time
            var recordsAfterUpdate = await Context.Transport.AsNoTracking()
                .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime >= currentScheduleDepartureDateTime && t.ScheduleId != currentTransport.ScheduleId)
                .OrderBy(t => t.EventDateTime)
                .FirstOrDefaultAsync();

            if (recordsAfterUpdate != null)
            {
                if ((currentSchedule.Direction == "IN" && recordsAfterUpdate.Direction == "IN") ||
                    (currentSchedule.Direction == "OUT" && recordsAfterUpdate.Direction == "OUT"))
                {
                    //throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                    return false;
                }
            }

            // Check for records before the updated schedule's departure time
            var recordsBeforeUpdate = await Context.Transport.AsNoTracking()
                .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime <= currentScheduleArrivalDateTime && t.ScheduleId != currentTransport.ScheduleId)
                .OrderByDescending(t => t.EventDateTime)
                .FirstOrDefaultAsync();

            var externalData = await Context.Transport.AsNoTracking().Where(x => x.Direction == "EXTERNAL" && x.EventDate.Value.Date == currentSchedule.EventDate.Date && x.EmployeeId == currentTransport.EmployeeId).ToListAsync();
            if (externalData.Count > 0)
            {
                foreach (var item in externalData)
                {
                    var itemScheduleData = await Context.TransportSchedule.AsNoTracking().Where(x => x.Id == item.ScheduleId).FirstOrDefaultAsync();
                    if (itemScheduleData != null)
                    {
                        DateTime itemScheduleDepartureDateTime = itemScheduleData.EventDate.Date.Add(DateTime.ParseExact(itemScheduleData.ETD, "HHmm", null).TimeOfDay);
                        DateTime itemScheduleArrivalDateTime = itemScheduleData.EventDate.Date.Add(DateTime.ParseExact(itemScheduleData.ETA, "HHmm", null).TimeOfDay);

                        if (itemScheduleDepartureDateTime > currentScheduleDepartureDateTime || itemScheduleDepartureDateTime > currentScheduleArrivalDateTime || itemScheduleArrivalDateTime > currentScheduleDepartureDateTime || itemScheduleArrivalDateTime > currentScheduleArrivalDateTime)
                        {
                            // throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                            return false;
                        }
                    }
                }
            }


            if (recordsBeforeUpdate != null)
            {
                if ((recordsBeforeUpdate.Direction == "IN" && currentSchedule.Direction == "IN") ||
                    (recordsBeforeUpdate.Direction == "OUT" && currentSchedule.Direction == "OUT"))
                {
                    //throw new BadRequestException("Updating this record would result in an invalid direction sequence.");

                    return false;

                }
            }


            return true;

        }






        public async Task TransportUpdateValidDirectionSequenceCheck(int transportId, int scheduleId)
        {
            var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId)
                                         join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
                                         select new
                                         {
                                             Id = schedule.Id,
                                             ActiveTransportId = activeTransport.Id,
                                             ETD = schedule.ETD,
                                             EventDate = schedule.EventDate,
                                             Direction = activeTransport.Direction,
                                         }).FirstOrDefaultAsync();

            var currentTransport = await Context.Transport.AsNoTracking().Where(x => x.Id == transportId).FirstOrDefaultAsync();
            if (currentTransport != null)
            {
                if (currentSchedule != null)
                {
                    DateTime currentScheduleDateTime = currentSchedule.EventDate.Date.Add(DateTime.ParseExact(currentSchedule.ETD, "HHmm", null).TimeOfDay);

                    var recordsAfterUpdate = await Context.Transport
                        .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime >= currentScheduleDateTime && t.ScheduleId != currentTransport.ScheduleId)
                        .OrderBy(t => t.EventDateTime)
                        .FirstOrDefaultAsync();

                    if (recordsAfterUpdate != null)
                    {
                        if ((currentSchedule.Direction == "IN" && recordsAfterUpdate.Direction == "IN") ||
                            (currentSchedule.Direction == "OUT" && recordsAfterUpdate.Direction == "OUT"))
                        {
                            throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                        }
                    }

                    var recordsBeforeUpdate = await Context.Transport
                        .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime <= currentScheduleDateTime && t.ScheduleId != currentTransport.ScheduleId)
                        .OrderByDescending(t => t.EventDateTime)
                        .FirstOrDefaultAsync();

                    if (recordsBeforeUpdate != null)
                    {
                        if ((recordsBeforeUpdate.Direction == "IN" && currentSchedule.Direction == "IN") ||
                            (recordsBeforeUpdate.Direction == "OUT" && currentSchedule.Direction == "OUT"))
                        {
                            throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                        }
                    }
                }
                else
                {
                    throw new BadRequestException("Schedule not found");
                }
            }
            else
            {
                throw new BadRequestException("Old Transport not found");
            }
        }



        public async Task TransportUpdateValidDirectionSequenceCheckForRequest(int oldScheduleId, int scheduleId, int employeeId)
        {
            var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId)
                                         join activeTransport in Context.ActiveTransport on schedule.ActiveTransportId equals activeTransport.Id
                                         select new
                                         {
                                             Id = schedule.Id,
                                             ActiveTransportId = activeTransport.Id,
                                             EventDate = schedule.EventDate,
                                             ETD = schedule.ETD,
                                             Direction = activeTransport.Direction,
                                         }).FirstOrDefaultAsync();

            var currentTransport = await Context.Transport.AsNoTracking().Where(x => x.ScheduleId == oldScheduleId && x.EmployeeId == employeeId).FirstOrDefaultAsync();
            if (currentTransport != null)
            {
                if (currentSchedule != null)
                {
                    DateTime currentScheduleDateTime = currentSchedule.EventDate.Date.Add(DateTime.ParseExact(currentSchedule.ETD, "HHmm", null).TimeOfDay);

                    var recordsAfterUpdate = await Context.Transport
                        .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime >= currentScheduleDateTime && t.ScheduleId != currentTransport.ScheduleId)
                        .OrderBy(t => t.EventDateTime)
                        .FirstOrDefaultAsync();

                    if (recordsAfterUpdate != null)
                    {
                        if ((currentSchedule.Direction == "IN" && recordsAfterUpdate.Direction == "IN") ||
                            (currentSchedule.Direction == "OUT" && recordsAfterUpdate.Direction == "OUT"))
                        {
                            throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                        }
                    }

                    var recordsBeforeUpdate = await Context.Transport.AsNoTracking()
                        .Where(t => t.EmployeeId == currentTransport.EmployeeId && t.EventDateTime <= currentScheduleDateTime && t.ScheduleId != currentTransport.ScheduleId)
                        .OrderByDescending(t => t.EventDateTime)
                        .FirstOrDefaultAsync();

                    if (recordsBeforeUpdate != null)
                    {
                        if ((recordsBeforeUpdate.Direction == "IN" && currentSchedule.Direction == "IN") ||
                            (recordsBeforeUpdate.Direction == "OUT" && currentSchedule.Direction == "OUT"))
                        {
                            throw new BadRequestException("Updating this record would result in an invalid direction sequence.");
                        }
                    }
                }
                else
                {
                    throw new BadRequestException("Schedule not found");
                }
            }
            else
            {
                throw new BadRequestException("Old Transport not found");
            }
        }


        #region ExternalAddTravelCheck

        public async Task TransportExternalAddValidCheck(int EmployeeId, int firstScheduleId, int? lastScheduleId)
        {

            var currentEmployee = await GetEmployeeAsync(EmployeeId);
            ValidateEmployee(currentEmployee);
            if (lastScheduleId.HasValue)
            {
                var lastScheduleData = await GetScheduleTransportDataAsync(lastScheduleId.Value);
                var firstScheduleData = await GetScheduleTransportDataAsync(firstScheduleId);

                if (firstScheduleData != null && lastScheduleData != null)
                {
                    await ValidateScheduleAsync(EmployeeId, lastScheduleData, firstScheduleData.Direction);


                    DateTime firstScheduleDateTime = firstScheduleData.EventDate.Date.Add(DateTime.ParseExact(firstScheduleData.ETD, "HHmm", null).TimeOfDay);
                    DateTime firstScheduleArrivalDateTime = firstScheduleData.EventDate.Date.Add(DateTime.ParseExact(firstScheduleData.ETA, "HHmm", null).TimeOfDay);

                    DateTime lastScheduleDateTime = lastScheduleData.EventDate.Date.Add(DateTime.ParseExact(lastScheduleData.ETD, "HHmm", null).TimeOfDay);
                    DateTime lastScheduleArrivalDateTime = lastScheduleData.EventDate.Date.Add(DateTime.ParseExact(lastScheduleData.ETA, "HHmm", null).TimeOfDay);


                    if (firstScheduleDateTime.Date == lastScheduleData.EventDate.Date)
                    {
                        if (firstScheduleDateTime > lastScheduleDateTime || firstScheduleArrivalDateTime > lastScheduleDateTime)
                        {
                            throw new BadRequestException("The 'First' schedule's departure or arrival time cannot be after the 'Last' schedule's departure or arrival time on the same day. Please correct the schedule times.");
                        }
                    }


                    var dateAnotherTransport = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == EmployeeId && x.EventDate.Value.Date == firstScheduleDateTime.Date).FirstOrDefaultAsync();

                    if (dateAnotherTransport != null)
                    {
                        if (dateAnotherTransport.EventDateTime >= firstScheduleDateTime || dateAnotherTransport.EventDateTime >= firstScheduleArrivalDateTime)
                        {
                            throw new BadRequestException("If the flight time overlaps, please choose another time");
                        }

                        if (dateAnotherTransport.EventDateTime > lastScheduleDateTime || dateAnotherTransport.EventDateTime > lastScheduleArrivalDateTime)
                        {
                            throw new BadRequestException("If the flight time overlaps, please choose another time");
                        }
                    }

                    var nextTransport = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == EmployeeId && x.EventDateTime < firstScheduleDateTime).OrderByDescending(x => x.EventDateTime).FirstOrDefaultAsync();





                    if (firstScheduleData.EventDate.Date > lastScheduleData.EventDate.Date)
                    {
                        throw new BadRequestException("The first schedule date cannot be after the last schedule date.");
                    }
                }
                else {
                    throw new BadRequestException("Schedule data not found");
                }



            }
            else
            {
                var firstScheduleData = await GetScheduleTransportDataAsync(firstScheduleId);
                await ValidateScheduleAsync(EmployeeId, firstScheduleData, firstScheduleData.Direction);

                DateTime firstScheduleDateTime = firstScheduleData.EventDate.Date.Add(DateTime.ParseExact(firstScheduleData.ETD, "HHmm", null).TimeOfDay);
                DateTime firstScheduleArrivalDateTime = firstScheduleData.EventDate.Date.Add(DateTime.ParseExact(firstScheduleData.ETA, "HHmm", null).TimeOfDay);


                var dateAnotherTransport = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == EmployeeId && x.EventDate.Value.Date == firstScheduleDateTime.Date).FirstOrDefaultAsync();

                if (dateAnotherTransport != null)
                {
                    if (dateAnotherTransport.EventDateTime >= firstScheduleDateTime || dateAnotherTransport.EventDateTime >= firstScheduleArrivalDateTime)
                    {
                        throw new BadRequestException("If the flight time overlaps, please choose another time");
                    }
                }





            }

            return;
        }


        private async Task ValidateScheduleAsync(int employeeId, GetExternalScheduleTransportData scheduleData, string direction)
        {
            if (scheduleData == null)
                throw new BadRequestException("Schedule not found. Please choose a valid schedule.");

            var onsiteStatus = await Context.EmployeeStatus
                .Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date == scheduleData.EventDate && x.RoomId != null)
                .FirstOrDefaultAsync();

            if (onsiteStatus != null)
                throw new BadRequestException("The selected employee is currently on-site and cannot be scheduled for external travel.");



            if (direction != "EXTERNAL")
                throw new BadRequestException("Only external direction schedules are allowed. Please choose a valid schedule.");
        }


        private async Task<GetExternalScheduleTransportData?> GetScheduleTransportDataAsync(int scheduleId)
        {
            var currentTransportData = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == scheduleId)
                                          join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
                                          select new GetExternalScheduleTransportData
                                          {
                                              ScheduleId = schedule.Id,
                                              ActiveTransportId = activeTransport.Id,
                                              EventDate = schedule.EventDate,
                                              ETD = schedule.ETD,   
                                              ETA = schedule.ETA,
                                              Direction = activeTransport.Direction
                                          }).FirstOrDefaultAsync();
            return currentTransportData;


        }


        private async Task<Employee?> GetEmployeeAsync(int employeeId)
        {
            return await Context.Employee.AsNoTracking().FirstOrDefaultAsync(x => x.Id == employeeId);
        }

        private void ValidateEmployee(Employee? employee)
        {
            if (employee == null)
                throw new BadRequestException("Employee not found. Please contact Administration team.");

            if (employee.Active != 1)
                throw new BadRequestException("Employee is deactivated. Please contact the Administration team.");
        }


        #endregion


        public async Task TransportExternalRescheduleValidCheck(int oldTransportId, int ScheduleId)
        {
            var oldData = await Context.Transport.AsNoTracking().Where(x => x.Id == oldTransportId).FirstOrDefaultAsync();
            if (oldData != null)
            {
                if (!oldData.EmployeeId.HasValue)
                {
                    throw new BadRequestException("Transport data not found");
                }

                var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == ScheduleId)
                                             join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
                                             select new
                                             {
                                                 Id = schedule.Id,
                                                 ActiveTransportId = activeTransport.Id,
                                                 EventDate = schedule.EventDate,
                                                 ETD = schedule.ETD,
                                                 Direction = activeTransport.Direction,
                                             }).FirstOrDefaultAsync();
                if (currentSchedule != null)
                {
                    DateTime currentScheduleDateTime = currentSchedule.EventDate.Date.Add(DateTime.ParseExact(currentSchedule.ETD, "HHmm", null).TimeOfDay);

                    if (currentSchedule.Direction == "EXTERNAL")
                    {
                        var employeeOnsiteData = await Context.EmployeeStatus.AsNoTracking()
                            .Where(x => x.EmployeeId == oldData.EmployeeId && x.EventDate == currentSchedule.EventDate && x.RoomId != null)
                            .FirstOrDefaultAsync();
                        if (employeeOnsiteData != null)
                        {
                            throw new BadRequestException("The selected employee is currently on-site and cannot be scheduled for external travel.");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        throw new BadRequestException("Only external direction schedules are allowed. Please choose a valid schedule.");
                    }
                }
                else
                {
                    throw new BadRequestException("New transport schedule data not found");
                }
            }
            else
            {
                throw new BadRequestException("Old transport data not found");
            }
        }

        //public async Task TransportExternalRescheduleValidCheck(int oldTransportId, int ScheduleId)
        //{

        //    var oldData = await Context.Transport.AsNoTracking().Where(x => x.Id == oldTransportId).FirstOrDefaultAsync();
        //    if (oldData != null)
        //    {
        //        if (!oldData.EmployeeId.HasValue)
        //        {
        //            throw new BadRequestException("Transport data not found");
        //        }


        //        var currentSchedule = await (from schedule in Context.TransportSchedule.AsNoTracking().Where(x => x.Id == ScheduleId)
        //                                     join activeTransport in Context.ActiveTransport.AsNoTracking() on schedule.ActiveTransportId equals activeTransport.Id
        //                                     select new
        //                                     {
        //                                         Id = schedule.Id,
        //                                         ActiveTransportId = activeTransport.Id,
        //                                         EventDate = schedule.EventDate,
        //                                         ETD  = schedule.ETD,
        //                                         Direction = activeTransport.Direction,
        //                                     }).FirstOrDefaultAsync();
        //        if (currentSchedule != null)
        //        {
        //            if (currentSchedule.Direction == "EXTERNAL")
        //            {
        //                var employeeOnsiteData = await Context.EmployeeStatus.AsNoTracking()
        //                    .Where(x => x.EmployeeId == oldData.EmployeeId && x.EventDate == currentSchedule.EventDate && x.RoomId != null)
        //                    .FirstOrDefaultAsync();
        //                if (employeeOnsiteData != null)
        //                {

        //                    throw new BadRequestException("The selected employee is currently on-site and cannot be scheduled for external travel.");
        //                }
        //                else {
        //                    return;
        //                }

        //            }
        //            else {
        //                throw new BadRequestException("Only external direction schedules are allowed. Please choose a valid schedule.");
        //            }

        //        }
        //        else {
        //            throw new BadRequestException("New transport schedule data not found");
        //        }
        //    }
        //    else {
        //        throw new BadRequestException("Old transport data not found");
        //    }
        //}


    }


    public class GetExternalScheduleTransportData
    {
        public int ScheduleId { get; set; }
        public int ActiveTransportId { get; set; }

        public string ETD { get; set; }

        public string ETA { get; set; }
        public DateTime EventDate { get; set; }

        public string? Direction { get; set; }  

    }

}