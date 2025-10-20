using Application.Features.ReportJobFeature.GetReportJobLog;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public partial class JobExecuteServiceRepository
    {

        private async Task CreateJobLogSuccess(int jobId, string? descr)
        {
            if (descr != null && descr.Length > 500)
            {
                descr = descr.Substring(0, 500);
            }

            var newRecord = new ReportJobLog
            {
                Active = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                ExecuteDate = DateTime.Now,
                Descr = descr,
                ExecuteStatus = "Success",
                ReportJobId = jobId
            };

            Context.ReportJobLog.Add(newRecord);
            await  DeleteOldJobLog();
            await Context.SaveChangesAsync();


        }


        private async Task CreateJobLogFailed(int jobId, string message)
        {
            if (message != null && message.Length > 500)
            {
                message = message.Substring(0, 500);
            }

            var newRecord = new ReportJobLog
            {
                Active = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                ExecuteDate = DateTime.Now,
                ExecuteStatus = "Error",
                Descr = message,
                ReportJobId = jobId
            };

            Context.ReportJobLog.Add(newRecord);

            await Context.SaveChangesAsync();
        }


        private async Task DeleteOldJobLog()
        {
            var oldDate = DateTime.Today.AddMonths(-1);
            var oldata = await Context.ReportJobLog.Where(c => c.DateCreated.Value.Date < oldDate).ToListAsync();
            if (oldata.Count > 0)
            {
                Context.ReportJobLog.RemoveRange(oldata);
            }
            
        }




    }
}
