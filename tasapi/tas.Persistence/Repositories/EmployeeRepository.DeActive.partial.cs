using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeFeature.DeActiveEmployee;
using tas.Application.Features.EmployeeFeature.DeleteEmployeeTransport;
using tas.Application.Features.EmployeeFeature.DeleteEmployeeTransportBulk;
using tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheck;
using tas.Application.Features.EmployeeFeature.EmployeeDeActiveDateCheckMultiple;
using tas.Application.Features.EmployeeFeature.ReActiveEmployee;
using tas.Domain.Entities;

namespace tas.Persistence.Repositories
{
    public partial class EmployeeRepository
    {
        public async Task<List<DeActiveEmployeeResponse>> DeActiveEmployee(DeActiveEmployeeRequest request, CancellationToken cancellationToken)
        {

            List<DeActiveEmployeeResponse> skippedEmployees = new List<DeActiveEmployeeResponse>();
            foreach (var item in request.Employees)
            {
                    var currentEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId && x.Active == 1).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    var complenceDataCheck = await Context.EmployeeStatus.AsNoTracking()
                        .Where(x => x.EmployeeId == item.EmployeeId && x.EventDate.Value.Date == item.EventDate.Date && x.RoomId != null)
                        .FirstOrDefaultAsync();

                    var complenceDataTransportCheck = await Context.Transport.AsNoTracking()
                        .Where(x => x.EmployeeId == item.EmployeeId && x.EventDate.Value.Date == item.EventDate.Date)
                        .FirstOrDefaultAsync();


                    if (complenceDataCheck == null && complenceDataTransportCheck == null)
                    {

                        await DeActiveEmployeeDelete(item.EmployeeId, cancellationToken);
                        currentEmployee.Active = 0;
                        currentEmployee.RoomId = null;
                        currentEmployee.CompletionDate = DateTime.Now;
                        var empHisDat = new EmployeeHistory
                        {
                            Comment = item.Comment,
                            Active = 1,
                            DateCreated = DateTime.Now,
                            EventDate = item.EventDate.Date,
                            TerminationTypeId = item.DemobTypeTypeId,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            EmployeeId = item.EmployeeId,
                            Action = "Termination"
                        };
                        Context.EmployeeHistory.Add(empHisDat);
                        Context.Employee.Update(currentEmployee);
                    }
                    else
                    {
                        skippedEmployees.Add(new DeActiveEmployeeResponse { EmployeeId = currentEmployee.Id, FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}", SAPID = currentEmployee.SAPID });
                    }
                }



            }

            return skippedEmployees;
        }

        public async Task<EmployeeDeActiveDateCheckResponse> EmployeeDeActiveDateCheck(EmployeeDeActiveDateCheckRequest request, CancellationToken cancellationToken)
        {
            var onsiteResult = await   Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate.Value.Date 
             == request.EventDate.Date && x.EmployeeId == request.EmployeeId && x.RoomId != null).FirstOrDefaultAsync();

            var travelResult = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date == request.EventDate.Date && x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
            var todaytravelresult = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date == DateTime.Today.Date && x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();

