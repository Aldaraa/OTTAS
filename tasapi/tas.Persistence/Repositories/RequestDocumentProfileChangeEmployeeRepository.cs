using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeFeature.GetEmployee;
using tas.Application.Features.EmployeeFeature.StatusEmployee;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChangeTemp;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;
using tas.Application.Features.RequestDocumentProfileChangeFeature.UpdateRequestDocumentProfileChangeTemp;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CompleteRequestDocumentProfileChangeTemp;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Service;

namespace tas.Persistence.Repositories
{
    public class RequestDocumentProfileChangeEmployeeRepository : BaseRepository<RequestDocumentProfileChangeEmployee>, IRequestDocumentProfileChangeEmployeeRepository
    {
        private readonly HTTPUserRepository _hTTPUserRepository;
        private readonly IConfiguration _configuration;
        private readonly CacheService _memoryCache;


        public RequestDocumentProfileChangeEmployeeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, CacheService memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _hTTPUserRepository = hTTPUserRepository;
            _configuration = configuration;
            _memoryCache = memoryCache; 
        }





        #region CreateProfileChangeTemp



        public async Task<int> CreateRequestDocumentProfileChangeTemp(CreateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
            var assignedGroup = await Context.RequestGroupEmployee.Where(x => x.EmployeeId == request.changeRequestData.assignedEmployeeId).FirstOrDefaultAsync();
            var reqDocument = new RequestDocument();
            reqDocument.CurrentAction = request.changeRequestData.action;
            reqDocument.Active = 1;
            reqDocument.DateCreated = DateTime.Now;
            reqDocument.UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id;
            reqDocument.Description = reqDocument.Description;
            reqDocument.EmployeeId = request.Employee.EmployeeId;
            reqDocument.DocumentType = RequestDocumentType.ProfileChanges;
            reqDocument.DocumentTag = "temp";

            reqDocument.AssignedEmployeeId = request.changeRequestData?.assignedEmployeeId;
            reqDocument.AssignedRouteConfigId = request.changeRequestData?.nextGroupId;
            Context.RequestDocument.Add(reqDocument);
            await Context.SaveChangesAsync();

            await SaveDocumentEmpoyeeTemp(request, reqDocument.Id, cancellationToken);
            await SaveDocumentHistoryTemp(request, reqDocument.Id);

            await Context.SaveChangesAsync();
            return reqDocument.Id;

        }


        private async Task SaveDocumentHistoryTemp(CreateRequestDocumentProfileChangeTempRequest request, int DocumentId)
        { 
            var reqHist = new RequestDocumentHistory();
            reqHist.Comment = request.changeRequestData.comment;
            reqHist.Active = 1;
            reqHist.DateCreated = DateTime.Now;
            reqHist.UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.DocumentId = DocumentId;
            reqHist.ActionEmployeeId = _hTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.CurrentAction = request.changeRequestData.action;
            Context.RequestDocumentHistory.Add(reqHist);

        }

        public async Task SaveDocumentEmpoyeeTemp(CreateRequestDocumentProfileChangeTempRequest request, int DocumentId, CancellationToken cancellationToken)
        {

            var newRecord = new RequestDocumentProfileChangeEmployeeTemp
            {
                Active = 1,
                CostCodeId = request.Employee.CostCodeId,
                DepartmentId = request.Employee.DepartmentId,
                DateCreated = DateTime.Now,
                StartDate = request.Employee.StartDate,
                EndDate = request.Employee.EndDate,
                EmployerId = request.Employee.EmployerId,
                PositionId = request.Employee.PositionId,
                DocumentId = DocumentId,
                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
            };

            Context.RequestDocumentProfileChangeEmployeeTemp.Add(newRecord);
            await Task.CompletedTask;
        }


        #endregion



