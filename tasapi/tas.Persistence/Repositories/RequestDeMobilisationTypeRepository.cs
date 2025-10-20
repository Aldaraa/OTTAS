using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Application.Features.RequestDeMobilisationTypeFeature.GetAllRequestDeMobilisationType;
using tas.Application.Repositories;
using tas.Application.Utils;
using tas.Domain.Entities;
using tas.Persistence.Context;

namespace tas.Persistence.Repositories
{
    public class RequestDeMobilisationTypeRepository : BaseRepository<RequestDeMobilisationType>, IRequestDeMobilisationTypeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HTTPUserRepository _hTTPUserRepository;
        public RequestDeMobilisationTypeRepository(DataContext context, IConfiguration configuration, HTTPUserRepository hTTPUserRepository) : base(context, configuration, hTTPUserRepository)
        {
            _configuration = configuration;
            _hTTPUserRepository = hTTPUserRepository;

        }


        public async Task<List<GetAllRequestDeMobilisationTypeResponse>> GetAllData(GetAllRequestDeMobilisationTypeRequest request, CancellationToken cancellationToken)
        {
            var Data = new List<RequestDeMobilisationType>();
            if (request.status.HasValue)
            {
                Data = await Context.RequestDeMobilisationType.Where(x => x.Active == request.status).ToListAsync(cancellationToken);

            }
            else
            {
                Data = await Context.RequestDeMobilisationType.ToListAsync(cancellationToken);
            }

            var returnData = new List<GetAllRequestDeMobilisationTypeResponse>();
            foreach (var item in Data)
            {


                var newData = new GetAllRequestDeMobilisationTypeResponse
                {
                    Id = item.Id,
                    Active = item.Active,
                    Code = item.Code,
                    DateCreated = item.DateCreated,
                    DateUpdated = item.DateUpdated,
                    Description = item.Description
                };
                returnData.Add(newData);
            }

            return returnData.OrderByDescending(x => x.DateCreated).ToList(); ;

        }

    }
}
