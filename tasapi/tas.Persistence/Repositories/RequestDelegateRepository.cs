using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDelegateFeature.AllRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.CreateRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.DeleteRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.UpdateRequestDelegate;
using tas.Application.Features.RequestDocumentFeature.GetDocumentList;
using tas.Application.Repositories;
using tas.Domain.Common;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public sealed class RequestDelegateRepository : BaseRepository<RequestDelegates>, IRequestDelegateRepository
    {
        //Task CreateData(CreateRequestDelegateRequest request, CancellationToken cancellationToken);

        //Task UpdateData(UpdateRequestDelegateRequest request, CancellationToken cancellationToken);

        //Task<List<AllRequestDelegateResponse>> AllData(AllRequestDelegateRequest request, CancellationToken cancellationToken);

        private readonly HTTPUserRepository _hTTPUserRepository;


        public RequestDelegateRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _hTTPUserRepository = hTTPUserRepository;
        }


        public async Task<List<AllRequestDelegateResponse>> AllData(AllRequestDelegateRequest request, CancellationToken cancellationToken)
        {

            if (_hTTPUserRepository.LogCurrentUser()?.Role == "SystemAdmin")
            {

                IQueryable<RequestDelegates> delegateFilter = Context.RequestDelegates;

                if (request.fromEmpoyeeId.HasValue)
                {
                    delegateFilter = delegateFilter.Where(x => x.FromEmployeeId == request.fromEmpoyeeId);
                }

                if (request.startDate.HasValue && request.endDate.HasValue)
                {
                    delegateFilter = delegateFilter.Where(x => x.EndDate >= request.startDate && x.EndDate <= request.endDate);
                }


                var result = from reqdelegate in Context.RequestDelegates
                             where delegateFilter.Contains(reqdelegate)
                             join toEmployee in Context.Employee on reqdelegate.ToEmployeeId equals toEmployee.Id into toEmployeeData
                             from toEmployee in toEmployeeData.DefaultIfEmpty()
                             join fromEmployee in Context.Employee on reqdelegate.FromEmployeeId equals fromEmployee.Id into fromEmployeeData
                             from fromEmployee in fromEmployeeData.DefaultIfEmpty()
                             select new AllRequestDelegateResponse
                             {
                                 Id = reqdelegate.Id,
                                 fromEmployeeId = reqdelegate.FromEmployeeId,
                                 fromEmployeeFullname = $"{fromEmployee.Firstname} {fromEmployee.Lastname}",
                                 toEmployeeFullname = $"{toEmployee.Firstname} {toEmployee.Lastname}",
                                 toEmployeeId = reqdelegate.ToEmployeeId,
                                 StartDate = reqdelegate.StartDate,
                                 EndDate = reqdelegate.EndDate,
                             };
                return await result.ToListAsync();
            }
            else
            {

                IQueryable<RequestDelegates> delegateFilter = Context.RequestDelegates;

                if (request.fromEmpoyeeId.HasValue)
                {
                    delegateFilter = delegateFilter.Where(x => x.FromEmployeeId == request.fromEmpoyeeId);
                }

                if (request.startDate.HasValue && request.endDate.HasValue)
                {
                    delegateFilter = delegateFilter.Where(x => x.EndDate >= request.startDate && x.EndDate <= request.endDate);
                }

                var userId = _hTTPUserRepository.LogCurrentUser()?.Id;
                var result = from reqdelegate in Context.RequestDelegates
                             where delegateFilter.Contains(reqdelegate)
                             join toEmployee in Context.Employee on reqdelegate.ToEmployeeId equals toEmployee.Id into toEmployeeData
                             from toEmployee in toEmployeeData.DefaultIfEmpty()
                             join fromEmployee in Context.Employee on reqdelegate.FromEmployeeId equals fromEmployee.Id into fromEmployeeData
                             from fromEmployee in fromEmployeeData.DefaultIfEmpty()
                             select new AllRequestDelegateResponse
                             {
                                 Id = reqdelegate.Id,
                                 fromEmployeeId = reqdelegate.FromEmployeeId,
                                 fromEmployeeFullname = $"{fromEmployee.Firstname} {fromEmployee.Lastname}",
                                 toEmployeeFullname = $"{toEmployee.Firstname} {toEmployee.Lastname}",
                                 toEmployeeId = reqdelegate.ToEmployeeId,
                                 StartDate = reqdelegate.StartDate,
                                 EndDate = reqdelegate.EndDate,
                             };
                return await result.Where(x=> x.fromEmployeeId == userId.Value || x.toEmployeeId == userId.Value ).ToListAsync();
            }




        }

        public async Task UpdateData(UpdateRequestDelegateRequest request, CancellationToken cancellationToken)
        {
            var currentData =await Context.RequestDelegates.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null) {
                currentData.FromEmployeeId = request.fromEmployeeId;
                currentData.ToEmployeeId = request.toEmployeeId;
                currentData.DateUpdated = DateTime.Now;
                currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                currentData.Active = 1;
                currentData.StartDate = request.startDate;
                currentData.EndDate = request.endDate;

                Context.RequestDelegates.Update(currentData);
            }

          await Task.CompletedTask;
            

        }

        public async Task CreateData(CreateRequestDelegateRequest request, CancellationToken cancellationToken)
        {
            var newData = new RequestDelegates
            {
                FromEmployeeId = request.fromEmployeeId,
                ToEmployeeId = request.toEmployeeId,
                DateCreated = DateTime.Now,
                UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id,
                Active = 1,
                StartDate = request.startDate,
                EndDate = request.endDate
            };

            Context.RequestDelegates.Add(newData);

            await Task.CompletedTask;
        }

        public async Task DeleteData(DeleteRequestDelegateRequest request, CancellationToken cancellationToken)
        {
            var currentData = await Context.RequestDelegates.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (currentData != null)
            {
                Context.RequestDelegates.Remove(currentData);
            }

            await Task.CompletedTask;


        }
    }
}
