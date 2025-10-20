using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestNonSiteTravelOptionDataFeature.UpdateRequestNonSiteTravelOptionData;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.CreateRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.DeleteRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.GetRequestNonSiteTravelOptionFinal;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateItineraryOption;
using tas.Application.Features.RequestNonSiteTravelOptionFeature.UpdateRequestNonSiteTravelOption;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestNonSiteTravelOptionRepository : IBaseRepository<RequestNonSiteTravelOption>
    {
        Task CreateOptionData(CreateRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken);
        Task UpdateOptionData(UpdateRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken);

        Task DeleteOptionData(DeleteRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken);


        Task UpdateOptionFullData(UpdateRequestNonSiteTravelOptionDataRequest request, CancellationToken cancellationToken);

        Task<List<GetRequestNonSiteTravelOptionResponse>> GetData(GetRequestNonSiteTravelOptionRequest request, CancellationToken cancellationToken);
        
        
        Task<List<GetRequestNonSiteTravelOptionFinalResponse>> GetFinalOptionData(GetRequestNonSiteTravelOptionFinalRequest request, CancellationToken cancellationToken);

        

        Task UpdateItinerary(UpdateItineraryOptionRequest request, CancellationToken cancellationToken);



    }
}
