using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.CreateRequestNonSiteTravelFlight;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.DeleteRequestNonSiteTravelFlight;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.UpdateRequestNonSiteTravelFlight;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public  class RequestNonSiteTravelFlightRepository : BaseRepository<RequestNonSiteTravelFlight>, IRequestNonSiteTravelFlightRepository
    {
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestNonSiteTravelFlightRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _HTTPUserRepository = hTTPUserRepository;
        }




        public async Task CreateRequestNonSiteTravelFlight(CreateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {
            var currentDocument =await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {
                    var newRecord = new RequestNonSiteTravelFlight
                    {
                        DocumentId = request.DocumentId,
                        ETD = request.ETD,
                        TravelDate = request.TravelDate,
                        FavorTime = request.FavorTime,
                        ArriveLocationId = request.ArriveLocationId,
                        DepartLocationId = request.DepartLocationId,
                        Comment = request.Comment,
                        Active = 1,
                        DateCreated = DateTime.Now,
                        UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id
                    };

                    Context.RequestNonSiteTravelFlight.Add(newRecord);

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
                        Comment = "Added Flight"

                    };

                    Context.RequestDocumentHistory.Add(history);



                }
                else
                {
                    throw new BadRequestException("This task cannot be modified.");
                }
            }
            else {
                throw new BadRequestException("Task not found.");
            }
            await Task.CompletedTask;
        }


        public async Task UpdateRequestNonSiteTravelFlight(UpdateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {

            var currentRecord = await Context.RequestNonSiteTravelFlight.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == currentRecord.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {

                    if (currentRecord != null)
                    {
                        currentRecord.DocumentId = request.DocumentId;
                        currentRecord.ETD = request.ETD;
                        currentRecord.TravelDate = request.TravelDate;
                        currentRecord.FavorTime = request.FavorTime;
                        currentRecord.ArriveLocationId = request.ArriveLocationId;
                        currentRecord.DepartLocationId = request.DepartLocationId;
                        currentRecord.Comment = request.Comment;
                        currentRecord.Active = 1;
                        currentRecord.DateUpdated = DateTime.Now;
                        currentRecord.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;

                        Context.RequestNonSiteTravelFlight.Update(currentRecord);

                        currentDocument.DateUpdated = DateTime.Now;
                        currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        currentDocument.CurrentAction = RequestDocumentAction.Saved;
                        Context.RequestDocument.Update(currentDocument);

                        var history = new RequestDocumentHistory
                        {
                            CurrentAction = RequestDocumentAction.Saved,
                            ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id,
                            Active = 1,
                            DateCreated = DateTime.Now,
                            UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                            DocumentId = currentDocument.Id,
                            Comment = "Updated Flight"

                        };

                        Context.RequestDocumentHistory.Add(history);

                    }
                    else {
                        throw new BadRequestException("Record not found.");
                    }

                }
                else {
                    throw new BadRequestException("This task cannot be modified.");
                }
            }
            
            await Task.CompletedTask;

        }


        public async Task DeleteRequestNonSiteTravelFlight(DeleteRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken)
        {

            var currentRecord = await Context.RequestNonSiteTravelFlight.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == currentRecord.DocumentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Completed || currentDocument.CurrentAction != RequestDocumentAction.Cancelled)
                {

                    if (currentRecord != null)
                    {
                        Context.RequestNonSiteTravelFlight.Remove(currentRecord);

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
                            Comment = "Deleted Flight"

                        };

                        Context.RequestDocumentHistory.Add(history);

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

            await Task.CompletedTask;

        }
    }
}
