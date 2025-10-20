using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestNonSiteTravelOptionDataFeature.UpdateRequestNonSiteTravelOptionData;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.CreateRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.DeleteRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOptionFinal;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateItineraryOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateRequestNonSiteTravelOption;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestNonSiteTravelOptionRepository : BaseRepository<RequestNonSiteTravelOption>, IRequestNonSiteTravelOptionRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestNonSiteTravelOptionRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task CreateOptionData(CreateRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken)
        {

            var optionCount = await Context.RequestNonSiteTravelOption.Where(x => x.DocumentId == request.DocumentId).CountAsync();

            if (request.newcost.HasValue)
            {

                if (request.optionData.Count == 1)
                {

                    var currentDocumentOptions = await Context.RequestNonSiteTravelOption.Where(x => x.DocumentId == request.DocumentId).ToListAsync();
                    foreach (var item in currentDocumentOptions)
                    {
                        item.Selected = 0;
                        Context.RequestNonSiteTravelOption.Update(item);



                    }

                    var  data =await Context.RequestNonSiteTravel.Where(x => x.DocumentId == request.DocumentId).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        data.Cost2 = request.newcost;
                        Context.RequestNonSiteTravel.Update(data);
                    }
                    var newRecord = new RequestNonSiteTravelOption
                    {
                        Active = 1,
                        DateCreated = DateTime.Now,
                        OptionData = request.optionData[0].optiontext,
                        Selected = 1,
                        DocumentId = request.DocumentId,
                        SelectedUserId = Convert.ToInt32(_HTTPUserRepository.LogCurrentUser().Id),
                        Cost = request.newcost,
                        DueDate = DateTime.Now,
                        OptionIndex = optionCount + 1,

                    };
                    Context.RequestNonSiteTravelOption.Add(newRecord);
                    
                    
                }


            }
            else {

                var currentTravelData = await Context.RequestNonSiteTravel.Where(x => x.DocumentId == request.DocumentId).FirstOrDefaultAsync();
                if (currentTravelData != null)
                {

                    if (request.newcost.HasValue)
                    {
                        currentTravelData.Cost2 = request.newcost.Value;
                        Context.RequestNonSiteTravel.Update(currentTravelData);
                    }

                    foreach (var item in request.optionData)
                    {

                        var newRecord = new RequestNonSiteTravelOption
                        {
                            Active = 1,
                            DateCreated = DateTime.Now,
                            OptionData = item.optiontext,
                            Selected = item.selected == true ? 1 : 0,
                            DocumentId = request.DocumentId,
                            SelectedUserId = Convert.ToInt32(_HTTPUserRepository.LogCurrentUser().Id),
                            Cost = ExptractPrice(item.optiontext),
                            DueDate = item.DueDate,
                            OptionIndex = optionCount + 1,

                        };
                        optionCount++;  
                        Context.RequestNonSiteTravelOption.Add(newRecord);
                    }
                    if (request.optionData.Count > 0)
                    {
                        var reqHist = new RequestDocumentHistory();
                        reqHist.Comment = "Added Travel Option";
                        reqHist.Active = 1;
                        reqHist.DateCreated = DateTime.Now;
                        reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                        reqHist.DocumentId = request.DocumentId;
                        reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
                        reqHist.CurrentAction = RequestDocumentAction.Saved;
                        Context.RequestDocumentHistory.Add(reqHist);
                    }
                }

            }




            await Task.CompletedTask;
        }

        private decimal? ExptractPrice(string costData)
        {
            string pattern = @"(price|amount|cost|money)\s*:\s*([\d,]+(\.\d{1,2})?)";
            MatchCollection matches = Regex.Matches(costData.ToLower().Trim(), pattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    return Convert.ToDecimal(match.Groups[2].Value);
                }
            }

            return 0;
        }

        public async Task UpdateOptionData(UpdateRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken)
        {

            var currentData = await Context.RequestNonSiteTravelOption.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {

                var documentOtherOptions = await Context.RequestNonSiteTravelOption.Where(x => x.DocumentId == currentData.DocumentId).ToListAsync();

                foreach (var item in documentOtherOptions)
                {
                    item.SelectedUserId = Convert.ToInt32(_HTTPUserRepository.LogCurrentUser()?.Id);
                    item.Selected = item.Id == request.Id ? 1 : 0;
                    item.Active = 1;
                    item.DateUpdated = DateTime.Now;
                    item.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    Context.RequestNonSiteTravelOption.Update(item);
                }


                var travelData =await Context.RequestNonSiteTravel.Where(x => x.DocumentId == currentData.DocumentId).FirstOrDefaultAsync();
                if (travelData != null)
                { 
                    travelData.Cost =currentData.Cost;
                    travelData.DateUpdated = DateTime.Now;
                    travelData.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    Context.RequestNonSiteTravel.Update(travelData);
                }


                var reqHist = new RequestDocumentHistory();
                reqHist.Comment = "Updated Travel Option";
                reqHist.Active = 1;
                reqHist.DateCreated = DateTime.Now;
                reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.DocumentId = currentData.DocumentId;
                reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
                reqHist.CurrentAction = RequestDocumentAction.Saved;
                Context.RequestDocumentHistory.Add(reqHist);


            }
            await Task.CompletedTask;
        }


        public async Task<List<GetRequestNonSiteTravelOptionResponse>> GetData(GetRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken)
        {
         var optionData = await  Context.RequestNonSiteTravelOption
                .Where(x => x.DocumentId == request.DocumentId).ToListAsync();
            var returnData = new List<GetRequestNonSiteTravelOptionResponse>();
            foreach (var item in optionData)
            {
                string? groupname = "";
                var currentEmployee =await Context.Employee.Where(x => x.Id == item.SelectedUserId).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    var employeeGroup = await Context.RequestGroupEmployee.Where(c => c.EmployeeId == currentEmployee.Id).FirstOrDefaultAsync();
                    if (employeeGroup != null)
                    {
                        var currentGroup = await Context.RequestGroup.Where(x => x.Id == employeeGroup.RequestGroupId).FirstOrDefaultAsync();
                        groupname = currentGroup?.Description;
                    }
                }


                var newRecord = new GetRequestNonSiteTravelOptionResponse
                {
                    Id = item.Id,
                    DateCreated = DateTime.Now,
                    OptionData = item.OptionData,
                    DueDate = item.DueDate,
                    Cost = item.Cost,
                    Selected = item.Selected,
                    SelectedUserId = item.SelectedUserId,
                    SelectedUserName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}",
                    SelectedUserTeam = groupname,
                    OptionIndex = item.OptionIndex,
                };
                returnData.Add(newRecord);
            }

            return returnData;
                
        }



        public async Task UpdateOptionFullData(UpdateRequestNonSiteTravelOptionDataRequest request, CancellationToken cancellationToken)
        {
         var currentOption = await   Context.RequestNonSiteTravelOption.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentOption != null)
            {
                currentOption.OptionData = request.optionData;
                currentOption.DueDate = request.DueDate;
                currentOption.Cost = request.Cost;
                currentOption.DateUpdated = DateTime.Now;
                currentOption.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                Context.RequestNonSiteTravelOption.Update(currentOption);

            }
        }


        #region UpdateItinerary
        public async Task UpdateItinerary(UpdateItineraryOptionRequest request, CancellationToken cancellationToken) 
        {

            var currentDocument =await Context.RequestDocument.Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();
            var data = await Context.RequestNonSiteTravel.Where(x => x.DocumentId == request.DocumentId).FirstOrDefaultAsync();

            var optionCount = await Context.RequestNonSiteTravelOption.Where(x => x.DocumentId == request.DocumentId).CountAsync();


            if (currentDocument != null)
            {
                var newRecord = new RequestNonSiteTravelOption
                {
                    Active = 1,
                    DateCreated = DateTime.Now,
                    OptionData = request.optionText,
                    Selected = 0,
                    DocumentId = request.DocumentId,
                    UpdateItinerary = 1,
                    Cost = request.AdditionalCost,
                    Comment = request.Comment,  
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    OptionIndex = optionCount + 1
                    

                };
                Context.RequestNonSiteTravelOption.Add(newRecord);
                if (data != null) {
                    var sumUpdatedCost = await Context.RequestNonSiteTravelOption.Where(x => x.UpdateItinerary == 1 && x.DocumentId == request.DocumentId).SumAsync(x => x.Cost);
                    if (data != null)
                    {
                        data.Cost2 = sumUpdatedCost + request.AdditionalCost;
                        Context.RequestNonSiteTravel.Update(data);
                    }


                }

            }


        }
        #endregion


        #region GetFinalOptionData
        public async Task<List<GetRequestNonSiteTravelOptionFinalResponse>> GetFinalOptionData(GetRequestNonSiteTravelOptionFinalRequest request, CancellationToken cancellationToken)
        {
            var optionData = await Context.RequestNonSiteTravelOption.AsNoTracking()
                    .Where(x => x.DocumentId == request.DocumentId && ( x.Selected == 1 || x.UpdateItinerary == 1)).ToListAsync();
            var returnData = new List<GetRequestNonSiteTravelOptionFinalResponse>();
            foreach (var item in optionData)
            {
                string? groupname = "";
                var currentEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.SelectedUserId).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    var employeeGroup = await Context.RequestGroupEmployee.AsNoTracking().Where(c => c.EmployeeId == currentEmployee.Id).FirstOrDefaultAsync();
                    if (employeeGroup != null)
                    {
                        var currentGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == employeeGroup.RequestGroupId).FirstOrDefaultAsync();
                        groupname = currentGroup?.Description;
                    }
                }


                var newRecord = new GetRequestNonSiteTravelOptionFinalResponse
                {
                    Id = item.Id,
                    DateCreated = DateTime.Now,
                    OptionData = item.OptionData,
                    Cost = item.Cost,
                    Selected = item.Selected,
                    SelectedUserId = item.SelectedUserId,
                    SelectedUserName = $"{currentEmployee?.Firstname} {currentEmployee?.Lastname}",
                    OptionIndex = item.OptionIndex,
                    Comment = item.Comment,
                    Status = item.Selected == 1 ? "ISSUED" : "UPDATED",
                    SelectedUserTeam = groupname

                };
                returnData.Add(newRecord);
            }

            return returnData;
        }
        #endregion


        #region DeleteData

        public async Task DeleteOptionData(DeleteRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken)
        {
            if (_HTTPUserRepository.LogCurrentUser()?.Role == "TravelAdmin" || _HTTPUserRepository.LogCurrentUser()?.Role == "SystemAdmin")
            {
                var currentData = await Context.RequestNonSiteTravelOption.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                if (currentData != null)
                {

                    Context.RequestNonSiteTravelOption.Remove(currentData);

                    var reqHist = new RequestDocumentHistory();
                    reqHist.Comment = "Delete Travel Option";
                    reqHist.Active = 1;
                    reqHist.DateCreated = DateTime.Now;
                    reqHist.UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqHist.DocumentId = currentData.DocumentId;
                    reqHist.ActionEmployeeId = _HTTPUserRepository.LogCurrentUser()?.Id;
                    reqHist.CurrentAction = RequestDocumentAction.Saved;
                    Context.RequestDocumentHistory.Add(reqHist);


                }
            }
            else {
                throw new BadRequestException("Access denied");
            }


            await Task.CompletedTask;
        }


        #endregion




    }
}
