using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestAirportFeature.CreateRequestAirport;
using tas.Application.Features.RequestAirportFeature.GetAllRequestAirport;
using tas.Application.Features.RequestAirportFeature.SearchRequestAirport;
using tas.Application.Features.RequestAirportFeature.UpdateRequestAirport;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestAirportRepository : IBaseRepository<RequestAirport>
    {
        Task<List<GetAllRequestAirportResponse>> GetAll(GetAllRequestAirportRequest request, CancellationToken cancellationToken);
        Task<List<SearchRequestAirportResponse>> SearchData(SearchRequestAirportRequest request, CancellationToken cancellationToken);

        Task CreateAirport(CreateRequestAirportRequest request, CancellationToken cancellationToken);

        Task UpdateAirport(UpdateRequestAirportRequest request, CancellationToken cancellationToken);
    }
}
