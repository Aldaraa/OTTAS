using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportScheduleFeature.SearchTransportSchedule;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ITransportScheduleCalculateRepository : IBaseRepository<TransportSchedule>
    {

        Task CalculateByScheduleId(int ScheduleId, CancellationToken cancellationToken);


    }

}