        //      Task CreateRequestDocumentProfileChange(CreateRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken);
        public async Task<int> CreateRequestDocumentProfileChange(CreateRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {
            //if (request.ChangeRequestData.AssignedEmployeeId == null && request.ChangeRequestData.Action == RequestDocumentAction.Submitted)
            //{
            //    throw new BadRequestException("Assign Employee not null");
            //}
            var assignedGroup = await Context.RequestGroupEmployee.Where(x => x.EmployeeId == request.ChangeRequestData.AssignedEmployeeId).FirstOrDefaultAsync();
            var reqDocument = new RequestDocument();
            reqDocument.CurrentAction = request.ChangeRequestData.Action;
            reqDocument.Active = 1;
            reqDocument.DateCreated = DateTime.Now;
            reqDocument.UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id;
            reqDocument.Description = reqDocument.Description;
            reqDocument.EmployeeId = request.Employee.EmployeeId;
            reqDocument.DocumentType = RequestDocumentType.ProfileChanges;

            reqDocument.AssignedEmployeeId = request.ChangeRequestData?.AssignedEmployeeId;
            reqDocument.AssignedRouteConfigId = request.ChangeRequestData?.NextGroupId;
            Context.RequestDocument.Add(reqDocument);
            await Context.SaveChangesAsync();

            await SaveDocumentEmpoyee(request, reqDocument.Id, cancellationToken);
            await SaveDocumentHistory(request, reqDocument.Id);
            await Context.SaveChangesAsync(); 

            return reqDocument.Id;

        }




        private async Task SaveDocumentHistory(CreateRequestDocumentProfileChangeRequest request, int DocumentId)
        {

            var reqHist = new RequestDocumentHistory();
            reqHist.Comment = request.ChangeRequestData.Comment;
            reqHist.Active = 1;
            reqHist.DateCreated = DateTime.Now;
            reqHist.UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.DocumentId = DocumentId;
            reqHist.ActionEmployeeId = _hTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.CurrentAction = request.ChangeRequestData.Action;
            Context.RequestDocumentHistory.Add(reqHist);
            await Task.CompletedTask;

        }


        public async Task UpdateRequestDocumentProfileChange(UpdateRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {

            var currentData = await Context.RequestDocumentProfileChangeEmployee.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            
            if (currentData != null)
            {
                var currentDocument = await Context.RequestDocument.Where(x => x.Id == currentData.DocumentId).FirstOrDefaultAsync();
                if (currentDocument != null)
                {
                    if (currentDocument.CurrentAction == RequestDocumentAction.Cancelled || currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {
                        throw new BadRequestException($"The data you provided is currently in a \"{currentDocument.CurrentAction}\" state, which means it cannot be  modified.");
                    }
                    else {

                        currentData.Active = 1;
                        currentData.ADAccount = request.ADAccount;
                        currentData.CommenceDate = request.CommenceDate;
                        currentData.ContractNumber = request.ContractNumber;
                        currentData.Lastname = request.Lastname;
                        currentData.Firstname = request.Firstname;
                        currentData.MFirstname = request.MFirstname;
                        currentData.MLastname = request.MLastname;
                        currentData.CostCodeId = request.CostCodeId;
                        currentData.DepartmentId = request.DepartmentId;
                        currentData.Email = request.Email;
                        currentData.Dob = request.Dob;
                        currentData.Gender = request.Gender;
                        currentData.NRN = request.NRN;
                        currentData.EmergencyContactMobile = request.EmergencyContactMobile;
                        currentData.EmployerId = request.EmployerId;
                        currentData.HotelCheck = request.HotelCheck;
                        currentData.FlightGroupMasterId = request.FlightGroupMasterId;
                        currentData.LocationId = request.LocationId;
                        currentData.NationalityId = request.NationalityId;
                        currentData.Mobile = request.Mobile;
                        currentData.EmergencyContactName = request?.EmergencyContactName;
                        currentData.LoginEnabled = request?.LoginEnabled;
                        currentData.PassportExpiry = request?.PassportExpiry;
                        currentData.PassportNumber = request?.PassportNumber;
                        currentData.PeopleTypeId = request.PeopleTypeId;
                        currentData.PickUpAddress = request.PickUpAddress;
                        currentData.PersonalMobile = request.PersonalMobile;
                        currentData.PositionId = request.PositionId;
                        currentData.RoomId = request.RoomId;
                        currentData.RosterId = request.RosterId;
                        currentData.Shiftid = request.ShiftId;
                        currentData.SiteContactEmployeeId = request.SiteContactEmployeeId;
                        currentData.SAPID = request.SAPID;
                        currentData.stateId = request.StateId;
                        currentData.PassportName = request.PassportName;
                        currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        currentData.DateUpdated = DateTime.Now;

                        Context.RequestDocumentProfileChangeEmployee.Update(currentData);
                        await Task.CompletedTask;
                    }
                }


            }


        }



        public async Task SaveDocumentEmpoyee(CreateRequestDocumentProfileChangeRequest request, int DocumentId, CancellationToken cancellationToken)
        {

            var mnnationality = await Context.Nationality.AsNoTracking().Where(x => x.Code == "MN").FirstOrDefaultAsync();

            if (mnnationality != null)
            {
                var tasprofile = await Context.Employee.Where(c=> c.Id == request.Employee.EmployeeId).FirstOrDefaultAsync();
                if (tasprofile.NationalityId == mnnationality.Id)
                {
                    var newRecord = new RequestDocumentProfileChangeEmployee
                    {
                        Active = 1,
                        ADAccount = request.Employee.ADAccount,
                        CommenceDate = request.Employee.CommenceDate,
                        ContractNumber = request.Employee.ContractNumber,
                        Lastname = request.Employee.Lastname,
                        Firstname = request.Employee.Firstname,
                        MFirstname = request.Employee.MFirstname,
                        MLastname = request.Employee.MLastname,
                        CostCodeId = request.Employee.CostCodeId,
                        DepartmentId = request.Employee.DepartmentId,
                        Email = request.Employee.Email,
                        Dob = request.Employee.Dob,
                        EmployeeId = request.Employee.EmployeeId,
                        DateCreated = DateTime.Now,
                        NRN = request.Employee.NRN,
                        Gender = request.Employee.Gender,
                        EmergencyContactMobile = request.Employee.EmergencyContactMobile,
                        EmployerId = request.Employee.EmployerId,
                        HotelCheck = request.Employee.HotelCheck,
                        FlightGroupMasterId = request.Employee.FlightGroupMasterId,
                        LocationId = request.Employee.LocationId,
                        NationalityId = request.Employee.NationalityId,
                        Mobile = request.Employee.Mobile,
                        EmergencyContactName = request.Employee?.EmergencyContactName,
                        LoginEnabled = request.Employee?.LoginEnabled,
                        PeopleTypeId = request.Employee.PeopleTypeId,
                        PickUpAddress = request.Employee.PickUpAddress,
                        PersonalMobile = request.Employee.PersonalMobile,
                        PositionId = request.Employee.PositionId,
                        RoomId = request.Employee.RoomId,
                        RosterId = request.Employee.RosterId,
                        Shiftid = request.Employee.ShiftId,
                        SiteContactEmployeeId = request.Employee.SiteContactEmployeeId,
                        SAPID = request.Employee.SAPID,
                        stateId = request.Employee.StateId,
                        DocumentId = DocumentId,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    };

                    Context.RequestDocumentProfileChangeEmployee.Add(newRecord);
                    await Task.CompletedTask;
                }
                else {
                    var newRecord = new RequestDocumentProfileChangeEmployee
                    {
                        Active = 1,
                        ADAccount = request.Employee.ADAccount,
                        CommenceDate = request.Employee.CommenceDate,
                        ContractNumber = request.Employee.ContractNumber,
                        Lastname = request.Employee.Lastname,
                        Firstname = request.Employee.Firstname,
                        MFirstname = request.Employee.MFirstname,
                        MLastname = request.Employee.MLastname,
                        CostCodeId = request.Employee.CostCodeId,
                        DepartmentId = request.Employee.DepartmentId,
                        Email = request.Employee.Email,
                        Dob = request.Employee.Dob,
                        EmployeeId = request.Employee.EmployeeId,
                        DateCreated = DateTime.Now,
                        Gender = request.Employee.Gender,
                        EmergencyContactMobile = request.Employee.EmergencyContactMobile,
                        EmployerId = request.Employee.EmployerId,
                        NRN = request.Employee.NRN,
                        HotelCheck = request.Employee.HotelCheck,
                        FlightGroupMasterId = request.Employee.FlightGroupMasterId,
                        LocationId = request.Employee.LocationId,
                        NationalityId = request.Employee.NationalityId,
                        Mobile = request.Employee.Mobile,
                        EmergencyContactName = request.Employee?.EmergencyContactName,
                        LoginEnabled = request.Employee?.LoginEnabled,
                        PassportExpiry = request.Employee?.PassportExpiry,
                        PassportNumber = request.Employee?.PassportNumber,
                        PeopleTypeId = request.Employee?.PeopleTypeId,
                        PickUpAddress = request.Employee.PickUpAddress,
                        PersonalMobile = request.Employee.PersonalMobile,
                        PositionId = request.Employee.PositionId,
                        RoomId = request.Employee.RoomId,
                        RosterId = request.Employee.RosterId,
                        Shiftid = request.Employee.ShiftId,
                        SiteContactEmployeeId = request.Employee.SiteContactEmployeeId,
                        SAPID = request.Employee.SAPID,
                        stateId = request.Employee.StateId,
                        DocumentId = DocumentId,
                        PassportName = request.Employee.PassportName,
                        UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                    };

                    Context.RequestDocumentProfileChangeEmployee.Add(newRecord);
                    await Task.CompletedTask;
                }
                    
            }
            else {
                var newRecord = new RequestDocumentProfileChangeEmployee
                {
                    Active = 1,
                    ADAccount = request.Employee.ADAccount,
                    CommenceDate = request.Employee.CommenceDate,
                    ContractNumber = request.Employee.ContractNumber,
                    Lastname = request.Employee.Lastname,
                    Firstname = request.Employee.Firstname,
                    MFirstname = request.Employee.MFirstname,
                    MLastname = request.Employee.MLastname,
                    CostCodeId = request.Employee.CostCodeId,
                    DepartmentId = request.Employee.DepartmentId,
                    Email = request.Employee.Email,
                    Dob = request.Employee.Dob,
                    EmployeeId = request.Employee.EmployeeId,
                    DateCreated = DateTime.Now,
                    Gender = request.Employee.Gender,
                    EmergencyContactMobile = request.Employee.EmergencyContactMobile,
                    EmployerId = request.Employee.EmployerId,
                    HotelCheck = request.Employee.HotelCheck,
                    FlightGroupMasterId = request.Employee.FlightGroupMasterId,
                    LocationId = request.Employee.LocationId,
                    NationalityId = request.Employee.NationalityId,
                    Mobile = request.Employee.Mobile,
                    EmergencyContactName = request.Employee?.EmergencyContactName,
                    LoginEnabled = request.Employee?.LoginEnabled,
                    PassportExpiry = request.Employee?.PassportExpiry,
                    PassportNumber = request.Employee?.PassportNumber,
                    NRN = request.Employee?.NRN,
                    PeopleTypeId = request.Employee.PeopleTypeId,
                    PickUpAddress = request.Employee.PickUpAddress,
                    PersonalMobile = request.Employee.PersonalMobile,
                    PositionId = request.Employee.PositionId,
                    RoomId = request.Employee.RoomId,
                    RosterId = request.Employee.RosterId,
                    Shiftid = request.Employee.ShiftId,
                    SiteContactEmployeeId = request.Employee.SiteContactEmployeeId,
                    SAPID = request.Employee.SAPID,
                    stateId = request.Employee.StateId,
                    DocumentId = DocumentId,
                    PassportName = request.Employee.PassportName,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                };

                Context.RequestDocumentProfileChangeEmployee.Add(newRecord);
                await Task.CompletedTask;
            }

          
        }



        public async Task<GetRequestDocumentProfileChangeResponse> GetRequestDocumentProfile(GetRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
            var returnData = new GetRequestDocumentProfileChangeResponse();
            if (currentDocument != null)
            {

                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync(cancellationToken);
                var assignEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync(cancellationToken);
                var updatedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdUpdated).FirstOrDefaultAsync(cancellationToken);
                var createdEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdCreated).FirstOrDefaultAsync(cancellationToken);



                var deleagationEmployee = await Context.RequestDelegates.Where(x => x.FromEmployeeId == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync();

                var userId = _hTTPUserRepository.LogCurrentUser()?.Id;

                returnData.Id = currentDocument.Id;
                returnData.RequestedDate = currentDocument.DateCreated;
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.EmployeeFullName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                returnData.AssignedEmployeeFullName = $"{assignEmployee?.Firstname} {assignEmployee?.Lastname}";
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.DocumentType = currentDocument.DocumentType;
                returnData.UpdatedInfo = $"{currentDocument.DateUpdated} {updatedEmployee?.Firstname} {updatedEmployee?.Lastname}";
                returnData.RequesterFullName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}";
                returnData.RequesterMail = createdEmployee?.Email;
                returnData.RequesterMobile = createdEmployee?.PersonalMobile;
                returnData.RequestUserId = createdEmployee?.Id;
                returnData.AssignedEmployeeId = currentDocument.AssignedEmployeeId;
                returnData.AssignedRouteConfigId = currentDocument.AssignedRouteConfigId;
                returnData.EmployeeId = currentDocument?.EmployeeId;
                returnData.DelegateEmployeeId = deleagationEmployee?.ToEmployeeId;
                returnData.DocumentTag = currentDocument?.DocumentTag;


                try
                {
                    returnData.DaysAway = currentDocument.DaysAwayDate.HasValue ? (DateTime.Today.Subtract(currentDocument.DaysAwayDate.Value).Days) * (-1) : (DateTime.Today.Subtract(currentDocument.DateCreated.Value).Days) * (-1);
                }
                catch (Exception)
                {

                    returnData.DaysAway = 0;
                }


                int Id = currentDocument.EmployeeId.Value;
                var employeeStatuses = await GetEmployeeStatusDates(Id, DateTime.Now);
                var employeeTransports = await EmployeeTransports(Id, DateTime.Now);

                var employeeTransport = Context.Transport.FirstOrDefault(x => x.EmployeeId == Id);

                var activeGroups = await Context.GroupMaster
                    .Where(x => x.Active == 1 && x.ShowOnProfile == 1)
                    .Select(x => x.Id)
                    .ToListAsync();


                var employeeGroupData = await Context.GroupMembers
                    .Where(x => x.EmployeeId == Id && (x.GroupMasterId == null || activeGroups.Contains(x.GroupMasterId.Value)))
                    .Select(x => new GetRequestDocumentProfileChangeEmployeeInfoGroup
                    {
                        Id = x.Id,
                        GroupDetailId = x.GroupDetailId,
                        GroupMasterId = x.GroupMasterId
                    })
                    .ToListAsync();

                DateTime today = DateTime.Today;
                var employeeNextTransport =await Context.Transport.AsNoTracking()
                        .Where(t => t.EventDate.HasValue && t.EventDate.Value.Date >= today && t.EmployeeId == Id)
                        .OrderBy(t => t.EventDate)
                        .FirstOrDefaultAsync();

                //var employeeHistory = await (from history in Context.EmployeeHistory.Where(e => e.EmployeeId == Id)
                //                             join terminationType in Context.TerminationType on
                //                             history.TerminationTypeId equals terminationType.Id into terminationData
                //                             from terminationType in terminationData.DefaultIfEmpty()
                //                             select new GetProfileEmployeeHistory
                //                             {
                //                                 Id = history.Id,
                //                                 Action = history.Action,
                //                                 Comment = history.Comment,
                //                                 EventDate = history.EventDate,
                //                                 TerminationTypeName = $"{terminationType.Code} {terminationType.Description}",
                //                                 EmployeeId = history.EmployeeId

                //                             }).Where(x => x.EmployeeId == Id).ToListAsync();



               var  returnDataEmployee = await (from employee in Context.RequestDocumentProfileChangeEmployee.Where(x=> x.DocumentId == request.DocumentId)
                                        join state in Context.State on employee.stateId equals state.Id into stateData
                                        from state in stateData.DefaultIfEmpty()
                                        join shift in Context.Shift on employee.Shiftid equals shift.Id into shiftData
                                        from shift in shiftData.DefaultIfEmpty()
                                        join costcode in Context.CostCodes on employee.CostCodeId equals costcode.Id into costcodeData
                                        from costcode in costcodeData.DefaultIfEmpty()
                                        join department in Context.Department on employee.DepartmentId equals department.Id into departmentData
                                        from department in departmentData.DefaultIfEmpty()
                                        join position in Context.Position on employee.PositionId equals position.Id into positionData
                                        from position in positionData.DefaultIfEmpty()
                                        join roster in Context.Roster on employee.RosterId equals roster.Id into rosterData
                                        from roster in rosterData.DefaultIfEmpty()
                                        join location in Context.Location on employee.LocationId equals location.Id into locationData
                                        from location in locationData.DefaultIfEmpty()
                                        join peopletype in Context.PeopleType on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                        from peopletype in peopletypeData.DefaultIfEmpty()
                                        join room in Context.Room on employee.RoomId equals room.Id into roomData
                                        from room in roomData.DefaultIfEmpty()
                                        join flightgroupmaster in Context.FlightGroupMaster on employee.FlightGroupMasterId equals flightgroupmaster.Id into flightgroupmasterData
                                        from flightgroupmaster in flightgroupmasterData.DefaultIfEmpty()
                                        join employer in Context.Employer on employee.EmployerId equals employer.Id into employerData
                                        from employer in employerData.DefaultIfEmpty()
                                        join nationality in Context.Nationality on employee.NationalityId equals nationality.Id into nationalityData
                                        from nationality in nationalityData.DefaultIfEmpty()
                                        join sitecontactempployee in Context.Employee on employee.SiteContactEmployeeId equals sitecontactempployee.Id into employeeData
                                        from sitecontactempdeployee in employeeData.DefaultIfEmpty()
                                        select new GetRequestDocumentProfileChangeEmployee
                                        {

                                            Id = employee.Id,
                                            EmployeeId = employee.EmployeeId,
                                            Lastname = employee.Lastname,
                                            Firstname = employee.Firstname,
                                            MLastname = employee.MLastname,
                                            MFirstname = employee.MFirstname,
                                            Mobile = employee.Mobile,
                                            Email = employee.Email,
                                            EmployerName = employer.Description,
                                            StateId = employee.stateId,
                                            StateName = state.Description,
                                            ShiftId = employee.Shiftid,
                                            ShiftName = shift.Description,
                                            CostCodeName = costcode.Description,
                                            DepartmentName = department.Name,
                                            PositionName = position.Description,
                                            RosterName = roster.Name,
                                            LocationName = location.Description,
                                            PeopleTypeName = peopletype.Code,
                                            RoomNumber = room.Number,
                                            RoomTypeId = room.RoomTypeId,
                                            FlightGroupMasterName = flightgroupmaster.Description,
                                            SAPID = employee.SAPID,
                                            CostCodeId = employee.CostCodeId,
                                            FlightGroupMasterId = employee.FlightGroupMasterId,
                                            LocationId = employee.LocationId,
                                            PeopleTypeId = employee.PeopleTypeId,
                                            RosterId = employee.RosterId,
                                            DepartmentId = employee.DepartmentId,
                                            Active = employee.Active,
                                            ADAccount = employee.ADAccount,
                                            CommenceDate = employee.CommenceDate,
                                            ContractNumber = employee.ContractNumber,
                                            Dob = employee.Dob,
                                            NRN = employee.NRN,
                                            Gender = employee.Gender,
                                            HotelCheck = employee.HotelCheck,
                                            NationalityId = employee.NationalityId,
                                            SiteContactEmployeeId = employee.SiteContactEmployeeId,
                                            DateCreated = employee.DateCreated,
                                            LoginEnabled = employee.LoginEnabled,
                                            NationalityName = nationality.Description,
                                            DateUpdated = employee.DateUpdated,
                                            PassportExpiry = employee.PassportExpiry,
                                            PassportImage = employee.PassportImage,
                                            PassportNumber = employee.PassportNumber,
                                            PositionId = employee.PositionId,
                                            PersonalMobile = employee.PersonalMobile,
                                            EmergencyContactMobile = employee.EmergencyContactMobile,
                                            EmergencyContactName = employee.EmergencyContactName,
                                            PassportName = employee.PassportName,
                                            PickUpAddress = employee.PickUpAddress,
                                            SiteContactEmployeeLastname = sitecontactempdeployee.Lastname,
                                            SiteContactEmployeeFirstname = sitecontactempdeployee.Firstname,
                                            SiteContactEmployeeMobile = sitecontactempdeployee.Mobile,
                                            EmployerId = employee.EmployerId,
                                            RoomId = employee.RoomId,
                                            NextRosterDate = employeeNextTransport == null ? null : employeeNextTransport.EventDate,
                                            employeeStatusDates = employeeStatuses,
                                            employeeTransports = employeeTransports,
                                            GroupData = employeeGroupData
                                        }).FirstOrDefaultAsync(cancellationToken);
                if (returnDataEmployee != null) {
                    returnDataEmployee.employeeStatusDates = employeeStatuses;
                    returnDataEmployee.employeeTransports = employeeTransports;
                    returnData.ChangeInfo = await GetChangeInfo(returnDataEmployee, cancellationToken);
                    returnData.employee = returnDataEmployee;
                }
            }

            return returnData;
        }





        private async Task<List<GetRequestDocumentProfileChangeEmployeeChangeInfo>> GetChangeInfo(GetRequestDocumentProfileChangeEmployee requestEmployeeData, CancellationToken cancellationToken)
        {

            var mnnationality = await Context.Nationality.AsNoTracking().Where(x => x.Code == "MN").FirstOrDefaultAsync();
            List<string> skipfields = new List<string> { "employeeStatusDates", "NextRosterDate", "employeeTransports", "GroupData", "PassportImage", "DateCreated", "DateUpdated",
            "Id", "StateId", "RoomTypeId", "PeopleTypeId", "DepartmentId", "RosterId", "PositionId", "RoomNumber", "RoomId", "EmployerId", "NationalityId", "NRN", "LoginEnabled"
            };

            if (mnnationality != null)
            {
                var tasprofile = await Context.Employee.AsNoTracking().Where(c => c.Id == requestEmployeeData.EmployeeId).FirstOrDefaultAsync();
                if (tasprofile != null)
                {
                    if (tasprofile.NationalityId == mnnationality.Id)
                    {
                        skipfields.Add("PassportExpiry");
                        skipfields.Add("PassportName");
                        skipfields.Add("PassportNumber");


                    }
                }

            }
                    var returnData = new List<GetRequestDocumentProfileChangeEmployeeChangeInfo>();
                    var tasEmployeeData = await (from employee in Context.Employee.Where(x => x.Id == requestEmployeeData.EmployeeId)
                                                 join state in Context.State on employee.StateId equals state.Id into stateData
                                                 from state in stateData.DefaultIfEmpty()
                                                 join shift in Context.Shift on employee.Shiftid equals shift.Id into shiftData
                                                 from shift in shiftData.DefaultIfEmpty()
                                                 join costcode in Context.CostCodes on employee.CostCodeId equals costcode.Id into costcodeData
                                                 from costcode in costcodeData.DefaultIfEmpty()
                                                 join department in Context.Department on employee.DepartmentId equals department.Id into departmentData
                                                 from department in departmentData.DefaultIfEmpty()
                                                 join position in Context.Position on employee.PositionId equals position.Id into positionData
                                                 from position in positionData.DefaultIfEmpty()
                                                 join roster in Context.Roster on employee.RosterId equals roster.Id into rosterData
                                                 from roster in rosterData.DefaultIfEmpty()
                                                 join location in Context.Location on employee.LocationId equals location.Id into locationData
                                                 from location in locationData.DefaultIfEmpty()
                                                 join peopletype in Context.PeopleType on employee.PeopleTypeId equals peopletype.Id into peopletypeData
                                                 from peopletype in peopletypeData.DefaultIfEmpty()
                                                 join room in Context.Room on employee.RoomId equals room.Id into roomData
                                                 from room in roomData.DefaultIfEmpty()
                                                 join flightgroupmaster in Context.FlightGroupMaster on employee.FlightGroupMasterId equals flightgroupmaster.Id into flightgroupmasterData
                                                 from flightgroupmaster in flightgroupmasterData.DefaultIfEmpty()
                                                 join employer in Context.Employer on employee.EmployerId equals employer.Id into employerData
                                                 from employer in employerData.DefaultIfEmpty()
                                                 join nationality in Context.Nationality on employee.NationalityId equals nationality.Id into nationalityData
                                                 from nationality in nationalityData.DefaultIfEmpty()
                                                 join sitecontactempployee in Context.Employee on employee.SiteContactEmployeeId equals sitecontactempployee.Id into employeeData
                                                 from sitecontactempdeployee in employeeData.DefaultIfEmpty()
                                                 select new GetRequestDocumentProfileChangeEmployee
                                                 {

                                                     Id = employee.Id,
                                                     EmployeeId = employee.Id,
                                                     Lastname = employee.Lastname,
                                                     Firstname = employee.Firstname,
                                                     MLastname = employee.MLastname,
                                                     MFirstname = employee.MFirstname,
                                                     Mobile = employee.Mobile,
                                                     Email = employee.Email,
                                                     EmployerName = employer.Description,
                                                     StateId = employee.StateId,
                                                     StateName = state.Description,
                                                     ShiftId = employee.Shiftid,
                                                     ShiftName = shift.Description,
                                                     CostCodeName = costcode.Description,
                                                     DepartmentName = department.Name,
                                                     PositionName = position.Description,
                                                     RosterName = roster.Name,
                                                     LocationName = location.Description,
                                                     PeopleTypeName = peopletype.Code,
                                                     RoomNumber = room.Number,
                                                     RoomTypeId = room.RoomTypeId,
                                                     FlightGroupMasterName = flightgroupmaster.Description,
                                                     SAPID = employee.SAPID,
                                                     CostCodeId = employee.CostCodeId,
                                                     FlightGroupMasterId = employee.FlightGroupMasterId,
                                                     LocationId = employee.LocationId,
                                                     PeopleTypeId = employee.PeopleTypeId,
                                                     RosterId = employee.RosterId,
                                                     DepartmentId = employee.DepartmentId,
                                                     Active = employee.Active,
                                                     ADAccount = employee.ADAccount,
                                                     CommenceDate = employee.CommenceDate,
                                                     ContractNumber = employee.ContractNumber,
                                                     Dob = employee.Dob,
                                                     Gender = employee.Gender,
                                                     HotelCheck = employee.HotelCheck,
                                                     NationalityId = employee.NationalityId,
                                                     SiteContactEmployeeId = employee.SiteContactEmployeeId,
                                                     DateCreated = employee.DateCreated,
                                                     LoginEnabled = employee.LoginEnabled,
                                                     NationalityName = nationality.Description,
                                                     DateUpdated = employee.DateUpdated,
                                                     PassportExpiry = employee.PassportExpiry,
                                                     PassportNumber = employee.PassportNumber,
                                                     PositionId = employee.PositionId,
                                                     PersonalMobile = employee.PersonalMobile,
                                                     EmergencyContactMobile = employee.EmergencyContactMobile,
                                                     EmergencyContactName = employee.EmergencyContactName,
                                                     PassportName = employee.PassportName,
                                                     PickUpAddress = employee.PickUpAddress,
                                                     SiteContactEmployeeLastname = sitecontactempdeployee.Lastname,
                                                     SiteContactEmployeeFirstname = sitecontactempdeployee.Firstname,
                                                     SiteContactEmployeeMobile = sitecontactempdeployee.Mobile,
                                                     EmployerId = employee.EmployerId,
                                                     RoomId = employee.RoomId,
                                                     CompletionDate = employee.CompletionDate
                                                 }).FirstOrDefaultAsync(cancellationToken);



                    foreach (var field in tasEmployeeData.GetType().GetProperties())
                    {

                        var oldValue = field.GetValue(tasEmployeeData);
                        var newValue = field.GetValue(requestEmployeeData);
                        if (Convert.ToString(oldValue) != Convert.ToString(newValue))
                        {

                            if (skipfields.IndexOf(field.Name) == -1)
                            {
                        
                                var newRecord = new GetRequestDocumentProfileChangeEmployeeChangeInfo
                                {
                                    FieldName = field.Name,
                                    OldValue = Convert.ToString(oldValue),
                                    NewValue = Convert.ToString(newValue)

                                };

                                returnData.Add(newRecord);
                            }


                        }


                    }

                    return returnData;

                
              
           
           
        }








        private async Task<List<GetRequestDocumentProfileChangeEmployeeTransport>> EmployeeTransports(int employeeId, DateTime currentDate)
        {

            DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddDays(-15);
            DateTime endOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1).AddDays(-1).AddMonths(2);


            var query = from t in Context.Transport
                        join at in Context.ActiveTransport on t.ActiveTransportId equals at.Id into activeTransportGroup
                        from activeTransport in activeTransportGroup.DefaultIfEmpty()
                        join ts in Context.TransportSchedule on t.ActiveTransportId equals ts.ActiveTransportId into activeTransportSchedule
                        from ts in activeTransportSchedule.DefaultIfEmpty()
                        where t.EmployeeId == employeeId && t.EventDate >= startOfMonth
                        && t.EventDate <= endOfMonth
                        && ts.EventDate.Date == t.EventDate
                        select new GetRequestDocumentProfileChangeEmployeeTransport
                        {
                            Description = ts.Description,
                            Direction = t.Direction,
                            EventDate = t.EventDate

                        };

            return await query.OrderBy(x => x.EventDate).ToListAsync();
        }

