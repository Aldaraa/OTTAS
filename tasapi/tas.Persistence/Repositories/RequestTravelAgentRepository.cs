using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.GetAllRequestTravelAgent;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestTravelAgentRepository : BaseRepository<RequestTravelAgent>, IRequestTravelAgentRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestTravelAgentRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }


        public async Task<List<GetAllRequestTravelAgentResponse>> GetAllData(GetAllRequestTravelAgentRequest request, CancellationToken cancellationToken)
        {
            var Data = new List<RequestTravelAgent>();
            if (request.status.HasValue)
            {
                Data = await Context.RequestTravelAgent.Where(x => x.Active == request.status).ToListAsync(cancellationToken);

            }
            else
            {
                Data = await Context.RequestTravelAgent.ToListAsync(cancellationToken);

            }

            var returnData = new List<GetAllRequestTravelAgentResponse>();
            foreach (var item in Data)
            {


                var newData = new GetAllRequestTravelAgentResponse
                {
                    Id = item.Id,
                    Active = item.Active,
                    Description = item.Description,
                    DateCreated = item.DateCreated,
                    DateUpdated = item.DateUpdated,
                };
                returnData.Add(newData);
            }

            return returnData.OrderByDescending(x => x.DateCreated).ToList(); ;

        }
    }



}
