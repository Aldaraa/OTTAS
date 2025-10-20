using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.RequestDelegateFeature.AllRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.CreateRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.DeleteRequestDelegate;
using tas.Application.Features.RequestDelegateFeature.UpdateRequestDelegate;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
        public interface IRequestDelegateRepository : IBaseRepository<RequestDelegates>
        {
            Task CreateData(CreateRequestDelegateRequest request, CancellationToken cancellationToken);

            Task UpdateData(UpdateRequestDelegateRequest request, CancellationToken cancellationToken);

             Task DeleteData(DeleteRequestDelegateRequest request, CancellationToken cancellationToken);

             Task<List<AllRequestDelegateResponse>> AllData(AllRequestDelegateRequest request, CancellationToken cancellationToken);




    }


}
