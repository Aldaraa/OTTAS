using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDeMobilisationTypeFeature.GetAllRequestDeMobilisationType;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestDeMobilisationTypeRepository : IBaseRepository<RequestDeMobilisationType>
    {
        Task<List<GetAllRequestDeMobilisationTypeResponse>> GetAllData(GetAllRequestDeMobilisationTypeRequest request, CancellationToken cancellationToken);
    }
}
