using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.DashboardRequestFeature.GetDocumentDashboard;

namespace tas.Application.Repositories
{

    public interface IDashboardRequestRepository
    {
        Task<GetDocumentDashboardResponse> GetDocumentDashboard(GetDocumentDashboardRequest request, CancellationToken cancellationToken);
    }

}