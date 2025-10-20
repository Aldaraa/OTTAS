using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestNonSiteTicketConfigFeature.ExtractOptioRequestNonSiteTicket;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IRequestNonSiteTicketConfigRepository : IBaseRepository<RequestNonSiteTicketConfig>
    {
        Task<ExtractOptioRequestNonSiteTicketResponse> ExtractOption(ExtractOptioRequestNonSiteTicketRequest request, CancellationToken cancellationToken);


    }
}
