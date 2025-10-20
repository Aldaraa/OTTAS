using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.GroupDetailFeature.CreateGroupDetail;
using tas.Application.Features.GroupDetailFeature.GetAllGroupDetail;
using tas.Application.Features.SysFilesFeature.MultiUploadSysFile;
using tas.Application.Features.SysFilesFeature.UploadSysFiles;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class SysFileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SysFileController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<ActionResult> Upload([FromForm]  UploadSysFileRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }


        [HttpPost("multi")]
        public async Task<ActionResult> MultiUpload([FromForm] MultiUploadSysFileRequest request,
    CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);
        }

    }
}
