using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.NotificationFeature.EmployeeNoticationShortcut
{
    public sealed record EmployeeNoticationShortcutResponse
    {
        public int NewNotificationCount { get; set; }

        public List<EmployeeNoticationShortcutDetail> EmployeeNoticationShortcutDetails { get; set; }
    }

    public sealed record EmployeeNoticationShortcutDetail
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public string? RelativeTime { get; set; }

        public int? ViewStatus { get; set; }
             
        public string? NotifIndex { get; set; }

        public string? ChangeEmployee { get; set; }
    }
}
