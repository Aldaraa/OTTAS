using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using tas.Application.Features.NationalityFeature.GetAllNationality;
using tas.Application.Features.NotificationFeature.NotificationGetAll;
using tas.Application.Features.NotificationFeature.NotificationGetDetail;
using tas.Application.Repositories;
using tas.Application.Service;

namespace tas.WebAPI.Controllers.Tas
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [RoleAuthorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly HTTPUserRepository _hTTPUserRepository;
        private readonly NotificationHub _notificationHub;

        public NotificationController(IMediator mediator, ILogger<NotificationController> logger, IHubContext<NotificationHub> hubContext, HTTPUserRepository hTTPUserRepository, NotificationHub notificationHub)
        {
            _mediator = mediator;
            _logger = logger;
            _hubContext = hubContext;
            _hTTPUserRepository = hTTPUserRepository;
            _notificationHub = notificationHub;
        }

        [HttpGet("detail/{NotifIndex}")]
        public async Task<ActionResult<NotificationGetDetailResponse>> GetDetail(string NotifIndex, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new NotificationGetDetailRequest(NotifIndex), cancellationToken);
             await   _notificationHub.UserNotification(HttpContext.User.Identity.Name);

            return Ok(response);

        }

        [HttpPost("all")]
        public async Task<ActionResult<NotificationGetDetailResponse>> GetAll(NotificationGetAllRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(response);

        }

    }
}
