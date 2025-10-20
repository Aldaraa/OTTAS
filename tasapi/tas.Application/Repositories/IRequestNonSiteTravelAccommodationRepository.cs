using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.CreateRequestNonSiteTravelAccommodation;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.DeleteRequestNonSiteTravelAccommodation;
using tas.Application.Features.RequestNonSiteTravelAccommodationFeature.UpdateRequestNonSiteTravelAccommodation;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestNonSiteTravelAccommodationRepository :IBaseRepository<RequestNonSiteTravelAccommodation>
    {
        Task CreateRequestNonSiteTravelAccommodation(CreateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken);
        Task<int> UpdateRequestNonSiteTravelAccommodation(UpdateRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken);

        Task<int> DeleteRequestNonSiteTravelAccommodation(DeleteRequestNonSiteTravelAccommodationRequest request, CancellationToken cancellationToken);


    }
}
