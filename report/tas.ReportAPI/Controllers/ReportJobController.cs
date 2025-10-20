using Application.Common.Exceptions;
using Application.Features.ReportJobFeature.BuildReport;
using Application.Features.ReportJobFeature.CreateReportJobDaily;
using Application.Features.ReportJobFeature.CreateReportJobMonthly;
using Application.Features.ReportJobFeature.CreateReportJobRuntime;
using Application.Features.ReportJobFeature.CreateReportJobTimeValidate;
using Application.Features.ReportJobFeature.CreateReportJobWeekly;
using Application.Features.ReportJobFeature.DeleteReportJob;
using Application.Features.ReportJobFeature.GetAllReportJob;
using Application.Features.ReportJobFeature.GetAvailableTime;
using Application.Features.ReportJobFeature.GetAvailableTimeSlots;
using Application.Features.ReportJobFeature.GetReportJob;
using Application.Features.ReportJobFeature.GetReportJobLog;
using Application.Features.ReportJobFeature.GetSessionList;
using Application.Features.ReportJobFeature.KillSession;
using Application.Features.ReportJobFeature.TestReportJob;
using Application.Features.ReportJobFeature.UpdateReportJobDaily;
using Application.Features.ReportJobFeature.UpdateReportJobMonthly;
using Application.Features.ReportJobFeature.UpdateReportJobRuntime;
using Application.Features.ReportJobFeature.UpdateReportJobWeekly;
using Application.Features.ReportTemplateFeature.GetReportTemplateData;
using MediatR;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace tas.ReportAPI.Controllers
{
    [Route("rapi/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
    public class ReportJobController : ControllerBase
    {
        private readonly IMediator _mediator;


        public ReportJobController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllJob(int? templateId, string? keyword, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllReportJobRequest(templateId, keyword), cancellationToken);
            return Ok(response);
        }



        [HttpGet("{Id}")]
        public async Task<ActionResult> GetJob(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetReportJobRequest(Id), cancellationToken);
            return Ok(response);
        }

        [HttpPost("daily")]
        public async Task<ActionResult> CreateDailyJob(CreateReportJobDailyRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("weekly")]
        public async Task<ActionResult> CreateWeeklyJob(CreateReportJobWeeklyRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("monthly")]
        public async Task<ActionResult> CreateMonthlyJob(CreateReportJobMonthlyRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("runtime")]
        public async Task<ActionResult> CreateRunTimeJob(CreateReportJobRuntimeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("daily")]
        public async Task<ActionResult> UpdateDailyJob(UpdateReportJobDailyRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("weekly")]
        public async Task<ActionResult> UpdateWeeklyJob(UpdateReportJobWeeklyRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPut("monthly")]
        public async Task<ActionResult> UpdateMonthlyJob(UpdateReportJobMonthlyRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut("runtime")]
        public async Task<ActionResult> UpdateRunTimeJob(UpdateReportJobRuntimeRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult> DeleteJob(int Id, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(new DeleteReportJobRequest(Id), cancellationToken);
            return Ok(response);
        }


        [HttpPost("validatetime")]
        public async Task<ActionResult> ValidateJobDuplcation(CreateReportJobTimeValidateRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet("test/{Id}")]
        public async Task<ActionResult> TestJob(int Id, CancellationToken cancellationToken)
        {

            var response = await _mediator.Send(new TestReportJobRequest(Id), cancellationToken);

            if (response.ExcelFiles == null || response.ExcelFiles.Count == 0)
            {
                throw new BadRequestException("No data available for the report.");
            }

            if (response.ExcelFiles.Count == 1)
            {
                var excelFile = response.ExcelFiles[0];
                Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                Response.Headers.Add("Content-Disposition", $"attachment; filename={excelFile.Filename}");
                return File(excelFile.ExcelData.ToArray(), "application/xlsx");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < response.ExcelFiles.Count; i++)
                    {
                        var item = response.ExcelFiles[i];
                        string tempName = $"{item.Filename}_{i}_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.xlsx";
                        var zipEntry = archive.CreateEntry(!string.IsNullOrWhiteSpace(item.Filename) ? item.Filename : tempName);

                        using (var entryStream = zipEntry.Open())
                        using (var fileStream = new MemoryStream(item.ExcelData))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }

                memoryStream.Position = 0;

                Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                Response.Headers.Add("Content-Disposition", $"attachment; filename={response.ReportName}_{DateTime.Now:yyyy-MM-dd_HH_mm_ss}.zip");
                return File(memoryStream.ToArray(), "application/zip");
            }

        }




        [HttpPost("buildreport")]
        public async Task<ActionResult> BuildReport(BuildReportRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response.ExcelFiles == null || response.ExcelFiles.Count == 0)
            {
                throw new BadRequestException("No data available for the report.");
            }

            if (response.ExcelFiles.Count == 1)
            {
                var excelFile = response.ExcelFiles[0];
                Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                Response.Headers.Add("Content-Disposition", $"attachment; filename={excelFile.Filename}");
                return File(excelFile.ExcelData.ToArray(), "application/xlsx");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < response.ExcelFiles.Count; i++)
                    {
                        var item = response.ExcelFiles[i];
                        string tempName = $"{item.Filename}";
                        var zipEntry = archive.CreateEntry(!string.IsNullOrWhiteSpace(item.Filename) ? item.Filename : tempName);

                        using (var entryStream = zipEntry.Open())
                        using (var fileStream = new MemoryStream(item.ExcelData))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }

                memoryStream.Position = 0;
                Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                Response.Headers.Add("Content-Disposition", $"attachment; filename={response.ReportName}_{DateTime.Now:yyyy-MM-dd_HH_mm_ss}.zip");
                return File(memoryStream.ToArray(), "application/zip");

            }

        }




        [HttpGet("sessions")]
        public async Task<ActionResult> GetSessions(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetSessionListRequest(), cancellationToken);
            return Ok(response);

        }


        [HttpPost("killsession")]
        public async Task<ActionResult> KillSessions(KillSessionRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }


        [HttpGet("inactivetime/{scheduleDateTime}")]
        public async Task<ActionResult> GetAvailableTime(DateTime scheduleDateTime, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAvailableTimeRequest(scheduleDateTime), cancellationToken);
            return Ok(response);

        }


        [HttpGet("GetAvailableTimeSlots")]
        public async Task<ActionResult> GetAvailableTimeSlots(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAvailableTimeSlotsRequest(), cancellationToken);
            return Ok(response);

        }




        [HttpGet("log/{Id}")]
        public async Task<ActionResult> GetJobLog(int Id, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetReportJobLogRequest(Id), cancellationToken);
            return Ok(response);

        }
    }
}
