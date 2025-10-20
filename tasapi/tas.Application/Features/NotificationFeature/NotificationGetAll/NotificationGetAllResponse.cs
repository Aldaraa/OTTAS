using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Application.Features.EmployeeFeature.SearchEmployee;
using tas.Domain.Common;

namespace tas.Application.Features.NotificationFeature.NotificationGetAll
{

    public sealed record NotificationGetAllResponse : BasePaginationResponse<NotificationGetAllResult>
    {

    }



    public sealed record NotificationGetAllResult 
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public string? RelativeTime { get; set; }

        public int? ViewStatus { get; set; }

        public string? NotifIndex { get; set; }

        public string? ChangeEmployee { get; set; }
    }

}