        private async Task<List<GetRequestDocumentProfileChangeEmployeeStatusDate>> GetEmployeeStatusDates(int EmployeeId, DateTime currentDate)
        {

            DateTime startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddDays(-15);
            DateTime endOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1).AddDays(-1).AddMonths(2);
            var query = from es in Context.EmployeeStatus
                        join s in Context.Shift on es.ShiftId equals s.Id into shiftGroup
                        from shift in shiftGroup.DefaultIfEmpty()
                        where es.EmployeeId == EmployeeId
                            && es.EventDate >= startOfMonth
                            && es.EventDate <= endOfMonth
                        select new GetRequestDocumentProfileChangeEmployeeStatusDate
                        {
                            EventDate = es.EventDate,
                            ShiftCode = shift.Code,
                            Direction = Context.Transport.FirstOrDefault(x => x.EmployeeId == EmployeeId && x.EventDate == es.EventDate).Direction,
                            Color = Context.Color.FirstOrDefault(x => x.Id == shift.ColorId).Code
                            //Schedule = GetStatusDateSchedule(EmployeeId, es.EventDate)

                        };

            List<GetRequestDocumentProfileChangeEmployeeStatusDate> dates = query.ToList();
                foreach (var item in dates)
                {
                    if (item.Direction != null)
                    {
                        item.Schedule = await GetStatusDateSchedule(EmployeeId, item.EventDate, item.Direction);
                    }



                }
           


