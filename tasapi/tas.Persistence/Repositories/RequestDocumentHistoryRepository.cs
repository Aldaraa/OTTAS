using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentHistoryFeature.GetRequestDocumentHistory;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public sealed class RequestDocumentHistoryRepository : BaseRepository<RequestDocumentHistory>, IRequestDocumentHistoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public RequestDocumentHistoryRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;

        }


        public async Task<List<GetRequestDocumentHistoryResponse>> GetRequestDocumentHistories(GetRequestDocumentHistoryRequest request, CancellationToken cancellationToken)
        { 
            var returnData = new List<GetRequestDocumentHistoryResponse>();

            var histories = await Context.RequestDocumentHistory.AsNoTracking().Where(x => x.DocumentId == request.DocumentId).OrderByDescending(x => x.DateCreated).ToListAsync();

            var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == request.DocumentId).FirstOrDefaultAsync();

            if (currentDocument != null) {
                foreach (var item in histories)
                {
                    string? ActionEmployeeGroupName = "";

                    string? grouptag = string.Empty;
                    var itemEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == item.ActionEmployeeId).FirstOrDefaultAsync();
                    var requestGroupConfigIds = await Context.RequestGroupConfig
                                .AsNoTracking().Where(x => x.Document == currentDocument.DocumentType)
                                .Select(x => x.GroupId).ToListAsync();

                    var employeeGroup = await Context.RequestGroupEmployee.AsNoTracking().Where(x => x.EmployeeId == itemEmployee.Id && requestGroupConfigIds.Contains(x.RequestGroupId) ).FirstOrDefaultAsync();
                    if (employeeGroup != null)
                    {



                        var currentGroup = await Context.RequestGroup.Where(x => x.Id == employeeGroup.RequestGroupId).FirstOrDefaultAsync();
                        if (currentGroup != null)
                        {
                            ActionEmployeeGroupName = currentGroup.Description;
                        }
                    }

                    string? assignedGroupName = null;
                    if (item.AssignedGroupId.HasValue)
                    {
                        var currengroup = await Context.RequestGroup.AsNoTracking().Where(x => x.Id == item.AssignedGroupId).Select(x => new { x.Description, x.GroupTag }).FirstOrDefaultAsync();
                        assignedGroupName = currengroup.Description;
                        if (currengroup != null)
                        {
                            grouptag = currengroup.GroupTag;
                        }
                        else {
                            grouptag = string.Empty;
                        }
                    }

                    if (currentDocument.DocumentType == RequestDocumentType.NonSiteTravel)
                    {
                        if (item.CurrentAction == RequestDocumentAction.Approved)
                        {

                            var newHistory = new GetRequestDocumentHistoryResponse
                            {
                                Id = item.Id,
                                Comment = item.Comment,
                                CreateDate = item.DateCreated,
                                CurrentAction = grouptag == "linemanager" ? item.CurrentAction : "Confirmed",
                                ActionEmployeeId = item.ActionEmployeeId,
                                //   ActionEmployeeFullName = $"{itemEmployee?.Firstname} {itemEmployee?.Lastname}  ({ActionEmployeeGroupName})",
                                ActionEmployeeFullName = $"{itemEmployee?.Firstname} {itemEmployee?.Lastname}" +
                         (!string.IsNullOrEmpty(ActionEmployeeGroupName) ? $" ({ActionEmployeeGroupName})" : ""),
                            AssignedGroupName = assignedGroupName
                            };

                            returnData.Add(newHistory);
                        }
                        else {
                            var newHistory = new GetRequestDocumentHistoryResponse
                            {
                                Id = item.Id,
                                Comment = item.Comment,
                                CreateDate = item.DateCreated,
                                CurrentAction = item.CurrentAction,
                                ActionEmployeeId = item.ActionEmployeeId,
                             //   ActionEmployeeFullName = $"{itemEmployee?.Firstname} {itemEmployee?.Lastname}  ({ActionEmployeeGroupName})",
                                ActionEmployeeFullName = $"{itemEmployee?.Firstname} {itemEmployee?.Lastname}" +  (!string.IsNullOrEmpty(ActionEmployeeGroupName) ? $" ({ActionEmployeeGroupName})" : ""),
                            AssignedGroupName = assignedGroupName
                            };

                            returnData.Add(newHistory);
                        }

                    }
                    else
                    {
                        var newHistory = new GetRequestDocumentHistoryResponse
                        {
                            Id = item.Id,
                            Comment = item.Comment,
                            CreateDate = item.DateCreated,
                            CurrentAction = item.CurrentAction,
                            ActionEmployeeId = item.ActionEmployeeId,
                            // ActionEmployeeFullName = $"{itemEmployee?.Firstname} {itemEmployee?.Lastname}  ({ActionEmployeeGroupName})",
                            ActionEmployeeFullName = $"{itemEmployee?.Firstname} {itemEmployee?.Lastname}" +
                         (!string.IsNullOrEmpty(ActionEmployeeGroupName) ? $" ({ActionEmployeeGroupName})" : ""),
                        AssignedGroupName = assignedGroupName
                        };

                        returnData.Add(newHistory);
                    }


                }

                return returnData;
            }
            else {
                return new List<GetRequestDocumentHistoryResponse>();
            }

         
        }
    }
}
