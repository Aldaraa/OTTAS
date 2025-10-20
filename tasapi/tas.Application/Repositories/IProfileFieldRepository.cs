using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ProfileFieldFeature.GetAllProfileField;
using tas.Application.Features.ProfileFieldFeature.UpdateProfileField;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IProfileFieldRepository : IBaseRepository<ProfileField>
    {

        Task<List<GetAllProfileFieldResponse>> GetAllData(GetAllProfileFieldRequest request, CancellationToken cancellationToken);

        Task UpdateProfileField(UpdateProfileFieldRequest request, CancellationToken cancellationToken);

    }
}
