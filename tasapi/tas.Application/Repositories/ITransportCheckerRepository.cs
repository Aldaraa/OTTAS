using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.TransportFeature.GetEmployeeTransport;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ITransportCheckerRepository : IBaseRepository<Transport>
    {
        Task TransportAddValidDirectionSequenceCheck(int EmployeeId, int INScheduleId, int OutScheduleId);
        Task TransportUpdateValidDirectionSequenceCheck(int oldTransportId, int ScheduleId);
        Task TransportRescheduleValidDirectionSequenceCheck(int oldTransportId, int ScheduleId);
        Task<bool> TransportRescheduleValidDirectionSequenceCheckStatus(int oldTransportId, int ScheduleId);

        Task TransportUpdateValidDirectionSequenceCheckForRequest(int oldSchduleId, int ScheduleId, int EmployeeId);

        Task TransportExternalAddValidCheck(int EmployeeId, int firstScheduleId, int? lastScheduleId);


        Task TransportExternalRescheduleValidCheck(int oldTransportId, int ScheduleId);



    }
}
