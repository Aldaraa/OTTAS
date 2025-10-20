using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Features.RequestDocumentFeature.CompleteRequestDocumentSiteTravelReschedule
{
    public sealed class CompleteRequestDocumentSiteTravelRescheduleResponse
    {
        public int? OldScheduleId { get; set; }
        public int? NewScheduleId { get;set; }
    }
}
