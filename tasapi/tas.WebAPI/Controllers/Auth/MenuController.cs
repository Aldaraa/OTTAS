using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tas.Application.Features.SysMenuFeature.GetAllMenu;
using tas.Application.Features.UserFeatures.CreateUser;
using tas.Application.Features.UserFeatures.GetAllUser;

namespace tas.WebAPI.Controllers.Auth
{

    [Route("api/auth/[controller]")]
    [ApiController]
    [Authorize]

    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllMenuResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetAllMenuRequest(), cancellationToken);
            return Ok(response);
        }





    }
}
