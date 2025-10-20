using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ShiftFeature.GetAllShift;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface IShiftRepository : IBaseRepository<Shift>
    {
        Task<Shift> GetbyId(int Id, CancellationToken cancellationToken);

        Task<List<GetAllShiftResponse>> GetAllShift(GetAllShiftRequest request, CancellationToken cancellationToken);


    }
}
