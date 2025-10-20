using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.VisitEventFeature.CreateVisitEvent;
using tas.Application.Features.VisitEventFeature.DeleteVisitEvent;
using tas.Application.Features.VisitEventFeature.GetAllVisitEvent;
using tas.Application.Features.VisitEventFeature.GetVisitEvent;
using tas.Application.Features.VisitEventFeature.ReplaceProfile;
using tas.Application.Features.VisitEventFeature.ReplaceProfileMultiple;
using tas.Application.Features.VisitEventFeature.ReplaceProfileUndo;
using tas.Application.Features.VisitEventFeature.SetTransport;
using tas.Application.Features.VisitEventFeature.UpdateVisitEvent;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace tas.Persistence.Repositories
{

    public class VisitEventRepository : BaseRepository<VisitEvent>, IVisitEventRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public VisitEventRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;
        }

        //Confirmed, Pending, Planning 





        public async Task<List<GetAllVisitEventResponse>> GetAllData(GetAllVisitEventRequest request, CancellationToken cancellationToken)
        {
            var result = new List<VisitEvent>();

            IQueryable<VisitEvent> VisitEventQuery = Context.VisitEvent;

            if (request.startDate.HasValue && request.endDate.HasValue && request.startDate.Value.Date.Year > 2000)
            {
                VisitEventQuery = VisitEventQuery.Where(x => x.StartDate.Date >= request.startDate.Value.Date && x.EndDate.Date <= request.endDate.Value.Date);
            }
            else {
                DateTime StartDate = DateTime.Now.AddMonths(-1);
                DateTime EndDate = StartDate.AddMonths(12);
                VisitEventQuery = VisitEventQuery.Where(x => x.StartDate.Date >= StartDate.Date && x.EndDate.Date <= EndDate.Date);
            }
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                VisitEventQuery = VisitEventQuery.Where(x => x.Name.Contains(request.Name));
            }

            var returnData = await (from visitEvent in VisitEventQuery
                                    join requesterEmployee in Context.Employee on visitEvent.UserIdCreated
                                        equals requesterEmployee.Id into requestEmployeeData
                                    from requesterEmployee in requestEmployeeData.DefaultIfEmpty()
                                    join inschedule in Context.TransportSchedule on visitEvent.InScheduleId
                                        equals inschedule.Id into inScheduleData
                                    from inschedule in inScheduleData.DefaultIfEmpty()
                                    join outschedule in Context.TransportSchedule on visitEvent.OutScheduleId
                                        equals outschedule.Id into outScheduleData
                                    from outschedule in outScheduleData.DefaultIfEmpty()
                                    select new GetAllVisitEventResponse
                                    {
                                        Id = visitEvent.Id,
                                        StartDate = visitEvent.StartDate,
                                        EndDate = visitEvent.EndDate,
                                        Name = visitEvent.Name,
                                        Requester =$"{requesterEmployee.Firstname} {requesterEmployee.Lastname}", 
                                        DateCreated = visitEvent.DateCreated,
                                        Active = visitEvent.Active,
                                        InDescr = inschedule.Description,
                                        InScheduleId = inschedule.Id,
                                        OutDescr = outschedule.Description,
                                        OutScheduleId = outschedule.Id,
                                        HeadCount = visitEvent.HeadCount,
                                        DateUpdated = visitEvent.DateUpdated,

                                    }).ToListAsync();
            return returnData;
        }


        public async Task<GetVisitEventResponse> GetData(GetVisitEventRequest request, CancellationToken cancellationToken)
        {
            var CurrentData =await Context.VisitEvent.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            var returnEmlpoyees = new List<GetVisitEventResponseEmployees>();
            if (CurrentData != null)
            {
                var EmployeeData = await (from eventEmployee in Context.VisitEventlEmployees.Where(x => x.EventId == request.Id)
                                          join emp in Context.Employee on eventEmployee.EmployeeId equals emp.Id into empData
                                          from emp in empData.DefaultIfEmpty()
                                          select new
                                          {
                                              Id = eventEmployee.Id,
                                              EmployeeId = eventEmployee.EmployeeId,
                                              Lastname = emp.Lastname,
                                              Firstname = emp.Firstname,
                                              Active = emp.Active

                                          }).ToListAsync();

                var VisitEventInSchedules =await Context.TransportSchedule.Where(x => x.Id == CurrentData.InScheduleId).FirstOrDefaultAsync();
                var VisitEventOutSchedules =await Context.TransportSchedule.Where(x => x.Id == CurrentData.OutScheduleId).FirstOrDefaultAsync();


                foreach (var item in EmployeeData)
                {
                    var employeeINSchedule =await Context.Transport.AsNoTracking()
                        .Where(x => x.EmployeeId == item.EmployeeId && x.ScheduleId == CurrentData.InScheduleId)
                        .FirstOrDefaultAsync();

                    var employeeOUTchedule =await Context.Transport.AsNoTracking()
                        .Where(x => x.EmployeeId == item.EmployeeId && x.ScheduleId == CurrentData.OutScheduleId)
                        .FirstOrDefaultAsync();




                    var newData = new GetVisitEventResponseEmployees
                    {
                        Id = item.Id,
                        Lastname = item.Lastname,
                        Firstname = item.Firstname,
                        EmployeeId = item.EmployeeId,
                        Active = item.Active,
                        InEventDate = employeeINSchedule?.EventDate,
                        InScheduleId = employeeINSchedule?.ScheduleId,
                        InStatus = employeeINSchedule?.Status,
                        OutEventDate = employeeOUTchedule?.EventDate,
                        OutScheduleId = employeeOUTchedule?.ScheduleId,
                        OutStatus = employeeOUTchedule?.Status,
                        OutDescr = VisitEventOutSchedules?.Description,
                        InDescr = VisitEventInSchedules?.Description

                    };

                    returnEmlpoyees.Add(newData);

                }

                var returnData = new GetVisitEventResponse
                {
                    Id = CurrentData.Id,
                    DateCreated = CurrentData.DateCreated,
                    StartDate = CurrentData.StartDate,
                    EndDate = CurrentData.EndDate,
                    HeadCount = CurrentData.HeadCount,
                    Name = CurrentData.Name,
                    Employees = returnEmlpoyees,
                    InScheduleId = CurrentData.InScheduleId,
                    OutScheduleId =CurrentData.OutScheduleId,
                };

                return returnData;



            }
            else
            {
                return new GetVisitEventResponse();
            }



        }

        public async Task CreateData(CreateVisitEventRequest request, CancellationToken cancellationToken) 
        {

            if (request.startDate <= request.endDate)
            {
                //    using var transaction = Context.Database.BeginTransaction();
                var newData = new VisitEvent
                {
                    StartDate = request.startDate,
                    EndDate = request.endDate,
                    Name = request.Name,
                    Active = 1,
                    HeadCount = request.HeadCount,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    DateCreated = DateTime.Now,
                    InScheduleId = request.InScheduleId,
                    OutScheduleId = request.OutScheduleId
                };

                Context.VisitEvent.Add(newData);
                await Context.SaveChangesAsync();
                var EmployeeSaveStatus = await CreateEventEmployee(newData.Id, newData.HeadCount, request.people);
                if (EmployeeSaveStatus)
                {

                    var empIds = await Context.VisitEventlEmployees.Where(x => x.EventId == newData.Id).Select(x => x.EmployeeId).ToListAsync();
                    await SetTransport(empIds, newData.Id, request.InScheduleId, request.OutScheduleId);
                }



            }
            else {
                throw new BadRequestException("Schedule is wrong");
            }
        }


        #region UpdateEvent


        public async Task UpdateData(UpdateVisitEventRequest request, CancellationToken cancellationToken)
        {
            var dsShift = await Context.Shift.Where(x => x.Code == "DS").FirstOrDefaultAsync();
            if (dsShift == null)
            {
                throw new BadRequestException("DS shift not registered");
            }
            var currenEvent = await Context.VisitEvent.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            if (currenEvent != null)
            {
                var oldInschedule =await Context.TransportSchedule.Where(x => x.Id == currenEvent.InScheduleId).FirstOrDefaultAsync(cancellationToken);
                var oldOutschedule =await Context.TransportSchedule.Where(x => x.Id == currenEvent.OutScheduleId).FirstOrDefaultAsync(cancellationToken);

                var newInschedule = await Context.TransportSchedule.Where(x => x.Id == request.InScheduleId).FirstOrDefaultAsync(cancellationToken);
                var newOutschedule = await Context.TransportSchedule.Where(x => x.Id == request.OutScheduleId).FirstOrDefaultAsync(cancellationToken);
                await UpdateVisitEventHeadCount(currenEvent.Id, request.HeadCount);


                var visitEventEmployee = await Context.VisitEventlEmployees.Where(x => x.EventId == request.Id).ToListAsync(cancellationToken);
                int VirtualRoomId =await Context.Room.Where(x => x.VirtualRoom == 1).Select(x => x.Id).FirstOrDefaultAsync();
                bool inscheduleChange = false;
                bool outscheduleChange = false;
                List<int> EmployeeIds = visitEventEmployee.Select(x => x.EmployeeId).ToList();



                if (newInschedule?.EventDate <=newOutschedule?.EventDate)
                {
                    if (visitEventEmployee.Count > 0)
                    {
                        if (currenEvent.InScheduleId != request.InScheduleId || request.HeadCount != currenEvent.HeadCount)
                        {
                            inscheduleChange = true;


                            var EmployeeTransport = await Context.Transport.Where(x => EmployeeIds.Contains(x.EmployeeId.Value) && x.ScheduleId == currenEvent.InScheduleId).ToListAsync();

                            foreach (var item in EmployeeTransport)
                            {
                                Context.Transport.Remove(item);                               
                            }
                            await SetTransportUpdateIN(EmployeeIds, request.Id,/* request.InScheduleId,*/ newInschedule);
                        }

                        if (currenEvent.OutScheduleId != request.OutScheduleId || request.HeadCount != currenEvent.HeadCount)
                        {
                            outscheduleChange = true;

                            var EmployeeTransport = await Context.Transport.Where(x => EmployeeIds.Contains(x.EmployeeId.Value) && x.ScheduleId == currenEvent.OutScheduleId).ToListAsync();

                            foreach (var item in EmployeeTransport)
                            {
                                Context.Transport.Remove(item);
                            }
                            await SetTransportUpdateOUT(EmployeeIds, request.Id, /*request.OutScheduleId,*/ newOutschedule);
                        }


                        if (outscheduleChange == true || inscheduleChange == true)
                        {

                            var employeeStatus = await Context.EmployeeStatus.Where(x => EmployeeIds.Contains(x.EmployeeId.Value) && x.EventDate >= oldInschedule.EventDate.Date && x.EventDate <= oldOutschedule.EventDate.Date).ToListAsync();

                            foreach (var item in employeeStatus)
                            {
                                Context.EmployeeStatus.Remove(item);
                            }


                            foreach (var currentEmpId in EmployeeIds)
                            {
                                var empSaveStatus = await SaveEmployeeStatus(currentEmpId, newInschedule.EventDate.Date, newOutschedule.EventDate.Date, VirtualRoomId, dsShift.Id, request.Id);
                            }

                        }

                        currenEvent.StartDate = newInschedule.EventDate.Date;
                        currenEvent.EndDate = newOutschedule.EventDate.Date;
                        currenEvent.InScheduleId = newInschedule.Id;
                        currenEvent.OutScheduleId = newOutschedule.Id;
                        currenEvent.Name = request.Name;
                        currenEvent.DateUpdated = DateTime.Now;
                        currenEvent.HeadCount = visitEventEmployee.Count;
                        currenEvent.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                        Context.VisitEvent.Update(currenEvent);



                    }


                }
                else {
                    throw new BadRequestException("Change event wrong value");
                }

            }
            else {
                throw new BadRequestException("Event not found");
            }

        }


        private async Task UpdateVisitEventHeadCount(int eventId, int newHeadCount)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var currentData = await Context.VisitEvent.Where(x => x.Id == eventId).FirstOrDefaultAsync();
                    var eventEmployees = await Context.VisitEventlEmployees
                        .Where(x => x.EventId == eventId).ToListAsync();

                    if (currentData == null)
                    {
                        return; // Early return if no event is found
                    }

                    var empIds = new List<int>();

                    if (eventEmployees.Count > newHeadCount)
                    {
                        int diffCount = eventEmployees.Count - newHeadCount;

                        // Get the employees to remove
                        var employeesToRemove = eventEmployees.Take(diffCount).ToList();
                        foreach (var emp in employeesToRemove)
                        {
                            var currentEmployee = await Context.Employee.Where(x => x.Id == emp.EmployeeId).FirstOrDefaultAsync();
                            if (currentEmployee != null)
                            {
                                var employeeINTransport = await Context.Transport.Where(x => x.EmployeeId == currentEmployee.Id && x.ScheduleId == currentData.InScheduleId).FirstOrDefaultAsync();
                                var employeeOUTTransport = await Context.Transport.Where(x => x.EmployeeId == currentEmployee.Id && x.ScheduleId == currentData.OutScheduleId).FirstOrDefaultAsync();
                                var employeeStatus = await Context.EmployeeStatus.Where(x => x.EmployeeId == currentEmployee.Id && x.EventDate >= currentData.StartDate.Date && x.EventDate <= currentData.EndDate).ToListAsync();

                                Context.EmployeeStatus.RemoveRange(employeeStatus);
                                if (employeeOUTTransport != null) Context.Transport.Remove(employeeOUTTransport);
                                if (employeeINTransport != null) Context.Transport.Remove(employeeINTransport);


                                Context.VisitEventlEmployees.Remove(emp);
                                if (currentEmployee?.Active == 2)
                                {
                                    empIds.Add(currentEmployee.Id);
                                }
                            }

                        }
                    }
                    else if (eventEmployees.Count < newHeadCount)
                    {
                        int diffCount = newHeadCount - eventEmployees.Count;
                        for (int i = 0; i < diffCount; i++)
                        {
                            int employeeId = await SaveEmployee($"F-{DateTime.Today.ToString("yyMMddHHmm")}-{i + 1}", $"L-{DateTime.Today.ToString("yyMMddHHmm")}-{i + 1}");

                            var newData = new VisitEventlEmployees
                            {
                                EventId = eventId,
                                EmployeeId = employeeId,
                                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                DateCreated = DateTime.Now,
                                Active = 1
                            };
                            Context.VisitEventlEmployees.Add(newData);
                        }
                    }
                    else
                    {
                        transaction.Commit(); // Commit the transaction if no changes are needed
                        return;
                    }

                    await Context.SaveChangesAsync();
                    transaction.Commit();
                    await DeleteEmployeeForce(empIds);
                    return;
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Roll back the transaction on error
                    throw; // Optionally rethrow the exception or handle it based on your error policy
                }
            }
        }



        private async Task DeleteEmployeeForce(List<int> EmpIds)
        {
            var deleteEmployees = await Context.Employee.Where(x => EmpIds.Contains(x.Id)).ToListAsync();
            Context.Employee.RemoveRange(deleteEmployees);
        }





        private async Task SetTransportUpdateIN(List<int> EmpIds, int eventId, TransportSchedule CurrentIntransport)
        {

            var dsShift = await Context.Shift.Where(x => x.Code == "DS").FirstOrDefaultAsync();
            if (dsShift == null)
            {
                throw new BadRequestException("DS shift not registered");
            }

            var InTransportCount = await Context.Transport.Where(x => x.ScheduleId == CurrentIntransport.Id).CountAsync();

            int IntransportActiveCount = CurrentIntransport.Seats.Value - InTransportCount;

            for (int i = 0; i < EmpIds.Count; i++)
            {
                string IntransportStatus = "Confirmed";
                if (IntransportActiveCount <= 0)
                {
                    IntransportStatus = "Over Booked";
                }
                var currentEmpId = EmpIds[i];
                var newINTransport = new Transport
                {
                    EmployeeId = currentEmpId,
                    ScheduleId = CurrentIntransport.Id,
                    ActiveTransportId = CurrentIntransport.ActiveTransportId,
                    Active = 1,
                    Direction = "IN",
                    
                    EventDate = CurrentIntransport.EventDate.Date,
                    Status = IntransportStatus,
                    EventDateTime = CurrentIntransport.EventDate,
                    DateCreated = DateTime.Now,
                    SeatBlock = 1,
                    ChangeRoute = $"Seat block eventId #{eventId} change schedule",
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    DateUpdated = DateTime.Now
                };


                Context.Transport.Add(newINTransport);
                if (IntransportActiveCount > 0)
                {
                    IntransportActiveCount--;
                }

            }

        }



        private async Task SetTransportUpdateOUT(List<int> EmpIds, int eventId,/* int outScheduleId,*/ TransportSchedule CurrentOutTransport )
        {

            var dsShift = await Context.Shift.Where(x => x.Code == "DS").FirstOrDefaultAsync();
            if (dsShift == null)
            {
                throw new BadRequestException("DS shift not registered");
            }
            var OutTransportCount = await Context.Transport.Where(x => x.ScheduleId == CurrentOutTransport.Id).CountAsync();
            int OuttransportActiveCount = CurrentOutTransport.Seats.Value - OutTransportCount;

            int VirtualRoomId = await Context.Room.Where(x => x.VirtualRoom == 1).Select(x => x.Id).FirstOrDefaultAsync();



            for (int i = 0; i < EmpIds.Count; i++)
            {
                string OuttransportStatus = "Confirmed";
                if (OuttransportActiveCount <= 0)
                {
                    OuttransportStatus = "Over Booked";
                }
                var currentEmpId = EmpIds[i];


                var newOUTTransport = new Transport
                {
                    EmployeeId = currentEmpId,
                    ScheduleId = CurrentOutTransport.Id,
                    ActiveTransportId = CurrentOutTransport.ActiveTransportId,
                    Direction = "OUT",
                    Active = 1,
                    EventDate = CurrentOutTransport.EventDate,
                    Status = OuttransportStatus,
                    EventDateTime = CurrentOutTransport.EventDate,
                    DateCreated = DateTime.Now,
                    SeatBlock = 1,
                    ChangeRoute = $"Seat block eventId #{eventId} change schedule",
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    DateUpdated = DateTime.Now
                };

                Context.Transport.Add(newOUTTransport);
                if (OuttransportActiveCount > 0)
                {
                    OuttransportActiveCount--;
                }

            }

        }



        #endregion

        private async Task SetTransport(List<int> EmpIds, int eventId, int inScheduleId, int outScheduleId)
        {

            var dsShift = await Context.Shift.Where(x => x.Code == "DS").FirstOrDefaultAsync();
            if (dsShift == null)
            {
                throw new BadRequestException("DS shift not registered");
            }
            var currentEvent =await Context.VisitEvent.Where(x => x.Id == eventId).Select(x => new { x.Id, x.StartDate, x.EndDate }).FirstOrDefaultAsync();
            var CurrentIntransport =await Context.TransportSchedule.Where(x => x.Id == inScheduleId).FirstOrDefaultAsync();
            var CurrentOutTransport = await Context.TransportSchedule.Where(x => x.Id == outScheduleId).FirstOrDefaultAsync();

            var InTransportCount =await Context.Transport.Where(x => x.ScheduleId == inScheduleId).CountAsync();
            var OutTransportCount =await Context.Transport.Where(x => x.ScheduleId == outScheduleId).CountAsync();

            int IntransportActiveCount = CurrentIntransport.Seats.Value- InTransportCount;
            int OuttransportActiveCount = CurrentOutTransport.Seats.Value - OutTransportCount;

            int VirtualRoomId =await Context.Room.Where(x => x.VirtualRoom == 1).Select(x => x.Id).FirstOrDefaultAsync();



            for (int i = 0; i < EmpIds.Count; i++)
            {
                string IntransportStatus = "Confirmed";
                if (IntransportActiveCount <= 0)
                {
                    IntransportStatus = "Over Booked";
                }
                string OuttransportStatus = "Confirmed";
                if (OuttransportActiveCount <= 0)
                {
                    OuttransportStatus = "Over Booked";
                }
                var currentEmpId = EmpIds[i];
                var newINTransport = new Transport
                {
                    EmployeeId = currentEmpId,
                    ScheduleId = inScheduleId,
                    ActiveTransportId = CurrentIntransport.ActiveTransportId,
                    Active = 1,
                    Direction = "IN",
                    EventDate = currentEvent.StartDate.Date,
                    Status = IntransportStatus,
                    EventDateTime = currentEvent.StartDate.Date,
                    DateCreated = DateTime.Now,
                    SeatBlock = 1,
                    ChangeRoute = $"Seat block eventId #{eventId}",
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                };


                var newOUTTransport = new Transport
                {
                    EmployeeId = currentEmpId,
                    ScheduleId = outScheduleId,
                    ActiveTransportId = CurrentOutTransport.ActiveTransportId,
                    Direction = "OUT",
                    Active = 1,
                    EventDate = currentEvent.EndDate.Date,
                    Status = OuttransportStatus,
                    EventDateTime = currentEvent.EndDate.Date,
                    DateCreated = DateTime.Now,
                    SeatBlock = 1,
                    ChangeRoute = $"Seat block eventId #{eventId}",
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                };

                Context.Transport.Add(newINTransport);
                Context.Transport.Add(newOUTTransport);
                var empSaveStatus =await SaveEmployeeStatus(currentEmpId, currentEvent.StartDate, currentEvent.EndDate, VirtualRoomId, dsShift.Id, eventId);

                if (IntransportActiveCount > 0)
                {
                    IntransportActiveCount--;
                }

                if (OuttransportActiveCount > 0)
                {
                    OuttransportActiveCount--;

                }

            }
             await Context.SaveChangesAsync();

        }


        public async Task EventEmployeeSetTransport(SetTransportRequest request, CancellationToken cancellationToken)
        {
            var dsShift =await Context.Shift.Where(x => x.Code == "DS").FirstOrDefaultAsync();
            if (dsShift == null)
            {
                throw new BadRequestException("DS shift not registered");
            }
            var currentEvent =await Context.VisitEvent.Where(x => x.Id == request.EventId).Select(x => new { x.Id, x.StartDate, x.EndDate }).FirstOrDefaultAsync();
            var CurrentIntransport =await Context.TransportSchedule.Where(x=> x.Id == request. InScheduleId).FirstOrDefaultAsync();
            var CurrentOutTransport =await Context.TransportSchedule.Where(x=> x.Id == request.OutScheduleId).FirstOrDefaultAsync();
            var InTransportCount =await Context.Transport.Where(x => x.ScheduleId == request.InScheduleId).CountAsync();
            var OutTransportCount =await Context.Transport.Where(x => x.ScheduleId == request.OutScheduleId).CountAsync();
            int? IntransportActiveCount = CurrentIntransport?.Seats - InTransportCount;
            int? OuttransportActiveCount = CurrentOutTransport?.Seats - OutTransportCount;
            int VirtualRoomId =await Context.Room.Where(x => x.VirtualRoom == 1).Select(x => x.Id).FirstOrDefaultAsync();

            for (int i = 0; i < request.EmpIds.Count; i++)
            {
                string IntransportStatus = "Confirmed";
                if (IntransportActiveCount <= 0)
                {
                    IntransportStatus = "Over Booked";
                }
                string OuttransportStatus = "Confirmed";
                if (OuttransportActiveCount <= 0)
                {
                    OuttransportStatus = "Over Booked";
                }
                var currentEmpId = request.EmpIds[i];
                var newINTransport = new Transport
                {
                    EmployeeId = currentEmpId,
                    ScheduleId = request.InScheduleId,
                    ActiveTransportId = CurrentIntransport.ActiveTransportId,
                    Active = 1,
                    Direction = "IN",
                    EventDate = currentEvent?.StartDate.Date,
                    Status = IntransportStatus,
                    EventDateTime = currentEvent?.StartDate.Date,
                    DateCreated = DateTime.Now,
                    SeatBlock = 1,
                    ChangeRoute = $"Seat block eventId #{request.EventId}",
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                };


                var newOUTTransport = new Transport
                {
                    EmployeeId = currentEmpId,
                    ScheduleId = request.OutScheduleId,
                    ActiveTransportId = CurrentOutTransport.ActiveTransportId,
                    Direction = "OUT",
                    Active = 1,
                    EventDate = currentEvent?.EndDate.Date,
                    Status = OuttransportStatus,
                    EventDateTime = currentEvent?.EndDate.Date,
                    DateCreated = DateTime.Now,
                    SeatBlock  =1,
                    ChangeRoute = $"Seat block eventId #{request.EventId}",
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                };

                Context.Transport.Add(newINTransport);
                Context.Transport.Add(newOUTTransport);
                var empSaveStatus =await SaveEmployeeStatus(currentEmpId, currentEvent.StartDate, currentEvent.EndDate, VirtualRoomId, dsShift.Id, request.EventId);
            
                if (IntransportActiveCount > 0)
                {
                    IntransportActiveCount--;
                }

                if (OuttransportActiveCount > 0)
                {
                    OuttransportActiveCount--;

                }



            }
            // Context.SaveChanges();
            await  Task.CompletedTask;


        }

        private async Task<bool> SaveEmployeeStatus(int EmployeeId, DateTime startDate, DateTime EndDate, int VirtualRoomId, int shiftId, int eventId)
        {
            try
            {
                if (startDate.Date < EndDate)
                {
                    var currentDate = startDate;
                    while (currentDate < EndDate)
                    {
                        var newData = new EmployeeStatus
                        {
                            Active = 1,
                            RoomId = VirtualRoomId,
                            EmployeeId = EmployeeId,
                            ShiftId = shiftId,
                            EventDate = currentDate.Date,
                            DateCreated = DateTime.Now,
                            ChangeRoute = $"Seat block eventId #{eventId}",
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id

                        };

                        Context.EmployeeStatus.Add(newData);
                        currentDate = currentDate.AddDays(1);
                    }

                    return true;
                }
                else if (startDate.Date == EndDate)
                {
                    return true;
                }
                else {
                    return false;
                }

            }
            catch (Exception)
            {

                return false;
            }
          
        }



        //F-2402011812-3 L-2402011812-3


        private async Task<bool> CreateEventEmployee(int EventId, int HeadCount, List<CreateVisitEventRequestPeoples> peoples)
        {
            int currentCount = 1;


            foreach (var item in peoples)
            {
                if (item.Id.HasValue)
                {
                    var newData = new VisitEventlEmployees
                    {
                        EventId = EventId,
                        EmployeeId = item.Id.Value,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        DateCreated = DateTime.Now,
                        Active = 1
                    };

                    Context.VisitEventlEmployees.Add(newData);
                }
                else {


                    string lastName = string.IsNullOrEmpty(item.last) ? $"SeatBlock_{currentCount}_LN" : item.last;
                    string firstName = string.IsNullOrEmpty(item.first) ? $"SeatBlock_{currentCount}_FN" : item.first;

                    int EmployeeId = await SaveEmployee(lastName, firstName);

                    if (EmployeeId == 0)
                    {
                        return false;
                    }
                    var newData = new VisitEventlEmployees
                    {
                        EventId = EventId,
                        EmployeeId = EmployeeId,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                        DateCreated = DateTime.Now,
                        Active = 1
                    };

                    Context.VisitEventlEmployees.Add(newData);
                    currentCount++;
                }
            }
            await  Context.SaveChangesAsync();
            return true;
        }

        private async Task<int> SaveEmployee(string lastname, string firstname)
        {

            try
            {
                var newEmployee = new Employee
                {
                    Active = 2,
                    Firstname = firstname,
                    Lastname = lastname,
                    Mobile = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    DateCreated = DateTime.Now,
                    UserIdCreated= _hTTPUserRepository.LogCurrentUser()?.Id,
                    
                };
                Context.Employee.Add(newEmployee);
                await Context.SaveChangesAsync();
                return newEmployee.Id;
            }
            catch (Exception ex)
            {
                var aa = 0;
                return 0;
            }

        }


        public async Task EventEmployeeReplaceProfile(ReplaceProfileRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {

                    var currentEvent = await Context.VisitEvent.Where(x => x.Id == request.EventId).FirstOrDefaultAsync();
                    bool employeeReplaceSuccess = false;
                        var newEmployeeSiteStatus = await CheckOnsiteDates(request.newEmployeeId, currentEvent.StartDate, currentEvent.EndDate);




                    if (newEmployeeSiteStatus)
                    {
                        var oldEmployeeRoomData = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.oldEmployeeId
                        ).ToListAsync();


                        var oldEmployeeTransportData = await Context.Transport.Where(x => x.EmployeeId == request.oldEmployeeId).ToListAsync();
                        foreach (var itemRoom in oldEmployeeRoomData)
                        {
                            itemRoom.EmployeeId = request.newEmployeeId;
                            itemRoom.DateUpdated = DateTime.Now;
                            itemRoom.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                            itemRoom.ChangeRoute = $"Replace profile seat block  #{currentEvent.Id}";
                            Context.EmployeeStatus.Update(itemRoom);
                        }

                        foreach (var itemTransport in oldEmployeeTransportData)
                        {
                            itemTransport.EmployeeId = request.newEmployeeId;
                            itemTransport.DateUpdated = DateTime.Now;
                            itemTransport.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                            itemTransport.ChangeRoute = $"Replace profile seat block  #{currentEvent.Id}";
                            Context.Transport.Update(itemTransport);
                        }

                        employeeReplaceSuccess = true;



                    }
                    else {
                        throw new BadRequestException("It is not possible to replace because this employee is onsite");
                    }


                    if (employeeReplaceSuccess)
                    { 

                        var itemEventEmployee = await Context.VisitEventlEmployees.Where(x => x.EmployeeId == request.oldEmployeeId).FirstOrDefaultAsync();
                        if (itemEventEmployee != null)
                        {
                            itemEventEmployee.EmployeeId = request.newEmployeeId;
                            itemEventEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                            itemEventEmployee.DateUpdated = DateTime.Now;
                            Context.VisitEventlEmployees.Update(itemEventEmployee);
                        }
                        

                    


                        await Context.SaveChangesAsync();

                        var itemEmployee = await Context.Employee.Where(x => x.Id == request.oldEmployeeId).FirstOrDefaultAsync();
                        if (itemEmployee != null)
                        {
                            Context.Employee.Remove(itemEmployee);
                        }
                    }


                    await Context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"An error occurred: {ex.Message}");

                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw new BadRequestException(ex.Message);
                }

            }

        }


        #region UNDO

        public async Task EventEmployeeReplaceProfileUndo(ReplaceProfileUndoRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {

                    var currentEmployee =await Context.Employee.Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();

                    if (currentEmployee != null)
                    {
                        if (currentEmployee.Active == 1)
                        {
                            var currentEvent = await Context.VisitEvent.Where(x => x.Id == request.EventId).FirstOrDefaultAsync();
                            if (currentEvent != null)
                            {

                                DateTime.Now.ToString("HHm");
                                string Firstname = $"F-{currentEvent.StartDate.ToString("yyMMdd")}{DateTime.Now.ToString("HHm")}";
                                string Lastname = $"L-{currentEvent.StartDate.ToString("yyMMdd")}{DateTime.Now.ToString("HHm")}";

                                int newEmployeeId = await SaveEmployee(Firstname, Lastname);
                                if (newEmployeeId > 0)
                                {
                                    var oldEmployeeRoomData = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.EmployeeId
                                  && x.EventDate.Value.Date >= currentEvent.StartDate.Date && x.EventDate <= currentEvent.EndDate.Date
                                  ).ToListAsync();


                                    var oldEmployeeTransportData = await Context.Transport.Where(x => x.EmployeeId == request.EmployeeId && x.EventDate.Value.Date >= currentEvent.StartDate.Date && x.EventDate <= currentEvent.EndDate.Date).ToListAsync();
                                    foreach (var itemRoom in oldEmployeeRoomData)
                                    {
                                        itemRoom.EmployeeId = newEmployeeId;
                                        itemRoom.DateUpdated = DateTime.Now;
                                        itemRoom.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                        itemRoom.ChangeRoute = $"Undo profile seat block  #{currentEvent.Id}";
                                        Context.EmployeeStatus.Update(itemRoom);
                                    }

                                    foreach (var itemTransport in oldEmployeeTransportData)
                                    {
                                        itemTransport.EmployeeId = newEmployeeId;
                                        itemTransport.DateUpdated = DateTime.Now;
                                        itemTransport.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                        itemTransport.ChangeRoute = $"Undo profile seat block  #{currentEvent.Id}";
                                        Context.Transport.Update(itemTransport);
                                    }


                                    var oldEmployee = await Context.VisitEventlEmployees.Where(x => x.EmployeeId == request.EmployeeId && x.EventId == request.EventId).FirstOrDefaultAsync();

                                    if (oldEmployee != null)
                                    {
                                        oldEmployee.EmployeeId = newEmployeeId;
                                        oldEmployee.DateUpdated = DateTime.Now;
                                        oldEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                                        Context.VisitEventlEmployees.Update(oldEmployee);
                                    }


                                    await Context.SaveChangesAsync();
                                    await transaction.CommitAsync();

                                }

                            }


                        }
                    
                    }
                   
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"An error occurred: {ex.Message}");

                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw new BadRequestException(ex.Message);
                }

            }



        }


        #endregion



        private async Task<bool> CheckOnsiteDates(int employeeId, DateTime startDate, DateTime endDate)
        {
         var  employeeStatusOnsiteDates = await Context.EmployeeStatus.Where(x => x.EventDate.Value.Date >= startDate
         && x.EventDate <= endDate && x.RoomId != null && x.EmployeeId == employeeId).ToListAsync();
            if (employeeStatusOnsiteDates.Count > 0)
            {
                return false;
            }
            else { 
                return true;
            }
        }


        public async Task<List<ReplaceProfileMultipleResponse>> EventEmployeeReplaceProfileMultiple(ReplaceProfileMultipleRequest request, CancellationToken cancellationToken)
        {

            var skippedEmployees = new List<skippedEmployees>();
            var successEmployees = new List<int>();


            var returnData = new List<ReplaceProfileMultipleResponse>();
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {

                foreach (var item in request.Employees)
                {

                    try
                    {
                        if (!successEmployees.Contains(item.newEmployeeId))
                        {
                            var currentEvent = await Context.VisitEvent.Where(x => x.Id == request.EventId).FirstOrDefaultAsync();
                            bool employeeReplaceSuccess = false;
                            var newEmployeeSiteStatus = await CheckOnsiteDates(item.newEmployeeId, currentEvent.StartDate, currentEvent.EndDate);
                            if (newEmployeeSiteStatus)
                            {
                                var oldEmployeeRoomData = await Context.EmployeeStatus.Where(x => x.EmployeeId == item.oldEmployeeId
                                ).ToListAsync();


                                var oldEmployeeTransportData = await Context.Transport.Where(x => x.EmployeeId == item.oldEmployeeId).ToListAsync();
                                foreach (var itemRoom in oldEmployeeRoomData)
                                {
                                    itemRoom.EmployeeId = item.newEmployeeId;
                                    itemRoom.DateUpdated = DateTime.Now;
                                    itemRoom.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    itemRoom.ChangeRoute = $"Replace profile seat block  #{currentEvent.Id}";
                                    Context.EmployeeStatus.Update(itemRoom);
                                }

                                foreach (var itemTransport in oldEmployeeTransportData)
                                {
                                    itemTransport.EmployeeId = item.newEmployeeId;
                                    itemTransport.DateUpdated = DateTime.Now;
                                    itemTransport.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    itemTransport.ChangeRoute = $"Replace profile seat block  #{currentEvent.Id}";
                                    Context.Transport.Update(itemTransport);
                                }

                                employeeReplaceSuccess = true;
                                successEmployees.Add(item.newEmployeeId);
                            }
                            else
                            {
                                skippedEmployees.Add(new skippedEmployees { EmployeeId = item.oldEmployeeId, Reason = "Replace profile onsite" });
                            }

                            if (employeeReplaceSuccess)
                            {

                                var itemEventEmployee = await Context.VisitEventlEmployees.Where(x => x.EmployeeId == item.oldEmployeeId).FirstOrDefaultAsync();
                                if (itemEventEmployee != null)
                                {
                                    itemEventEmployee.EmployeeId = item.newEmployeeId;
                                    itemEventEmployee.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    itemEventEmployee.DateUpdated = DateTime.Now;
                                    Context.VisitEventlEmployees.Update(itemEventEmployee);
                                }

                                await Context.SaveChangesAsync();

                                var itemEmployee = await Context.Employee.Where(x => x.Id == item.oldEmployeeId).FirstOrDefaultAsync();
                                if (itemEmployee != null)
                                {
                                    Context.Employee.Remove(itemEmployee);
                                }
                            }
                        }
                        else
                        {
                            skippedEmployees.Add(new skippedEmployees { EmployeeId = item.oldEmployeeId, Reason = "Employee duplicated" });
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"An error occurred: {ex.Message}");

                        if (transaction != null)
                        {
                            transaction.Rollback();
                        }
                        skippedEmployees.Add(new skippedEmployees { EmployeeId = item.oldEmployeeId, Reason = ex.Message });
                    }
                }

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }


            if (skippedEmployees.Count > 0)
            {

                var skippedEmpIds = skippedEmployees.Select(x => x.EmployeeId).ToList();
                returnData = await (from employee in Context.Employee
                                    where skippedEmpIds.Contains(employee.Id)
                                    select new ReplaceProfileMultipleResponse
                                    {
                                        EmployeeId = employee.Id,
                                        Firstname = employee.Firstname,
                                        Lastname = employee.Lastname
                                    }).ToListAsync();
                foreach (var item in returnData)
                {
                    item.Reason = skippedEmployees.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)?.Reason;
                }
                return returnData;
            }
            else {
                return returnData;
            }
        }



        #region DeletData



        public async Task DeleteData(DeleteVisitEventRequest request, CancellationToken cancellationToken)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var currentData = await Context.VisitEvent.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                    var eventEmployees = await Context.VisitEventlEmployees
                        .Where(x => x.EventId == request.Id).ToListAsync();

                    if (currentData == null)
                    {
                        return; // Early return if no event is found
                    }

                    var empIds = new List<int>();
                    foreach (var emp in eventEmployees)
                    {
                        var currentEmployee = await Context.Employee.Where(x => x.Id == emp.EmployeeId).FirstOrDefaultAsync();
                        if (currentEmployee != null)
                        {
                            if (currentEmployee.Active == 2)
                            {
                                var employeeINTransport = await Context.Transport.Where(x => x.EmployeeId == currentEmployee.Id && x.ScheduleId == currentData.InScheduleId).FirstOrDefaultAsync();
                                var employeeOUTTransport = await Context.Transport.Where(x => x.EmployeeId == currentEmployee.Id && x.ScheduleId == currentData.OutScheduleId).FirstOrDefaultAsync();
                                var employeeStatus = await Context.EmployeeStatus.Where(x => x.EmployeeId == currentEmployee.Id && x.EventDate >= currentData.StartDate.Date && x.EventDate <= currentData.EndDate).ToListAsync();

                                Context.EmployeeStatus.RemoveRange(employeeStatus);
                                if (employeeOUTTransport != null) Context.Transport.Remove(employeeOUTTransport);
                                if (employeeINTransport != null) Context.Transport.Remove(employeeINTransport);

                            }
                            Context.VisitEventlEmployees.Remove(emp);
                            if (currentEmployee?.Active == 2)
                            {
                                empIds.Add(currentEmployee.Id);
                            }
                        }

                    }

                
                    await Context.SaveChangesAsync();
                    transaction.Commit();
                    Context.VisitEvent.Remove(currentData);
                    await DeleteEmployeeForce(empIds);
                    return;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new BadRequestException(ex.Message);
                }
            }
        }

        #endregion

    }




    public class skippedEmployees
    { 
        public int EmployeeId { get; set; }

        public string? Reason { get; set; }


    }
}
