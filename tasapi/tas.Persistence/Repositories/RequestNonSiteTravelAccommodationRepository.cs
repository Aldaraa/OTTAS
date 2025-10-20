using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.EmployeeStatusFeature.GetDateRangeStatus;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.CreateRequestNonSiteTravelAccommodation;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.DeleteRequestNonSiteTravelAccommodation;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.UpdateRequestNonSiteTravelAccommodation;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestNonSiteTravelAccommodationRepository : BaseRepository<RequestNonSiteTravelAccommodation>, IRequestNonSiteTravelAccommodationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestNonSiteTravelAccommodationRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }




        public async Task CreateRequestNonSiteTravelAccommodation(CreateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {


            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {
                    var currentHotelData = new RequestLocalHotel();
  
                    decimal lateCheckOutCost = 0;
                    decimal earlyCheckInCost = 0;

                    if (!string.IsNullOrWhiteSpace(request.Hotel))
                    {

                        if (request.EarlyCheckIn.HasValue)
                        {
                            if (request.EarlyCheckIn.Value == 1)
                            {

                                earlyCheckInCost = request.EarlyCheckInCost.Value;


                            }
                        }
                        if (request.LateCheckOut.HasValue)
                        {
                            if (request.LateCheckOut.Value == 1)
                            {
                                lateCheckOutCost = request.LateCheckOutCost.Value;


                            }
                        }
                    }

                    var newRecord = new RequestNonSiteTravelAccommodation
                    {
                        DocumentId = request.DocumentId,
                        City = request.City,
                        Hotel = request.Hotel,
                        HotelLocation = request.HotelLocation,
                        FirstNight = request.FirstNight.Value,
                        LastNight = request.LastNight.Value,
                        Comment = request.Comment,
                        PaymentCondition = request.PaymentCondition,
                        LateCheckOut = request.LateCheckOut,
                        EarlyCheckIn = request.EarlyCheckIn,
                        DayCost = request.DayCost,
                        LateCheckOutCost = lateCheckOutCost ,
                        EarlyCheckInCost =earlyCheckInCost,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        AddCost = request.AddCost,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                    };

                    Context.RequestNonSiteTravelAccommodation.Add(newRecord);

                    currentDocument.DateUpdated = DateTime.Now;
                    currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    Context.RequestDocument.Update(currentDocument);

                    var history = new RequestDocumentHistory
                    {
                        CurrentAction = RequestDocumentAction.Saved,
                        ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                        DocumentId = request.DocumentId,
                        Comment = "Added Accommodation data"

                    };

                    Context.RequestDocumentHistory.Add(history);
                }
                else
                {
                    throw new BadRequestException("This task cannot be modified.");
                }
            }
            else
            {
                throw new BadRequestException("Task not found.");
            }
            await Task.CompletedTask;
        }

        private async Task<RequestLocalHotel?> GetHotelData(string hotelName)
        {
          var currentHotelData =await  Context.RequestLocalHotel.Where(x => x.Description == hotelName).FirstOrDefaultAsync();
            if (currentHotelData != null)
            {
                return currentHotelData;
            }
            else {
                return null;
            }

        }


        public async Task<int> UpdateRequestNonSiteTravelAccommodation(UpdateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {

            var currentRecord = await Context.RequestNonSiteTravelAccommodation.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == currentRecord.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {

                    if (currentRecord != null)
                    {
                        currentRecord.DocumentId = request.DocumentId;
                        currentRecord.City = request.City;
                        currentRecord.Hotel = request.Hotel;
                        currentRecord.HotelLocation = request.HotelLocation;
                        currentRecord.PaymentCondition = request.PaymentCondition;
                        currentRecord.Comment = request.Comment;
                        currentRecord.Active = 1;
                        currentRecord.DateUpdated = DateTime.Now;
                        currentRecord.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        currentRecord.Comment = request.Comment;
                        currentRecord.EarlyCheckIn = request.EarlyCheckIn;
                        currentRecord.FirstNight = request.FirstNight;
                        currentRecord.LastNight = request.LastNight;
                        currentRecord.LateCheckOut = request.LateCheckOut;
                        currentRecord.DayCost = request.DayCost;
                        currentRecord.LateCheckOutCost = request.LateCheckOutCost;
                        currentRecord.EarlyCheckInCost = request.EarlyCheckInCost;
                        currentRecord.AddCost = request.AddCost.HasValue ? request.AddCost : currentRecord.AddCost;


                        Context.RequestNonSiteTravelAccommodation.Update(currentRecord);

                        currentDocument.DateUpdated = DateTime.Now;
                        currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        Context.RequestDocument.Update(currentDocument);

                        var history = new RequestDocumentHistory
                        {
                            CurrentAction = RequestDocumentAction.Saved,
                            ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                            Active = 1,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            DocumentId = request.DocumentId,
                            Comment = "Updated Accommodation data"

                        };

                        Context.RequestDocumentHistory.Add(history);

                        return currentRecord.DocumentId;

                    }
                    else
                    {
                        throw new BadRequestException("Record not found.");
                    }

                }
                else
                {
                    throw new BadRequestException("This task cannot be modified.");
                }
            }

            return 0;


        }


        public async Task<int> DeleteRequestNonSiteTravelAccommodation(DeleteRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken)
        {

            var currentRecord = await Context.RequestNonSiteTravelAccommodation.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == currentRecord.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {

                    if (currentRecord != null)
                    {
                        Context.RequestNonSiteTravelAccommodation.Remove(currentRecord);

                        currentDocument.DateUpdated = DateTime.Now;
                        currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        Context.RequestDocument.Update(currentDocument);

                        var history = new RequestDocumentHistory
                        {
                            CurrentAction = RequestDocumentAction.Saved,
                            ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                            Active = 1,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            DocumentId = currentDocument.Id,
                            Comment = "Deleted Accommodation data"

                        };
                        Context.RequestDocumentHistory.Add(history);

                        return currentDocument.Id;
                    }
                    else
                    {
                        throw new BadRequestException("Record not found.");
                    }

                }
                else
                {
                    throw new BadRequestException("This task cannot be modified.");
                }
            }
            return 0;

        }
    }
}
