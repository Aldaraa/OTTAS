using AutoMapper.Configuration.Annotations;
using FluentValidation.Validators;
using MediatR;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestDocumentFeature.ApproveRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CancelRequestDocument;
using tas.Application.Features.RequestDocumentFeature.CheckDemobRequest;
using tas.Application.Features.RequestDocumentFeature.CheckDuplicateRequestDocument;
using tas.Application.Features.RequestDocumentFeature.DeclineRequestDocument;
using tas.Application.Features.RequestDocumentFeature.ExistingBookingRequestDocument;
using tas.Application.Features.RequestDocumentFeature.GenerateCompletedDeclinedChange;
using tas.Application.Features.RequestDocumentFeature.GenerateDescriptionTest;
using tas.Application.Features.RequestDocumentFeature.GetDocumentList;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListCancelled;
using tas.Application.Features.RequestDocumentFeature.GetDocumentListInpersonate;
using tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelGroup;
using tas.Application.Features.RequestDocumentFeature.GetNonSiteTravelMaster;
using tas.Application.Features.RequestDocumentFeature.GetRequestDocumentMyInfo;
using tas.Application.Features.RequestDocumentFeature.RemoveCancelRequestDocument;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Domain.Enums;
using tas.Persistence.Context;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace tas.Persistence.Repositories
{
    public partial class RequestDocumentRepository : BaseRepository<RequestDocument>, IRequestDocumentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly CacheService _memoryCache;
        private readonly IMailSmtpConfigRepository _mailSmtpConfigRepository;
        private readonly ITransportCheckerRepository _TransportCheckerRepository;


        public RequestDocumentRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository, IEmployeeRepository employeeRepository, IMailSmtpConfigRepository mailSmtpConfigRepository, ITransportCheckerRepository transportCheckerRepository, CacheService memoryCache) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
            _employeeRepository = employeeRepository;   
            _mailSmtpConfigRepository = mailSmtpConfigRepository;
            _TransportCheckerRepository = transportCheckerRepository;
            _memoryCache = memoryCache;


        }


        #region DocumentEmployeeCheck 
        public async Task DocumentEmployeeActiveCheck(int documentId)
        {


            var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {


                if (currentDocument.DocumentType == RequestDocumentType.SiteTravel && currentDocument.DocumentTag == "ADD")
                {
                    return;
                }
                else {
                   await _employeeRepository.EmployeeActiveCheck(Convert.ToInt32(currentDocument.EmployeeId));
                }

            }
            else {
                throw new BadRequestException("Document not found");   
            }


        }


        public async Task DocumentEmployeeActiveCompleteAddTravelCheck(int documentId)
        {

            var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {

            //    await _employeeRepository.EmployeeActiveCheck(Convert.ToInt32(currentDocument.EmployeeId));

                var currenttravelAdd = await Context.RequestSiteTravelAdd.AsNoTracking().Where(x => x.DocumentId == currentDocument.Id).FirstOrDefaultAsync();
                if (currenttravelAdd != null)
                {


                    await _TransportCheckerRepository.TransportAddValidDirectionSequenceCheck(currentDocument.EmployeeId.Value, currenttravelAdd.inScheduleId, currenttravelAdd.outScheduleId);
                }


            }
            else
            {
                throw new BadRequestException("Document not found");
            }


        }



        public async Task DocumentEmployeeActiveCompleteExternalAddTravelCheck(int documentId)
        {

            var currentDocument = await Context.RequestDocument.AsNoTracking().Where(x => x.Id == documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {

                await _employeeRepository.EmployeeActiveCheck(Convert.ToInt32(currentDocument.EmployeeId));

                var currenttravelAdd = await Context.RequestExternalTravelAdd.Where(x => x.DocumentId == currentDocument.Id).FirstOrDefaultAsync();
                if (currenttravelAdd != null)
                {
                    await _TransportCheckerRepository.TransportExternalAddValidCheck(currentDocument.EmployeeId.Value, currenttravelAdd.FirstScheduleId, currenttravelAdd.LastScheduleId);
                }


            }
            else
            {
                throw new BadRequestException("Document not found");
            }


        }


        #endregion



        #region DocumentList

        public async Task<GetDocumentListResponse> GetDocumentList(GetDocumentListRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;
            IQueryable<RequestDocument> documentFilter = Context.RequestDocument;

            var currentUser = _HTTPUserRepository.LogCurrentUser();
            var role = currentUser?.Role;
            var userId = currentUser?.Id;

            // Apply filters based on request.model properties
            if (!request.model.Id.HasValue)
            {

                if (request.model.startDate.HasValue && request.model.endDate.HasValue)
                {

                    var startDate = request.model.startDate.Value.Date;
                    var endDate = request.model.endDate.Value.Date;

                    endDate = endDate.AddDays(1);
                    documentFilter = documentFilter
                        .Where(x => x.DateCreated.Value.Date >= startDate
                        && x.DateCreated.Value.Date <= endDate);
                }

                if (request.model.startDate.HasValue && !request.model.endDate.HasValue)
                {

                    if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Pending && request.model.DocumentType == RequestDocumentType.NonSiteTravel)
                    {
                        var startDate = request.model.startDate.Value.Date;
                        documentFilter = documentFilter
                            .Where(x => x.DateCreated.Value.Date >= startDate.AddMonths(-6)
                           ) ;
                    }
                    else {
                        var startDate = request.model.startDate.Value.Date;
                        documentFilter = documentFilter
                            .Where(x => x.DateCreated.Value.Date >= startDate
                           );
                    }


                }


                if (!string.IsNullOrWhiteSpace(request.model.DocumentType))
                {



                    documentFilter = documentFilter.Where(x => x.DocumentType == request.model.DocumentType);
                }

                if (request.model.RequestedEmployeeId.HasValue)
                {
                    documentFilter = documentFilter.Where(x => x.UserIdCreated == request.model.RequestedEmployeeId);
                }

                if (request.model.AssignedEmployeeId.HasValue)
                {
                    documentFilter = documentFilter.Where(x => x.AssignedEmployeeId == request.model.AssignedEmployeeId);
                }

                if (request.model.LastModifiedDate.HasValue)
                {
                    var lastModifiedDate = request.model.LastModifiedDate.Value.Date;
                    documentFilter = documentFilter.Where(x => x.DateUpdated.Value.Date == lastModifiedDate);
                }

                if (request.model.ApprovelType.HasValue)
                {
                    var requestGroupConfigIds = await Context.RequestGroupConfig
                        .Where(x => x.GroupId == request.model.ApprovelType.Value)
                        .Select(x => x.Id)
                        .ToListAsync();

                    if (requestGroupConfigIds.Any())
                    {
                        documentFilter = documentFilter.Where(x => requestGroupConfigIds.Contains(x.AssignedRouteConfigId.Value));
                    }
                }

                // Apply status filtering
                if (!string.IsNullOrWhiteSpace(request.model.RequestDocumentSearchCurrentStep))
                {
                    if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Cancelled)
                    {
                        documentFilter = documentFilter.Where(x => x.CurrentAction == RequestDocumentAction.Cancelled);
                    }
                    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Completed)
                    {
                        documentFilter = documentFilter.Where(x => x.CurrentAction == RequestDocumentAction.Completed);
                    }
                    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Pending)
                    {
                        if (request.model.DocumentType == RequestDocumentType.NonSiteTravel || request.model.DocumentType == RequestDocumentType.DeMobilisation)
                        {
                            documentFilter = documentFilter.Where(x => x.CurrentAction != RequestDocumentAction.Completed && x.CurrentAction != RequestDocumentAction.Cancelled  && x.CurrentAction != RequestDocumentAction.Declined
                             || 
                             (x.UserIdCreated == userId &&
                             x.CurrentAction != RequestDocumentAction.Completed && x.CurrentAction != RequestDocumentAction.Cancelled)
                            );
                        }
                        else {
                            documentFilter = documentFilter.Where(x => x.CurrentAction != RequestDocumentAction.Completed && x.CurrentAction != RequestDocumentAction.Cancelled);
                        }
                    }
                    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Declined)
                    {
                        documentFilter = documentFilter.Where(x => x.CurrentAction == RequestDocumentAction.Declined);
                    }
                }

                // Apply keyword filtering
                if (!string.IsNullOrWhiteSpace(request.model.Keyword))
                {


                    var keyword = request.model.Keyword.Trim().Replace("\r\n", "").Replace("\n", "").ToLowerInvariant();
                    var documentKeyWordDatas = await (from document in documentFilter
                                                      join employee in Context.Employee on document.EmployeeId equals employee.Id
                                                      select new
                                                      {
                                                          DocumentId = document.Id,
                                                          EmployeeFullName = (employee.Firstname + " " + employee.Lastname).ToLowerInvariant(),
                                                          Description = Transliterator.NormalizeString(document.Description)
                                                      }).ToListAsync();

                    var keywordDocIds = documentKeyWordDatas
                        .Where(x => x.EmployeeFullName.Contains(keyword) || (x.Description != null &&
                     x.Description.Contains(keyword)))
                        .Select(x => x.DocumentId)
                        .ToList();

                    documentFilter = documentFilter.Where(x => keywordDocIds.Contains(x.Id));
                }
            }
            else
            {
                documentFilter = documentFilter.Where(x => x.Id == request.model.Id);
            }

            documentFilter = documentFilter.Where(x => x.Active == 1);

            // Apply role-based filters
            if (role == "DepartmentManager" || role == "DepartmentAdmin")
            {
                var requestDelegateIds = await Context.RequestDelegates
                    .AsNoTracking()
                    .Where(c => c.EndDate > DateTime.Now && c.ToEmployeeId == userId)
                    .Select(x => x.FromEmployeeId)
                    .ToListAsync();

                if (requestDelegateIds.Any())
                {
                    documentFilter = documentFilter.Where(x =>
                        (x.AssignedEmployeeId.HasValue && requestDelegateIds.Contains(x.AssignedEmployeeId.Value)) ||
                        x.AssignedEmployeeId == userId ||
                        x.UserIdCreated == userId);
                }
                else
                {
                    documentFilter = documentFilter.Where(x => x.AssignedEmployeeId == userId || x.UserIdCreated == userId);
                }
            }
            else if (role == "Guest")
            {
                documentFilter = documentFilter.Where(x => x.UserIdCreated == userId);
            }

            var result = from document in documentFilter
                         join requestedEmployee in Context.Employee.AsNoTracking() on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                         from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                         join documentEmployee in Context.Employee.AsNoTracking() on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                         from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                         join employer in Context.Employer.AsNoTracking() on documentEmployee.EmployerId equals employer.Id into employerData
                         from employer in employerData.DefaultIfEmpty()
                         join updateEmployee in Context.Employee.AsNoTracking() on document.UserIdUpdated equals updateEmployee.Id into updateEmployeeData
                         from updateEmployee in updateEmployeeData.DefaultIfEmpty()
                         select new GetDocumentList
                         {
                             Id = document.Id,
                             Description = document.Description,
                             DocumentType = document.DocumentType,
                             RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname}",
                             RequestedDate = document.DateCreated,
                             EmployeeFullName = $"{documentEmployee.Firstname} {documentEmployee.Lastname}",
                             EmployerName = employer.Description,
                             EmployerId = employer.Id,
                             EmployeeId = document.EmployeeId,
                             UpdatedInfo = document.UpdatedInfo,
                             CurrentStatus = document.CurrentAction == "Approved" ? "In Progress" : document.CurrentAction,
                             AssignedEmployeeFullName = null,
                             AssignedGroupName = null,
                             DaysAway = document.DaysAwayDate.HasValue ? (DateTime.Today.Date.Subtract(document.DaysAwayDate.Value.Date).Days) * (-1) : (DateTime.Today.Subtract(document.DateCreated.Value.Date).Days) * (-1),
                             DocumentTag = document.DocumentTag,
                             CreatedDate = document.DateCreated,
                             AssignedEmployeeId = document.AssignedEmployeeId,
                             UserIdCreated = document.UserIdCreated,
                             DaysAwayDate = document.DaysAwayDate
                         };

            var totalCount = await result.CountAsync();

            var retData = await result
                .OrderBy(x => x.DaysAwayDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (request.model.EmployerId.HasValue)
            {
                retData = retData.Where(x => x.EmployerId == request.model.EmployerId).ToList();
            }

            foreach (var item in retData)
            {
                item.AssignedEmployeeFullName = await GetAssignedEmployeeInfo(item.Id);
                item.AssignedGroupName = await GetAssignedGroupInfo(item.Id);
                item.CurrentStatusGroup = $"{item.CurrentStatus} {item.AssignedGroupName}";
            }

            var returnData = new GetDocumentListResponse
            {
                data = retData.OrderBy(x => x.DaysAwayDate).ToList(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = totalCount
            };

            return returnData;
        }





        public async Task<GetDocumentListResponse> GetDocumentList2(GetDocumentListRequest request, CancellationToken cancellationToken)
        {
            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;
            IQueryable<RequestDocument> documentFilter = Context.RequestDocument;




            var role = _HTTPUserRepository.LogCurrentUser()?.Role;
            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;


            if (!request.model.Id.HasValue)
            {

                if (request.model.startDate.HasValue && request.model.endDate.HasValue)
                {

                    var startDate = request.model.startDate.Value.Date;
                    var endDate = request.model.endDate.Value.Date;

                    endDate = endDate.AddDays(1);
                    documentFilter = documentFilter
                        .Where(x => x.DateCreated.Value.Date >= startDate
                        && x.DateCreated.Value.Date <= endDate);
                }

                if (request.model.startDate.HasValue && !request.model.endDate.HasValue)
                {

                    var startDate = request.model.startDate.Value.Date;
                    documentFilter = documentFilter
                        .Where(x => x.DateCreated.Value.Date >= startDate
                       );
                }

                if (request.model.Id.HasValue)
                {
                    documentFilter = documentFilter.Where(x => x.Id == request.model.Id);
                }
                if (!string.IsNullOrWhiteSpace(request.model.DocumentType))
                {
                    if (request.model.DocumentType == RequestDocumentType.SiteTravel)
                    {
                        documentFilter = documentFilter.Where(x =>  x.DocumentType == RequestDocumentType.SiteTravel);
                    }
                    else
                    {
                        documentFilter = documentFilter.Where(x => x.DocumentType == request.model.DocumentType);
                    }

                }
                if (request.model.RequestedEmployeeId.HasValue)
                {
                    documentFilter = documentFilter.Where(x => x.UserIdCreated == request.model.RequestedEmployeeId);
                }
                if (request.model.AssignedEmployeeId.HasValue)
                {
                    documentFilter = documentFilter.Where(x => x.AssignedEmployeeId == request.model.AssignedEmployeeId);
                }
                if (request.model.LastModifiedDate.HasValue)
                {
                    documentFilter = documentFilter.Where(x => x.DateUpdated.Value.Date == request.model.LastModifiedDate.Value.Date);
                }

                if (request.model.ApprovelType.HasValue)
                {
                    var requestGroupConfigIds = await Context.RequestGroupConfig.Where(x => x.GroupId == request.model.ApprovelType.Value).Select(x => x.Id).ToListAsync();
                    if (requestGroupConfigIds.Count > 0)
                    {
                        documentFilter = documentFilter.Where(x => requestGroupConfigIds.Contains(x.AssignedRouteConfigId.Value));
                    }
                }


                if (!string.IsNullOrWhiteSpace(request.model.Keyword))
                {

                    

                    try
                    {

                        var documentKeyWordDatas = await (from document in documentFilter
                                                          join employee in Context.Employee.AsNoTracking() on document.EmployeeId equals employee.Id
                                                          select new
                                                          {
                                                              DocumentId = document.Id,
                                                              EmployeeFullName = employee.Firstname + " " + employee.Lastname,
                                                          }).ToListAsync();

                        var keywordDocIds = documentKeyWordDatas
                            .Where(x => x.EmployeeFullName.Contains(request.model.Keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                            .Select(x => x.DocumentId)
                            .ToList();

                        // Perform the final filtering in memory
                        var documentsFromDb = await documentFilter.ToListAsync();

                        //documentFilter = documentsFromDb
                        //    .Where(x => keywordDocIds.Contains(x.Id) ||
                        //                (x.Description != null && x.Description.IndexOf(request.model.Keyword.Trim(), StringComparison.OrdinalIgnoreCase) >= 0))
                        //    .AsQueryable();


                        //var documentKeyWordDatas = await (from document in documentFilter
                        //                                  join employee in Context.Employee on document.EmployeeId equals employee.Id
                        //                                  select new
                        //                                  {
                        //                                      DocumentId = document.Id,
                        //                                      EmployeeFullName = employee.Firstname + " " + employee.Lastname,
                        //                                  }).ToListAsync();


                        //var keywordDocIds = documentKeyWordDatas
                        // .Where(x => x.EmployeeFullName.Contains(request.model.Keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                        // .Select(x => x.DocumentId)
                        // .ToList();

                        // documentFilter = documentFilter.Where(x => keywordDocIds.Contains(x.Id) || x.Description.Replace().ToLowerInvariant().Contains(request.model.Keyword.ToLowerInvariant().Trim()));


                        var filteredDocuments = documentsFromDb
                    .Where(x => keywordDocIds.Contains(x.Id) ||
                    (x.Description != null &&
                                 x.Description.Replace("\r\n", "").Replace("\n", "").IndexOf(request.model.Keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                    .AsQueryable();

                    }
                    catch (Exception)
                    {
                        documentFilter = documentFilter;
                    }




                }



                if (!string.IsNullOrWhiteSpace(request.model.RequestDocumentSearchCurrentStep))
                {
                    if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Cancelled)
                    {
                        documentFilter = documentFilter.Where(x => x.CurrentAction == RequestDocumentSearchCurrentStep.Cancelled);
                    }
                    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Completed)
                    {
                        documentFilter = documentFilter.Where(x => x.CurrentAction == RequestDocumentAction.Completed);
                    }
                    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Pending)
                    {
                        documentFilter = documentFilter.Where(x => x.CurrentAction != RequestDocumentAction.Completed && x.CurrentAction != RequestDocumentAction.Cancelled);
                    }
                    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Declined)
                    {
                        documentFilter = documentFilter.Where(x => x.CurrentAction == RequestDocumentAction.Declined);
                    }



                }

                //if (!string.IsNullOrWhiteSpace(request.model.RequestDocumentSearchCurrentStep))
                //{
                //    // Fetch the data from the database first
                //    var documentsFromDb = await documentFilter.ToListAsync();

                //    // Perform the filtering in memory
                //    if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Cancelled)
                //    {
                //        documentsFromDb = documentsFromDb.Where(x => x.CurrentAction == RequestDocumentAction.Cancelled).ToList();
                //    }
                //    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Completed)
                //    {
                //        documentsFromDb = documentsFromDb.Where(x => x.CurrentAction == RequestDocumentAction.Completed).ToList();
                //    }
                //    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Pending)
                //    {
                //        documentsFromDb = documentsFromDb.Where(x => x.CurrentAction != RequestDocumentAction.Completed && x.CurrentAction != RequestDocumentAction.Cancelled).ToList();
                //    }
                //    else if (request.model.RequestDocumentSearchCurrentStep == RequestDocumentSearchCurrentStep.Declined)
                //    {
                //        documentsFromDb = documentsFromDb.Where(x => x.CurrentAction == RequestDocumentAction.Declined).ToList();
                //    }

                //    // Convert back to IQueryable if further query is needed
                //    documentFilter = documentsFromDb.AsQueryable();
                //}

            }
            else {
                documentFilter = documentFilter.Where(x => x.Id == request.model.Id);
            }

            documentFilter = documentFilter.Where(x => x.Active == 1);

            if (role == "DepartmentManager" || role == "DepartmentAdmin")
            {

                var requestDelegateIds = await Context.RequestDelegates.AsNoTracking().Where(c => c.EndDate > DateTime.Now && c.ToEmployeeId == userId).Select(x => x.FromEmployeeId).ToListAsync();

                if (requestDelegateIds.Count > 0)
                {
                    documentFilter = documentFilter.Where(x =>
                          (x.AssignedEmployeeId.HasValue && requestDelegateIds.Contains(x.AssignedEmployeeId.Value)) ||
                          x.AssignedEmployeeId == userId ||
                          x.UserIdCreated == userId);
                }
                else
                {
                    documentFilter = documentFilter.Where(x => x.AssignedEmployeeId == userId || x.UserIdCreated == userId);
                }
            }


            if (role == "Guest")
            {
                documentFilter = documentFilter.Where(x => x.UserIdCreated == userId);
            }





            var result = from document in Context.RequestDocument.AsNoTracking()
                         where documentFilter.Contains(document)
                         join requestedEmployee in Context.Employee.AsNoTracking() on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                         from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                         join documentEmployee in Context.Employee.AsNoTracking() on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                         from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                         join employer in Context.Employer.AsNoTracking() on documentEmployee.EmployerId equals employer.Id into employerData
                         from employer in employerData.DefaultIfEmpty()
                         join updateEmployee in Context.Employee.AsNoTracking() on document.UserIdUpdated equals updateEmployee.Id into updateEmployeeData
                         from updateEmployee in updateEmployeeData.DefaultIfEmpty()
                         select new GetDocumentList
                         {
                             Id = document.Id,
                             Description = document.Description,
                             DocumentType = document.DocumentType,
                             RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname}",
                             RequestedDate = document.DateCreated,
                             EmployeeFullName = $"{documentEmployee.Firstname} {documentEmployee.Lastname}",
                             EmployerName = $"{employer.Description}",
                             EmployerId = employer.Id,
                             EmployeeId = document.EmployeeId,
                             UpdatedInfo = document.UpdatedInfo,
                             CurrentStatus = document.CurrentAction == "Approved" ? "In Progress " : document.CurrentAction,
                             AssignedEmployeeFullName = null,
                             AssignedGroupName = null,
                             DaysAway =document.DaysAwayDate.HasValue ? (DateTime.Today.Date.Subtract(document.DaysAwayDate.Value.Date).Days) *(-1) : (DateTime.Today.Subtract(document.DateCreated.Value.Date).Days) * (-1),
                             DocumentTag = document.DocumentTag,
                             CreatedDate = document.DateCreated,
                             AssignedEmployeeId = document.AssignedEmployeeId,
                             UserIdCreated = document.UserIdCreated,
                             DaysAwayDate = document.DaysAwayDate

                         };




            //if (role == "DepartmentAdmin")
            //{

            //    retData = retData.Where(x => x.UserIdCreated == _HTTPUserRepository.LogCurrentUser()?.Id).ToList();



            //}




            var totalCount = await result.CountAsync();

            var retData = await result
                .OrderBy(x => x.DaysAwayDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();


            if (request.model.EmployerId.HasValue)
            {
                retData = retData.Where(x => x.EmployerId == request.model.EmployerId).ToList() ;
            }

            //if (!string.IsNullOrWhiteSpace(request.model.Keyword))
            //{
            //    retData = retData.Where(x => x.EmployeeFullName.Contains(request.model.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            //}

    


            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            foreach (var item in retData)
            {

                // var description = await GetDocumentDescription(item);
                item.AssignedEmployeeFullName = await GetAssignedEmployeeInfo(item.Id);
                item.AssignedGroupName = await GetAssignedGroupInfo(item.Id);
                item.CurrentStatusGroup = $"{item.CurrentStatus} {item.AssignedGroupName}";

            }


            Console.WriteLine($"**************************************---------AWAIT  DATA-------------********************************Execution Time: {watch.ElapsedMilliseconds} ms");


            var reData = new List<GetDocumentList>();

            var returnData = new GetDocumentListResponse
            {
                data = retData.OrderBy(x=> x.DaysAwayDate).ToList(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = totalCount
            };

            return returnData;

        }




        public async Task<List<int>> DelegateDepartmentEmployeeIds(int userId)
        {
            var requestDelegateIds = await Context.RequestDelegates.AsNoTracking().Where(c => c.EndDate > DateTime.Now && c.ToEmployeeId == userId).Select(x => x.FromEmployeeId).ToListAsync();
            var returnData = new List<int>();
            if (requestDelegateIds.Count > 0)
            {

                var departmentIds = await Context.DepartmentAdmin.AsNoTracking().Where(x => requestDelegateIds.Contains(x.EmployeeId)).Select(x => x.DepartmentId).ToListAsync();

                List<int> RoleDepartmenIds = new List<int>();
                foreach (var item in departmentIds)
                {
                    RoleDepartmenIds.Add(item);
                    var retIds = await GetAllChildDepartmentIds(item);
                    RoleDepartmenIds.AddRange(retIds);
                }



                var ManagerDepartmentIds = await Context.DepartmentManager.AsNoTracking().Where(x => requestDelegateIds.Contains(x.EmployeeId)).Select(x => x.DepartmentId).ToListAsync();

                foreach (var item in ManagerDepartmentIds)
                {
                    RoleDepartmenIds.Add(item);
                    var retIds = await GetAllChildDepartmentIds(item);
                    RoleDepartmenIds.AddRange(retIds);
                }

                if (RoleDepartmenIds.Count > 0)
                {
                    returnData = await Context.Employee.AsNoTracking().Where(x => RoleDepartmenIds.Contains(x.DepartmentId.Value)).Select(x => x.Id).ToListAsync();
                }
                else
                {
                    returnData.AddRange(requestDelegateIds);
                }
            }

            return returnData;
        }



                                                                                                                                                                                                                                                                                    

        #endregion


        #region GenerateDescription




        public async Task GenerateDescriptionTest(GenerateDescriptionTestRequest request, CancellationToken cancellationToken) 
        {
            if (request.Ids.Count > 0)
            {
                foreach (var item in request.Ids)
                {
                    await GenerateDescription(item, cancellationToken);
                }

            }
            else {
                var ids = await Context.RequestDocument.Select(c => new { c.Id }).ToListAsync();
                if (ids.Count > 0) {
                    foreach (var item in ids)
                    {
                        await GenerateDescription(item.Id, cancellationToken);
                    }
                }
            }
        }



        #endregion



        #region GenerateCompletedDeclinedChange
        public async Task GenerateCompletedDeclinedChange(GenerateCompletedDeclinedChangeRequest request, CancellationToken cancellationToken)
        {
            var completedDocuments = await  Context.RequestDocument.Where(x => x.CurrentAction == RequestDocumentAction.Completed && x.CompletedUserId == null).ToListAsync();
            var declinedDocuments = await  Context.RequestDocument.Where(x => x.CurrentAction == RequestDocumentAction.Declined && x.CompletedUserId == null).ToListAsync();

            foreach (var item in completedDocuments)
            {
                var docHistory = await  Context.RequestDocumentHistory.Where(x => x.DocumentId == item.Id).ToListAsync();
                if (docHistory.Count > 0)
                {
                    var currentDocHist = docHistory.Where(c => c.CurrentAction == RequestDocumentAction.Completed).FirstOrDefault();
                    if (currentDocHist != null) { 
                        item.CompletedDate = currentDocHist.DateCreated;
                        item.CompletedUserId = currentDocHist.UserIdCreated;

                        Context.RequestDocument.Update(item);
                    }
                }

            }


            foreach (var item in declinedDocuments)
            {
                var docHistory = await Context.RequestDocumentHistory.Where(x => x.DocumentId == item.Id).ToListAsync();
                if (docHistory.Count > 0)
                {
                    var currentDocHist = docHistory.Where(c => c.CurrentAction == RequestDocumentAction.Declined).FirstOrDefault();
                    if (currentDocHist != null)
                    {
                        item.DeclinedDate = currentDocHist.DateCreated;
                        item.DeclinedUserId = currentDocHist.UserIdCreated;

                        Context.RequestDocument.Update(item);
                    }
                }

            }

           await Context.SaveChangesAsync(cancellationToken);    



        }
        #endregion



        //1011


        private async Task<string?> GetAssignedGroupInfo(int documentId)
        {
            var currentData = await Context.RequestDocument.AsNoTracking()
                .Where(x => x.Id == documentId).FirstOrDefaultAsync();
            if (currentData != null)
            {
                var p_groupdata = await (from configData in Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == currentData.AssignedRouteConfigId)
                                         join groupData in Context.RequestGroup.AsNoTracking()
                                         on configData.GroupId equals groupData.Id into groupDatas
                                         from groupData in groupDatas.DefaultIfEmpty()
                                         select new
                                         {
                                             groupName = groupData.Description
                                         }).FirstOrDefaultAsync();
                if (p_groupdata != null)
                {
                    return p_groupdata.groupName;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }

        }


        private async Task<string?> GetAssignedEmployeeInfo(int documentId)
        {
            var currentData = await Context.RequestDocument.AsNoTracking()
                .Where(x => x.Id == documentId)
                .OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                var currentAssignedEmployee = await Context.Employee.AsNoTracking().Where(x => x.Id == currentData.AssignedEmployeeId).FirstOrDefaultAsync();
                if (currentAssignedEmployee != null)
                {
                    return $"{currentAssignedEmployee.Firstname} {currentAssignedEmployee.Lastname}";
                }
                else {
                    return string.Empty;
                }
            }
            else {
                return string.Empty;
            }

        }

        public async Task<GetNonSiteTravelMasterResponse> GetMaster(GetNonSiteTravelMasterRequest request, CancellationToken cancellationToken)
        {
            var returnData = new GetNonSiteTravelMasterResponse();

            var paymentCondition = new List<string>();
            paymentCondition.Add(RequestDocumentPaymentCondition.PersonalExpenses);
            paymentCondition.Add(RequestDocumentPaymentCondition.OrganizationalExpenses);

            returnData.PaymentCondition = paymentCondition;
            var searchDocumentActions = new List<string>();
            searchDocumentActions.Add(RequestDocumentSearchCurrentStep.Pending);
            searchDocumentActions.Add(RequestDocumentSearchCurrentStep.Cancelled);
            searchDocumentActions.Add(RequestDocumentSearchCurrentStep.Declined);
            searchDocumentActions.Add(RequestDocumentSearchCurrentStep.Completed);

            returnData.DocumentSearchCurrentStep = searchDocumentActions;


            var RequestDocumentTypes = new List<string>();
            RequestDocumentTypes.Add(RequestDocumentType.NonSiteTravel);
            RequestDocumentTypes.Add(RequestDocumentType.ProfileChanges);
            RequestDocumentTypes.Add(RequestDocumentType.SiteTravel);
            RequestDocumentTypes.Add(RequestDocumentType.DeMobilisation);


            var RequestDocumentFavorTimes = new List<string>();
            RequestDocumentFavorTimes.Add(RequestDocumentFavorTime.EarlyMorning);
            RequestDocumentFavorTimes.Add(RequestDocumentFavorTime.Morning);
            RequestDocumentFavorTimes.Add(RequestDocumentFavorTime.Lunch);
            RequestDocumentFavorTimes.Add(RequestDocumentFavorTime.Afternoon);
            RequestDocumentFavorTimes.Add(RequestDocumentFavorTime.Evening);
            RequestDocumentFavorTimes.Add(RequestDocumentFavorTime.LateNight);

            returnData.RequestDocumentFavorTime = RequestDocumentFavorTimes;
            returnData.RequestDocumentType = RequestDocumentTypes;
            return returnData;
        }
   

        public async Task CancelRequestDocument(CancelRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                currentDocument.CurrentAction = RequestDocumentAction.Cancelled;
                currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentDocument.DateUpdated = DateTime.Now;
                Context.RequestDocument.Update(currentDocument);
                if (currentDocument.DocumentTag == "ADD" && currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                {
                    var currentDetail = await Context.RequestSiteTravelAdd.Where(x => x.DocumentId == currentDocument.Id).FirstOrDefaultAsync();
                    if (currentDetail != null)
                    {
                        currentDetail.RoomId = null;
                        Context.RequestSiteTravelAdd.Update(currentDetail);
                    }
                }



                var newRecord = new RequestDocumentHistory
                {
                    Comment = request.Comment,
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    CurrentAction = RequestDocumentAction.Cancelled,
                    ActionEmployeeId = request.ImpersonateUserId.HasValue == true ? request.ImpersonateUserId : _HTTPUserRepository.LogCurrentUser()?.Id,
                    DocumentId = request.Id
                };
                Context.RequestDocumentHistory.Add(newRecord);
            }



            await Task.CompletedTask;
        }
        
        
        
        public async Task RemoveCancelRequestDocument(RemoveCancelRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.documentId).FirstOrDefaultAsync();
            if (currentDocument != null)
            {

                currentDocument.CurrentAction = RequestDocumentAction.Cancelled;
                if (currentDocument.CurrentAction == RequestDocumentAction.Cancelled)
                {
                    currentDocument.Active = 0;

                    Context.RequestDocument.Update(currentDocument);

                }
            }



            await Task.CompletedTask;
        }


        public async Task DeclineRequestDocument(DeclineRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentDocument != null)
            {
                int? oldDocumentAssignedGrouId = currentDocument.AssignedRouteConfigId;

                currentDocument.CurrentAction = RequestDocumentAction.Declined;
                currentDocument.AssignedEmployeeId = currentDocument.UserIdCreated;
                currentDocument.AssignedRouteConfigId = null;
                currentDocument.DeclinedDate = DateTime.Now;
                currentDocument.DeclinedUserId = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                currentDocument.DateUpdated = DateTime.Now;

                Context.RequestDocument.Update(currentDocument);

                if (currentDocument.DocumentTag == "ADD" && currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                {
                    var currentDetail = await Context.RequestSiteTravelAdd.Where(x => x.DocumentId == currentDocument.Id).FirstOrDefaultAsync();
                    if (currentDetail != null)
                    {
                        currentDetail.RoomId = null;
                        Context.RequestSiteTravelAdd.Update(currentDetail);
                    }
                }

                var assignedOldGroupId = await Context.RequestGroupConfig.Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();
                var newRecord = new RequestDocumentHistory
                {
                    Comment = request.Comment,
                    Active = 1,
                    DateCreated = DateTime.Now,
                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                    CurrentAction = RequestDocumentAction.Declined,
                    ActionEmployeeId = request.ImpersonateUserId.HasValue == true ? request.ImpersonateUserId : _HTTPUserRepository.LogCurrentUser()?.Id,
                    DocumentId = request.Id,
                    AssignedGroupId =  assignedOldGroupId?.GroupId,

                };
                Context.RequestDocumentHistory.Add(newRecord);
            }
            await Task.CompletedTask;
        }



        public async Task ApproveRequestDocument(ApproveRequestDocumentRequest request, CancellationToken cancellationToken)
        {
            var currentDocument = await Context.RequestDocument.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            int? assignedEmployeeId = null;
            int? userId = _HTTPUserRepository.LogCurrentUser()?.Id;
            var userrole = _HTTPUserRepository.LogCurrentUser()?.Role;


    

            if (currentDocument != null)
            {
                if (currentDocument.CurrentAction != RequestDocumentAction.Cancelled && currentDocument.CurrentAction != RequestDocumentAction.Completed)
                {
                    var currentGroupConfig = await (from groupConfig in Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == currentDocument.AssignedRouteConfigId)
                                                    join requestGroup in Context.RequestGroup.AsNoTracking() on groupConfig.GroupId equals requestGroup.Id
                                                    select new { groupConfig.Id, requestGroup.GroupTag }).FirstOrDefaultAsync();

                    if (currentGroupConfig == null)
                    {
                        throw new BadRequestException("The request group configuration is incomplete. Please contact the admin team.");

                    }

                    if (currentDocument.CurrentAction == RequestDocumentAction.Approved
                        && currentDocument.AssignedRouteConfigId == request.NextGroupId)

                    {
                        throw new BadRequestException("This task already Approved");
                    }
                    if (currentDocument.CurrentAction == RequestDocumentAction.Declined && currentDocument.UserIdCreated != userId/* || userrole != "SystemAdmin"*/)
                    {
                        throw new BadRequestException("This request has already been declined. Unable to approve");
                    }

                    if (userrole != "SystemAdmin"  &&  userId == currentDocument?.EmployeeId && currentGroupConfig.GroupTag == "linemanager")
                    {
                        throw new BadRequestException("You cannot approve your own request. It must be reviewed and approved by another user.");
                    }

                    else
                    {

                        var approveAccess = await CheckApproveUser(userrole, userId.Value, currentDocument);

                        if (approveAccess)
                        {
                            bool lineManagerSendMail = false;

                            var approvalGroup = await Context.RequestGroup.AsNoTracking().Where(x => x.GroupTag == "dataapproval").FirstOrDefaultAsync();

                            if (approvalGroup != null)
                            {

                                var currentApprovolGroup = await Context.RequestGroupEmployee.AsNoTracking().Where(x => x.EmployeeId == userId && x.RequestGroupId == approvalGroup.Id).FirstOrDefaultAsync();

                                int? oldDocumentAssignedGrouId = currentDocument.AssignedRouteConfigId;

                                if (currentDocument.AssignedEmployeeId != null)
                                {
                                    if (currentDocument.AssignedEmployeeId != userId)
                                    {
                                        assignedEmployeeId = currentDocument.AssignedEmployeeId;
                                        lineManagerSendMail = true;
                                    }
                                }

                                currentDocument.CurrentAction = RequestDocumentAction.Approved;
                                currentDocument.AssignedRouteConfigId = request.NextGroupId;
                                currentDocument.AssignedEmployeeId = request.assignEmployeeId.HasValue ? request.assignEmployeeId : null;
                                currentDocument.UserIdUpdated = _HTTPUserRepository.LogCurrentUser()?.Id;
                                currentDocument.DateUpdated = DateTime.Now;
                                Context.RequestDocument.Update(currentDocument);


                                var assignedOldGroupId = await Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == oldDocumentAssignedGrouId).FirstOrDefaultAsync();

                                var newRecord = new RequestDocumentHistory
                                {
                                    Comment = request.Comment,
                                    Active = 1,
                                    DateCreated = DateTime.Now,
                                    UserIdCreated = _HTTPUserRepository.LogCurrentUser()?.Id,
                                    CurrentAction = RequestDocumentAction.Approved,
                                    ActionEmployeeId = request.ImpersonateUserId.HasValue == true ? request.ImpersonateUserId : _HTTPUserRepository.LogCurrentUser()?.Id,
                                    AssignedGroupId = assignedOldGroupId?.GroupId,

                                    DocumentId = request.Id
                                };
                                Context.RequestDocumentHistory.Add(newRecord);




                                if ((currentDocument.DocumentType == RequestDocumentType.SiteTravel || currentDocument.DocumentType == RequestDocumentType.ProfileChanges) && currentApprovolGroup != null)
                                {
                                    var currentEmployee = await Context.Employee.Where(x => x.Id == currentDocument.EmployeeId).FirstOrDefaultAsync();
                                    if (currentEmployee != null)
                                    {
                                        if (currentEmployee.Active != 1)
                                        {
                                            currentEmployee.Active = 1;
                                            Context.Employee.Update(currentEmployee);

                                            string cacheEntityName = $"Employee_{currentEmployee.Id}";
                                            _memoryCache.Remove($"API::{cacheEntityName}");

                                        }

                                    }


                                }

                                if (currentDocument.DocumentType == RequestDocumentType.SiteTravel)
                                {
                                    if (lineManagerSendMail)
                                    {
                                        if (assignedEmployeeId.HasValue && userId.HasValue)
                                        {
                                            await _mailSmtpConfigRepository.SendMailRequestDocumentLineManageNotification(request.Id, assignedEmployeeId.Value, userId.Value, cancellationToken);
                                        }

                                    }
                                }

                            }
                            else
                            {
                                throw new BadRequestException("Please register the Data Approval group");
                            }
                        }
                        else
                        {
                            throw new BadRequestException("This request cannot be approved.");
                        }
                    }
                
                }
                else {
                    throw new BadRequestException("This task already Completed");
                }

            }



            await Task.CompletedTask;
        }


        private async Task<bool> CheckApproveUser(string role, int employeeId, RequestDocument currentDocument)
        {
            if (role == "SystemAdmin" || role == "AccomAdmin" || role == "TravelAdmin" || role == "DataApproval")
            {
                return true;
            }
            else {


                return true;
                //var currentGroupConfigData =  await Context.RequestGroupConfig.AsNoTracking().Where(x => x.Id == currentDocument.AssignedRouteConfigId).FirstOrDefaultAsync();
                //if (currentGroupConfigData != null)
                //{
                //    var currentData = await Context.RequestGroupEmployee.AsNoTracking()
                //          .Where(x => x.EmployeeId == employeeId && x.RequestGroupId == currentGroupConfigData.GroupId).FirstOrDefaultAsync();
                //    if (currentData != null)
                //    {
                //        return true;
                //    }
                //    else {
                //        return false;
                //    }

                //}
                //else {
                //    return false;
                //}
            }
        }




       






        public async Task<List<CheckDuplicateRequestDocumentResponse>> CheckDuplicateRequestDocument(CheckDuplicateRequestDocumentRequest request, CancellationToken cancellationToken)
        {

            var notActiveDocumentAction = new List<string> { RequestDocumentAction.Cancelled, RequestDocumentAction.Completed };

            var sdate = request.startDate;

            if ( request.DocumentType.ToLower() == RequestDocumentType.NonSiteTravel.ToLower())
            {

                var currentData = await (from flight in Context.RequestNonSiteTravelFlight.AsNoTracking()
                                         where flight.TravelDate == request.startDate
                                         join doc in Context.RequestDocument
                                         on flight.DocumentId equals doc.Id
                                         where doc.EmployeeId.Value == request.EmployeeId
                                         select doc.Id
                                        ).ToListAsync();
                if (currentData.Count > 0)
                {



                    var returnData = await (from doc in Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == request.DocumentType
                                               && !notActiveDocumentAction.Contains(x.CurrentAction) && currentData.Contains(x.Id)
                                              && x.DateCreated.Value.Date == DateTime.Today.Date && x.EmployeeId == request.EmployeeId)
                                            join assignedEmployee in Context.Employee.AsNoTracking() on doc.AssignedEmployeeId equals assignedEmployee.Id into assignedEmployeeData
                                            from assignedEmployee in assignedEmployeeData.DefaultIfEmpty()

                                            join requestEmployee in Context.Employee.AsNoTracking() on doc.UserIdCreated equals requestEmployee.Id into requestEmployeeData
                                            from requestEmployee in requestEmployeeData.DefaultIfEmpty()
                                            select new CheckDuplicateRequestDocumentResponse
                                            {
                                                Id = doc.Id,
                                                AssignedEmployee = assignedEmployee != null ? $"{assignedEmployee.Firstname} {assignedEmployee.Lastname}" : string.Empty,
                                                CurrentAction = doc.CurrentAction,
                                                Description = requestEmployee != null ? $"{requestEmployee.Firstname}  {requestEmployee.Lastname}" : string.Empty,
                                                CreatedAt = doc.DateCreated,
                                                DocumentTag = doc.DocumentTag,
                                                DocumentType = doc.DocumentType
                                            }
                        ).ToListAsync();

                    return returnData;
                }
                else {
                    return new List<CheckDuplicateRequestDocumentResponse>();
                }


            }
            else
            {
                var returnData = await (from doc in Context.RequestDocument.AsNoTracking().Where(x => x.DocumentType == request.DocumentType
                   && !notActiveDocumentAction.Contains(x.CurrentAction)
                  && x.DateCreated.Value.Date == DateTime.Today.Date && x.EmployeeId == request.EmployeeId)
                                        join assignedEmployee in Context.Employee.AsNoTracking() on doc.AssignedEmployeeId equals assignedEmployee.Id into assignedEmployeeData
                                        from assignedEmployee in assignedEmployeeData.DefaultIfEmpty()

                                        join requestEmployee in Context.Employee.AsNoTracking() on doc.UserIdCreated equals requestEmployee.Id into requestEmployeeData
                                        from requestEmployee in requestEmployeeData.DefaultIfEmpty()
                                        select new CheckDuplicateRequestDocumentResponse
                                        {
                                            Id = doc.Id,
                                            AssignedEmployee = assignedEmployee != null ? $"{assignedEmployee.Firstname} {assignedEmployee.Lastname}" : string.Empty,
                                            CurrentAction = doc.CurrentAction,
                                            Description = requestEmployee != null ? $"{requestEmployee.Firstname}  {requestEmployee.Lastname}" : string.Empty,
                                            CreatedAt = doc.DateCreated,
                                            DocumentTag = doc.DocumentTag,
                                            DocumentType = doc.DocumentType
                                        }
                 ).ToListAsync();
                return returnData;
            }

        }



        public async Task<ExistingBookingRequestDocumentResponse> ExistingBookingRequestDocument(ExistingBookingRequestDocumentRequest request, CancellationToken cancellationToken)
        {


            var currentEmployee = await (from employee in Context.Employee.AsNoTracking().Where(x => x.Id == request.EmployeeId)
                                         join employer in Context.Employer.AsNoTracking() on employee.EmployerId equals employer.Id into employerData
                                         from employer in employerData.DefaultIfEmpty()
                                         join department in Context.Department.AsNoTracking() on employee.DepartmentId equals department.Id into departmentData
                                         from department in departmentData.DefaultIfEmpty()
                                         select new
                                         {
                                             Id = employee.Id,
                                             Lastname = employee.Lastname,
                                             Firstname = employee.Firstname,
                                             EmployerName = employer.Description,
                                             DepartmentName = department.Name,
                                             SAPID = employee.SAPID
                                         }).FirstOrDefaultAsync();

            

            var returnDat = new ExistingBookingRequestDocumentResponse();
            if (currentEmployee != null)
            {
                IQueryable<RequestDocument> documentFilter = Context.RequestDocument.AsNoTracking();
                documentFilter = documentFilter.Where(x => x.EmployeeId == request.EmployeeId && x.DocumentType == RequestDocumentType.DeMobilisation
                    || x.DocumentType == RequestDocumentType.ProfileChanges && x.CurrentAction == RequestDocumentAction.Submitted
                );

                var resultPending = from document in Context.RequestDocument.AsNoTracking()
                                    where document.EmployeeId == request.EmployeeId &&
                                    document.CurrentAction != RequestDocumentAction.Cancelled &&
                                    document.CurrentAction != RequestDocumentAction.Completed &&
                                    document.CurrentAction != RequestDocumentAction.Declined


                                    join requestedEmployee in Context.Employee on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                                    from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                                    join documentEmployee in Context.Employee on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                                    from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                                    join updateEmployee in Context.Employee on document.UserIdUpdated equals updateEmployee.Id into updateEmployeeData
                                    from updateEmployee in updateEmployeeData.DefaultIfEmpty()
                                    select new ExistingBookingRequestDocumentPending
                                    {
                                        Id = document.Id,
                                        Description = document.Description,
                                        DocumentType = document.DocumentType,
                                        DocumentTag = document.DocumentTag,
                                        RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname}",
                                        RequestedDate = document.DateCreated,
                                        CurrentStatus = "Pending",
                                        AssignedEmployeeFullName = null
                                    };


                var EmployeeDocumentIds = await Context.RequestDocument.Where(x => x.EmployeeId == request.EmployeeId).Select(x => x.Id).ToArrayAsync();


                IQueryable<RequestNonSiteTravelFlight> documentFlightQuery = Context.RequestNonSiteTravelFlight;
                documentFlightQuery = documentFlightQuery.Where(x => EmployeeDocumentIds.Contains(x.DocumentId));





                var NonSiteRequest = from flight in Context.RequestNonSiteTravelFlight
                                     where documentFlightQuery.Contains(flight)
                                     join departlocation in Context.RequestAirport on flight.DepartLocationId equals departlocation.Id into DepartLocationData
                                     from departlocation in DepartLocationData.DefaultIfEmpty()
                                     join arriveLocation in Context.RequestAirport on flight.ArriveLocationId equals arriveLocation.Id into ArriveLocationData
                                     from arriveLocation in ArriveLocationData.DefaultIfEmpty()
                                     select new ExistingBookingRequestDocumentNonSiteTravel
                                     {
                                         Id = flight.Id,
                                         FavorTime = flight.FavorTime,
                                         ETD = flight.ETD,
                                         ArriveLocationId = flight.ArriveLocationId,
                                         ArriveLocationName = arriveLocation.Description,
                                         DepartLocationId = flight.DepartLocationId,
                                         DepartLocationName = departlocation.Description,
                                         Comment = flight.Comment,
                                         DocumentId = flight.DocumentId,
                                         TravelDate = flight.TravelDate,
                                     };





                var SiteTravelBefore = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date < DateTime.Today
                                              && x.EmployeeId == request.EmployeeId)
                                               join schedule in Context.TransportSchedule on transport.ScheduleId equals schedule.Id into scheduleData
                                               from schedule in scheduleData.DefaultIfEmpty()
                                               select new ExistingBookingRequestDocumentSiteTravel
                                               {
                                                   Id = transport.Id,
                                                   Code = schedule.Code,
                                                   Description = schedule.Description,
                                                   Direction = transport.Direction,
                                                   EventDate = transport.EventDateTime,
                                                   Status = transport.Status
                                               }).OrderByDescending(x=> x.EventDate).Take(2).ToListAsync();



                var SiteTravelAfter = await (from transport in Context.Transport.AsNoTracking().Where(x => x.EventDate.Value.Date >= DateTime.Today
                                              && x.EmployeeId == request.EmployeeId)
                                               join schedule in Context.TransportSchedule on transport.ScheduleId equals schedule.Id into scheduleData
                                               from schedule in scheduleData.DefaultIfEmpty()
                                               select new ExistingBookingRequestDocumentSiteTravel
                                               {
                                                   Id = transport.Id,
                                                   Code = schedule.Code,
                                                   Description = schedule.Description,
                                                   Direction = transport.Direction,
                                                   EventDate = transport.EventDateTime,
                                                   Status = transport.Status
                                               }).OrderBy(x=>x.EventDate).Take(24).ToListAsync();
                if (SiteTravelBefore.Count > 0)
                {
                    SiteTravelAfter.AddRange(SiteTravelBefore);
                }





                returnDat.NonSiteTravel = await NonSiteRequest.AsNoTracking().ToListAsync();
                returnDat.SiteTravel = SiteTravelAfter.OrderBy(x=> x.EventDate).ToList();
               // returnDat.OtherRequest = await OtherRequestResult.AsNoTracking().ToListAsync();
                returnDat.PendingRequest = await resultPending.AsNoTracking().ToListAsync();
                returnDat.SAPID = currentEmployee.SAPID;
                returnDat.FullName = $"{currentEmployee.Firstname} {currentEmployee.Lastname}";
                returnDat.Department = currentEmployee.DepartmentName;
                returnDat.Employer = currentEmployee.EmployerName;
                returnDat.EmployeeId = currentEmployee.Id;


            }



            return returnDat;



        }

        private async Task<List<ExistingBookingRequestDocumentSiteTravel>> GetDateSchedule(int EmployeeId, CancellationToken cancellationToken)
        {

            var transportData = await Context.Transport
                .Where(x => x.EmployeeId == EmployeeId && x.EventDate.Value > DateTime.Now)
                .OrderBy(x => x.EventDate).Take(4).ToListAsync();
            var returnData = new List<ExistingBookingRequestDocumentSiteTravel>();
            foreach (var transport in transportData)
            {
                var newRecord = new ExistingBookingRequestDocumentSiteTravel

                {
                    Id = transport.Id,

                };
            }

            return returnData;
        }


        //GetNonSiteTravelGroup

        public async Task<List<GetNonSiteTravelGroupResponse>> GetNonSiteTravelGroup(GetNonSiteTravelGroupRequest request, CancellationToken cancellationToken)
        {
            var data = await Context.RequestGroupConfig
                .Where(x => x.Document == RequestDocumentType.NonSiteTravel).ToListAsync();
            var returnData = new List<GetNonSiteTravelGroupResponse>();
            foreach (var item in data)
            {
                var currentGroup = await Context.RequestGroup.Where(x => x.Id == item.GroupId).FirstOrDefaultAsync();
                if (currentGroup != null)
                {
                    var newRecord = new GetNonSiteTravelGroupResponse
                    {
                        Id = item.GroupId,
                        Description = currentGroup.Description
                    };

                    returnData.Add(newRecord);

                }
            }

            return returnData;
        
        }


        //   Task<GetDocumentDashboardResponse> GetDocumentDashboard(GetDocumentDashboardRequest request, CancellationToken cancellationToken);

   


        #region InpersonateDocumentList

        public async Task<List<GetDocumentListInpersonateResponse>> GetDocumentListInpersonate(GetDocumentListInpersonateRequest request, CancellationToken cancellationToken)
        {
            IQueryable<RequestDocument> documentFilter = Context.RequestDocument;
            documentFilter = documentFilter.Where(x => x.AssignedEmployeeId == request.inpersonateUserId);

            documentFilter = documentFilter.Where(x => x.CurrentAction != RequestDocumentAction.Cancelled && x.CurrentAction != RequestDocumentAction.Completed);
            var result = await (from document in Context.RequestDocument
                                where documentFilter.Contains(document)
                                join requestedEmployee in Context.Employee on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                                from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                                join documentEmployee in Context.Employee on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                                from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                                join employer in Context.Employer on documentEmployee.EmployerId equals employer.Id into employerData
                                from employer in employerData.DefaultIfEmpty()
                                join updateEmployee in Context.Employee on document.UserIdUpdated equals updateEmployee.Id into updateEmployeeData
                                from updateEmployee in updateEmployeeData.DefaultIfEmpty()
                                select new GetDocumentListInpersonateResponse
                                {
                                    Id = document.Id,
                                    Description = document.Description,
                                    DocumentType = document.DocumentType,
                                    RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname}",
                                    RequestedDate = document.DateCreated,
                                    EmployeeFullName = $"{documentEmployee.Firstname} {documentEmployee.Lastname}",
                                    EmployerName = $"{employer.Code} {employer.Description}",
                                    EmployerId = employer.Id,
                                    UpdatedInfo = $"{updateEmployee.Firstname} {updateEmployee.Lastname} {document.DateUpdated}",
                                    CurrentStatus = document.CurrentAction == "Approved" ? "In Progress " : document.CurrentAction,
                                    AssignedEmployeeFullName = null,
                                    DaysAway = DateTime.Now.Subtract(document.DateCreated.Value).Days,
                                    DocumentTag = document.DocumentTag
                                }).ToListAsync();




            foreach (var item in result)
            {

                // var description = await GetDocumentDescription(item);
                item.AssignedEmployeeFullName =await GetAssignedEmployeeInfo(item.Id);
                item.AssignedGroupName =await GetAssignedGroupInfo(item.Id);
                item.CurrentStatusGroup = $"{item.CurrentStatus} {item.AssignedGroupName}";
            }

            var reData = new List<GetDocumentListInpersonateResponse>();
            foreach (var item in result)
            {

                reData.Add(await GetInpersonateDocumentDescription(item));
            }




            return reData;

        }


        private async Task<GetDocumentListInpersonateResponse> GetInpersonateDocumentDescription(GetDocumentListInpersonateResponse getData)
        {
            string? documentData = getData.DocumentType;

            getData.UpdatedInfo = "";
            var lastUpdateData = await Context.RequestDocumentHistory.Where(x => x.DocumentId == getData.Id).OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
            if (lastUpdateData != null)
            {
                var currentEmployee = await Context.Employee.Where(x => x.Id == lastUpdateData.ActionEmployeeId).Select(x => new { x.Lastname, x.Firstname }).FirstOrDefaultAsync();
                if (currentEmployee != null)
                {
                    getData.UpdatedInfo = $" {currentEmployee.Firstname} {currentEmployee.Lastname} {lastUpdateData.DateCreated}";
                }


            }

            if (getData.DocumentType == RequestDocumentType.SiteTravel)
            {
                if (getData.DocumentTag == "ADD")
                {
                    var currentData = await Context.RequestSiteTravelAdd.Where(x => x.DocumentId == getData.Id).FirstOrDefaultAsync();
                    if (currentData != null)
                    {
                        var currentSchule = await Context.TransportSchedule.Where(x => x.Id == currentData.inScheduleId).FirstOrDefaultAsync();
                        if (currentSchule != null)
                        {
                            var currentActivetransport = await Context.ActiveTransport.Where(x => x.Id == currentSchule.ActiveTransportId).FirstOrDefaultAsync();

                            getData.Description = $"{getData.DocumentType} {getData.DocumentTag} {currentSchule?.EventDate} {currentActivetransport?.Direction}";

                            try
                            {
                                getData.DaysAway = DateTime.Now.Subtract(currentSchule.EventDate).Days * (-1);
                            }
                            catch (Exception)
                            {

                                getData.DaysAway = 0;
                            }

                        }
                        else
                        {
                            getData.Description = $"{getData.DocumentType} {getData.DocumentTag} {currentSchule?.EventDate}";
                        }

                    }
                    else
                    {
                        getData.Description = $"{getData.DocumentType} {getData.DocumentTag}";
                    }

                    return getData;
                }
                if (getData.DocumentTag == "REMOVE")
                {
                    var currentData = await Context.RequestSiteTravelRemove.Where(x => x.DocumentId == getData.Id).FirstOrDefaultAsync();
                    if (currentData != null)
                    {
                        var currentSchule = await Context.TransportSchedule.Where(x => x.Id == currentData.FirstScheduleId).FirstOrDefaultAsync();
                        if (currentSchule != null)
                        {
                            var currentActivetransport = await Context.ActiveTransport.Where(x => x.Id == currentSchule.ActiveTransportId).FirstOrDefaultAsync();

                            getData.Description = $"{getData.DocumentType} {getData.DocumentTag} {currentSchule?.EventDate} {currentActivetransport?.Direction}";

                            try
                            {
                                getData.DaysAway = (DateTime.Now.Subtract(currentSchule.EventDate).Days) * (-1);
                            }
                            catch (Exception)
                            {
                            }

                        }
                        else
                        {
                            getData.Description = $"{getData.DocumentType} {getData.DocumentTag} {currentSchule?.EventDate}";
                        }

                    }
                    else
                    {
                        getData.Description = $"{getData.DocumentType} {getData.DocumentTag}";
                    }

                    return getData;
                }
                if (getData.DocumentTag == "RESCHEDULE")
                {
                    var currentData = await Context.RequestSiteTravelReschedule.Where(x => x.DocumentId == getData.Id).FirstOrDefaultAsync();
                    if (currentData != null)
                    {
                        var currentSchule = await Context.TransportSchedule.Where(x => x.Id == currentData.ReScheduleId).FirstOrDefaultAsync();
                        if (currentSchule != null)
                        {
                            var currentActivetransport = await Context.ActiveTransport.Where(x => x.Id == currentSchule.ActiveTransportId).FirstOrDefaultAsync();

                            getData.Description = $"{getData.DocumentType} {getData.DocumentTag} {currentSchule?.EventDate} {currentActivetransport?.Direction}";

                            try
                            {
                                getData.DaysAway = (DateTime.Now.Subtract(currentSchule.EventDate).Days) * (-1);
                            }
                            catch (Exception)
                            {
                            }

                        }
                        else
                        {
                            getData.Description = $"{getData.DocumentType} {getData.DocumentTag} {currentSchule?.EventDate}";
                        }

                    }
                    else
                    {
                        getData.Description = $"{getData.DocumentType} {getData.DocumentTag}";
                    }

                    return getData;
                }
            }
            if (getData.DocumentType == RequestDocumentType.NonSiteTravel)
            {
                var currentDataAccomodation = await Context.RequestNonSiteTravelAccommodation.Where(x => x.DocumentId == getData.Id).FirstOrDefaultAsync();
                var currentDataFlight = await (
                    from flight in Context.RequestNonSiteTravelFlight.Where(x => x.DocumentId == getData.Id)
                    join departLocation in Context.RequestAirport on flight.DepartLocationId equals departLocation.Id into departLocationData
                    from departLocation in departLocationData.DefaultIfEmpty()
                    join arriveLocation in Context.RequestAirport on flight.ArriveLocationId equals arriveLocation.Id into arriveLocationData
                    from arriveLocation in arriveLocationData.DefaultIfEmpty()
                    select new
                    {
                        Id = flight.Id,
                        ArriveLocationCode = arriveLocation.Code,
                        ArriveLocationDescription = arriveLocation.Description,
                        ArriveLocationCountry = arriveLocation.Country,
                        DepartLocationCode = departLocation.Code,
                        DepartLocationDescription = departLocation.Description,
                        DepartLocationCountry = departLocation.Country,
                        TravelDate = flight.TravelDate
                    }
                ).FirstOrDefaultAsync();


                if (currentDataFlight != null && currentDataAccomodation != null)
                {
                    getData.Description = $"{getData.DocumentType} - Accommodation & Flight - {currentDataFlight.DepartLocationCode} {currentDataFlight.DepartLocationDescription} {currentDataFlight.DepartLocationCountry} to  {currentDataFlight.ArriveLocationCode} {currentDataFlight.ArriveLocationDescription} {currentDataFlight.ArriveLocationCountry} {currentDataAccomodation.Hotel} {currentDataAccomodation.City}";
                    if (currentDataFlight.TravelDate.HasValue)
                    {
                        getData.DaysAway = (DateTime.Now.Subtract(currentDataFlight.TravelDate.Value.Date).Days) * (-1);
                    }
                    else
                    {
                        if (currentDataAccomodation.FirstNight.HasValue)
                        {
                            getData.DaysAway = (DateTime.Now.Subtract(currentDataAccomodation.FirstNight.Value.Date).Days) * (-1);
                        }
                        else
                        {
                            getData.DaysAway = 0;
                        }
                    }



                }

                if (currentDataFlight == null && currentDataAccomodation != null)
                {
                    getData.Description = $"{getData.DocumentType} - Accommodation - {currentDataAccomodation.Hotel} {currentDataAccomodation.City}";
                    if (currentDataAccomodation.FirstNight.HasValue)
                    {
                        getData.DaysAway = (DateTime.Now.Subtract(currentDataAccomodation.FirstNight.Value.Date).Days) * (-1);
                    }


                }
                if (currentDataFlight != null && currentDataAccomodation == null)
                {
                    getData.Description = $"{getData.DocumentType} - Flight - {currentDataFlight.DepartLocationCode} {currentDataFlight.DepartLocationDescription} {currentDataFlight.DepartLocationCountry} to  {currentDataFlight.ArriveLocationCode}" +
                        $" {currentDataFlight.ArriveLocationDescription} {currentDataFlight.ArriveLocationCountry}";
                    if (currentDataFlight.TravelDate.HasValue)
                    {
                        getData.DaysAway = (DateTime.Now.Subtract(currentDataFlight.TravelDate.Value.Date).Days) * (-1);
                    }
                }
                else
                {
                    getData.Description = $"{getData.DocumentType}";
                }


                return getData;
            }

            if (getData.DocumentType == RequestDocumentType.ProfileChanges)
            {
                var currentData = await Context.RequestDocumentProfileChangeEmployee.Where(x => x.DocumentId == getData.Id).FirstOrDefaultAsync();
                if (currentData != null)
                {
                    getData.Description = $"{getData.DocumentType} {currentData.Firstname} {currentData.Lastname}";
                    if (currentData.CommenceDate.HasValue)
                    {
                        getData.DaysAway = (DateTime.Now.Subtract(currentData.CommenceDate.Value.Date).Days) * (-1);
                    }
                }
                else
                {
                    getData.Description = $"{getData.DocumentType}";
                }

                return getData;
            }
            if (getData.DocumentType == RequestDocumentType.DeMobilisation)
            {
                var currentData = await Context.RequestDeMobilisation.Where(x => x.DocumentId == getData.Id).FirstOrDefaultAsync();
                if (currentData != null)
                {
                    var currentDemobType = await Context.RequestDeMobilisationType.Where(x => x.Id == currentData.RequestDeMobilisationTypeId).FirstOrDefaultAsync();
                    getData.Description = $"{getData.DocumentType}  {currentDemobType?.Description}";
                    if (currentData.CompletionDate.HasValue)
                    {
                        getData.DaysAway = (DateTime.Now.Subtract(currentData.CompletionDate.Value.Date).Days) * (-1);
                    }

                }
                else
                {
                    getData.Description = $"{getData.DocumentType}";
                }

                return getData;
            }

            return getData;
        }


        #endregion


        #region CancelledDocumentList 
        public async Task<GetDocumentListCancelledResponse> GetDocumentListCancelled(GetDocumentListCancelledRequest request, CancellationToken cancellationToken)
        {


            int pageSize = request.pageSize == 0 ? 10 : request.pageSize;
            int pageIndex = request.pageIndex;
            IQueryable<RequestDocument> documentFilter = Context.RequestDocument;

            var role = _HTTPUserRepository.LogCurrentUser()?.Role;
            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;

            var NotAllowDocs = new List<string>();
            NotAllowDocs.Add(RequestDocumentAction.Cancelled);
            NotAllowDocs.Add(RequestDocumentAction.Completed);

            documentFilter = documentFilter.Where(x => !NotAllowDocs.Contains(x.CurrentAction));

            if(request.model.startDate.HasValue && request.model.endDate.HasValue)
            {

                var startDate = request.model.startDate.Value.Date;
                var endDate = request.model.endDate.Value.Date;

                endDate = endDate.AddDays(1);
                documentFilter = documentFilter
                    .Where(x => x.DateCreated.Value.Date >= startDate
                    && x.DateCreated.Value.Date <= endDate);
            }

            if (request.model.Id.HasValue)
            {
                documentFilter = documentFilter.Where(x => x.Id == request.model.Id);
            }
            if (!string.IsNullOrWhiteSpace(request.model.DocumentType))
            {
                documentFilter = documentFilter.Where(x => x.DocumentType == request.model.DocumentType);
            }
            if (request.model.RequestedEmployeeId.HasValue)
            {
                documentFilter = documentFilter.Where(x => x.UserIdCreated == request.model.RequestedEmployeeId);
            }
            if (request.model.AssignedEmployeeId.HasValue)
            {
                documentFilter = documentFilter.Where(x => x.AssignedEmployeeId == request.model.AssignedEmployeeId);
            }
            if (request.model.LastModifiedDate.HasValue)
            {
                documentFilter = documentFilter.Where(x => x.DateUpdated.Value.Date == request.model.LastModifiedDate.Value.Date);
            }

            if (request.model.ApprovelType.HasValue)
            {
                var requestGroupConfigIds = await Context.RequestGroupConfig.Where(x => x.GroupId == request.model.ApprovelType.Value).Select(x => x.Id).ToListAsync();
                if (requestGroupConfigIds.Count > 0)
                {
                    documentFilter = documentFilter.Where(x => requestGroupConfigIds.Contains(x.AssignedRouteConfigId.Value));
                }
            }







            documentFilter = documentFilter.Where(x => x.Active == 1);


            var result = from document in Context.RequestDocument.AsNoTracking()
                         where documentFilter.Contains(document)
                         join requestedEmployee in Context.Employee.AsNoTracking() on document.UserIdCreated equals requestedEmployee.Id into requestedEmployeeData
                         from requestedEmployee in requestedEmployeeData.DefaultIfEmpty()
                         join documentEmployee in Context.Employee.AsNoTracking() on document.EmployeeId equals documentEmployee.Id into documentEmployeeData
                         from documentEmployee in documentEmployeeData.DefaultIfEmpty()
                         join employer in Context.Employer.AsNoTracking() on documentEmployee.EmployerId equals employer.Id into employerData
                         from employer in employerData.DefaultIfEmpty()
                         join updateEmployee in Context.Employee.AsNoTracking() on document.UserIdUpdated equals updateEmployee.Id into updateEmployeeData
                         from updateEmployee in updateEmployeeData.DefaultIfEmpty()
                         select new GetDocumentListCancelled
                         {
                             Id = document.Id,
                             Description = document.Description,
                             DocumentType = document.DocumentType,
                             RequesterFullName = $"{requestedEmployee.Firstname} {requestedEmployee.Lastname}",
                             RequestedDate = document.DateCreated,
                             EmployeeFullName = $"{documentEmployee.Firstname} {documentEmployee.Lastname}",
                             EmployerName = $"{employer.Description}",
                             EmployerId = employer.Id,
                             UpdatedInfo = document.UpdatedInfo,
                             CurrentStatus = document.CurrentAction == "Approved" ? "Pending" : document.CurrentAction,
                             AssignedEmployeeFullName = null,
                             AssignedEmployeeId = document.AssignedEmployeeId,
                             AssignedGroupName = null,
                             DaysAway = document.DaysAwayDate.HasValue ? (DateTime.Now.Subtract(document.DaysAwayDate.Value).Days) * (-1) : (DateTime.Now.Subtract(document.DateCreated.Value).Days) * (-1),
                             DocumentTag = document.DocumentTag,
                             
                         };



            var retData = await result.ToListAsync();


            if (request.model.EmployerId.HasValue)
            {
                retData = retData.Where(x => x.EmployerId == request.model.EmployerId).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.model.Keyword))
            {
                retData = retData.Where(x => x.EmployeeFullName.Contains(request.model.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            retData = retData.OrderBy(x => x.DaysAway).ToList();

            foreach (var item in retData)
            {
                item.AssignedEmployeeFullName = await GetAssignedEmployeeInfo(item.Id);
              //  var description = await GetCancelledDocumentDescription(item);
                item.AssignedEmployeeFullName =await GetAssignedEmployeeInfo(item.Id);
                item.AssignedGroupName = GetAssignedGroupInfo(item.Id).Result;
            }




            var returnData = new GetDocumentListCancelledResponse
            {
                data = retData
                .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToList<GetDocumentListCancelled>(),
                pageSize = pageSize,
                currentPage = pageIndex,
                totalcount = retData.Count()
            };

            return returnData;


        }



        #endregion


        #region RequestMyInfo

        public async Task<GetRequestDocumentMyInfoResponse> GetRequestDocumentMyInfo(GetRequestDocumentMyInfoRequest request, CancellationToken cancellationToken)
        {
            var userId = _HTTPUserRepository.LogCurrentUser()?.Id;

            var guestRole = await Context.SysRole.AsNoTracking().Where(x => x.RoleTag == "Guest").FirstOrDefaultAsync();
            if (guestRole == null)
            {
                throw new BadRequestException("The duty register is missing.");
            }
            
            var result = await (from employee in Context.Employee.AsNoTracking().Where(x => x.Id == userId)
                                join role in Context.SysRoleEmployees on employee.Id equals role.EmployeeId into roleData
                                from role in roleData.DefaultIfEmpty()
                                
                                join rolemaster in Context.SysRole on role.RoleId equals rolemaster.Id into rolemasterData
                                from rolemaster in rolemasterData.DefaultIfEmpty()
                                

                                join requestgroupemployee in Context.RequestGroupEmployee on employee.Id equals requestgroupemployee.EmployeeId into requestgroupemployeeData
                                from requestgroupemployee in requestgroupemployeeData.DefaultIfEmpty()   
                                join requestgroup in Context.RequestGroup on requestgroupemployee.RequestGroupId equals requestgroup.Id into requestgroupData
                                from requestgroup in requestgroupData.DefaultIfEmpty()
                                select new GetRequestDocumentMyInfoResponse
                                {
                                    EmployeeId = employee.Id,
                                    Fullname = $"{employee.Firstname} {employee.Lastname}",
                                    ApprovalGroupId = requestgroup.Id,
                                    RoleId = role != null ? role.Id : guestRole.Id,
                                    Rolename = role != null ? rolemaster.Name : guestRole.Name,
                                    ApprovalGroupName =requestgroup.Description


                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                throw new ForBiddenException(" You do not have the required permissions to access this employee's information.");
            }

            var groupIds = await  Context.RequestGroupEmployee.AsNoTracking().Where(x => x.EmployeeId == userId).Select(x => x.RequestGroupId).ToListAsync();

            var delegatesEmployeeIds = await Context.RequestDelegates.Where(x => x.ToEmployeeId == userId
                && x.EndDate.Date >= DateTime.Today && x.StartDate.Date <= DateTime.Today
            ).Select(x => x.FromEmployeeId).ToListAsync();


            if (groupIds.Count > 0)
            {
              var Ids = await  Context.RequestGroupConfig.Where(x => groupIds.Contains(x.GroupId)).Select(x=> x.Id).ToListAsync();
                result.ApprovalGroupIds = Ids;
            }

            result.GroupIds = groupIds;
            result.LineManagerIds = delegatesEmployeeIds;
            return result;
        }

        #endregion



        #region CheckDemobRequest 

        public async Task CheckDemobRequest(CheckDemobRequestRequest request, CancellationToken cancellationToken)
        {
            var documentActions = new List<string?>() {RequestDocumentAction.Cancelled, RequestDocumentAction.Completed };

            var demobPendingDocument =await Context.RequestDocument.AsNoTracking().Where(c => c.EmployeeId == request.EmployeeId && !documentActions.Contains(c.CurrentAction) && c.DocumentType == RequestDocumentType.DeMobilisation).FirstOrDefaultAsync();
            if (demobPendingDocument != null)
            {
                throw new BadRequestException($"Employee has a pending Demobilization request ({demobPendingDocument.Id}) from {demobPendingDocument.DateCreated.Value.ToString("yyyy-MM-dd")}. Cannot create new request.");
            }

        }

        #endregion

    }
}
