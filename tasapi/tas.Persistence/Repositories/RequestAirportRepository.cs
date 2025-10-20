using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Common.Exceptions;
using tas.Application.Features.RequestAirportFeature.CreateRequestAirport;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Features.RequestAirportFeature.SearchRequestAirport;
using tas.Application.Features.RequestAirportFeature.UpdateRequestAirport;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{

    public class RequestAirportRepository : BaseRepository<RequestAirport>, IRequestAirportRepository
    {
        private readonly HTTPUserRepository _hTTPUserRepository;
       

        public RequestAirportRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _hTTPUserRepository = hTTPUserRepository;
        }
        
        public async Task CreateAirport(CreateRequestAirportRequest request, CancellationToken cancellationToken)
        {
           var oldata = await Context.RequestAirport.AsNoTracking().Where(x => x.Code == request.Code).FirstOrDefaultAsync();
            if (oldata == null)
            {
                var neRecord = new RequestAirport
                {
                    Code = request.Code,
                    Active = 1,
                    DateCreated = DateTime.Now,
                    Description = request.Description,
                    OrderIndex = 0,
                    Country = request.Country,
                    DateUpdated = DateTime.Now,
                    UserIdCreated = _hTTPUserRepository.LogCurrentUser()?.Id
                };
                Context.RequestAirport.Add(neRecord);
            }
            else {
                throw new BadRequestException("AIRPORT code has already been registered.");
            }
        }


        public async Task UpdateAirport(UpdateRequestAirportRequest request, CancellationToken cancellationToken)
        {
            var oldData = await Context.RequestAirport.AsNoTracking()
                .Where(x => x.Code == request.Code && x.Id != request.Id).FirstOrDefaultAsync(cancellationToken);
            if (oldData == null)
            {
                var currentData = await Context.RequestAirport
                .Where(x=> x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                if (currentData != null)
                {
                    currentData.DateUpdated = DateTime.Now;
                    currentData.Country = request.Country;
                    currentData.Description = request.Description;
                    currentData.Code = request.Code;
                    currentData.UserIdUpdated = _hTTPUserRepository.LogCurrentUser()?.Id;
                    Context.RequestAirport.Update(currentData);
                }
                else {
                    throw new BadRequestException("AIRPORT is not found.");
                }
            }
            else {
                throw new BadRequestException("AIRPORT code has already been registered.");
            }
        }


        public async Task<List<GetAllRequestAirportResponse>> GetAll(GetAllRequestAirportRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<GetAllRequestAirportResponse>();
            if (request.status.HasValue)
            {
                returnData = await Context.RequestAirport.AsNoTracking().Where(x => x.Active == request.status).Select(x => new GetAllRequestAirportResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    Country = x.Country,
                    OrderIndex = x.OrderIndex.Value,
                    Active = x.Active,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated

                }).OrderByDescending(x=> x.DateUpdated).ToListAsync(cancellationToken);
            }
            else {
                returnData = await Context.RequestAirport.AsNoTracking().Select(x => new GetAllRequestAirportResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    Country = x.Country,
                    OrderIndex = x.OrderIndex.Value,
                    Active = x.Active,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated

                }).OrderByDescending(x=> x.DateUpdated).ToListAsync(cancellationToken);
            }


            return returnData;
        }


        public async Task<List<SearchRequestAirportResponse>> SearchData(SearchRequestAirportRequest request, CancellationToken cancellationToken)
        {
            var returnData = new List<SearchRequestAirportResponse>();


            var query = Context.RequestAirport.AsNoTracking().AsQueryable();

            // Filter by keyword if provided
            if (!string.IsNullOrWhiteSpace(request.keyword))
            {
                string keyword = request.keyword.Trim().ToLower();
                query = query.Where(e => e.Code.Contains(keyword)
                                     || e.Country.Contains(keyword)
                                     || e.Description.Contains(keyword));
            }

            var retData =await query.ToListAsync(cancellationToken);
                returnData = retData.Select(x => new SearchRequestAirportResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    Country = x.Country,
                    OrderIndex = x.OrderIndex,
                    Active = x.Active,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated

                }).ToList();
           
          

            return returnData;
        }

    }

}
