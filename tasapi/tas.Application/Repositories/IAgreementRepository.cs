using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.AgreementFeature.CreateAgreement;
using tas.Application.Features.AgreementFeature.GetAgreement;
using tas.Application.Features.BedFeature.GetAllBed;
using tas.Application.Features.BedFeature.GetBed;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IAgreementRepository : IBaseRepository<Agreement>
    {
        Task CreateData(CreateAgreementRequest request, CancellationToken cancellationToken);

        Task<GetAgreementResponse> GetData(GetAgreementRequest request, CancellationToken cancellationToken);


    }

}
