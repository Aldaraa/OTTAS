using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class ReportJob : BaseEntity
    {
        [Required]
        public string Code { get; set; }

        public string? Description { get; set; }

        public string? ScheduleType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? SubscriptionMail { get; set; }

        public string? ScheduleCommandParameter { get; set; }


        public string? ScheduleCommand { get; set; }

        public int? ReportTemplateId { get; set; }

        public DateTime? NextExecuteDate { get; set; }
    }
}
