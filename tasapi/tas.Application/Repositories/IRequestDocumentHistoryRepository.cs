using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDocumentHistoryFeature.GetRequestDocumentHistory;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface IRequestDocumentHistoryRepository : IBaseRepository<RequestDocumentHistory>
    {
        Task<List<GetRequestDocumentHistoryResponse>> GetRequestDocumentHistories(GetRequestDocumentHistoryRequest request, CancellationToken cancellationToken);
    }
}
