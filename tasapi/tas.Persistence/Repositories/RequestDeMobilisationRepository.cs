using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.CompleteRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.CreateRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.GetRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentDeMobilisationFeature.UpdateRequestDocumentDeMobilisation;
using tas.Application.Features.RequestDocumentProfileChangeFeature.CreateRequestDocumentProfileChange;
using tas.Application.Features.RequestDocumentProfileChangeFeature.GetRequestDocumentProfileChange;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestDeMobilisationRepository : BaseRepository<RequestDeMobilisation>, IRequestDeMobilisationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMemoryCache _memoryCache;
        public RequestDeMobilisationRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, IMemoryCache memoryCache, IEmployeeRepository employeeRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _memoryCache = memoryCache;
            _employeeRepository = employeeRepository;
        }


        #region GetData

        public async Task<GetRequestDocumentDeMobilisationResponse> GetRequestDocumentDeMobilisation(GetRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetRequestDocumentDeMobilisationResponse();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.DocumentId && x.DocumentType == RequestDocumentType.DeMobilisation).FirstOrDefaultAsync();
            if (currentDocument != null)
            {

                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync(cancellationToken);
                var assignEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync(cancellationToken);
                var updatedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdUpdated).FirstOrDefaultAsync(cancellationToken);
                var createdEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.UserIdCreated).FirstOrDefaultAsync(cancellationToken);

                var deleagationEmmployee = await Context.RequestDelegates.Where(x => x.FromEmployeeId == currentDocument.AssignedEmployeeId).FirstOrDefaultAsync();

                var userId = _HTTPUserRepository.LogCurrentUser()?.Id;

                returnData.Id = currentDocument.Id;
                returnData.RequestedDate = currentDocument.DateCreated;
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.EmployeeFullName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}";
                returnData.AssignedEmployeeFullName = $"{assignEmployee?.Firstname} {assignEmployee?.Lastname}";
                returnData.CurrentStatus = currentDocument.CurrentAction;
                returnData.RequestUserId = createdEmployee?.Id;
                returnData.DocumentType = currentDocument.DocumentType;
                returnData.UpdatedInfo = $"{currentDocument.DateUpdated} {updatedEmployee?.Firstname} {updatedEmployee?.Lastname}";
                returnData.RequesterFullName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}";
                returnData.RequesterMail = createdEmployee?.Email;
                returnData.RequesterMobile = createdEmployee?.PersonalMobile;
                returnData.AssignedEmployeeId = currentDocument.AssignedEmployeeId;
                returnData.AssignedRouteConfigId = currentDocument.AssignedRouteConfigId;
                //      returnData.DaysAway = currentDocument?.DateCreated.Value.Date.Subtract(DateTime.Now).Days;
                returnData.EmployeeId = currentDocument?.EmployeeId;
                returnData.DelegateEmployeeId = deleagationEmmployee?.ToEmployeeId;

                try
                {
                    returnData.DaysAway = currentDocument.DaysAwayDate.HasValue ? (DateTime.Today.Subtract(currentDocument.DaysAwayDate.Value).Days) * (-1) : (DateTime.Today.Subtract(currentDocument.DateCreated.Value).Days) * (-1);
                }
                catch (Exception)
                {

                    returnData.DaysAway = 0;
                }


                var currentDeMobData = await Context.RequestDeMobilisation.Where(x => x.DocumentId == request.DocumentId).FirstOrDefaultAsync();
                if (currentDeMobData != null)
                {
                    var returnDeMobInfo = new GetRequestDocumentDeMobilisationInfo
                    {
                        Id = currentDeMobData.Id,
                        CompletionDate = currentDeMobData.CompletionDate,
                        DocumentId = currentDeMobData?.Id,
                        EmployerId = currentDeMobData?.EmployerId,
                        RequestDeMobilisationTypeId = currentDeMobData?.RequestDeMobilisationTypeId,
                        Comment = currentDeMobData?.Comment

                    };

                    returnData.info = returnDeMobInfo;


                }

            }

            return returnData;


        }



        #endregion

        #region CreateDat


        public async Task<int> CreateRequestDocumentDeMobilisation(CreateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {


            var employeeFutureData = await Context.EmployeeStatus.Where(x => x.EmployeeId == request.RequestData.EmployeeId && x.EventDate.Value.Date == DateTime.Today && x.RoomId != null).FirstOrDefaultAsync();
            var employeetransportData = await Context.Transport.Where(x => x.EmployeeId == request.RequestData.EmployeeId &&  x.EventDate.Value.Date == DateTime.Today.Date ).FirstOrDefaultAsync();


            if (employeeFutureData == null && employeetransportData == null)
            {
                var assignedGroup = await Context.RequestGroupEmployee.Where(x => x.EmployeeId == request.RequestData.AssignedEmployeeId).FirstOrDefaultAsync();
                var reqDocument = new RequestDocument();
                reqDocument.CurrentAction = request.RequestData.Action;
                reqDocument.Active = 1;
                reqDocument.DateCreated = DateTime.Now;
                reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqDocument.Description = reqDocument.Description;
                reqDocument.EmployeeId = request.RequestData.EmployeeId;
                reqDocument.DocumentType = RequestDocumentType.DeMobilisation;

                reqDocument.AssignedEmployeeId = request.RequestData?.AssignedEmployeeId;
                reqDocument.AssignedRouteConfigId = request.RequestData?.nextGroupId;
                Context.RequestDocument.Add(reqDocument);
                await Context.SaveChangesAsync();
                await SaveDemobilisation(request, reqDocument.Id);
                await SaveDocumentHistory(request, reqDocument.Id);
                return reqDocument.Id;
            }
            else {
                throw new BadRequestException("This employee is in ONSITE status and cannot be changed in the today");
            }



        }

        private async Task SaveDemobilisation(CreateRequestDocumentDeMobilisationRequest request, int DocumentId)
        {

            var reqHist = new RequestDeMobilisation();
            reqHist.Comment = request.DeMobilisationData.Comment;
            reqHist.Active = 1;
            reqHist.DateCreated = DateTime.Now;
            reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.DocumentId = DocumentId;
            reqHist.CompletionDate = request.DeMobilisationData.CompletionDate;
            reqHist.EmployerId = request.DeMobilisationData.EmployerId;
            reqHist.RequestDeMobilisationTypeId = request.DeMobilisationData.RequestDeMobilisationTypeId;
            Context.RequestDeMobilisation.Add(reqHist);
            await Task.CompletedTask;

        }

        private async Task SaveDocumentHistory(CreateRequestDocumentDeMobilisationRequest request, int DocumentId)
        {

            var reqHist = new RequestDocumentHistory();
            reqHist.Comment = request.RequestData.Comment;
            reqHist.Active = 1;
            reqHist.DateCreated = DateTime.Now;
            reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.DocumentId = DocumentId;
            reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.CurrentAction = request.RequestData.Action;
            Context.RequestDocumentHistory.Add(reqHist);
            await Task.CompletedTask;

        }


        #endregion


        #region UpdateData
        public async Task UpdateRequestDocumentDeMobilisation(UpdateRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {

            var currentData = await Context.RequestDeMobilisation.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

            if (currentData != null)
            {
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentData.DateUpdated = DateTime.Now;
                currentData.CompletionDate = request.CompletionDate;
                currentData.EmployerId = request.EmployerId;
                currentData.RequestDeMobilisationTypeId = request.RequestDeMobilisationTypeId;
                currentData.Comment = request.Comment;

                Context.RequestDeMobilisation.Update(currentData);

                var reqHist = new RequestDocumentHistory();
                reqHist.Comment = request.Comment;
                reqHist.Active = 1;
                reqHist.DateCreated = DateTime.Now;
                reqHist.CurrentAction = RequestDocumentAction.Saved;
                reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.DocumentId = currentData.DocumentId;
                Context.RequestDocumentHistory.Add(reqHist);
                await Task.CompletedTask;
            }


        }


        #endregion


        #region CompleteData


        public async Task CompleteRequestDocumentDeMobilisation(CompleteRequestDocumentDeMobilisationRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.DocumentType == RequestDocumentType.DeMobilisation)
                {

                    if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {
                        throw new BadRequestException("This task already Completed");
                    }
                    else
                    {


                        var employeeFutureData = await Context.EmployeeStatus.AsNoTracking().Where(x => x.EmployeeId == currentDocument.EmployeeId && x.EventDate.Value.Date == DateTime.Today && x.RoomId != null).FirstOrDefaultAsync();

                        var employeeFutureTransportData = await Context.Transport.AsNoTracking().Where(x => x.EmployeeId == currentDocument.EmployeeId && x.EventDate.Value.Date == DateTime.Today).FirstOrDefaultAsync();



                        if (employeeFutureData == null && employeeFutureTransportData == null)
                        {


                            var currentData = await Context.RequestDeMobilisation.Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();
                            if (currentData != null)
                            {

                                if (currentDocument.EmployeeId.HasValue)
                                {
                                    await DeActiveExecuteEmployee(Convert.ToInt32(currentDocument.EmployeeId), $"{currentData.Comment}", cancellationToken);
                                    _HTTPUserRepository.ClearAllEmployeeCache();

                                }

                            }

                            currentDocument.CurrentAction = RequestDocumentAction.Completed;
                            currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                            currentDocument.CompletedUserId = _HTTPUserRepository.LogCurrentUser()?.Id;
                            currentDocument.CompletedDate = DateTime.Now;
                            Context.RequestDocument.Update(currentDocument);
                            var newHistoryRecord = new RequestDocumentHistory
                            {
                                Comment = request.comment,
                                Active = 1,
                                DateCreated = DateTime.Now,
                                UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                CurrentAction = RequestDocumentAction.Completed,
                                ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                                DocumentId = request.documentId
                            };
                            Context.RequestDocumentHistory.Add(newHistoryRecord);

                            await Task.CompletedTask;

                        }
                        else {
                            throw new BadRequestException("This employee is in ONSITE status and cannot be changed in the today");
                        }
                    }
                }
            }

            
        }



        private async Task DeActiveExecuteEmployee(int employeeId, string comment, CancellationToken cancellationToken)
        {
            var currentEmployee = await Context.Employee.Where(x => x.Id == employeeId && x.Active == 1).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {

                await _employeeRepository.DeActiveEmployeeDelete(employeeId, cancellationToken);
               // await DeleteMoreData(employeeId);
                currentEmployee.Active = 0;
                currentEmployee.RoomId = null;
                var empHisDat = new EmployeeHistory
                {
                    Comment = "DeActiive from Requst",
                    Active = 1,
                    DateCreated = DateTime.Now,
                    EventDate = DateTime.Today,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    EmployeeId =employeeId,
                    Action = "Termination"
                };

                Context.EmployeeHistory.Add(empHisDat);
                Context.Employee.Update(currentEmployee);


            }
           
            await Task.CompletedTask;
        }


   

        #endregion


    }
}
