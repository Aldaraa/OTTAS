using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tas.Application.Features.MultipleBookingFeature.MultipleBookingAddTransport;
using tas.Application.Features.MultipleBookingFeature.MultipleBookingPreviewTransport;
using tas.Application.Features.TransportFeature.AddExternalTravel;
using tas.Application.Features.TransportFeature.AddTravelTransport;
using tas.Application.Features.TransportFeature.CheckDataRequest;
using tas.Application.Features.TransportFeature.CreateNoGoShow;
using tas.Application.Features.TransportFeature.DeleteNoGoShow;
using tas.Application.Features.TransportFeature.DeleteScheduleTransport;
using tas.Application.Features.TransportFeature.EmployeeDateTransport;
using tas.Application.Features.TransportFeature.EmployeeExistingTransport;
using tas.Application.Features.TransportFeature.EmployeeTransportGoShow;
using tas.Application.Features.TransportFeature.EmployeeTransportNoShow;
using tas.Application.Features.TransportFeature.GetDataRequest;
using tas.Application.Features.TransportFeature.GetEmployeeTransport;
using tas.Application.Features.TransportFeature.GetEmployeeTransportAll;
using tas.Application.Features.TransportFeature.GetScheduleDetailTransport;
using tas.Application.Features.TransportFeature.RemoveExternalTransport;
using tas.Application.Features.TransportFeature.RemoveTransport;
using tas.Application.Features.TransportFeature.ReScheduleExternalTransport;
using tas.Application.Features.TransportFeature.ReScheduleMultiple;
using tas.Application.Features.TransportFeature.ReScheduleUpdate;
using tas.Application.Features.TransportFeature.SearchReSchedulePeople;
using tas.Application.Features.TransportFeature.TransportBookingInfo;
using tas.Application.Features.TransportScheduleFeature.TransportScheduleInfo;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface ITransportRepository : IBaseRepository<Transport>
    {

        Task<List<GetEmployeeTransportResponse>> EmployeeTransportSchedule(GetEmployeeTransportRequest request, CancellationToken cancellationToken);
        Task<List<GetEmployeeTransportAllResponse>> EmployeeTransportAllSchedule(GetEmployeeTransportAllRequest request, CancellationToken cancellationToken);


        Task<List<GetScheduleDetailTransportResponse>> ScheduleDetail(GetScheduleDetailTransportRequest request, CancellationToken cancellationToken);

        Task AddTravel(AddTravelTransportRequest request, CancellationToken cancellationToken);

        Task ValidateAddTravel(AddTravelTransportRequest request, CancellationToken cancellationToken);

        Task DeleteSchedules(DeleteScheduleTransportRequest request, CancellationToken cancellationToken);

        Task<SearchReSchedulePeopleResponse> SearchReschuleData(SearchReSchedulePeopleRequest request, CancellationToken cancellationToken);

        Task<List<ReScheduleMultipleResponse>> ReScheduleMultiple(ReScheduleMultipleRequest request, CancellationToken cancellationToken);

        Task<EmployeeDateTransportResponse> EmployeeDateTransportSchedule(EmployeeDateTransportRequest request, CancellationToken cancellationToken);


        Task<RemoveTransportResponse> RemoveSchedule(RemoveTransportRequest request, CancellationToken cancellationToken);

        Task<List<EmployeeTransportNoShowResponse>> EmployeeTransportNoShow(EmployeeTransportNoShowRequest request, CancellationToken cancellationToken);

        Task<List<EmployeeTransportGoShowResponse>> EmployeeTransportGoShow(EmployeeTransportGoShowRequest request, CancellationToken cancellationToken);


        Task<int?> ReScheduleUpdate(ReScheduleUpdateRequest request, CancellationToken cancellationToken);

        Task<List<EmployeeExistingTransportResponse>> EmployeeExistingTransportSchedule(EmployeeExistingTransportRequest request, CancellationToken cancellationToken);


        Task<GetDataRequestResponse> GetDataRequestRequestChange(GetDataRequestRequest request, CancellationToken cancellationToken);

        Task CheckDataRequestRequestChange(CheckDataRequestRequest request, CancellationToken cancellationToken);

        Task AddExternalTravel(AddExternalTravelRequest request, CancellationToken cancellationToken);

        Task RemoveExternalTransport(RemoveExternalTransportRequest request, CancellationToken cancellationToken);

        Task ReScheduleExternalTransport(ReScheduleExternalTransportRequest request, CancellationToken cancellationToken);


        Task<List<TransportBookingInfoResponse>> TransportBookingInfo(TransportBookingInfoRequest request, CancellationToken cancellationToken);

        Task CreateNoGoShow(CreateNoGoShowRequest request, CancellationToken cancellationToken);

        Task DeleteNoGoShow(DeleteNoGoShowRequest request, CancellationToken cancellationToken);

        Task<List<MultipleBookingPreviewTransportResponse>> MultipleBookingPreviewTransport(MultipleBookingPreviewTransportRequest request, CancellationToken cancellationToken);


        Task<List<MultipleBookingAddTransportResponse>> MultipleBookingAddTransport(MultipleBookingAddTransportRequest request, CancellationToken cancellationToken);









    }
}
