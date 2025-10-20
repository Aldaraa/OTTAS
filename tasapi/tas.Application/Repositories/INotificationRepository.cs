using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.GroupDetailFeature.GetAllGroupDetail;
using tas.Application.Features.NotificationFeature.EmployeeNoticationShortcut;
using tas.Application.Features.NotificationFeature.NotificationGetAll;
using tas.Application.Features.NotificationFeature.NotificationGetDetail;
using tas.Domain.Entities;

namespace tas.Application.Repositories
{

    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<NotificationGetDetailResponse> NotificationGetDetail(NotificationGetDetailRequest request, CancellationToken cancellationToken);

        Task<EmployeeNoticationShortcutResponse> EmployeeNoticationShortcut(EmployeeNoticationShortcutRequest request, CancellationToken cancellationToken);

        Task<NotificationGetAllResponse> NotificationGetAll(NotificationGetAllRequest request, CancellationToken cancellationToken);


    }
}
