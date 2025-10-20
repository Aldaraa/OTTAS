using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.BusstopFeature.UpdateBusstop;
using tas.Application.Features.TransportScheduleFeature.BusstopTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.CreateScheduleActiveTransport;
using tas.Application.Features.TransportScheduleFeature.CreateScheduleDriveTransport;
using tas.Application.Features.TransportScheduleFeature.GetDateDriveTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.GetMonthTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.GetScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.ManageTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.RemoveScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.SearchTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.SeatInfoTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleExport;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo;
using tas.Application.Features.TransportScheduleFeature.UpdateDescription;
using tas.Application.Features.TransportScheduleFeature.UpdateScheduleBusstop;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportSchedule;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETD;
using tas.Application.Features.TransportScheduleFeature.UpdateTransportScheduleRealETDByDate;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{
    public interface ITransportScheduleRepository : IBaseRepository<TransportSchedule>
    {
        Task<List<SearchTransportScheduleResponse>> Search(SearchTransportScheduleRequest request, CancellationToken cancellationToken);


        Task<ManageTransportScheduleResponse> ManageTransportSchedule(ManageTransportScheduleRequest request, CancellationToken cancellationToken);

        Task<GetDateDriveTransportScheduleResponse> GetDateDriveTransportSchedule(GetDateDriveTransportScheduleRequest request, CancellationToken cancellationToken);

        Task<BusstopTransportScheduleResponse> BusstopTransportSchedule(BusstopTransportScheduleRequest request, CancellationToken cancellationToken);



        Task ChangeSchedule(UpdateTransportScheduleRequest request, CancellationToken cancellationToken);


        Task<List<GetMonthTransportScheduleResponse>> GetMonthTransportSchedule(GetMonthTransportScheduleRequest request, CancellationToken cancellationToken);

        Task CreateSchedule(CreateScheduleActiveTransportRequest request);
        Task CreateScheduleDrive(CreateScheduleDriveTransportRequest request);

        Task<TransportScheduleInfoResponse> GetTransportScheduleInfo(TransportScheduleInfoRequest request, CancellationToken cancellationToken);


        Task UpdateScheduleDescription(UpdateDescriptionRequest request, CancellationToken cancellationToken);

        Task UpdateTransportScheduleRealETD(UpdateTransportScheduleRealETDRequest request, CancellationToken cancellationToken);

        Task  UpdateTransportScheduleRealETDByDate(UpdateTransportScheduleRealETDByDateRequest request, CancellationToken cancellationToken);

        Task<SeatInfoTransportScheduleResponse> SeatInfoTransportSchedule(SeatInfoTransportScheduleRequest request, CancellationToken cancellationToken);

        Task<TransportScheduleExportResponse> TransportScheduleExport(TransportScheduleExportRequest request, CancellationToken cancellationToken);

        Task UpdateScheduleScheduleBusstop(UpdateScheduleBusstopRequest request, CancellationToken cancellationToken);

        Task<List<GetScheduleBusstopResponse>> GetScheduleBusstop(GetScheduleBusstopRequest request, CancellationToken cancellationToken);

        Task RemoveScheduleScheduleBusstop(RemoveScheduleBusstopRequest request, CancellationToken cancellationToken);





    }
}