            var FutureTransportValidationStatusData = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date > DateTime.Today.Date && x.EmployeeId == request.EmployeeId).AnyAsync();

            if (onsiteResult != null && travelResult != null || todaytravelresult != null)
            {
                return new EmployeeDeActiveDateCheckResponse { DateValidationStatus = false, FutureTransportValidationStatus = FutureTransportValidationStatusData };
            }
            else {
                return new EmployeeDeActiveDateCheckResponse { DateValidationStatus = true, FutureTransportValidationStatus = FutureTransportValidationStatusData };
            }

        }


        public async Task<List<EmployeeDeActiveDateCheckMultipleResponse>> EmployeeDeActiveDateCheckMultiple(EmployeeDeActiveDateCheckMultipleRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<EmployeeDeActiveDateCheckMultipleResponse>();


            foreach (var item in request.data.OrderBy(x=> x.Index))
            {
                var onsiteResult = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EventDate.Value.Date == item.EventDate && x.EmployeeId == item.EmployeeId && x.RoomId != null).FirstOrDefaultAsync();

                var travelResult = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date == item.EventDate && x.EmployeeId == item.EmployeeId).FirstOrDefaultAsync();
                var todaytravelresult = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date == DateTime.Today.Date && x.EmployeeId == item.EmployeeId).FirstOrDefaultAsync();

                var FutureTransportValidationStatusData = await Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date > DateTime.Today.Date && x.EmployeeId == item.EmployeeId).AnyAsync();

                if (onsiteResult != null && travelResult != null || todaytravelresult != null)
                {
                   returnData.Add(new EmployeeDeActiveDateCheckMultipleResponse { DateValidationStatus = false, FutureTransportValidationStatus = FutureTransportValidationStatusData, EmployeeId = item.EmployeeId, Index =item.Index });
                }
                else
                {
                    returnData.Add(new EmployeeDeActiveDateCheckMultipleResponse { DateValidationStatus = true, FutureTransportValidationStatus = FutureTransportValidationStatusData,  EmployeeId = item.EmployeeId, Index = item.Index });
                }
            }


            return returnData;

        }







        public async Task ReActiveEmployee(ReActiveEmployeeRequest request, CancellationToken cancellationToken)
        {

            foreach (var item in request.Employees)
            {

                    var currentEmployee = await Context.Employee.Where(x => x.Id == item.EmployeeId && x.Active != 1).FirstOrDefaultAsync();
                    if (currentEmployee != null)
                    {
                        currentEmployee.Active = 1;
                        currentEmployee.CommenceDate = DateTime.Now;
                        currentEmployee.DepartmentId = item.DepartmentId;
                        currentEmployee.EmployerId = item.EmployerId;
                        currentEmployee.CostCodeId = item.CostCodeId;
                        var empHisDat = new EmployeeHistory
                        {
                            Active = 1,
                            DateCreated = DateTime.Now,

                            EventDate = item.EventDate.Date,
                            UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                            EmployeeId = item.EmployeeId,
                            Action = "Join"
                        };
                        Context.EmployeeHistory.Add(empHisDat);

                        Context.Employee.Update(currentEmployee);


                    }
            }

            await Task.CompletedTask;
        }



     
        


        #region DeleteTransport On RosterExecuteBefore

        public async Task DeleteTransport(DeleteEmployeeTransportRequest request, CancellationToken cancellationToken)
        {
            var rostercheckDate =await OnsiteCheckEmployeeByRoster(request.employeeId, request.OnsiteDate);
            if (rostercheckDate)
            {
                var onsiteStatus = await OnsiteCheckEmployeeByRoster(request.employeeId, request.OnsiteDate.Date);
                if (onsiteStatus)
                {
                    var firstINTransport = await Context.Transport
                     .Where(x => x.EmployeeId == request.employeeId && x.EventDate.Value.Date <= request.OnsiteDate.Date && x.Direction == "IN").OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();



    

                    if (firstINTransport != null)
                    {

                        var inafterOUTTransport = await Context.Transport
                             .Where(x => x.EmployeeId == request.employeeId && x.EventDate.Value.Date <= request.OnsiteDate.Date && x.EventDate >= firstINTransport.EventDate && x.Direction == "OUT").OrderBy(x => x.EventDate).FirstOrDefaultAsync();


                        var firstOUTTransport = await Context.Transport
                             .Where(x => x.EmployeeId == request.employeeId && x.EventDate.Value.Date >= request.OnsiteDate.Date && x.Direction == "OUT").OrderBy(x => x.EventDate).FirstOrDefaultAsync();


                        if (firstINTransport != null && firstOUTTransport != null && inafterOUTTransport == null)
                        {


                            var employeeOnsiteStatusDates = await Context.EmployeeStatus
                                 .Where(x => x.EmployeeId == request.employeeId
                                 && x.EventDate >= firstINTransport.EventDate.Value.Date
                                 && x.EventDate < firstOUTTransport.EventDate.Value.Date).ToListAsync();

                            var currentRosterBreakShift = await Context.Shift.Where(x => x.Code == "RR").FirstOrDefaultAsync();

                            if (currentRosterBreakShift != null)
                            {
                                foreach (var item in employeeOnsiteStatusDates)
                                {
                                    item.RoomId = null;
                                    item.BedId = null;
                                    item.ShiftId = currentRosterBreakShift.Id;
                                    item.ChangeRoute = "Delete On site delete data Roster";
                                    item.DateUpdated = DateTime.Now;
                                    item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                }

                                Context.EmployeeStatus.UpdateRange(employeeOnsiteStatusDates);
                            }
                            else
                            {

                                Context.EmployeeStatus.RemoveRange(employeeOnsiteStatusDates);
                            }

                            firstINTransport.DateUpdated = DateTime.Now;
                            firstINTransport.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                            firstOUTTransport.DateUpdated = DateTime.Now;
                            firstOUTTransport.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                            firstINTransport.ChangeRoute = "Delete On site delete data Roster";
                            firstOUTTransport.ChangeRoute = "Delete On site delete data Roster";


                            Context.Transport.Remove(firstOUTTransport);
                            Context.Transport.Remove(firstINTransport);

                        }

                    }

                }
            }
            else {
                throw new BadRequestException("This data cannot be deleted because it is restricted by Roster rules. Please review the Roster requirements or contact the administrator for assistance.");
            }
       

            await Task.CompletedTask;
        }

        #endregion




        #region DeleteTransportBulk On RosterExecuteBefore

        public async Task DeleteTransportBulk(DeleteEmployeeTransportBulkRequest request, CancellationToken cancellationToken)
        {
            var currentRosterBreakShift = await Context.Shift.AsNoTracking().Where(x => x.Code == "RR").FirstOrDefaultAsync();

            foreach (var employeeId in request.employeeIds)
            {
                if (employeeId == 427397)
                {
                    var aa = 0;
                }
                var onsiteStatus = await OnsiteCheckEmployeeByRoster(employeeId, request.OnsiteDate.Date);
                if (onsiteStatus)
                {




                    var firstINTransport = await Context.Transport
                        .Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date <= request.OnsiteDate.Date && x.Direction == "IN").OrderByDescending(x => x.EventDate).FirstOrDefaultAsync();



                    if (firstINTransport != null)
                    {

                        var inafterOUTTransport = await Context.Transport
                             .Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date <= request.OnsiteDate.Date && x.EventDate >= firstINTransport.EventDate && x.Direction == "OUT").OrderBy(x => x.EventDate).FirstOrDefaultAsync();


                        var firstOUTTransport = await Context.Transport
                             .Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date >= request.OnsiteDate.Date && x.Direction == "OUT").OrderBy(x => x.EventDate).FirstOrDefaultAsync();




                        if (firstINTransport != null && firstOUTTransport != null && inafterOUTTransport == null)
                        {




                            var employeeOnsiteStatusDates = await Context.EmployeeStatus
                                 .Where(x => x.EmployeeId == employeeId
                                 && x.EventDate >= firstINTransport.EventDate.Value.Date
                                 && x.EventDate < firstOUTTransport.EventDate.Value.Date).ToListAsync();


                            if (currentRosterBreakShift != null)
                            {
                                foreach (var item in employeeOnsiteStatusDates)
                                {
                                    item.RoomId = null;
                                    item.BedId = null;
                                    item.ShiftId = currentRosterBreakShift.Id;
                                    item.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                                    item.DateUpdated = DateTime.Now;
                                    item.ChangeRoute = "Bulk roster RosterExecuteBefore delete";

                                }

                                Context.EmployeeStatus.UpdateRange(employeeOnsiteStatusDates);
                            }
                            else
                            {

                                Context.EmployeeStatus.RemoveRange(employeeOnsiteStatusDates);
                            }


                            firstOUTTransport.ChangeRoute = "Bulk roster RosterExecuteBefore  delete";
                            firstINTransport.ChangeRoute = "Bulk roster RosterExecuteBefore delete ";

                            Context.Transport.Remove(firstOUTTransport);
                            Context.Transport.Remove(firstINTransport);

                        }
                    }
                }



             
            }
         

            await Task.CompletedTask;
        }




        #endregion


        public async Task<bool> OnsiteCheckEmployeeByRoster(int employeeId, DateTime rosterStartDate)
        {

    
             var rosterDateOnsite =await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.RoomId != null && x.EventDate.Value.Date == rosterStartDate.Date).FirstOrDefaultAsync();



            //if (rosterDateOnsite == null) 
            //{
            //    return true;
            //}
            if (DateTime.Today > rosterStartDate)
            {
                return false;
            }
            

            if ((rosterStartDate.Date - DateTime.Today).Days <= 7)
            {
                var todayFromRosterStartDateOnsite = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.RoomId != null && x.EventDate.Value.Date >= DateTime.Today.Date  && x.EventDate.Value.Date <= rosterStartDate.Date).ToListAsync();
                if (todayFromRosterStartDateOnsite.Count > 0)
                {
                    return false;
                }
                else{
                    return true;
                }
            }
            else {
                if (rosterDateOnsite == null)
                {
                    return true;
                }
                else {

                    if ((rosterDateOnsite.EventDate.Value.Date - DateTime.Today).Days <= 7)
                    {
                        return false;
                    }
                    else {

             
                        var rosterbeforeLastTransport = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date <= rosterStartDate).OrderByDescending(x => x.EventDateTime).FirstOrDefaultAsync();
                        if ((rosterbeforeLastTransport.EventDate.Value.Date - DateTime.Today).Days >= 7)
                        {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }

                }
            }


        }



    }
}
