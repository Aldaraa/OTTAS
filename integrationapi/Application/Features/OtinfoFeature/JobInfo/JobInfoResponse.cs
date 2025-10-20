
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OtinfoFeature.JobInfo
{
    public sealed record JobInfoResponse
    {
        public string JobKey { get; set; }
        public DateTime? NextFireTime { get; set; }
        public DateTime? JobRunTime { get; set; }

        public int? RepeatCount { get; set; }



    }




}
