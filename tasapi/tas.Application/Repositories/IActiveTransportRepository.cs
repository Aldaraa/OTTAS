using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.ActiveTransportFeature.CreateSpecialActiveTransport;
using tas.Application.Features.ActiveTransportFeature.DeleteActiveTransport;
using tas.Application.Features.ActiveTransportFeature.ExtendActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetAllActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetCalendarActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetDateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.GetExtendActiveTransport;
using tas.Application.Features.ActiveTransportFeature.ScheduleListActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateAircraftCodeActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateBusstopActiveTransport;
using tas.Application.Features.ActiveTransportFeature.UpdateDescrActiveTransport;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{ 

    public interface IActiveTransportRepository : IBaseRepository<ActiveTransport>
    {
        //GetAllData//
        Task<GetAllActiveTransportResponse> GetAllData(GetAllActiveTransportRequest request, CancellationToken cancellationToken);

        Task<ScheduleListActiveTransportResponse> ScheduleList(ScheduleListActiveTransportRequest request, CancellationToken cancellationToken);

        Task CreateSpecial(CreateSpecialActiveTransportRequest request);

        Task<List<GetDateActiveTransportResponse>> GetDateData(GetDateActiveTransportRequest request, CancellationToken cancellationToken);

        Task<List<GetCalendarActiveTransportResponse>> GetCalendarData(GetCalendarActiveTransportRequest request, CancellationToken cancellationToken);

        Task DeleteTransportValidationDB(int ActiveTransportId, CancellationToken cancellationToken);

        Task DeActive(DeleteActiveTransportRequest request, CancellationToken cancellationToken);

        Task ChangeTransport(UpdateActiveTransportRequest request, CancellationToken cancellationToken);

        Task ChangeTransportDescription(UpdateDescrActiveTransportRequest request, CancellationToken cancellationToken);

        Task ChangeTransportAircraftCode(UpdateAircraftCodeActiveTransportRequest request, CancellationToken cancellationToken);



        Task<GetExtendActiveTransportResponse> GetExtendActiveTransport(GetExtendActiveTransportRequest request, CancellationToken cancellationToken);
        Task Extend(ExtendActiveTransportRequest request, CancellationToken cancellationToken);


        Task UpdateBusstopActiveTransport(UpdateBusstopActiveTransportRequest request, CancellationToken cancellationToken);





    }
}
