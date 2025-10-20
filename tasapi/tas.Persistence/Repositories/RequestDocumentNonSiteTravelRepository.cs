using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.CreateRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentNonSiteTravel;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentSiteTravelAdd;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelData;
using tas.Application.Features.RequestDocumentFeature.UpdateRequestDocumentNonSiteTravelEmployee;
using tas.Application.Features.RequestDocumentFeature.WaitingAgentRequestDocumentNonSiteTravel;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestDocumentNonSiteTravelRepository : BaseRepository<RequestDocument>, IRequestDocumentNonSiteTravelRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestDocumentNonSiteTravelRepository(DataContext Context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(Context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }


        public void CreateRequestDocumentNonSiteTravelValidate(CreateRequestDocumentNonSiteTravelRequest request)
        {
            if (request.flightData.Count == 0 && request.AccommodationData.Count == 0)
            {
                throw new BadRequestException("\r\nThe user should be informed that when submitting a travel request, they need to provide" +
                    " either accommodation details or flight details. Both sections cannot be left empty; at least one of them must be filled in.");
            }
        }




        public async Task<int> CreateRequestDocumentNonSiteTravel(CreateRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {

            using (var transaction = await Context.Database.BeginTransactionAsync(cancellationToken))
            {

                try
                {




                    var reqDocument = new RequestDocument();
                    reqDocument.CurrentAction = request.travelData.Action;
                    reqDocument.Description = request.travelData.Description;
                    reqDocument.Active = 1;
                    reqDocument.DateCreated = DateTime.Now;
                    reqDocument.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqDocument.Description = reqDocument.Description;
                    reqDocument.EmployeeId = request.travelData.EmployeeId;
                    reqDocument.DocumentType = RequestDocumentType.NonSiteTravel;

                    reqDocument.AssignedEmployeeId = request.travelData?.AssignedEmployeeId;
                    reqDocument.AssignedRouteConfigId = request.travelData?.NextGroupId;
                    Context.RequestDocument.Add(reqDocument);
                    await Context.SaveChangesAsync();
                    await SaveTravelData(request, reqDocument.Id, cancellationToken);
                    await SaveFlightData(request, reqDocument.Id, cancellationToken);
                    await SaveAccommodatin(request, reqDocument.Id, cancellationToken);
                    await SaveAttachment(request, reqDocument.Id, cancellationToken);
                    await SaveDocumentHistory(request, reqDocument.Id, cancellationToken);
                    await EmployeeCheckData(request, cancellationToken);
                    //    await  AutoApproveData(request, reqDocument.Id, cancellationToken);
                    var success = await Context.SaveChangesAsync();
                    await transaction.CommitAsync(cancellationToken);
                    return reqDocument.Id;


                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new BadRequestException(ex.Message);
                }
            }

        }


        private async Task EmployeeCheckData(CreateRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
            var currentEmployee = await  Context.Employee.Where(x => x.Id == request.travelData.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            {
                if (currentEmployee.Active != 1)
                {
                    currentEmployee.Active= 1;
                    currentEmployee.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    currentEmployee.DateUpdated = DateTime.Now;
                    Context.Employee.Update(currentEmployee);
                }
            
            }
        }

        private async Task SaveFlightData(CreateRequestDocumentNonSiteTravelRequest request, int DocumentId, CancellationToken cancellationToken)
        {

            foreach (var item in request.flightData)
            {
                var reqFlight = new RequestNonSiteTravelFlight();
                reqFlight.Active = 1;
                reqFlight.DateCreated = DateTime.Now;
                reqFlight.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqFlight.ArriveLocationId = item.ArriveLocationId;
                reqFlight.DepartLocationId = item.DepartLocationId;
                reqFlight.ETD = item.ETD;
                reqFlight.FavorTime = item.FavorTime;
                reqFlight.DocumentId = DocumentId;
                reqFlight.TravelDate = item.TravelDate;
                reqFlight.Comment = item.Comment;
                Context.RequestNonSiteTravelFlight.Add(reqFlight);
            }

            await Task.CompletedTask;
        }

        private async Task SaveTravelData(CreateRequestDocumentNonSiteTravelRequest request, int DocumentId, CancellationToken cancellationToken)
        {
            var retTravel = new RequestNonSiteTravel();
            retTravel.Active = 1;
            retTravel.RequestTravelPurposeId = request.travelData.RequestTravelPurposeId;
            retTravel.DateCreated = DateTime.Now;
            retTravel.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            retTravel.HigestCost = request.travelData.HigestCost;
            retTravel.DocumentId = DocumentId;

            Context.RequestNonSiteTravel.Add(retTravel);


            await Task.CompletedTask;
        }

        private async Task SaveAccommodatin(CreateRequestDocumentNonSiteTravelRequest request, int DocumentId, CancellationToken cancellationToken)
        {
            foreach (var item in request.AccommodationData)
            {

                var currentHotelData = new RequestLocalHotel();
                decimal dayCost = 0;
                decimal lateCheckOutCost = 0;
                decimal earlyCheckInCost = 0;

                if (!string.IsNullOrWhiteSpace(item.Hotel))
                {
                    currentHotelData = await GetHotelData(item.Hotel);
                    if (currentHotelData != null)
                    {

                        if (currentHotelData.DayCost.HasValue)
                        {
                            dayCost = currentHotelData.DayCost.Value;
                        }
                        if (item.EarlyCheckIn.HasValue)
                        {
                            if (item.EarlyCheckIn.Value == 1)
                            {
                                if (currentHotelData.EarlyCheckInCost.HasValue)
                                {
                                    earlyCheckInCost = currentHotelData.EarlyCheckInCost.Value;
                                }

                            }
                        }
                        if (item.LateCheckOut.HasValue)
                        {
                            if (item.LateCheckOut.Value == 1)
                            {
                                if (currentHotelData.LateCheckOutCost.HasValue)
                                {
                                    lateCheckOutCost = currentHotelData.LateCheckOutCost.Value;
                                }

                            }
                        }


                    }

                }


                var reqAccom = new RequestNonSiteTravelAccommodation();

                reqAccom.Active = 1;
                reqAccom.DateCreated = DateTime.Now;
                reqAccom.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqAccom.Hotel = item.Hotel;
                reqAccom.HotelLocation = item.HotelLocation;
                reqAccom.DocumentId = DocumentId;
                reqAccom.City = item.City;
                reqAccom.PaymentCondition = item.PaymentCondition;
                reqAccom.FirstNight = item.FirstNight;
                reqAccom.LastNight = item.LastNight;
                reqAccom.Comment = item.Comment;
                reqAccom.LateCheckOut = item.LateCheckOut;
                reqAccom.EarlyCheckIn = item.EarlyCheckIn;
                reqAccom.DayCost = dayCost;
                reqAccom.LateCheckOutCost = lateCheckOutCost;
                reqAccom.EarlyCheckInCost = earlyCheckInCost;
                Context.RequestNonSiteTravelAccommodation.Add(reqAccom);
            }

            await Task.CompletedTask;
        }


        private async Task<RequestLocalHotel?> GetHotelData(string hotelName)
        {
            var currentHotelData = await Context.RequestLocalHotel.AsNoTracking().Where(x => x.Description == hotelName).FirstOrDefaultAsync();
            if (currentHotelData != null)
            {
                return currentHotelData;
            }
            else
            {
                return null;
            }

        }


        private async Task SaveAttachment(CreateRequestDocumentNonSiteTravelRequest request, int DocumentId, CancellationToken cancellationToken)
        {
            foreach (var item in request.Files)
            {
                var currentFile = await Context.SysFile.Where(x => x.Id == item.FileAddressId).FirstOrDefaultAsync();
                if (currentFile != null)
                {
                    var reqAttach = new RequestDocumentAttachment();
                    reqAttach.Active = 1;
                    reqAttach.DateCreated = DateTime.Now;
                    reqAttach.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqAttach.DocumentId = DocumentId;
                    reqAttach.Description = item.Comment;
                    reqAttach.FileAddress = currentFile.FileAddress;
                    reqAttach.IncludeEmail = item.IncludeEmail;

                    Context.RequestNonSiteTravelAttachment.Add(reqAttach);
                }

            }
            await Task.CompletedTask;
        }

        private async Task SaveDocumentHistory(CreateRequestDocumentNonSiteTravelRequest request, int DocumentId, CancellationToken cancellationToken)
        {

            var reqHist = new RequestDocumentHistory();
            reqHist.Comment = request.RequestInfo.Comment;
            reqHist.Active = 1;
            reqHist.DateCreated = DateTime.Now;
            reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.DocumentId = DocumentId;
            reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
            reqHist.CurrentAction = request.travelData.Action;
            Context.RequestDocumentHistory.Add(reqHist);
            await Task.CompletedTask;

        }


        public async Task UpdateRequestDocumentNonSiteTravelEmployee(UpdateRequestDocumentNonSiteTravelEmployeeRequest request, CancellationToken cancellationToken)
        {
            var currentEmployee =await Context.Employee.Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
            if (currentEmployee != null)
            { 
                currentEmployee.PassportExpiry = request.PassportExpiry;
                currentEmployee.PassportNumber = request.PassportNumber;
                currentEmployee.PassportName = request.PassportName;
                currentEmployee.EmergencyContactMobile = request.EmergencyContactMobile;
                currentEmployee.EmergencyContactName = request.EmergencyContactName;
                currentEmployee.FrequentFlyer = request.FrequentFlyer;
                currentEmployee.DateUpdated = DateTime.Now;
                currentEmployee.Email = request.Email;
                currentEmployee.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;

                Context.Employee.Update(currentEmployee);

                var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
                if (currentDocument != null)
                {

                    var reqHist = new RequestDocumentHistory();
                    reqHist.Comment = "Employee  update";
                    reqHist.Active = 1;
                    reqHist.DateCreated = DateTime.Now;
                    reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqHist.DocumentId = request.DocumentId;
                    reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqHist.CurrentAction = RequestDocumentAction.Saved;
                    Context.RequestDocumentHistory.Add(reqHist);
                }

                await Task.CompletedTask;

            }

            await Task.CompletedTask;
        }
        
        
        public async Task UpdateRequestDocumentNonSiteTravelData(UpdateRequestDocumentNonSiteTravelDataRequest request, CancellationToken cancellationToken)
        {

           var currentData = await Context.RequestNonSiteTravel
                .Where(x => x.DocumentId == request.DocumentId).FirstOrDefaultAsync();
            if (currentData != null)
            {


               // currentData.RequestTravelPurposeId = request.RequestTravelPurposeId;
                currentData.RequestTravelAgentId = request.RequestTravelAgentId;
                currentData.RequestTravelAgentSureName = request.RequestTravelAgentSureName;
                currentData.Cost = request.Cost;
                currentData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentData.Active = 1;
                currentData.HigestCost = request.HighestCost;
                currentData.DateUpdated = DateTime.Now;
                currentData.RequestTravelPurposeId = request.RequestTravelPurposeId;
                currentData.Cost2 = request.Cost2;

                Context.RequestNonSiteTravel.Update(currentData);

                if (request.SelectedOptionId.HasValue)
                {
                    var documentOptions = await Context.RequestNonSiteTravelOption.Where(x => x.DocumentId == request.DocumentId).ToListAsync();
                    foreach (var item in documentOptions)
                    {
                        if (request.SelectedOptionId == item.Id)
                        {
                            item.DateUpdated = DateTime.Now;
                            item.SelectedUserId = Convert.ToInt32(_HTTPUserRepository.LogCurrentUser()?.Id);
                            item.Selected = 1;
                            item.Active = 1;
                            item.DateCreated = DateTime.Now;
                            item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            Context.RequestNonSiteTravelOption.Update(item);


                            var rnewHist = new RequestDocumentHistory();
                            rnewHist.Comment = $"Updated Travel Option";
                            rnewHist.Active = 1;
                            rnewHist.DateCreated = DateTime.Now;
                            rnewHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            rnewHist.DocumentId = currentData.DocumentId;
                            rnewHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
                            rnewHist.CurrentAction = RequestDocumentAction.Saved;
                            Context.RequestDocumentHistory.Add(rnewHist);
                        }
                        else {
                            item.DateUpdated = DateTime.Now;
                            item.Selected = 0;
                            item.Active = 1;
                            item.DateCreated = DateTime.Now;
                            item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                            Context.RequestNonSiteTravelOption.Update(item);

                        }
                    }
                }



                var reqHist = new RequestDocumentHistory();
                reqHist.Comment = "Updated travelData";
                reqHist.Active = 1;
                reqHist.DateCreated = DateTime.Now;
                reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.DocumentId = request.DocumentId;
                reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.CurrentAction = RequestDocumentAction.Saved;
                Context.RequestDocumentHistory.Add(reqHist);
                await Task.CompletedTask;
            }

        }




        public async Task<GetRequestDocumentNonSiteTravelResponse> GetRequestDocumentNonSiteTravel(GetRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        { 
            var returnData = new GetRequestDocumentNonSiteTravelResponse();

            var currentData = await Context.RequestDocument.Where(x => x.Id == request.documentId && x.DocumentType == RequestDocumentType.NonSiteTravel).FirstOrDefaultAsync(cancellationToken);
            if (currentData != null)
            {

                    var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentData.EmployeeId).FirstOrDefaultAsync(cancellationToken);
                    var assignEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentData.AssignedEmployeeId).FirstOrDefaultAsync(cancellationToken);
                    var updatedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentData.UserIdUpdated).FirstOrDefaultAsync(cancellationToken);
                    var createdEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentData.UserIdCreated).FirstOrDefaultAsync(cancellationToken);

                    var deleagationEmployee = await Context.RequestDelegates.AsNoTracking().Where(x => x.FromEmployeeId == currentData.AssignedEmployeeId).FirstOrDefaultAsync();

                    returnData.Id = currentData.Id;
                    returnData.CurrentStatus = currentData.CurrentAction;
                    returnData.DocumentType = currentData.DocumentType;
                    returnData.EmployeeFullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}";
                    returnData.EmployeeId = currentData.EmployeeId;
                    returnData.AssignedEmployeeFullName = $"{assignEmployee?.Firstname} {assignEmployee?.Lastname}";
        //            returnData.DaysAway = DateTime.Today.Subtract(currentData.DateCreated.Value).Days;


                    returnData.UpdatedInfo = $"{currentData.DateUpdated} {updatedEmployee?.Firstname} {updatedEmployee?.Lastname}";
                    returnData.RequestedDate = currentData.DateCreated;
                    returnData.RequesterFullName = $"{createdEmployee?.Firstname} {createdEmployee?.Lastname}";
                    returnData.RequesterMail = createdEmployee?.Email;
                    returnData.RequesterMobile = createdEmployee?.PersonalMobile;
                returnData.RequestUserId = currentData.UserIdCreated;
                    returnData.AssignedEmployeeId = currentData.AssignedEmployeeId;
                    returnData.AssignedRouteConfigId = currentData.AssignedRouteConfigId;
                    returnData.DelegateEmployeeId = deleagationEmployee?.ToEmployeeId;
                   var selecttedOptionData =  await Context.RequestNonSiteTravelOption.AsNoTracking().Where(x => x.DocumentId == request.documentId && x.Selected == 1).FirstOrDefaultAsync();

                    returnData.IssuedoptionId = selecttedOptionData?.Id;

                    var UserId = _HTTPUserRepository.LogCurrentUser()?.Id;


                try
                {
                    returnData.DaysAway = currentData.DaysAwayDate.HasValue ? (DateTime.Today.Subtract(currentData.DaysAwayDate.Value).Days) * (-1) : (DateTime.Today.Subtract(currentData.DateCreated.Value).Days) * (-1);
                }
                catch (Exception)
                {

                    returnData.DaysAway = 0;
                }


                var accommodationResult = from Accommodation in Context.RequestNonSiteTravelAccommodation
                             where Accommodation.DocumentId == request.documentId

                             select new GetRequestDocumentNonSiteTravelAccommodation
                             {
                                 Id = Accommodation.Id,
                                 HotelLocation = Accommodation.HotelLocation,
                                 Hotel = Accommodation.Hotel,
                                 FirstNight = Accommodation.FirstNight,
                                 LastNight = Accommodation.LastNight,
                                 City = Accommodation.City,
                                 PaymentCondition = Accommodation.PaymentCondition,
                                 DocumentId = Accommodation.DocumentId,
                                 Comment = Accommodation.Comment,
                                 EarlyCheckIn = Accommodation.EarlyCheckIn,
                                 AddCost = Accommodation.AddCost,
                                 LateCheckOut = Accommodation.LateCheckOut,
                                 DayCost = Accommodation.DayCost,
                                 EarlyCheckInCost = Accommodation.EarlyCheckInCost,
                                 LateCheckOutCost = Accommodation.LateCheckOutCost,
                                 NightOfNumbers = (Accommodation.LastNight.HasValue && Accommodation.FirstNight.HasValue)
                                                    ? (Accommodation.LastNight.Value.Date - Accommodation.FirstNight.Value.Date).Days
                                                    : (int?)null
                             };

                returnData.Accommodations = await accommodationResult.ToListAsync();

                var FlightInfo = new GetRequestNonSiteTravelFlightInfo();

                var flightDataResult = from flight in Context.RequestNonSiteTravelFlight.AsNoTracking()
                             where flight.DocumentId == request.documentId
                             join departlocation in Context.RequestAirport.AsNoTracking() on flight.DepartLocationId equals departlocation.Id into DepartLocationData
                             from departlocation in DepartLocationData.DefaultIfEmpty()
                             join arriveLocation in Context.RequestAirport.AsNoTracking() on flight.ArriveLocationId equals arriveLocation.Id into ArriveLocationData
                             from arriveLocation in ArriveLocationData.DefaultIfEmpty()
                             select new GetRequestNonSiteTravelFlight
                             {
                                 Id = flight.Id,
                                 FavorTime = flight.FavorTime,
                                 ETD = flight.ETD,
                                 ArriveLocationId = flight.ArriveLocationId,
                                 ArriveLocationName = arriveLocation.Description,
                                 DepartLocationId = flight.DepartLocationId,
                                 DepartLocationName = departlocation.Description,
                                 Comment = flight.Comment,
                                 DocumentId = request.documentId,
                                 TravelDate = flight.TravelDate,
                             };


                var traveldata = await Context.RequestNonSiteTravel.AsNoTracking().Where(x => x.DocumentId == request.documentId).FirstOrDefaultAsync();

                if (traveldata != null) {
                    var currenTravelPurpose = await Context.RequestTravelPurpose.Where(x => x.Id == traveldata.RequestTravelPurposeId).FirstOrDefaultAsync();


                FlightInfo.travelId = traveldata?.Id;
                    FlightInfo.RequestTravelPurposeId = traveldata?.RequestTravelPurposeId;
                    FlightInfo.RequestTravelAgentSureName = traveldata?.RequestTravelAgentSureName;
                    FlightInfo.RequestTravelAgentId = traveldata?.RequestTravelAgentId;
                    FlightInfo.Cost = traveldata?.Cost;
                    FlightInfo.Cost2 = traveldata?.Cost2;
                    FlightInfo.HighestCost = traveldata?.HigestCost;
                    FlightInfo.RequestTravelPurposeDescription = currenTravelPurpose?.Description;

                returnData.FlightInfo = FlightInfo;

                    returnData.FlightInfo.FlightData = await flightDataResult.ToListAsync();
                }

                return returnData;


            }


            return returnData;

        }



        #region Complete

        public async Task CompleteRequestDocumentNonSiteTravel(CompleteRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                {

                    if (currentDocument.CurrentAction == RequestDocumentAction.Completed)
                    {
                        throw new BadRequestException("This task already Completed");
                    }
                    else {

                        var currentEmloyee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync();
                        if (currentEmloyee != null) {
                            currentDocument.CurrentAction = RequestDocumentAction.Completed;
                            currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                            currentDocument.CompletedUserId = _HTTPUserRepository.LogCurrentUser()?.Id;
                            currentDocument.CompletedDate = DateTime.Now;


                            if (currentEmloyee.Active != 1)
                            {
                                currentEmloyee.Active = 1;
                                currentEmloyee.DateUpdated = DateTime.Now;
                                currentEmloyee.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                Context.Employee.Update(currentEmloyee);
                            }

                            Context.RequestDocument.Update(currentDocument);
                            var newHistoryRecord = new RequestDocumentHistory
                            {
                                Comment = RequestDocumentAction.Completed + " " + request.comment,
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
                            throw new BadRequestException("Employee not found");
                        }

                    }
                    

                }
            }
            await Task.CompletedTask;
        }



        public async Task WaitingAgentRequestDocumentNonSiteTravel(WaitingAgentRequestDocumentNonSiteTravelRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                {
                    currentDocument.CurrentAction = RequestDocumentAction.WaitingAgent;
                    Context.RequestDocument.Update(currentDocument);
                    var newHistoryRecord = new RequestDocumentHistory
                    {
                        Comment = RequestDocumentAction.Completed + " " + request.comment,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                        CurrentAction = RequestDocumentAction.WaitingAgent,
                        ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                        DocumentId = request.documentId
                    };
                    Context.RequestDocumentHistory.Add(newHistoryRecord);
                }
            }
            await Task.CompletedTask;
        }

        #endregion

    }
}