            return dates;

        }

        private async Task<string> GetStatusDateSchedule(int employeeId, DateTime? eventDate, string direction)
        {
            try
            {
                if (!eventDate.HasValue)
                {
                    return string.Empty;
                }

                var currentTransport =await Context.Transport.Where(x=>x.EmployeeId == employeeId && x.Direction == direction
                    && x.EventDate == eventDate).FirstOrDefaultAsync();

                var currentSchedule =await Context.TransportSchedule.Where(x=> x.ActiveTransportId == currentTransport.ActiveTransportId
                    && x.EventDate.Date == currentTransport.EventDate.Value.Date).FirstOrDefaultAsync();

                var currentActiveTransport =await Context.ActiveTransport.FirstOrDefaultAsync(x => x.Id == currentTransport.ActiveTransportId);

                var currentCarrier =await Context.Carrier.FirstOrDefaultAsync(x => x.Id == currentActiveTransport.CarrierId);

                if (currentTransport != null && currentSchedule != null)
                {
                    return string.Format("{0} {1} {2}", currentSchedule.Description, currentCarrier.Description, currentActiveTransport.Seats);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }

           
        }


        #region CompleteDocument

        public async Task CompleteRequestDocumentProfileChange(CompleteRequestDocumentProfileChangeRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.DocumentType == RequestDocumentType.ProfileChanges)
                {


                    if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {
                        throw new BadRequestException("This task already Completed");
                    }
                    else
                    {

                        var currentData = await Context.RequestDocumentProfileChangeEmployee.Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                        if (currentData != null)
                        {
                            await SaveEmployeeData(Convert.ToInt32(currentDocument.EmployeeId), request.documentId);
                            currentDocument.CurrentAction = RequestDocumentAction.Completed;
                            currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                            currentDocument.CompletedUserId = _hTTPUserRepository.LogCurrentUser()?.Id;
                            currentDocument.CompletedDate = DateTime.Now;
                            Context.RequestDocument.Update(currentDocument);
                            var newHistoryRecord = new RequestDocumentHistory
                            {
                                Comment = request.comment,
                                Active = 1,
                                DateCreated = DateTime.Now,
                                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                CurrentAction = RequestDocumentAction.Completed,
                                ActionEmployeeId = _hTTPUserRepository.LogCurrentUser()?.Id,
                                DocumentId = request.documentId
                            };


                            if (currentDocument.EmployeeId.HasValue)
                            {
                                string cacheEntityName = $"Employee_{currentDocument.EmployeeId}";
                                _memoryCache.Remove($"API::{cacheEntityName}");
                            }
                            Context.RequestDocumentHistory.Add(newHistoryRecord);
                        }
                    }

                  
                }
            }

            await Task.CompletedTask;

        }


        private async Task SaveEmployeeData(int employeeId, int documentId)
        {
            var currentEmployeeData = await Context.RequestDocumentProfileChangeEmployee.Where(x => x.EmployeeId == employeeId && x.DocumentId == documentId).FirstOrDefaultAsync();


            var mnnationality = await Context.Nationality.AsNoTracking().Where(x => x.Code == "MN").FirstOrDefaultAsync();

            if (currentEmployeeData != null)
            {
                var currentTasEmployeeData = await Context.Employee.Where(x => x.Id == employeeId).FirstOrDefaultAsync();
                if (currentTasEmployeeData != null)
                {

                    if (mnnationality != null)
                    {
                        if (mnnationality.Id != currentEmployeeData.NationalityId)
                        {
                            currentTasEmployeeData.PassportExpiry = currentEmployeeData.PassportExpiry;
                            currentTasEmployeeData.PassportName = currentEmployeeData.PassportName;
                            currentTasEmployeeData.PassportNumber = currentEmployeeData.PassportNumber;
                        }
                    }

                    currentTasEmployeeData.ADAccount = currentEmployeeData.ADAccount;
                    currentTasEmployeeData.LocationId = currentEmployeeData.LocationId;
                    currentTasEmployeeData.StateId = currentEmployeeData.stateId;
                    currentTasEmployeeData.RosterId = currentEmployeeData.RosterId;
                    currentTasEmployeeData.ContractNumber = currentEmployeeData.ContractNumber;
                    currentTasEmployeeData.MFirstname = currentEmployeeData.MFirstname;
                    currentTasEmployeeData.MLastname = currentEmployeeData.MLastname;
                    currentTasEmployeeData.Firstname = currentEmployeeData.Firstname;
                    currentTasEmployeeData.Lastname = currentEmployeeData.Lastname;
                    currentTasEmployeeData.CostCodeId = currentEmployeeData.CostCodeId;
                    currentTasEmployeeData.DepartmentId = currentEmployeeData.DepartmentId;
                    currentTasEmployeeData.Dob = currentEmployeeData.Dob;
                    currentTasEmployeeData.Email = currentEmployeeData.Email;
                    currentTasEmployeeData.EmergencyContactMobile = currentEmployeeData.EmergencyContactMobile;
                    currentTasEmployeeData.FlightGroupMasterId = currentEmployeeData.FlightGroupMasterId;
                    currentTasEmployeeData.EmployerId = currentEmployeeData.EmployerId;
                    currentTasEmployeeData.EmergencyContactName = currentEmployeeData.EmergencyContactName;
                    currentTasEmployeeData.Gender = currentEmployeeData.Gender;
                    currentTasEmployeeData.HotelCheck = currentEmployeeData.HotelCheck;
                    currentTasEmployeeData.NationalityId = currentEmployeeData.NationalityId;
                    currentTasEmployeeData.PeopleTypeId = currentEmployeeData.PeopleTypeId;
                    currentTasEmployeeData.PersonalMobile = currentEmployeeData.PersonalMobile;
                    currentTasEmployeeData.PickUpAddress = currentEmployeeData.PickUpAddress;
                    currentTasEmployeeData.CompletionDate = currentTasEmployeeData.CompletionDate;
                    currentTasEmployeeData.PositionId = currentEmployeeData.PositionId;
                    currentTasEmployeeData.SAPID = currentEmployeeData.SAPID;
                    currentTasEmployeeData.Shiftid = currentEmployeeData.Shiftid;
                    currentTasEmployeeData.Mobile = currentEmployeeData.Mobile;
                    currentTasEmployeeData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    currentTasEmployeeData.DateUpdated = DateTime.Now;

                    if (currentTasEmployeeData.Active != 1) {
                        currentTasEmployeeData.Active = 1;
                    }

                    Context.Employee.Update(currentTasEmployeeData);



                    var employeeStatusFuturePlan = await Context.EmployeeStatus
                        .Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date >= DateTime.Today)
                        .ToListAsync();

                    var employeeTransportFuturePlan = await
                        Context.Transport.Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date >= DateTime.Today)
                        .ToListAsync();
                    foreach (var item in employeeStatusFuturePlan)
                    {
                        item.CostCodeId = currentEmployeeData.CostCodeId;
                        item.DepId = currentEmployeeData.DepartmentId;
                        item.EmployerId = currentEmployeeData.EmployerId;
                        item.PositionId = currentEmployeeData.PositionId;
                        Context.EmployeeStatus.Update(item);
                    }


                    foreach (var item in employeeTransportFuturePlan)
                    {
                        item.CostCodeId = currentEmployeeData.CostCodeId;
                        item.DepId = currentEmployeeData.DepartmentId;
                        item.EmployerId = currentEmployeeData.EmployerId;
                        item.PositionId = currentEmployeeData.PositionId;
                        Context.Transport.Update(item);
                    }

                }

            }

            await Task.CompletedTask;
        }

        #endregion



        #region TempProfileGetData
        public async Task<GetRequestDocumentProfileChangeTempResponse> GetRequestDocumentProfileTemp(GetRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {

            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
            var returnData = new GetRequestDocumentProfileChangeTempResponse();
            if (currentDocument != null)
            {
                returnData = await Context.RequestDocumentProfileChangeEmployeeTemp.Where(x => x.DocumentId == request.DocumentId).Select(x => new GetRequestDocumentProfileChangeTempResponse
                {
                    Id = x.Id,
                    CostCodeId = x.CostCodeId,
                    DepartmentId = x.DepartmentId,
                    EmployerId = x.EmployerId,
                    PositionId = x.PositionId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Permanent = x.Permanent
                }).FirstOrDefaultAsync();

                return returnData;


            }
            else {
                throw new NotFoundException("Task not found");
            }
        }





        #endregion



        public async Task UpdateRequestDocumentProfileChangeTemp(UpdateRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
            var data = await Context.RequestDocumentProfileChangeEmployeeTemp.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (data != null)
            {
                data.StartDate = request.StartDate;
                data.EndDate = request.EndDate;
                data.PositionId = request.PositionId;
                data.DepartmentId = request.DepartmentId;
                data.EmployerId = request.EmployerId;
                data.CostCodeId = request.CostCodeId;
                data.DateUpdated = DateTime.Now;
                data.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                Context.RequestDocumentProfileChangeEmployeeTemp.Update(data);


                var reqHist = new RequestDocumentHistory();
                reqHist.Comment = "Update change data";
                reqHist.Active = 1;
                reqHist.DateCreated = DateTime.Now;
                reqHist.UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.DocumentId = data.DocumentId.Value;
                reqHist.ActionEmployeeId = _hTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.CurrentAction = "Saved";
                Context.RequestDocumentHistory.Add(reqHist);
                await Task.CompletedTask;
            }




        }



        #region CompleteDocumentTemp
        public async Task CompleteRequestDocumentProfileChangeTemp(CompleteRequestDocumentProfileChangeTempRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.DocumentType == RequestDocumentType.ProfileChanges)
                {


                    if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {
                        throw new BadRequestException("This task already Completed");
                    }
                    else
                    {

                        var currentData = await Context.RequestDocumentProfileChangeEmployeeTemp.Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                        if (currentData != null)
                        {
                            await SaveEmployeeDataTemp(Convert.ToInt32(currentDocument.EmployeeId), request.documentId);
                            currentDocument.CurrentAction = RequestDocumentAction.Completed;
                            currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                            currentDocument.CompletedUserId = _hTTPUserRepository.LogCurrentUser()?.Id;
                            currentDocument.CompletedDate = DateTime.Now;
                            Context.RequestDocument.Update(currentDocument);
                            var newHistoryRecord = new RequestDocumentHistory
                            {
                                Comment = request.comment,
                                Active = 1,
                                DateCreated = DateTime.Now,
                                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                                CurrentAction = RequestDocumentAction.Completed,
                                ActionEmployeeId = _hTTPUserRepository.LogCurrentUser()?.Id,
                                DocumentId = request.documentId
                            };


                            if (currentDocument.EmployeeId.HasValue)
                            {
                                string cacheEntityName = $"Employee_{currentDocument.EmployeeId}";
                                _memoryCache.Remove($"API::{cacheEntityName}");
                            }
                            Context.RequestDocumentHistory.Add(newHistoryRecord);
                        }
                    }


                }
            }

            await Task.CompletedTask;
        }


        private async Task SaveEmployeeDataTemp(int employeeId, int documentId)
        {
            var currentEmployeeData = await Context.RequestDocumentProfileChangeEmployeeTemp.Where(x => x.DocumentId == documentId).FirstOrDefaultAsync();
            if (currentEmployeeData != null)
            {
                var currentTasEmployeeData = await Context.Employee.Where(x => x.Id == employeeId).FirstOrDefaultAsync();
                if (currentTasEmployeeData != null)
                {
                    if (currentEmployeeData.Permanent == 1)
                    {

                        currentTasEmployeeData.PositionId = currentEmployeeData.PositionId == null ? currentTasEmployeeData.PositionId : currentEmployeeData.PositionId;
                        currentTasEmployeeData.DepartmentId = currentEmployeeData.DepartmentId == null ? currentTasEmployeeData.DepartmentId : currentEmployeeData.DepartmentId;
                        currentTasEmployeeData.CostCodeId = currentEmployeeData.CostCodeId == null ? currentTasEmployeeData.CostCodeId : currentEmployeeData.CostCodeId;
                        currentTasEmployeeData.EmployerId = currentEmployeeData.EmployerId == null ? currentTasEmployeeData.EmployerId : currentEmployeeData.EmployerId;
                        currentTasEmployeeData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        currentTasEmployeeData.DateUpdated = DateTime.Now;

                        Context.Employee.Update(currentTasEmployeeData);
                    }

                    await ChangeTempData(currentEmployeeData, documentId, employeeId);
                }



            }




            await Task.CompletedTask;
        }

        //private async Task ChangePermanentData(int employeeId)
        //{
        //    var currentEmployee = await Context.Employee.AsNoTracking()
        //            .Where(x => x.Id == employeeId).FirstOrDefaultAsync();
        //    if (currentEmployee != null)
        //    {

        //        int? costCodeId = currentEmployee.CostCodeId;
        //        int? departmentId = currentEmployee.DepartmentId;
        //        int? employerId = currentEmployee.EmployerId;
        //        int? positionId = currentEmployee.PositionId;
        //        var employeeStatusFuturePlan = await Context.EmployeeStatus
        //            .Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date >= DateTime.Today)
        //            .ToListAsync();

        //        var employeeTransportFuturePlan = await
        //            Context.Transport.Where(x => x.EmployeeId == employeeId && x.EventDate.Value.Date >= DateTime.Today)
        //            .ToListAsync();
        //        foreach (var item in employeeStatusFuturePlan)
        //        {
        //            item.CostCodeId = currentEmployee.CostCodeId;
        //            item.DepId = currentEmployee.DepartmentId;
        //            item.EmployerId = currentEmployee.EmployerId;
        //            item.PositionId = currentEmployee.PositionId;
        //            Context.EmployeeStatus.Update(item);
        //        }


        //        foreach (var item in employeeTransportFuturePlan)
        //        {
        //            item.CostCodeId = currentEmployee.CostCodeId;
        //            item.DepId = currentEmployee.DepartmentId;
        //            item.EmployerId = currentEmployee.EmployerId;
        //            item.PositionId = currentEmployee.PositionId;
        //            Context.Transport.Update(item);
        //        }
        //    }

        //}

        private async Task ChangeTempData(RequestDocumentProfileChangeEmployeeTemp currentEmployeeData, int documentId, int employeeId)
        {
            if (currentEmployeeData.StartDate != null)
            {
                DateTime startDate = currentEmployeeData.StartDate.Value.Date;
                DateTime endDate = DateTime.Now;    


                if (currentEmployeeData.Permanent == 1)
                {
                    var transportData = await Context.Transport
                              .Where(x => x.EmployeeId == employeeId
                              && x.EventDate.Value.Date >= startDate).ToListAsync();


                    var employeeStatusData = await Context.EmployeeStatus
                        .Where(x => x.EmployeeId == employeeId
                        && x.EventDate.Value.Date >= startDate).ToListAsync();

                    foreach (var transportItem in transportData)
                    {
                        transportItem.PositionId = currentEmployeeData.PositionId == null ? transportItem.PositionId : currentEmployeeData.PositionId;
                        transportItem.DepId = currentEmployeeData.DepartmentId == null ? transportItem.DepId : currentEmployeeData.DepartmentId;
                        transportItem.CostCodeId = currentEmployeeData.CostCodeId == null ? transportItem.CostCodeId : currentEmployeeData.CostCodeId;
                        transportItem.EmployerId = currentEmployeeData.EmployerId == null ? transportItem.EmployerId : currentEmployeeData.EmployerId;
                        transportItem.DateUpdated = DateTime.Now;
                        transportItem.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                        transportItem.ChangeRoute = $"Request from change #{documentId}";

                        Context.Transport.Update(transportItem);
                    }

                    foreach (var employeeStatusItem in employeeStatusData)
                    {
                        employeeStatusItem.PositionId = currentEmployeeData.PositionId == null ? employeeStatusItem.PositionId : currentEmployeeData.PositionId;
                        employeeStatusItem.DepId = currentEmployeeData.DepartmentId == null ? employeeStatusItem.DepId : currentEmployeeData.DepartmentId;
                        employeeStatusItem.CostCodeId = currentEmployeeData.CostCodeId == null ? employeeStatusItem.CostCodeId : currentEmployeeData.CostCodeId;
                        employeeStatusItem.EmployerId = currentEmployeeData.EmployerId == null ? employeeStatusItem.EmployerId : currentEmployeeData.EmployerId;
                        employeeStatusItem.DateUpdated = DateTime.Now;
                        employeeStatusItem.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                        employeeStatusItem.ChangeRoute = $"Request from change #{documentId}";

                        Context.EmployeeStatus.Update(employeeStatusItem);
                    }

                }
                else
                {
                    if (currentEmployeeData.StartDate != null && currentEmployeeData.EndDate != null)
                    {
                        endDate = currentEmployeeData.EndDate.Value.Date;

                        var transportData = await Context.Transport
                                  .Where(x => x.EmployeeId == employeeId
                                  && x.EventDate.Value.Date >= startDate && x.EventDate.Value.Date <= currentEmployeeData.EndDate).ToListAsync();


                        var employeeStatusData = await Context.EmployeeStatus
                            .Where(x => x.EmployeeId == employeeId
                            && x.EventDate.Value.Date >= startDate && x.EventDate.Value.Date <= endDate).ToListAsync();

                        foreach (var transportItem in transportData)
                        {
                            transportItem.PositionId = currentEmployeeData.PositionId == null ? transportItem.PositionId : currentEmployeeData.PositionId;
                            transportItem.DepId = currentEmployeeData.DepartmentId == null ? transportItem.DepId : currentEmployeeData.DepartmentId;
                            transportItem.CostCodeId = currentEmployeeData.CostCodeId == null ? transportItem.CostCodeId : currentEmployeeData.CostCodeId;
                            transportItem.EmployerId = currentEmployeeData.EmployerId == null ? transportItem.EmployerId : currentEmployeeData.EmployerId;
                            transportItem.DateUpdated = DateTime.Now;
                            transportItem.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                            transportItem.ChangeRoute = $"Request from change #{documentId}";

                            Context.Transport.Update(transportItem);
                        }

                        foreach (var employeeStatusItem in employeeStatusData)
                        {
                            employeeStatusItem.PositionId = currentEmployeeData.PositionId == null ? employeeStatusItem.PositionId : currentEmployeeData.PositionId;
                            employeeStatusItem.DepId = currentEmployeeData.DepartmentId == null ? employeeStatusItem.DepId : currentEmployeeData.DepartmentId;
                            employeeStatusItem.CostCodeId = currentEmployeeData.CostCodeId == null ? employeeStatusItem.CostCodeId : currentEmployeeData.CostCodeId;
                            employeeStatusItem.EmployerId = currentEmployeeData.EmployerId == null ? employeeStatusItem.EmployerId : currentEmployeeData.EmployerId;
                            employeeStatusItem.DateUpdated = DateTime.Now;
                            employeeStatusItem.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;

                            employeeStatusItem.ChangeRoute = $"Request from change #{documentId}";

                            Context.EmployeeStatus.Update(employeeStatusItem);
                        }
                    }
             

                }
            }




            
        }


        #endregion

    }
}
