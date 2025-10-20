using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.CreateRequestNonSiteTravelFlight;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.DeleteRequestNonSiteTravelFlight;
using tas.Application.Features.RequestNonSiteTravelFlightFeature.UpdateRequestNonSiteTravelFlight;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface  IRequestNonSiteTravelFlightRepository : IBaseRepository<RequestNonSiteTravelFlight>
    {
        Task CreateRequestNonSiteTravelFlight(CreateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken);

        Task UpdateRequestNonSiteTravelFlight(UpdateRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken);

        Task DeleteRequestNonSiteTravelFlight(DeleteRequestNonSiteTravelFlightRequest request, CancellationToken cancellationToken);



    }
}
