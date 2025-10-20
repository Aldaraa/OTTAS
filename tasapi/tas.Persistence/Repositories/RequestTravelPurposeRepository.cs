using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestTravelAgentFeature.GetAllRequestTravelAgent;
using tas.Application.Features.RequestTravelPurposeFeature.GetAllRequestTravelPurpose;
using tas.Application.Repositories;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public  class RequestTravelPurposeRepository : BaseRepository<RequestTravelPurpose>, IRequestTravelPurposeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _HTTPUserRepository;
        public RequestTravelPurposeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _HTTPUserRepository = hTTPUserRepository;
        }

        public async Task<List<GetAllRequestTravelPurposeResponse>> GetAllData(GetAllRequestTravelPurposeRequest request, CancellationToken cancellationToken)
        {
            var Data = new List<RequestTravelPurpose>();
            if (request.status.HasValue)
            {
                Data = await Context.RequestTravelPurpose.Where(x => x.Active == request.status).ToListAsync(cancellationToken);

            }
            else
            {
                Data = await Context.RequestTravelPurpose.ToListAsync(cancellationToken);

            }

            var returnData = new List<GetAllRequestTravelPurposeResponse>();
            foreach (var item in Data)
            {


                var newData = new GetAllRequestTravelPurposeResponse
                {
                    Id = item.Id,
                    Active = item.Active,
                    Code = item.Code,
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
