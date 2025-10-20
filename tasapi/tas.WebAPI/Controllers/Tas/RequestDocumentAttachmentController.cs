using Azure.Core;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.Word;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestDocument;
using tas.Application.Features.MailSmtpConfigFeature.SendMailRequestNonsiteDocument;
using tas.Application.Features.RequestDocumentAttachmentFeature.CreateRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.DeleteRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.DownloadRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.GetRequestDocumentAttachment;
using tas.Application.Features.RequestDocumentAttachmentFeature.UpdateRequestDocumentAttachment;
using tas.Application.Features.SysVersionNoteFeature.GetSysVersionNote;
using tas.Application.Service;
using tas.Domain.Common;

namespace tas.WebAPI.Controllers.Tas
{

    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class RequestDocumentAttachmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestDocumentAttachmentController> _logger;
        private readonly CacheService _cacheService;

        public RequestDocumentAttachmentController(IMediator mediator, ILogger<RequestDocumentAttachmentController> logger, CacheService cacheService)
        {
            _mediator = mediator;
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<List<GetRequestDocumentAttachmentResponse>>> GetRequestDocumentAttachment(int documentId, CancellationToken cancellationToken)
        {

            var cacheKey = $"GetRequestDocumentAttachment_{documentId}";

            if (!_cacheService.TryGetValue(cacheKey, out List<GetRequestDocumentAttachmentResponse> response))
            {
                response = await _mediator.Send(new GetRequestDocumentAttachmentRequest(documentId), cancellationToken);
                _cacheService.Set(cacheKey, response, GlobalConstants.ENDPOINT_MASTER_STATIC_MINUTE);
            }

            return Ok(response);

            //var response = await _mediator.Send(new GetRequestDocumentAttachmentRequest(documentId), cancellationToken);
            //return Ok(response);
        }


        [HttpGet("download/{documentId}")]
        public async Task<ActionResult<List<GetRequestDocumentAttachmentResponse>>> DownloadRequestDocumentAttachment(int documentId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new DownloadRequestDocumentAttachmentRequest(documentId), cancellationToken);
            if (response.Attachments.Count > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in response.Attachments)
                        {


                            string fullPath = Directory.GetCurrentDirectory() +  item;

                            if (System.IO.File.Exists(fullPath))
                            {
                                string filename = Path.GetFileName(fullPath);
                                var zipEntry = archive.CreateEntry(filename, CompressionLevel.Optimal);

                                using (var entryStream = zipEntry.Open())
                                using (var stream = System.IO.File.OpenRead(Directory.GetCurrentDirectory() + item))
                                {
                                   await stream.CopyToAsync(entryStream);
                                }
                            }

                        }
                    }

                    memoryStream.Position = 0;

                    Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                    return File(memoryStream.ToArray(), "application/zip", $"TAS_REQUEST_ATTACHMENTS_{documentId}_{DateTime.Now:yyyy-MM-dd_HH_mm_ss}.zip");
                }
            }
            else
            {
                return NoContent();
            }

            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult> CreateRequestDocumentAttachment(CreateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            var cacheKey = $"GetRequestDocumentAttachment_{request.DocumentId}";
            var response = await _mediator.Send(request, cancellationToken);
            _cacheService.Remove(cacheKey);
            await _mediator.Send(new SendMailRequestNonsiteDocumentRequest(request.DocumentId, "Attachnment updated"), cancellationToken);

            return Ok(response);
        }



        [HttpPut]
        public async Task<ActionResult> UpdateRequestDocumentAttachment(UpdateRequestDocumentAttachmentRequest request, CancellationToken cancellationToken)
        {
            var responseDocumentId = await _mediator.Send(request, cancellationToken);
            var cacheKey = $"GetRequestDocumentAttachment_{responseDocumentId}";
            _cacheService.Remove(cacheKey);
            return Ok(responseDocumentId);
        }


        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteRequestDocumentAttachment(int Id, CancellationToken cancellationToken)
        {
            var responseDocumentId = await _mediator.Send(new DeleteRequestDocumentAttachmentRequest(Id), cancellationToken);
            var cacheKey = $"GetRequestDocumentAttachment_{responseDocumentId}";
            _cacheService.Remove(cacheKey);
            return Ok();
        }


    }
}
